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
            TileMap map = new TileMap();
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
            TileMap map = new TileMap();

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
            TileMap map = new TileMap();
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
            TileMap map = new TileMap();
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
    }
}