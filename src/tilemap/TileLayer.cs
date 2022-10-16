using System.Threading.Tasks.Dataflow;

namespace Tile_Mapper.tilemap
{
    //Class to store and manipulate information related to tile layers
    public class TileLayer
    {
        //Variables

        private string tileSet;
        private int[,] tilePlacements;

        //Constructor for new layer
        public TileLayer(uint rows, uint columns, string tileSet)
        {
            this.tileSet = tileSet;
            this.tilePlacements = new int[rows,columns];

            //Initialize a blank layer
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    this.tilePlacements[i,j] = -1;
                }
            }
        }

        //Constructor for loaded layer
        public TileLayer(int[,] tiles, string tileSet)
        {
            this.tileSet = tileSet;
            this.tilePlacements = tiles;
        }

        //Method to get tile at location
        public int GetTile(uint x, uint y)
        {
            return this.tilePlacements[x,y];
        }

        //Method to set tile at location
        public void SetTile(uint x, uint y, int newId)
        {
            this.tilePlacements[x, y] = newId;
        }

        //Method to change the tile set for the layer
        public void SetTileSet(string tileSet)
        {
            this.tileSet = tileSet;
        }

        //Method to resize the layer
        public void Resize(uint rows, uint columns)
        {
            int[,] newTiles = new int[rows,columns];

            int oldRows = this.tilePlacements.GetLength(0);
            int oldCols = this.tilePlacements.GetLength(1);

            //Migrate current Layer
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if(i < oldRows && j < oldCols)
                    {
                        newTiles[i, j] = this.tilePlacements[i, j];
                    }
                    else
                    {
                        newTiles[i,j] = -1;
                    }
                }
            }

            this.tilePlacements = newTiles;
        }

        //Method to draw the layer at a location
        public void Draw(float x, float y, float scale)
        {
            throw new NotImplementedException();
        }
    }
}
