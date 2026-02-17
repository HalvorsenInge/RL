using EldritchDungeon.Core;

namespace EldritchDungeon.UI;

public class MessageScreen : Screen
{
    private readonly ASCIIRenderer _renderer;
    private IReadOnlyList<string>? _messages;
    private int _scrollOffset;
    private const int MaxVisibleLines = 22;

    public MessageScreen(ASCIIRenderer renderer)
    {
        _renderer = renderer;
    }

    public void SetMessages(IReadOnlyList<string> messages)
    {
        _messages = messages;
        // Start scrolled to bottom
        _scrollOffset = Math.Max(0, messages.Count - MaxVisibleLines);
    }

    public override void Render()
    {
        _renderer.Clear();

        int row = 0;
        _renderer.WriteString(1, row, "=== Message Log ===", ConsoleColor.Yellow);
        if (_messages != null)
            _renderer.WriteString(50, row, $"({_messages.Count} messages)", ConsoleColor.DarkGray);
        row += 2;

        if (_messages == null || _messages.Count == 0)
        {
            _renderer.WriteString(1, row, "No messages yet.", ConsoleColor.DarkGray);
        }
        else
        {
            int end = Math.Min(_scrollOffset + MaxVisibleLines, _messages.Count);
            for (int i = _scrollOffset; i < end; i++)
            {
                string msg = _messages[i];
                if (msg.Length > GameConstants.ScreenWidth - 2)
                    msg = msg[..(GameConstants.ScreenWidth - 4)] + "..";

                // Older messages are dimmer
                var color = i >= _messages.Count - 3 ? ConsoleColor.White
                    : i >= _messages.Count - 10 ? ConsoleColor.Gray
                    : ConsoleColor.DarkGray;

                _renderer.WriteString(1, row, msg, color);
                row++;
            }

            // Scroll indicator
            if (_messages.Count > MaxVisibleLines)
            {
                string scrollInfo = $"-- {_scrollOffset + 1}-{end} of {_messages.Count} --";
                if (_scrollOffset > 0)
                    scrollInfo = "^ " + scrollInfo;
                if (end < _messages.Count)
                    scrollInfo = scrollInfo + " v";
                _renderer.WriteString(1, GameConstants.ScreenHeight - 2, scrollInfo, ConsoleColor.DarkGray);
            }
        }

        _renderer.WriteString(1, GameConstants.ScreenHeight - 1,
            "[Up/Down] Scroll  [Home] Top  [End] Bottom  [Esc] Back", ConsoleColor.DarkGray);

        _renderer.Flush();
    }

    public override ScreenResult HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (_messages == null) return keyInfo.Key == ConsoleKey.Escape ? ScreenResult.Close : ScreenResult.None;

        int maxOffset = Math.Max(0, _messages.Count - MaxVisibleLines);

        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                return ScreenResult.Close;

            case ConsoleKey.UpArrow:
                if (_scrollOffset > 0) _scrollOffset--;
                break;

            case ConsoleKey.DownArrow:
                if (_scrollOffset < maxOffset) _scrollOffset++;
                break;

            case ConsoleKey.PageUp:
                _scrollOffset = Math.Max(0, _scrollOffset - MaxVisibleLines);
                break;

            case ConsoleKey.PageDown:
                _scrollOffset = Math.Min(maxOffset, _scrollOffset + MaxVisibleLines);
                break;

            case ConsoleKey.Home:
                _scrollOffset = 0;
                break;

            case ConsoleKey.End:
                _scrollOffset = maxOffset;
                break;
        }

        return ScreenResult.None;
    }
}
