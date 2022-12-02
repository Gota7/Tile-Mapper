namespace TileMapper
{
    // Class to contain the edit for sequential actions.
    public class SequentialEditAction : EditAction
    {
        // Variables.

        private EditAction[] _singleActions;

        // Constructor for edit.
        public SequentialEditAction(EditAction[] singleActions)
        {
            _singleActions = singleActions;
        }

        // Does the actions in order.
        public void Do()
        {
            for (int i = 0; i < _singleActions.Length; i++)
            {
                _singleActions[i].Do();
            }
        }

        // Undoes the actions in reverse.
        public void Undo()
        {
            for (int i = _singleActions.Length - 1; i >= 0; i--)
            {
                _singleActions[i].Undo();
            }
        }
    }
}
