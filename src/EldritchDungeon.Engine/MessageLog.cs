namespace EldritchDungeon.Engine;

public class MessageLog
{
    private const int MaxMessages = 100;
    private readonly List<string> _messages = new();

    public IReadOnlyList<string> Messages => _messages;

    public void Add(string message)
    {
        _messages.Add(message);
        if (_messages.Count > MaxMessages)
            _messages.RemoveAt(0);
    }

    public IReadOnlyList<string> GetRecent(int count)
    {
        if (count >= _messages.Count)
            return _messages;
        return _messages.GetRange(_messages.Count - count, count);
    }
}
