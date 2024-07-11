namespace TinyFunctionalLanguage.Parse;

public record struct Point(int Row, int Col) {}
public record struct Span(Point From, Point To) {}
