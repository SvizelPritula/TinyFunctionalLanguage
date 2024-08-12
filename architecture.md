# Architecture

The TFL compiler is a multi-pass compiler.
It turns TFL source code into a CLR dynamic assembly.

## AST

The most important data structure in the compiler is the Abstract Syntax Tree.
It contains a type for every kind of expression in TFL.
It's created by the Parser and then traversed by the other passes.
Some of the properties on the AST objects are `null` initially
and only get filled in during one of the later passes.

The AST revolves around the `IExpression` interface,
which represents an expression (or a statement).
There is also the `ITypeName` interface, which represents a name of a type,
and the `IDeclaration` interface, which represents any top-level declarations.

## Errors

The `ErrorSet` class is passed through every compilation stage.
It's job is to track all errors encountered during compilation.
Each error is represented by a `LanguageError` object,
which is usually simply created and added to the `ErrorSet`,
but it can also be thrown as an exception if needed.

## Tokenizer

The job of the tokenizer is simple: Split the program into tokens.
This involves recognising single- and double-character operators,
identifiers and keywords, as well as parsing literals.
It also provides the parser with location information,
thanks to the `SpannedReader` helper.

If the tokenizer finds and unknown character,
it will return a token with a type of `TokenType.Error`.

## Parser

The parser is a simple recursive descent parser.
It's responsible for building the AST.

It will try to keep parsing even when faced with a syntax error.
If it encounters a syntax error when parsing a function body,
it will skip to the next semicolon,
otherwise it will look for a `struct` or `func` keyword.

## Binding

The binding pass is responsible for name resolution.
It traverses the AST, keeps track of the current scope
and associates identifiers with variables, arguments, functions and structs.
It's also responsible for creating the objects that represent them.

It's *not* responsible for checking
if what the identifier refers to makes sense in context.
It's also *not* responsible for resolving struct field names,
since that requires type information.

If the binding pass encounters an unknown name,
it will emit an error and keep the relevant AST property set to null.
It's important that further passes account for that possibility.

## Type inference

The type inference pass traverses the AST
and computes the type of the result of each expression,
infers the types of variables and type-checks everything.
It's also responsible for turning `ITypeName`s into proper `IType`s
and resolving field names.

## Code generation

The codegen pass is responsible for compiling everything into CLR IL using `System.Reflection.Emit`.
It works in four main stages:

1. Declare all structs
2. Declare all functions
3. Fill in the field of all structs and compile constructors
4. Compile all functions

Each stage needs objects created in the previous stage.

Functions get compiled into global methods,
structs get compiled into reference types.
Each struct also gets a constructor and `==` and `!=` operator overload.

The codegen phase only runs if no errors were encountered during compilation.
It also doesn't report any errors itself, barring bugs,
codegen will always succeed if none of the prior stages reported any errors.
