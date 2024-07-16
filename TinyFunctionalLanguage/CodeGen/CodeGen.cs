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
            var func = (Function)decl.Name.Reference!;

            var method = module.DefineGlobalMethod(
                decl.Name.Name,
                MethodAttributes.Public | MethodAttributes.Static,
                func.ReturnType!.ClrType,
                func.Arguments.Select(a => a.Type!.ClrType!).ToArray()
            );

            func.Method = method;
        }

        foreach (FunctionDecl decl in program.Functions)
        {
            var func = (Function)decl.Name.Reference!;

            var generator = func.Method!.GetILGenerator();
            new CodeGenVisitor(generator).Generate(decl);
        }

        module.CreateGlobalFunctions();
        return module;
    }
}
