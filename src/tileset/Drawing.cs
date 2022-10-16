using Raylib_cs;

namespace TileMapper
{

    // Details of a tileset that change behavior.
    public partial class TileSet
    {

        // Calculate the tile dimensions in the X and Y directions.
        public Tuple<int, int> GetTileDimensions()
        {
            int availableWidth = _texture.width - TileInitialSpacingX;
            int availableHeight = _texture.height - TileInitialSpacingY;
            return new Tuple<int, int>(availableWidth / (TileWidth + TilePaddingX), availableHeight / (TileHeight + TilePaddingY));
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

        // Draw a tile to a given position.
        public void Draw(float x, float y, uint id, float scale)
        {

            // Get source rectange and check if invalid.
            var src = IdToSourceRectangle(id);
            if (src.x == -1.0f) return;

            // Draw otherwise.
            Raylib.DrawTexturePro(_texture, src, new Rectangle(x, y, TileWidth * scale, TileHeight * scale), System.Numerics.Vector2.Zero, 0.0f, Color.WHITE);

        }

    }

}