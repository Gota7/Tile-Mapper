

using TileMapper;

namespace Tile_Mapper
{
    // Class to contain the edit for placing a single tile.
    public class PlaceEditAction : EditAction
    {
        // Variables.

        private TileLayer _layer;

        private uint _x;
        private uint _y;

        private int _prevIndex;
        private int _nextIndex;

        // Constructor for edit.
        public PlaceEditAction(TileLayer layer, uint x, uint y, int prevIndex, int nextIndex)
        {
            _layer = layer;

            _x = x;
            _y = y;

            _prevIndex = prevIndex;
            _nextIndex = nextIndex;
        }

        // Places the tile at a set location.
        public void Do()
        {
            _layer.SetTile(_x, _y, _nextIndex);
        }

        // Sets the index of the location to before tile was placed.
        public void Undo()
        {
            _layer.SetTile(_x, _y, _prevIndex);
        }
    }
}
