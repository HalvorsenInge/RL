using EldritchDungeon.Core;

namespace EldritchDungeon.UI;

public class ASCIIRenderer
{
    private readonly int _width;
    private readonly int _height;
    private readonly char[,] _currentBuffer;
    private readonly char[,] _previousBuffer;
    private readonly ConsoleColor[,] _currentFgColors;
    private readonly ConsoleColor[,] _previousFgColors;

    public ASCIIRenderer(int width = GameConstants.ScreenWidth, int height = GameConstants.ScreenHeight)
    {
        _width = width;
        _height = height;
        _currentBuffer = new char[width, height];
        _previousBuffer = new char[width, height];
        _currentFgColors = new ConsoleColor[width, height];
        _previousFgColors = new ConsoleColor[width, height];

        Clear();
        // Initialize previous buffer to force full first draw
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _previousBuffer[x, y] = '\0';
                _previousFgColors[x, y] = ConsoleColor.Black;
            }
        }
    }

    public void Set(int x, int y, char glyph, ConsoleColor color = ConsoleColor.White)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
            return;

        _currentBuffer[x, y] = glyph;
        _currentFgColors[x, y] = color;
    }

    public void Clear()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _currentBuffer[x, y] = ' ';
                _currentFgColors[x, y] = ConsoleColor.White;
            }
        }
    }

    public void Flush()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (_currentBuffer[x, y] != _previousBuffer[x, y]
                    || _currentFgColors[x, y] != _previousFgColors[x, y])
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = _currentFgColors[x, y];
                    Console.Write(_currentBuffer[x, y]);

                    _previousBuffer[x, y] = _currentBuffer[x, y];
                    _previousFgColors[x, y] = _currentFgColors[x, y];
                }
            }
        }

        Console.ResetColor();
    }

    public void WriteString(int x, int y, string text, ConsoleColor color = ConsoleColor.White)
    {
        for (int i = 0; i < text.Length && x + i < _width; i++)
        {
            Set(x + i, y, text[i], color);
        }
    }
}
