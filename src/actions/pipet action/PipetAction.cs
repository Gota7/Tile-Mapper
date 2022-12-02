

namespace TileMapper
{
    // Action to select the tile at a location for use by other actions.
    internal class PipetAction : MapAction
    {
        // No actions are generated.
        public bool CanGenerate()
        {
            return false;
        }

        // No actions are generated;
        public EditAction GenerateAction()
        {
            return null;
        }

        // Nothing to interupt.
        public void Interrupt()
        {
            
        }

        // If mouse pressed, set the selected tile to the given tile
        public void Update(uint x, uint y, TileLayer layer, int tile)
        {
            throw new NotImplementedException();
        }
    }
}
