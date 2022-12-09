

using ImGuiNET;

namespace TileMapper
{

    // Action to fill area with the same tile.
    internal class FillAction : MapAction
    {
        // Variables.

        private List<PlaceEditAction> _singlePlacements;

        // Constructor.
        public FillAction()
        {
            _singlePlacements = new List<PlaceEditAction>();
        }

        // Returns true after fill.
        public bool CanGenerate()
        {
            return _singlePlacements.Count > 0;
        }

        // Returns the ties edited by the fill.
        public EditAction GenerateAction()
        {
            MultiplaceEditAction newEdit = new MultiplaceEditAction(_singlePlacements);

            _singlePlacements.Clear();

            return newEdit;
        }

        // No behavior needed as no continuous actions.
        public void Interrupt()
        {
        }

        // Uses depth-first search to determine area to fill.
        public void Update(uint x, uint y, TileLayer layer, int tile)
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                if (layer.GetTile(x, y) != tile)
                {
                    FillArea(x, y, layer, tile, layer.GetTile(x,y));
                }
            }
            else if(ImGui.IsMouseDown(ImGuiMouseButton.Right))
            {
                if (layer.GetTile(x, y) != -1)
                {
                    FillArea(x, y, layer, -1, layer.GetTile(x, y));
                }
            }
        }

        // Depth-first search method which replaces the tile along the way.
        private void FillArea(uint x, uint y, TileLayer layer, int tile, int initialTile)
        {
            // Replace tile.
            _singlePlacements.Add(new PlaceEditAction(layer, x, y, layer.GetTile(x, y), tile));

            layer.SetTile(x, y, tile);

            // Check adjacent tiles.
            if(x > 0 && layer.GetTile(x-1, y) == initialTile)
            {
                FillArea(x-1, y, layer, tile, initialTile);
            }

            if(y > 0 && layer.GetTile(x, y-1) == initialTile)
            {
                FillArea(x, y-1, layer, tile, initialTile);
            }

            if (x < layer.GetRows() - 1 && layer.GetTile(x+1, y) == initialTile)
            {
                FillArea(x+1, y, layer, tile, initialTile);
            }

            if (y < layer.GetCols() - 1 && layer.GetTile(x, y+1) == initialTile)
            {
                FillArea(x, y+1, layer, tile, initialTile);
            }
        }
    }
}
