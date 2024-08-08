using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage.CodeGen;

public static class CodeGen
{
    const string assemblyName = "TFL";

    public static Module Compile(Program program)
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule(assemblyName);

        foreach (StructDecl decl in program.Structs)
            DefineStruct(module, decl);

        foreach (FunctionDecl decl in program.Functions)
            DefineFunction(module, decl);

        foreach (StructDecl decl in program.Structs)
            CompileStruct(decl);

        foreach (FunctionDecl decl in program.Functions)
            CompileFunction(decl);

        module.CreateGlobalFunctions();
        return module;
    }

    static void DefineFunction(ModuleBuilder module, FunctionDecl decl)
    {
        var func = decl.Reference!;

        var method = module.DefineGlobalMethod(
            decl.Ident.Name,
            MethodAttributes.Public | MethodAttributes.Static,
            func.ReturnType!.ClrType,
            func.Arguments.Select(a => a.Type!.ClrType!).ToArray()
        );

        foreach (var (arg, idx) in @func.Arguments.Select((f, i) => (f, i)))
            method.DefineParameter(idx, ParameterAttributes.None, arg.Name);

        func.MethodBuilder = method;
    }

    static void CompileFunction(FunctionDecl decl)
    {
        var func = decl.Reference!;

        for (short i = 0; i < func.Arguments.Count; i++)
            func.Arguments[i].Index = i;

        var generator = func.MethodBuilder!.GetILGenerator();
        decl.Block.Accept(new CodeGenVisitor(generator));
        generator.Emit(OpCodes.Ret);
    }

    static void DefineStruct(ModuleBuilder module, StructDecl decl)
    {
        var @struct = decl.Reference!;
        var type = module.DefineType(decl.Ident.Name, TypeAttributes.Public);
        @struct.TypeBuilder = type;

        @struct.EqualOp = DefineOperator("op_Equality");
        @struct.NotEqualOp = DefineOperator("op_Inequality");

        MethodBuilder DefineOperator(string name)
        {
            var method = type.DefineMethod(
                name,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.SpecialName,
                typeof(bool),
                [type, type]
            );

            method.DefineParameter(0, ParameterAttributes.None, "lhs");
            method.DefineParameter(0, ParameterAttributes.None, "rhs");

            return method;
        }
    }

    static void CompileStruct(StructDecl decl)
    {
        var @struct = decl.Reference!;
        var builder = @struct.TypeBuilder!;

        foreach (var field in @struct.Fields)
            field.FieldInfo = builder.DefineField(
                field.Name,
                field.Type!.ClrType!,
                FieldAttributes.Public | FieldAttributes.InitOnly
            );

        CreateConstructor(decl);
        CompileEqualityCheck(decl);

        builder.CreateType();
    }

    static void CreateConstructor(StructDecl decl)
    {
        var @struct = decl.Reference!;
        var builder = @struct.TypeBuilder!;

        var constructor = builder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            @struct.Fields.Select(f => f.Type!.ClrType!).ToArray()
        );
        @struct.ConstructorInfo = constructor;

        foreach (var (field, idx) in @struct.Fields.Select((f, i) => (f, i)))
            constructor.DefineParameter(idx, ParameterAttributes.None, field.Name);

        var generator = constructor.GetILGenerator();

        foreach (var (field, idx) in @struct.Fields.Select((f, i) => (f, i)))
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg, (short)idx + 1);
            generator.Emit(OpCodes.Stfld, field.FieldInfo!);
        }

        generator.Emit(OpCodes.Ret);
    }

    static void CompileEqualityCheck(StructDecl decl)
    {
        var @struct = decl.Reference!;

        var equal = @struct.EqualOp!;
        var generator = equal.GetILGenerator();

        var trueLabel = generator.DefineLabel();
        var falseLabel = generator.DefineLabel();

        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Ceq);
        generator.Emit(OpCodes.Brtrue, trueLabel);

        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Brfalse, falseLabel);

        generator.Emit(OpCodes.Ldarg_1);
        generator.Emit(OpCodes.Brfalse, falseLabel);

        foreach (var field in @struct.Fields)
        {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field.FieldInfo!);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Ldfld, field.FieldInfo!);

            EmitEqualityCheck(generator, field.Type!);

            generator.Emit(OpCodes.Brfalse, falseLabel);
        }

        generator.MarkLabel(trueLabel);
        generator.Emit(OpCodes.Ldc_I4_1);
        generator.Emit(OpCodes.Ret);

        generator.MarkLabel(falseLabel);
        generator.Emit(OpCodes.Ldc_I4_0);
        generator.Emit(OpCodes.Ret);

        var notEqual = @struct.NotEqualOp!;
        var notEqualGenerator = notEqual.GetILGenerator();

        notEqualGenerator.Emit(OpCodes.Ldarg_0);
        notEqualGenerator.Emit(OpCodes.Ldarg_1);
        notEqualGenerator.Emit(OpCodes.Call, equal);

        notEqualGenerator.Emit(OpCodes.Ldc_I4_0);
        notEqualGenerator.Emit(OpCodes.Ceq);
        notEqualGenerator.Emit(OpCodes.Ret);
    }

    internal static void EmitEqualityCheck(ILGenerator generator, IType type, bool invert = false)
    {
        switch (type)
        {
            case IntType or BoolType:
                generator.Emit(OpCodes.Ceq);

                if (invert)
                {
                    generator.Emit(OpCodes.Ldc_I4_0);
                    generator.Emit(OpCodes.Ceq);
                }

                break;

            case StringType:
                generator.Emit(OpCodes.Call, invert ? stringNotEqualOp : stringEqualOp);
                break;

            case UnitType:
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Pop);
                generator.Emit(invert ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);

                break;

            case Struct { EqualOp: var equal, NotEqualOp: var notEqual }:
                generator.Emit(OpCodes.Call, invert ? notEqual! : equal!);

                break;

            default:
                throw new InvalidOperationException("Unexpected type");
        }
    }

    static readonly MethodInfo stringEqualOp = typeof(string).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public, [typeof(string), typeof(string)])!;
    static readonly MethodInfo stringNotEqualOp = typeof(string).GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public, [typeof(string), typeof(string)])!;
}
