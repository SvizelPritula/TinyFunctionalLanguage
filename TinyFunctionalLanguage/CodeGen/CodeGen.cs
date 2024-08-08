using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;

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
}
