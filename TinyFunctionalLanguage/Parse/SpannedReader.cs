namespace TinyFunctionalLanguage.Parse;

class SpannedReader(string str)
{
    int index = 0;
    Point point = new(1, 1);

    public Point Point => point;

    public int Peek()
    {
        if (index >= str.Length)
            return -1;

        return str[index];
    }

    public int Read()
    {
        int c = Peek();

        if (c >= 0)
        {
            index++;

            if (c == '\n')
            {
                point.Col = 1;
                point.Row++;
            }
            else
            {
                point.Col++;
            }
        }

        return c;
    }
}
