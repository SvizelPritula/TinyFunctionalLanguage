using System.Reflection;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage;

public static class Compiler
{
    public static Module Compile(string code)
    {
        var ast = Parser.Parse(new Tokenizer(code));

        BindingPass.Run(ast);
        TypeInferencePass.Run(ast);

        return CodeGen.CodeGen.Compile(ast);
    }
}
