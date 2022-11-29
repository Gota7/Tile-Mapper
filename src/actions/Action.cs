

namespace Tile_Mapper
{
    // Interface to genericise generating an edit object for an action.
    public interface Action
    {
        // Method to generate an edit object for an action.
        public EditAction GenetateAction();
    }
}
