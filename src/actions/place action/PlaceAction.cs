

using ImGuiNET;
using TileMapper;

namespace Tile_Mapper
{
    // Action to place a single tile at a location.
    public class PlaceAction : MapAction
    {
        // Variables.

        private bool _startedPlacing;
        private bool _finishedPlacing;

        private bool _isRemoving;

        private List<PlaceEditAction> _singlePlacements;

        // Constructor.
        public PlaceAction()
        {
            _startedPlacing = false;
            _finishedPlacing = false;

            _isRemoving = false;

            _singlePlacements = new List<PlaceEditAction>();
        }

        // Returns true when mouse released.
        public bool CanGenerate()
        {
            return _finishedPlacing;
        }

        // Groups all tile changes into single undoable action.
        public EditAction GenerateAction()
        {
            EditAction groupedPlacements = new MultiplaceEditAction(_singlePlacements);

            _singlePlacements.Clear();

            _startedPlacing = false;
            _finishedPlacing = false;

            return groupedPlacements;
        }

        // Finishes group of placements.
        public void Interrupt()
        {
            _finishedPlacing = _singlePlacements.Count > 0;
        }

        // Places or removes tiles when mouse held.
        public void Update(uint x, uint y, TileLayer layer, int tile)
        {
            // Canvas click, tile placement.

            if (ImGui.IsMouseDown(ImGuiMouseButton.Left)
                && (!_startedPlacing || !_isRemoving))
            {
                _startedPlacing = true;
                _isRemoving = false;

                if(layer.GetTile(x, y) != tile)
                {
                    _singlePlacements.Add(new PlaceEditAction(layer, x, y, layer.GetTile(x, y), tile));

                    layer.SetTile(x, y, tile);
                }
            }
            else if (ImGui.IsMouseDown(ImGuiMouseButton.Right)
                && (!_startedPlacing || _isRemoving))
            {
                _startedPlacing = true;
                _isRemoving = true;

                if (layer.GetTile(x, y) != -1)
                {
                    _singlePlacements.Add(new PlaceEditAction(layer, x, y, layer.GetTile(x, y), -1));

                    layer.SetTile(x, y, -1);
                }
            }
            else if (_singlePlacements.Count > 0)
            {
                _finishedPlacing = true;
            }
        }
    }
}
