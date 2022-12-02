using Raylib_cs;

namespace TileMapper
{

    // Details of a tileset that change behavior.
    public partial class TileSet
    {

        // Calculate the tile dimensions in the X and Y directions.
        public Tuple<uint, uint> GetTileDimensions()
        {

            // X size.
            uint availableWidth = (uint)_texture.width - TileInitialSpacingX;
            uint numTilesX = availableWidth / ((uint)TileWidth + TilePaddingX);
            if ((availableWidth % (TileWidth + TilePaddingX)) >= TileWidth) numTilesX++;

            // Y size.
            uint availableHeight = (uint)_texture.height - TileInitialSpacingY;
            uint numTilesY = availableHeight / ((uint)TileHeight + TilePaddingY);
            if ((availableHeight % (TileHeight + TilePaddingY)) >= TileHeight) numTilesY++;

            // Put it all together.
            return new Tuple<uint, uint>(numTilesX, numTilesY);

        }

        // Get source rectangle from tile ID.
        private Rectangle IdToSourceRectangle(uint id)
        {

            // Get X and Y positions of tile.
            var dimensions = GetTileDimensions();
            uint x = id % (uint)dimensions.Item1;
            uint y = id / (uint)dimensions.Item1;

            // Check for invalid ID.
            if (y >= dimensions.Item2) return new Rectangle(-1.0f, 0, 0, 0);

            // Get source rectangle positions.
            float offX = x * (TileWidth + TilePaddingX) + TileInitialSpacingX;
            float offY = y * (TileHeight + TilePaddingY) + TileInitialSpacingY;
            return new Rectangle(offX, offY, TileWidth, TileHeight);

        }

        // Get ID from a tile position in the grid.
        public uint GetID(uint x, uint y)
        {
            return y * GetTileDimensions().Item1 + x;
        }

        // Draw a tile to a given position.
        public void Draw(float x, float y, uint id, float scaleX = 1.0f, float scaleY = 1.0f)
        {

            // Get source rectange and check if invalid.
            var src = IdToSourceRectangle(id);
            if (src.x == -1.0f) return;

            // Draw otherwise.
            Raylib.DrawTexturePro(_texture, src, new Rectangle(x, y, TileWidth * scaleX, TileHeight * scaleY), System.Numerics.Vector2.Zero, 0.0f, Color.WHITE);

        }

    }

}