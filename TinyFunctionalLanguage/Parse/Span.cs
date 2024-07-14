namespace TinyFunctionalLanguage.Parse;

public record struct Point(int Row, int Col) { }

public record struct Span(Point From, Point To)
{
    public Span(Point point) : this(point, point) { }

    public override readonly string ToString() => $"{From.Row}:{From.Col} - {To.Row}:{To.Col}";
}
