using TinyFunctionalLanguage.Parse;

string program = Console.In.ReadToEnd();

Tokenizer tokenizer = new(program);

while (tokenizer.Peek().Type != TokenType.Eof)
    Console.WriteLine(tokenizer.Next());
