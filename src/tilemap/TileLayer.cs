using System.Threading.Tasks.Dataflow;

namespace TileMapper
{
    // Class to store and manipulate information related to tile .
    public class TileLayer
    {
        // Variables.

        private Tile[,] _tilePlacements;

        // Constructor for new layer.
        public TileLayer(uint rows, uint columns)
        {
            this._tilePlacements = new Tile[rows,columns];

            // Initialize a blank layer.
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this._tilePlacements[i,j] = new Tile("", -1);
                }
            }
        }

        // Constructor for loaded layer.
        public TileLayer(Tile[,] tiles)
        {
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
        public Tile GetTile(uint x, uint y)
        {
            return this._tilePlacements[x,y];
        }

        // Method to set tile at location.
        public void SetTile(uint x, uint y, int newId, String newTileSet)
        {
            this._tilePlacements[x, y].Id = newId;
            this._tilePlacements[x, y].TileSet = newTileSet;
        }

        // Method to resize the layer.
        public void Resize(uint rows, uint columns)
        {
            Tile[,] newTiles = new Tile[rows,columns];

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
                        this._tilePlacements[i,j] = new Tile("", -1);
                    }
                }
            }

            this._tilePlacements = newTiles;
        }
    }
}
