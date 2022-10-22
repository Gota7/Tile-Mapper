namespace TileMapper
{
    // Class to store and manipulate information related to current tile map.
    public class TileMap
    {
        // Variables.

        private string _filePath;

        private ushort _rows;
        private ushort _columns;

        private ushort _tileWidth;
        private ushort _tileHeight;

        private List<TileLayer> _layers;
        private int _currentLayer;

        // Constructor for new map.
        public TileMap()
        {
            this._filePath = "";
            this._currentLayer = -1;

            this._rows = 100;
            this._columns = 100;
        }

        // Constructor that loads from file.
        public TileMap(string filePath)
        {
            this._filePath = filePath;

            throw new NotImplementedException();
        }

        // Method to retrieve a layer for a given index.
        public TileLayer GetLayer(int index)
        {
            return index >= this._layers.Count ? null : this._layers[index];
        }

        // Method to get the current layer.
        public TileLayer GetCurrentLayer()
        {
            return this._layers[this._currentLayer];
        }

        // Method to change the current layer by index.
        public void SetCurrentLayer(int layerIndex)
        {
            this._currentLayer = layerIndex;
        }

        // Method to add a new layer to the map.
        public TileLayer AddLayer(string tileSet)
        {
            TileLayer newLayer = new TileLayer(this._rows, this._columns,tileSet);

            this._layers.Add(newLayer);
            this._currentLayer = this._layers.Count - 1;

            return newLayer;
        }

        // Method to remove layer by index.
        public void DeleteLayer(int index)
        {
            this._layers.RemoveAt(index);
        }

        // Method to remove layer by class.
        public void DeleteLayer(TileLayer layer)
        {
            this._layers.Remove(layer);
        }

        // Method to swap the index of two layers.
        public void SwapLayers(int firstIndex, int secondIndex)
        {
            // Ensuring the two layers are valid.
            if(firstIndex < this._layers.Count && secondIndex < this._layers.Count)
            {
                TileLayer temp = this._layers[firstIndex];
                this._layers[firstIndex] = this._layers[secondIndex];
                this._layers[secondIndex] = temp;
            }
        }

        // Method to return the number of layers.
        public int GetLayerCount()
        {
            return this._layers.Count;
        }

        // Method to resize the numbers of rows and columns.
        public void Resize(ushort newRows, ushort newCol)
        {
            this._rows = newRows;
            this._columns = newCol;

            // Resize each layer.
            foreach(TileLayer layer in this._layers)
            {
                layer.Resize(newRows,newCol);
            }
        }

        // Method to resize tile pixels.
        public void SetUnitSize(ushort newWidth, ushort newHeight)
        {
            this._tileWidth = newWidth;
            this._tileHeight = newHeight;
        }

        // Method to save current TileMap to a new file path.
        public void Save(string filePath)
        {
            this._filePath = filePath;

            this.Save();
        }

        // Method to save to stored file path.
        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
