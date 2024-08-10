using System.Reflection;
using TinyFunctionalLanguage.Bindings;
using TinyFunctionalLanguage.Parse;
using TinyFunctionalLanguage.Types;

namespace TinyFunctionalLanguage;

public static class Compiler
{
    public static Module Compile(string code)
    {
        ErrorSet errors = new();

        var ast = Parser.Parse(new Tokenizer(code, errors), errors);

        BindingPass.Run(ast, errors);
        TypeInferencePass.Run(ast, errors);

        if (errors.HasErrors)
            throw errors.Exception();

        return CodeGen.CodeGen.Compile(ast);
    }
}
