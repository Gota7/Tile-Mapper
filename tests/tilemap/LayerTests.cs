using TileMapper;
using Xunit;

namespace TileMapTests
{
    public class LayerTests
    {

        // Testing LYR-1 and LYR-2 in functional testing document.
        [Fact]
        public void TestTilePlacement()
        {
            TileLayer layer = new TileLayer(5, 5, "TestSet");
            uint x = 1;
            uint y = 2;
            int newTile = 3;

            // Ensuring default value is -1.
            Assert.Equal(-1, layer.GetTile(x, y));

            // Testing the changing and retrieval of a tile.

            layer.SetTile(x, y, newTile);

            Assert.Equal(newTile, layer.GetTile(x, y));
        }

        // Testing LYR-3 and LYR-4 in functional testing document.
        [Fact]
        public void TestOOBOperations()
        {
            TileLayer layer = new TileLayer(5, 5, "TestSet");
            uint x = 6;
            uint y = 2;

            // Testing LYR-3.
            Assert.Throws<System.IndexOutOfRangeException>(() => layer.SetTile(x, y, 3));

            // Testing LYR-4.
            Assert.Throws<System.IndexOutOfRangeException>(() => layer.GetTile(x, y));
        }

        // Testing LYR-5 and LYR-6 in functional testing document.
        [Fact]
        public void TestTileSet()
        {
            string initialSet = "TestSet";
            string newSet = "TestSet2";

            TileLayer layer = new TileLayer(5, 5, initialSet);

            // Testing LYR-6.

            Assert.Equal(initialSet, layer.TileSet);

            // Testing the changing and retrieval of a tile(LYR-5 and LYR-6).

            layer.TileSet = newSet;

            Assert.Equal(newSet, layer.TileSet);
        }

        // Testing the resize method.
        [Fact]
        public void TestResize()
        {
            ushort initialRows = 3;
            ushort newRows = 5;
            ushort initialCols = 4;
            ushort newCols = 2;

            uint x = 1;
            uint y = 1;
            int tile = 2;

            TileLayer layer = new TileLayer(initialRows, initialCols, "TestSet1");

            // Checking initial map size.

            Assert.Equal(initialRows, layer.GetRows());
            Assert.Equal(initialCols, layer.GetCols());

            // Changing tile to test later.

            layer.SetTile(x, y, tile);

            Assert.Equal(tile, layer.GetTile(x, y));

            //Checking resized size.

            layer.Resize(newRows, newCols);

            Assert.Equal(newRows, layer.GetRows());
            Assert.Equal(newCols, layer.GetCols());

            // Checking previously added tile and tiles added by increased width.

            Assert.Equal(tile, layer.GetTile(x, y));
            Assert.Equal(-1, layer.GetTile(4, 0));
        }
    }
}