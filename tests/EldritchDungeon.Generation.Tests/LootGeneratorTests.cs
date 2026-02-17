using EldritchDungeon.Core;
using EldritchDungeon.Entities;
using EldritchDungeon.Entities.Items;
using EldritchDungeon.Generation;
using EldritchDungeon.World;

namespace EldritchDungeon.Generation.Tests;

public class LootGeneratorTests
{
    private DungeonMap CreateTestMap()
    {
        var map = new DungeonMap();
        map.InitializeTiles(40, 20);

        // Create rooms
        var rooms = new[]
        {
            new Room { X = 1, Y = 1, Width = 8, Height = 6 },
            new Room { X = 15, Y = 1, Width = 8, Height = 6 },
            new Room { X = 1, Y = 10, Width = 8, Height = 6 },
            new Room { X = 15, Y = 10, Width = 8, Height = 6 },
        };

        foreach (var room in rooms)
        {
            map.Rooms.Add(room);
            for (int x = room.X; x < room.X + room.Width; x++)
                for (int y = room.Y; y < room.Y + room.Height; y++)
                    map.SetTile(x, y, TileType.Floor);
        }

        // Place player in room 0
        var player = new Player { Name = "Test" };
        player.InitializeComponents();
        map.PlaceActor(player, rooms[0].CenterX, rooms[0].CenterY);

        return map;
    }

    [Fact]
    public void PlaceLoot_DoesNotPlaceInRoom0()
    {
        var map = CreateTestMap();
        var generator = new LootGenerator(new Random(42));

        generator.PlaceLoot(map, 1);

        var room0 = map.Rooms[0];
        var itemsInRoom0 = map.Items.Where(i =>
            i.X >= room0.X && i.X < room0.X + room0.Width &&
            i.Y >= room0.Y && i.Y < room0.Y + room0.Height).ToList();

        Assert.Empty(itemsInRoom0);
    }

    [Fact]
    public void PlaceLoot_PlacesItemsOnWalkableTiles()
    {
        var map = CreateTestMap();
        var generator = new LootGenerator(new Random(42));

        generator.PlaceLoot(map, 1);

        foreach (var (item, x, y) in map.Items)
        {
            Assert.True(map.GetCell(x, y).IsWalkable,
                $"Item '{item.Name}' placed on non-walkable tile at ({x},{y})");
        }
    }

    [Theory]
    [InlineData(1, new[] { "Medieval" })]
    [InlineData(5, new[] { "EarlyModern" })]
    [InlineData(10, new[] { "Dieselpunk" })]
    [InlineData(15, new[] { "Lovecraftian" })]
    [InlineData(20, new[] { "Lovecraftian" })]
    public void PlaceLoot_TierAppropriateWeapons(int level, string[] expectedCategories)
    {
        var map = CreateTestMap();
        // Use a seed that produces many items
        var generator = new LootGenerator(new Random(1));

        // Run multiple times to get items
        for (int i = 0; i < 10; i++)
        {
            map.Items.Clear();
            generator.PlaceLoot(map, level);
        }

        var weapons = map.Items
            .Where(i => i.Item is Weapon)
            .Select(i => (Weapon)i.Item)
            .ToList();

        if (weapons.Count > 0)
        {
            foreach (var weapon in weapons)
            {
                Assert.Contains(weapon.Category.ToString(), expectedCategories);
            }
        }
    }

    [Fact]
    public void PlaceLoot_Level1_NoSanityPotions()
    {
        var map = CreateTestMap();
        var generator = new LootGenerator(new Random(42));

        // Run multiple times
        for (int i = 0; i < 20; i++)
        {
            map.Items.Clear();
            generator.PlaceLoot(map, 1);
        }

        var sanityItems = map.Items
            .Where(i => i.Item is Consumable c && c.SanityAmount > 0)
            .ToList();

        Assert.Empty(sanityItems);
    }

    [Fact]
    public void PlaceLoot_Level8_IncludesDeepOneWhiskey()
    {
        var map = CreateTestMap();
        bool foundWhiskey = false;

        // Run many times with different seeds to check availability
        for (int seed = 0; seed < 100 && !foundWhiskey; seed++)
        {
            map.Items.Clear();
            var generator = new LootGenerator(new Random(seed));
            generator.PlaceLoot(map, 8);

            foundWhiskey = map.Items.Any(i => i.Item.Name == "Deep One Whiskey");
        }

        Assert.True(foundWhiskey, "Deep One Whiskey should be available at level 8");
    }

    [Fact]
    public void PlaceLoot_Level19_IncludesVoidEssence()
    {
        var map = CreateTestMap();
        bool foundVoidEssence = false;

        for (int seed = 0; seed < 100 && !foundVoidEssence; seed++)
        {
            map.Items.Clear();
            var generator = new LootGenerator(new Random(seed));
            generator.PlaceLoot(map, 19);

            foundVoidEssence = map.Items.Any(i => i.Item.Name == "Void Essence");
        }

        Assert.True(foundVoidEssence, "Void Essence should be available at level 19+");
    }

    [Fact]
    public void PlaceLoot_DeterministicWithSameSeed()
    {
        var map1 = CreateTestMap();
        var map2 = CreateTestMap();

        new LootGenerator(new Random(42)).PlaceLoot(map1, 5);
        new LootGenerator(new Random(42)).PlaceLoot(map2, 5);

        Assert.Equal(map1.Items.Count, map2.Items.Count);

        for (int i = 0; i < map1.Items.Count; i++)
        {
            Assert.Equal(map1.Items[i].Item.Name, map2.Items[i].Item.Name);
            Assert.Equal(map1.Items[i].X, map2.Items[i].X);
            Assert.Equal(map1.Items[i].Y, map2.Items[i].Y);
        }
    }

    [Fact]
    public void PlaceLoot_ItemsAreClones()
    {
        var map = CreateTestMap();
        var generator = new LootGenerator(new Random(42));

        generator.PlaceLoot(map, 1);
        generator.PlaceLoot(map, 1);

        // If two items have the same name, they should be distinct objects
        var grouped = map.Items.GroupBy(i => i.Item.Name);
        foreach (var group in grouped.Where(g => g.Count() > 1))
        {
            var items = group.Select(g => g.Item).ToList();
            for (int i = 0; i < items.Count - 1; i++)
            {
                Assert.NotSame(items[i], items[i + 1]);
            }
        }
    }
}
