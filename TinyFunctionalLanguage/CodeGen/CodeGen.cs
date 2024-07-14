using System.Reflection;
using System.Reflection.Emit;
using TinyFunctionalLanguage.Ast;

namespace TinyFunctionalLanguage.CodeGen;

public static class CodeGen
{
    const string assemblyName = "TFL";

    public static Action Compile(Program program)
    {
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule(assemblyName);

        foreach (IDeclaration decl in program.Declarations)
        {
            if (decl is not FunctionDecl func)
                continue;

            var method = module.DefineGlobalMethod(decl.Name.Name, MethodAttributes.Public | MethodAttributes.Static, null, null);
            var generator = method.GetILGenerator();

            new CodeGenVisitor(generator).Generate(func);

        }

        module.CreateGlobalFunctions();

        MethodInfo main = module.GetMethod("main")!;
        return main.CreateDelegate<Action>();
    }
}
