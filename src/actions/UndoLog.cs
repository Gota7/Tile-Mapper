

namespace Tile_Mapper
{
    // Class to list the actions that are undoable.
    public class UndoLog
    {

        // Circular queue to store edits to be undone.
        private EditAction[] _actionLog;

        // Pointers in queue to the start, end, and current location.
        private int _top;
        private int _bottom;
        private int _index;

        // Constructor which takes the max size of the undo log.
        public UndoLog(int maxSize)
        {
            _actionLog = new EditAction[maxSize];
            
            _top = 0;
            _bottom = 0;
            _index = 0;
        }

        // Method to add to the undo queue.
        public void AddAction(EditAction action)
        {
            _actionLog[_index] = action;

            _index = (_index + 1) % _actionLog.Length;
            _top = _index;

            // Check if size limit reached.

            if (_top == _bottom)
            {
                _bottom = (_bottom + 1) % _actionLog.Length;
            }
        }

        // Method to undo from the queue.
        public void Undo()
        {
            // Check if there is action to be undone.
            if (_index != _bottom)
            {
                _index = (_index + _actionLog.Length - 1) % _actionLog.Length;
                _actionLog[_index].Undo();
            }
        }

        // Method to redo from the queue.
        public void Redo()
        {
            // Check if there is action to be redone
            if (_index != _top)
            {
                _actionLog[_index].Do();
                _index = (_index + 1) % _actionLog.Length;
            }
        }
    }
}
