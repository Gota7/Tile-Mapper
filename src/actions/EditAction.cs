

namespace Tile_Mapper
{
    // Interface for the doing/undoing of an action.
    public interface EditAction
    {
        // Method to do the action forwards.
        public void Do();

        // Method to undo the action.
        public void Undo();
    }
}
