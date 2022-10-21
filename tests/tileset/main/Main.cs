using System.IO;
using Raylib_cs;
using TileMapper;
using Xunit;

namespace TilesetTests;

public class MainTests
{

    [Fact]
    public void NewSaveLoad()
    {

        // Have to setup a raylib context in order to load data.
        Raylib.InitWindow(50, 50, "Tileset Test");

        // Read from an image.
        TileSet test = new TileSet("TestTileset.png", "Test Tileset");
        test.TileWidth = 2;
        test.TileHeight = 2;
        test.TileInitialSpacingX = 3;
        test.TileInitialSpacingY = 0;
        test.TilePaddingX = 1;
        test.TilePaddingY = 3;
        test.Save("TestTileset.tms");
        test.Dispose();

        // Read from the file. Save it again so we can compare hex differences.
        test = new TileSet("TestTileset.tms");
        test.Save("TestTileset2.tms");
        test.Dispose();

        // Ensure files are the same.
        byte[] t1 = File.ReadAllBytes("TestTileset.tms");
        byte[] t2 = File.ReadAllBytes("TestTileset2.tms");
        Assert.Equal(t1, t2);

        // Ensure properties match those set.
        Assert.Equal(2, test.TileWidth);
        Assert.Equal(2, test.TileHeight);
        Assert.Equal(3, test.TileInitialSpacingX);
        Assert.Equal(0, test.TileInitialSpacingY);
        Assert.Equal(1, test.TilePaddingX);
        Assert.Equal(3, test.TilePaddingY);

        // Close raylib context.
        Raylib.CloseWindow();

    }

    [Fact]
    public void CheckSize()
    {

        // Have to setup a raylib context in order to load data.
        Raylib.InitWindow(50, 50, "Tileset Test");

        // Read from an image.
        TileSet test = new TileSet("TestTileset.png", "Test Tileset");
        test.TileWidth = 2;
        test.TileHeight = 2;
        test.TileInitialSpacingX = 3;
        test.TileInitialSpacingY = 0;
        test.TilePaddingX = 1;
        test.TilePaddingY = 3;
        Assert.Equal(new System.Tuple<uint, uint>(2, 2), test.GetTileDimensions());
        test.Dispose();

        // Close raylib context.
        Raylib.CloseWindow();

    }

    [Fact]
    public void ChangeImage()
    {

        // Have to setup a raylib context in order to load data.
        Raylib.InitWindow(50, 50, "Tileset Test");

        // Read from an image and change it.
        TileSet test = new TileSet("TestTileset.png", "Test Tileset");
        test.ChangeImage("TestTileset.png");
        test.Dispose();

        // Close raylib context.
        Raylib.CloseWindow();

    }

}