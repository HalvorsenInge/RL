using EldritchDungeon.Engine;

namespace EldritchDungeon.Engine.Tests;

public class MessageLogTests
{
    [Fact]
    public void Add_StoresMessage()
    {
        var log = new MessageLog();
        log.Add("Hello");

        Assert.Single(log.Messages);
        Assert.Equal("Hello", log.Messages[0]);
    }

    [Fact]
    public void Add_CapsAtMaxMessages()
    {
        var log = new MessageLog();
        for (int i = 0; i < 110; i++)
            log.Add($"Message {i}");

        Assert.Equal(100, log.Messages.Count);
        Assert.Equal("Message 10", log.Messages[0]);
    }

    [Fact]
    public void GetRecent_ReturnsRequestedCount()
    {
        var log = new MessageLog();
        for (int i = 0; i < 10; i++)
            log.Add($"Message {i}");

        var recent = log.GetRecent(3);

        Assert.Equal(3, recent.Count);
        Assert.Equal("Message 7", recent[0]);
        Assert.Equal("Message 9", recent[2]);
    }

    [Fact]
    public void GetRecent_ReturnsAllWhenCountExceedsMessages()
    {
        var log = new MessageLog();
        log.Add("Only one");

        var recent = log.GetRecent(5);
        Assert.Single(recent);
    }
}
