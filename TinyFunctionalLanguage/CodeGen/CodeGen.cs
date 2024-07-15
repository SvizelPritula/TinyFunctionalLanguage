using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;
using TinyFunctionalLanguage.Bindings;

namespace TinyFunctionalLanguage.CodeGen;

public static class CodeGen
{
    const string assemblyName = "TFL";

    public static T Compile<T>(Program program) where T : Delegate
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule(assemblyName);

        foreach (IDeclaration decl in program.Declarations)
        {
            if (decl is not FunctionDecl funcDecl)
                continue;

            Function func = (Function)funcDecl.Name.Reference!;

            var method = module.DefineGlobalMethod(
                decl.Name.Name,
                MethodAttributes.Public | MethodAttributes.Static,
                func.ReturnType!.ClrType,
                func.Arguments.Select(a => a.Type!.ClrType!).ToArray()
            );

            var generator = method.GetILGenerator();

            new CodeGenVisitor(generator).Generate(funcDecl);

        }

        module.CreateGlobalFunctions();

        MethodInfo main = module.GetMethod("main")!;
        return main.CreateDelegate<T>();
    }
}
