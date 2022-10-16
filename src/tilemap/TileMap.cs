namespace Tile_Mapper.tilemap
{
    //Class to store and manipulate information related to current tile map
    public class TileMap
    {
        //Variables

        string filePath;

        private ushort rows;
        private ushort columns;

        private ushort tileWidth;
        private ushort tileHeight;

        private List<TileLayer> layers;
        private int currentLayer;

        //Constructor for new map
        public TileMap()
        {
            this.filePath = "";
            this.currentLayer = -1;

            this.rows = 100;
            this.columns = 100;
        }

        //Constructor that loads from file
        public TileMap(string filePath)
        {
            this.filePath = filePath;

            throw new NotImplementedException();
        }

        //Method to retrieve a layer for a given index
        public TileLayer GetLayer(int index)
        {
            return index >= this.layers.Count ? null : this.layers[index];
        }

        //Method to get the current layer
        public TileLayer GetCurrentLayer()
        {
            return this.layers[this.currentLayer];
        }

        //Method to change the current layer by index
        public void SetCurrentLayer(int layerIndex)
        {
            this.currentLayer = layerIndex;
        }

        //Method to add a new layer to the map
        public TileLayer AddLayer(string tileSet)
        {
            TileLayer newLayer = new TileLayer(rows,columns,tileSet);

            this.layers.Add(newLayer);
            this.currentLayer = layers.Count - 1;

            return newLayer;
        }

        //Method to remove layer by index
        public void DeleteLayer(int index)
        {
            this.layers.RemoveAt(index);
        }

        //Method to remove layer by class
        public void DeleteLayer(TileLayer layer)
        {
            this.layers.Remove(layer);
        }

        //Method to swap the index of two layers
        public void SwapLayers(int firstIndex, int secondIndex)
        {
            //Ensuring the two layers are valid
            if(firstIndex < this.layers.Count && secondIndex < this.layers.Count)
            {
                TileLayer temp = this.layers[firstIndex];
                this.layers[firstIndex] = this.layers[secondIndex];
                this.layers[secondIndex] = temp;
            }
        }

        //Method to return the number of layers
        public int GetLayerCount()
        {
            return this.layers.Count;
        }

        //Method to resize the numbers of rows and columns
        public void Resize(ushort newRows, ushort newCol)
        {
            this.rows = newRows;
            this.columns = newCol;

            //Resize each layer
            foreach(TileLayer layer in this.layers)
            {
                layer.Resize(newRows,newCol);
            }
        }

        //Method to resize tile pixels
        public void SetUnitSize(ushort newWidth, ushort newHeight)
        {
            this.tileWidth = newWidth;
            this.tileHeight = newHeight;
        }

        //Method to save current TileMap to a new file path
        public void Save(string filePath)
        {
            this.filePath = filePath;

            this.Save();
        }

        //Method to save to stored file path
        public void Save()
        {
            throw new NotImplementedException();
        }

        //Method to draw the map at a given position
        public void Draw(float x, float y, float scale)
        {
            throw new NotImplementedException();
        }
    }
}
