# Tiny Functional Language (TFL)

TFL is a tiny, functional programming language, hence the name.

## Data types

TFL has four primitive types:

- `int` - A 64-bit signed number, ranging from $-2^{63}$ to $2^{63} - 1$.
- `bool` - A boolean value (`true` or `false`).
- `string` - A sequence of characters, encoded in UTF-16.
- `unit` - The [unit type](https://en.wikipedia.org/wiki/Unit_type) has only one possible value.
  It's the return type of statements and functions that don't really return anything, like `void` is some other languages.

Additionally, its possible to define arbitrary structs.

All values in TFL are immutable.

## Operators

TFL supports five binary arithmetic operators: addition (`+`), subtraction (`-`),
multiplication (`*`) and integer division (`/`) and remainder (`%`).
There is also one unary operator: the negation (`-`) operator.
All arithmetic is checked, any overflow or division by zero will terminate the program.

Then there are six comparison operators.
The `==` and `!=` operators can be used on values of any type,
while the `<`, `>`, `<=`, `>=` operators can only be used on `int`s.

Finally, there are three logical operators:
and (`&`), or (`|`) and the unary not (`!`) operator.
The `&` and `|` operators currently don't short circuit,
meaning that both operands are always evaluated.

### Precedence

There are seven levels of operator precedence,
here listed in order of decreasing precedence:

1. unary `-`, `!`
2. `*`, `/`, `%`
3. `+`, `-`
4. `<`, `>`, `<=`, `>=`
5. `==`, `!=`
6. `&`
7. `|`

## Program structure

Programs in TFL consist of functions and structs:

```
struct Point {
    x: int;
    y: int;
}

func distance_squared(a: Point, b: Point): int {
    let dx = (a.x - b.x);
    let dy = (a.y - b.y);

    dx * dx + dy * dy
}
```

Functions and structs may be declared in any order.

## Functions

Since TFL is a functional language, its most important building block is a function.
A function starts with the `func` keyword, followed by the name of the function,
the argument list (including the argument types) and the return type, like so:

```
func add(a: int, b: int): int {
    a + b
}
```

The body of a function may contain multiple statements,
in which case they must be separated by semicolons:

```
func distance_squared(ax: int, ay: int, bx: int, by: int): int {
    let dx = (ax - bx);
    let dy = (ay - by);

    dx * dx + dy * dy
}
```

### Returning values

Unlike some other languages, there is no `return` statement.
To return a value, simply write an expression at the end of the function body
(without a trailing semicolon).
For example, this function will return the number 4:

```
func four(): int {
    4
}
```

If the last statement ends with a semicolon, then the function will return the `unit` type:

```
func do_nothing(x: int): unit {
    x *= 2;
}
```

Since functions in TFL have no side-effects, such a function isn't very useful.

### Calling functions

Functions can of course be called:

```
func double(n: int): int {
    n * n
}

func quadruple(n: int): int {
    double(double(n))
}
```

## Statements and control flow

Despite the fact that TFL is a functional language,
the bodies of functions are written in an imperative style.

### Variables

You can declare a variable using the `let` keyword:

```
func main(): unit {
    let cost_per_unit = 15;
    let units_bought = 4;
    let total_cost = cost_per_unit * units_bought; # 60
}
```

The type of the variable is automatically inferred.
Variables are mutable and can be assigned to:

```
func main(): unit {
    let n = 10;
    n = -n;     # -10
    n = n * n;  # 100
}
```

Aside from `=`, you can also use the `+=`, `-=`, `*=`, `/=`, `%=`, `&=` and `|=` operators
to quickly perform an arithmetic operation on a variable.

Variables can be shadowed by variables declared later.
A variable only gets added to the current scope after the `let` statement finishes executing,
so you can use the previous variable with a given name when declaring the next:

```
func main(): unit {
    let n = 10;
    let n = n / 2;  # 5
    let n = n == 5; # true
}
```

### Blocks

Statements can be grouped into blocks.
A block defines a scope. Variables created inside a scope
aren't accessible outside the scope:

```
func main(): int {
    let n = 1;

    {
        let n = n + 1; # 2
    };

    n # 1
}
```

Blocks aren't just statements, they are also expressions.
They can have a return value, just like functions:

```
func fifth_power(n: int): int {
    let fourth_power = {
        let n = n * n;
        n * n
    };

    fourth_power * n
}
```

The body of a function is actually just a block.

### If statements and expressions

`if` statements can be used to execute code conditionally:

```
func clamp(value: int, min: int, max: int): int {
    if value < min {
        value = min;
    };

    if value > max {
        value = max;
    };

    value
}
```

An `if` statement can be followed by an `else` branch:

```
func calculate_tax(price: int, reduced_rate: bool): int {
    if reduced_rate {
        price /= 5;
    } else {
        price /= 3;
    };

    price
}
```

`if` can also be used as an expression,
in the same way the ternary operator is used in some other languages:

```
func get_greeting(age: int): string {
    let person = if age < 18 {
        "child"
    } else {
        "adult"
    };

    "Dear " + person
}
```

### While statements

To create loops, you can use the `while` statement:

```
func factorial(n: int): int {
    let result = 1;

    while n > 0 {
        result *= n;
        n -= 1;
    };

    result
}
```

## Strings

TFL has very basic support for strings.
String literals are delimited with `"` characters.
You can use any escapes supported by JSON:

```
func main(): unit {
    let hello = "Hello!";
    let quotes = "\"Hello!\", he said.";
    let newlines = "This is one line.\nThis is another.";
    let emoji = "\u263A";
}
```

Newlines can also appear in literals unescaped:

```
func get_usage(): string {
"Usage: ./hello_world

Options:
  -h  Print usage"
}
```

Strings cannot currently be inspected in any way
(except for equality tests with `==` and `!=`),
but they can be created through concatenation:

```
func main(): string {
    let subject = "world";
    "Hello " + subject + "!"
}
```

You can also concatenate a `string` and an `int`:

```
func main(a: int, b: int): string {
    a + " + " + b + " = " + (a + b)
}
```

## Structs

Finally, you can declare structs using the `struct` keyword:

```
struct Person {
    name: string;
    age: int;
    is_student: bool;
}
```

Structs are heap-allocated,
which means that they can contain themselves recursively:

```
struct Node {
    value: int;
    left: Node;
    right: Node;
}
```

### Constructing structs

To create an instance of a struct,
simply call the structs name as if it were a function,
with each argument corresponding to one of the structs fields:

```
func get_person(): Person {
    Person("Gordon Freeman", 27, false)
}
```

### Accessing fields

You can use the dot "operator" to get the value of a field:

```
func has_discount(person: Person): bool {
    person.is_student & person.age <= 26
}
```

You can also use the `=` operator (and its friends `+=`, `-=`, etc.)
to change the value of a field.
This doesn't actually change the original struct,
it instead creates a new instance with the field changed:

```
func main(): unit {
    let person = Person("Gordon Freeman", 27, false);
    let copy = person;

    person.age += 1;

    let x = person.age; # 28
    let y = copy.age; # 27
}
```

You can chain field accesses arbitrarily:

```
func main(node: Node): unit {
    node.left.left.right.left.value = 0;
}
```

### Comparing structs

Structs can be compared using the `==` and `!=` operators.
They will compare the structs field by field:

```
func main(): unit {
    let a = Point(10, 20) == Point(10, 20); # true
    let b = Point(10, 20) == Point(10, 19); # false
}
```

### Null

Structs (and only structs) can also have the special value of `null`.
This is necessary to allow recursive structs to exists.
For type inference reasons, the `null` keyword must be followed by the name of the struct:

```
struct List {
    value: int;
    next: List;
}

func get_even_primes(): List {
    List(2, null List)
}
```

Trying to access a field of a `null` struct fill cause the program to terminate.
You can check if a struct is `null` using the `==` or `!=` operator:

```
func sum(list: List): int {
    let sum = 0;

    while list != null List {
        sum += list.value;
        list = list.next;
    };

    sum
}
```

## Comments

You can use the `#` symbol to create a comment.
The comment goes on until the end of the line:

```
func main(): int { # Declares a function called main
    let a = 5; # Assigns 5 to a variable
    a # Returns the variable
} # End the function
```

## CLI

To run programs, you can use the CLI.
It will simply run the function called `main` and print out whatever it returns.

The first argument of the CLI is the path to the program to execute:

```
> TinyFunctionalLanguage.Cli hello_world.tfl
Hello, world!
```

Any other arguments will be parsed and passed on to `main`.
Consider this program:

```
# hello.tfl
func main(name: string, age: int): string {
    "Hello, " + name + "!\nI've heard you're " + age + " years old."
}
```

This program can be executed like this:

```
> TinyFunctionalLanguage.Cli hello.tfl Benjamin 20
Hello, Benjamin!
I've heard you're 20 years old.
```

If `main` returns a struct, it will be printed out as a tree.
For example, this program:

```
# cat.tfl
struct Cat {
    name: string;
    age: int;
    is_black: bool;
}

func main(): Cat {
    Cat("Vendulka", 4, false)
}
```

Will print the following output:

```
> TinyFunctionalLanguage.Cli cat.tfl
Cat
  name = Vendulka
  age = 4
  is_black = False
```
