using PbvCompressor;

namespace TileMapper
{
    // Class to store and manipulate information related to current tile map.
    public class TileMap
    {
        // Variables.

        private string _filePath = "";
        public string Path => _filePath;

        private ushort _rows;
        private ushort _columns;

        public ushort TileWidth;
        public ushort TileHeight;

        private List<TileLayer> _layers = new List<TileLayer>();
        private int _currentLayer = -1;

        // Find TileSets from name, used by canvas to draw correct tile.
        private Dictionary<String, TileSet> _nameToSet = new Dictionary<string, TileSet>();

        // Constructor for new map.
        public TileMap(ushort rows, ushort columns, ushort tileWidth, ushort tileHeight)
        {
            this._rows = rows;
            this._columns = columns;
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;
        }

        // Constructor that loads from file.
        public TileMap(string filePath)
        {
            this._filePath = filePath;

            // Read data and uncompress.
            using (FileStream fileIn = new FileStream(filePath, FileMode.Open))
            {
                if (fileIn.ReadByte() != 'T' || fileIn.ReadByte() != 'M' || fileIn.ReadByte() != 'M')
                    throw new Exception("Can not load tileset " + filePath + ". Not a Tile-Mapper Tilemap.");
                fileIn.ReadByte(); // Skip version.
                using (MemoryStream uncompressed = new MemoryStream())
                {
                    PbvCompressorLZW compressor = new PbvCompressorLZW();
                    compressor.Decompress(fileIn, uncompressed);
                    uncompressed.Seek(0, SeekOrigin.Begin); // Decompress file and start at beginning of data.
                    using (BinaryReader r = new BinaryReader(uncompressed))
                    {
                        _rows = r.ReadUInt16();
                        _columns = r.ReadUInt16();
                        TileWidth = r.ReadUInt16();
                        TileHeight = r.ReadUInt16();
                        int numLayers = r.ReadInt32();
                        _layers = new List<TileLayer>();
                        for (int i = 0; i < numLayers; i++)
                        {
                            string tileset = r.ReadString();
                            int[,] tilePlacements = new int[_rows, _columns];
                            for (int x = 0; x < _rows; x++)
                            {
                                for (int y = 0; y < _columns; y++)
                                {
                                    tilePlacements[x, y] = r.ReadInt32();
                                }
                            }
                            _layers.Add(new TileLayer(tileset, tilePlacements));
                        }
                    }
                }
            }
        }

        // Method to retrieve the number of columns in map.
        public ushort GetCols()
        {
            return this._columns;
        }

        // Method to retrieve the number of rows in map.
        public ushort GetRows()
        {
            return this._rows;
        }

        // Method to retrieve a layer for a given index.
        public TileLayer GetLayer(int index)
        {
            if (index < this.GetLayerCount() && index >= 0)
            {
                return this._layers[index];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        // Get the index of the current layer.
        public int GetCurrentLayerIndex()
        {
            return _currentLayer;
        }

        // Method to get the current layer.
        public TileLayer GetCurrentLayer()
        {
            if (_currentLayer == -1) return null;
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
            TileLayer newLayer = new TileLayer(this._rows, this._columns, tileSet);

            this._layers.Add(newLayer);
            this._currentLayer = this._layers.Count - 1;

            return newLayer;
        }

        // Method to remove layer by index.
        public void DeleteLayer(int index)
        {
            if (index < this.GetLayerCount() && index >= 0)
            {
                this._layers.RemoveAt(index);
                if (this._currentLayer >= this.GetLayerCount())
                {
                    this.SetCurrentLayer(this.GetLayerCount() - 1);
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
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
            if(firstIndex < this.GetLayerCount() && secondIndex < this.GetLayerCount() && firstIndex >= 0 && secondIndex >= 0)
            {
                TileLayer temp = this._layers[firstIndex];
                this._layers[firstIndex] = this._layers[secondIndex];
                this._layers[secondIndex] = temp;

                if(this._currentLayer == firstIndex)
                {
                    this._currentLayer = secondIndex;
                }
                else if(this._currentLayer == secondIndex)
                {
                    this._currentLayer = firstIndex;
                }
            }
            else
            {
                throw new IndexOutOfRangeException();
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

        // Method to save current TileMap to a new file path.
        public void Save(string filePath)
        {
            this._filePath = filePath;
            this.Save();
        }

        // Method to save to stored file path.
        public void Save()
        {
            using (FileStream fileOut = new FileStream(_filePath, FileMode.Create))
            {
                using (MemoryStream uncompressed = new MemoryStream())
                {
                    using (BinaryWriter w = new BinaryWriter(uncompressed))
                    {
                        w.Write(_rows);
                        w.Write(_columns);
                        w.Write(TileWidth);
                        w.Write(TileHeight);
                        w.Write(_layers.Count);
                        foreach (var layer in _layers)
                        {
                            layer.Write(w);
                        }
                        fileOut.WriteByte((byte)'T');
                        fileOut.WriteByte((byte)'M');
                        fileOut.WriteByte((byte)'M');
                        fileOut.WriteByte(0); // Write out header and version.
                        PbvCompressorLZW compressor = new PbvCompressorLZW();
                        uncompressed.Seek(0, SeekOrigin.Begin);
                        compressor.Compress(uncompressed, fileOut); // Seek to beginning of uncompressed data stream and write out compressed data.
                    }
                }
            }
        }

        public void AddTileSet(TileSet ts)
        {
            if (!_nameToSet.ContainsKey(ts.Name)) _nameToSet.Add(ts.Name, ts);
        }

        public void RemoveTileSet(TileSet ts)
        {
            if (_nameToSet.ContainsKey(ts.Name)) _nameToSet.Remove(ts.Name);
        }

        // Given a name, a TileSet with that name will be returned.
        public TileSet NameToSet(String name)
        {
            try
            {
                return _nameToSet[name];
            }
            catch
            {
                return null;
            }
        }

    }
}
