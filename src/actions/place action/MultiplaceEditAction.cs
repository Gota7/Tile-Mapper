

using TileMapper;

namespace TileMapper
{
    // Class to contain the edit for placing multiple tiles.
    public class MultiplaceEditAction : EditAction
    {
        // Variables.

        private PlaceEditAction[] _singlePlacements;

        // Constructor for edit.
        public MultiplaceEditAction(List<PlaceEditAction> singlePlacements)
        {
            _singlePlacements = singlePlacements.ToArray();
        }

        // Places the tiles at set locations.
        public void Do()
        {
            for (int i = 0; i < _singlePlacements.Length; i++)
            {
                _singlePlacements[i].Do();
            }
        }

        // Sets the index of the locations to before tiles were placed.
        public void Undo()
        {
            for (int i = 0; i < _singlePlacements.Length; i++)
            {
                _singlePlacements[i].Undo();
            }
        }
    }
}
