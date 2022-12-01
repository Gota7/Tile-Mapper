

namespace TileMapper
{
    // Interface to genericise generating an edit object for an action.
    public interface MapAction
    {
        // Method to generate an edit object for an action.
        public EditAction GenerateAction();

        // Method to see if action can be generated.
        public bool CanGenerate();

        // Method to interrupt the action, such as an undo happening.
        public void Interrupt();

        // Method to pass information to the action to updat the current layer.
        public void Update(uint x, uint y, TileLayer layer, int tile);
    }
}
