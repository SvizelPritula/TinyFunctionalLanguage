using System.Diagnostics.CodeAnalysis;

namespace TinyFunctionalLanguage.Bindings;

class ScopedMap<TKey, TValue> where TKey : notnull
{
    readonly Dictionary<TKey, TValue> currentValues = [];
    readonly Stack<List<(TKey key, TValue? previousValue)>> stack = [];

    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue? value) => currentValues.TryGetValue(key, out value);

    public bool Insert(TKey key, TValue value)
    {
        bool wasPresent = currentValues.TryGetValue(key, out var previousValue);

        if (stack.Count > 0)
            stack.Peek().Add((key, previousValue));

        currentValues[key] = value;

        return wasPresent;
    }

    public void Push() => stack.Push([]);
    public void Pop()
    {
        foreach ((var key, var previousValue) in Enumerable.Reverse(stack.Pop()))
        {
            if (previousValue is not null)
                currentValues[key] = previousValue;
            else
                currentValues.Remove(key);
        }
    }
}
