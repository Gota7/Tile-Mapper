using System.Threading.Tasks.Dataflow;

namespace TileMapper
{
    // Class to store and manipulate information related to tile .
    public class TileLayer
    {
        // Variables.

        private string _tileSet;
        private int[,] _tilePlacements;

        // Constructor for new layer.
        public TileLayer(uint rows, uint columns, string tileSet)
        {
            this._tileSet = tileSet;
            this._tilePlacements = new int[rows,columns];

            // Initialize a blank layer.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this._tilePlacements[i,j] = -1;
                }
            }
        }

        // Constructor for loaded layer.
        public TileLayer(int[,] tiles, string tileSet)
        {
            this._tileSet = tileSet;
            this._tilePlacements = tiles;
        }

        // Method to retrieve the number of columns in layer.
        public ushort GetCols()
        {
            return (ushort)this._tilePlacements.GetLength(1);
        }

        // Method to retrieve the number of rows in layer.
        public ushort GetRows()
        {
            return (ushort)this._tilePlacements.GetLength(0);
        }

        // Method to get tile at location.
        public int GetTile(uint x, uint y)
        {
            return this._tilePlacements[x,y];
        }

        // Method to set tile at location.
        public void SetTile(uint x, uint y, int newId)
        {
            this._tilePlacements[x, y] = newId;
        }

        // Method to change the tile set for the layer.
        public string GetTileSet()
        {
            return this._tileSet;
        }

        // Method to change the tile set for the layer.
        public void SetTileSet(string tileSet)
        {
            this._tileSet = tileSet;
        }

        // Method to resize the layer.
        public void Resize(uint rows, uint columns)
        {
            int[,] newTiles = new int[rows,columns];

            int oldRows = this._tilePlacements.GetLength(0);
            int oldCols = this._tilePlacements.GetLength(1);

            // Migrate current Layer.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if(i < oldRows && j < oldCols)
                    {
                        newTiles[i, j] = this._tilePlacements[i, j];
                    }
                    else
                    {
                        newTiles[i,j] = -1;
                    }
                }
            }

            this._tilePlacements = newTiles;
        }
    }
}
