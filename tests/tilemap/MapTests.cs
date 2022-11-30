using System.IO;
using TileMapper;
using Xunit;

namespace TileMapTests
{
    public class MapTests
    {

        // Testing MAP-1 and MAP-3 in functional testing document.
        [Fact]
        public void TestAddLayer()
        {
            TileMap map = new TileMap(10, 10, 10, 10);
            TileLayer layer;

            Assert.Equal(0, map.GetLayerCount());

            layer = map.AddLayer("TestSet");

            // Ensuring added layer selected for MAP-1.
            Assert.Equal(map.GetCurrentLayer(), layer);
            Assert.Equal(1, map.GetLayerCount());

            // Testing retrival by index for MAP-3.
            Assert.Equal(map.GetLayer(0), layer);
        }

        // Testing MAP-2, MAP-4, and MAP-6 in functional testing document.
        [Fact]
        public void TestOOBOperations()
        {
            TileMap map = new TileMap(10, 10, 10, 10);

            // Testing MAP-2.
            Assert.Throws<System.IndexOutOfRangeException>(() => map.GetLayer(0));

            // Testing MAP-4.
            Assert.Throws<System.IndexOutOfRangeException>(() => map.DeleteLayer(0));

            // Testing MAP-6.
            Assert.Throws<System.IndexOutOfRangeException>(() => map.SwapLayers(0,1));
        }

        // Testing MAP-5 and MAP-7 in functional testing document.
        [Fact]
        public void TestReorderLayer()
        {
            TileMap map = new TileMap(10, 10, 10, 10);
            TileLayer layer1 = map.AddLayer("TestSet1");
            TileLayer layer2 = map.AddLayer("TestSet2");

            // Ensuring added layer selected for MAP-1.
            Assert.Equal(map.GetCurrentLayer(), layer2);

            // Retesting retrival by index for MAP-3.
            Assert.Equal(map.GetLayer(0), layer1);

            // Testing MAP-7.
            map.SwapLayers(0, 1);

            Assert.Equal(map.GetCurrentLayer(), layer2);
            Assert.Equal(map.GetLayer(0), layer2);
            Assert.Equal(map.GetLayer(1), layer1);

            // Testing MAP-5 by deleting final layer.

            TileLayer layer3 = map.AddLayer("TestSet3");

            Assert.Equal(map.GetCurrentLayer(), layer3);
            Assert.Equal(3, map.GetLayerCount());

            map.DeleteLayer(2);

            Assert.Equal(map.GetCurrentLayer(), layer1);
            Assert.Equal(2, map.GetLayerCount());

            // Testing MAP-5 by deleting layer that is not the final layer.

            map.SetCurrentLayer(0);

            Assert.Equal(map.GetCurrentLayer(), layer2);

            map.DeleteLayer(map.GetCurrentLayer());

            Assert.Equal(map.GetCurrentLayer(), layer1);
            Assert.Equal(1, map.GetLayerCount());
        }

        // Testing MAP-8 in functional testing document.
        [Fact]
        public void TestResizeLayer()
        {
            TileMap map = new TileMap(10, 10, 10, 10);
            TileLayer layer1 = map.AddLayer("TestSet1");
            TileLayer layer2;

            ushort newRows = (ushort)(map.GetRows() / 2);
            ushort newCols = (ushort)(map.GetCols() * 2);

            map.Resize(newRows, newCols);

            // Checking map size.

            Assert.Equal(newRows, map.GetRows());
            Assert.Equal(newCols, map.GetCols());

            //Checking layer size.

            Assert.Equal(newRows, layer1.GetRows());
            Assert.Equal(newCols, layer1.GetCols());

            // Checking size of new layer matches.

            layer2 = map.AddLayer("TestSet2");

            Assert.Equal(newRows, layer2.GetRows());
            Assert.Equal(newCols, layer2.GetCols());
        }

        // Test saving and loading.
        [Fact]
        public void TestSaveLoad()
        {

            // Save.
            TileMap map = new TileMap(10, 12, 11, 13);
            var layer1 = map.AddLayer("Layer1");
            var layer2 = map.AddLayer("Layer2");
            layer1.SetTile(3, 3, 7);
            layer2.SetTile(7, 7, 3);
            map.Save("Dummy.tmm");

            // Test properties load.
            TileMap lm = new TileMap("Dummy.tmm");
            Assert.Equal(10, lm.GetRows());
            Assert.Equal(12, lm.GetCols());
            Assert.Equal(11, lm.GetUnitWidth());
            Assert.Equal(13, lm.GetUnitHeight());
            Assert.Equal("Layer1", lm.GetLayer(0).TileSet);
            Assert.Equal("Layer2", lm.GetLayer(1).TileSet);
            Assert.Equal(7, lm.GetLayer(0).GetTile(3, 3));
            Assert.Equal(3, lm.GetLayer(1).GetTile(7, 7));
            lm.Save("Dummy2.tmm");

            // Check equality of files.
            byte[] t1 = File.ReadAllBytes("Dummy.tmm");
            byte[] t2 = File.ReadAllBytes("Dummy2.tmm");
            Assert.Equal(t1, t2);

        }

    }
}