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

        foreach (FunctionDecl decl in program.Functions)
        {
            var func = decl.Reference!;

            var method = module.DefineGlobalMethod(
                decl.Ident.Name,
                MethodAttributes.Public | MethodAttributes.Static,
                func.ReturnType!.ClrType,
                func.Arguments.Select(a => a.Type!.ClrType!).ToArray()
            );

            func.Method = method;
        }

        foreach (FunctionDecl decl in program.Functions)
            CompileFunction(decl);

        module.CreateGlobalFunctions();
        return module;
    }

    static void CompileFunction(FunctionDecl decl)
    {

        var func = decl.Reference!;
        var generator = func.Method!.GetILGenerator();

        for (short i = 0; i < func.Arguments.Count; i++)
            func.Arguments[i].Index = i;

        decl.Block.Accept(new CodeGenVisitor(generator));
        generator.Emit(OpCodes.Ret);
    }
}
