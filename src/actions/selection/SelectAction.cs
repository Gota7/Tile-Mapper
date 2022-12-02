

using ImGuiNET;
using System.Reflection.Emit;

namespace TileMapper
{
    // Action to select rectangular area of tiles and drag/drop them.
    internal class SelectAction : MapAction
    {

        // Enumerable to help with action state management
        private enum SelectionState
        {
            Idle,
            Pasted, // Second idling state if a selection was just pasted.
            Selecting,
            Dragging,
            Finished // State for when an outgoing edit is ready.
        }

        // Variables.

        private SelectionState _state;

        private int[,] _selection; // 2x2 showing ranges for x and y
        private int[,] _selectedTiles;

        private uint _initialX;
        private uint _initialY;

        private MultiplaceEditAction _removeInitialAction;
        private MultiplaceEditAction _placeDragAction;

        // Constructor.
        public SelectAction()
        {
            _state = SelectionState.Idle;

            _initialX = 0;
            _initialY = 0;

            _selection = null;
            _selectedTiles = null;

            _removeInitialAction = null;
            _placeDragAction = null;
        }

        // Returns true if a drag is finished.
        public bool CanGenerate()
        {
            return _state == SelectionState.Finished;
        }

        // Returns the overall edit done by dragging a selection
        public EditAction GenerateAction()
        {
            EditAction overallAction = null;

            _state = SelectionState.Idle;

            if (_removeInitialAction == null)
            {
                overallAction = _placeDragAction;
            }
            else
            {
                // Merge the two actions into one.

                EditAction[] sequence = { _removeInitialAction, _placeDragAction};

                overallAction = new SequentialEditAction(sequence);
            }

            _removeInitialAction = null;
            _placeDragAction = null;

            _selectedTiles = null;

            return overallAction;
        }

        // Stops the selection and finishes a drag.
        public void Interrupt()
        {
            if (_state == SelectionState.Dragging || _state == SelectionState.Pasted)
            {
                _state = SelectionState.Finished;
            }
            else
            {
                _state = SelectionState.Idle;
            }
        }

        public void Update(uint x, uint y, TileLayer layer, int tile)
        {
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                if (_state == SelectionState.Idle || _state == SelectionState.Pasted)
                {
                    _initialX = x;
                    _initialY = y;

                    // If clicking into selection, start dragging.
                    if (_selection != null
                        && x >= _selection[0, 0] && x <= _selection[0, 1]
                        && y >= _selection[1, 0] && y <= _selection[1, 1])
                    {
                        if (_state != SelectionState.Pasted)
                        {
                            //Remove initial copy and replaces it.

                            _selectedTiles = GetSelection(layer);

                            _removeInitialAction = FillSelection(layer, -1);
                            _placeDragAction = SetSelection(layer, _selectedTiles, _selection[0, 0], _selection[1, 0]);
                        }

                        _state = SelectionState.Dragging;
                    }
                    else if (_state != SelectionState.Pasted)
                    {
                        _state = SelectionState.Selecting;

                        _selection = new int[2, 2];

                        _selection[0, 0] = (int)x;
                        _selection[0, 1] = (int)x;

                        _selection[1, 0] = (int)y;
                        _selection[1, 1] = (int)y;
                    }
                    else
                    {
                        _state = SelectionState.Finished;
                    }
                }
                else if (_state == SelectionState.Selecting)
                {
                    // Stores selection based or range of x and y values.
                    _selection[0, 0] = (int)Math.Min(x, _initialX);
                    _selection[0, 1] = (int)Math.Max(x, _initialX);

                    _selection[1, 0] = (int)Math.Min(y, _initialY);
                    _selection[1, 1] = (int)Math.Max(y, _initialY);
                }
                else if (_state == SelectionState.Dragging)
                {
                    // Shift selection according to change in position then reset initial position.
                    if (x != _initialX || y != _initialY)
                    {
                        int xChange = (int)(x - _initialX);
                        int yChange = (int)(y - _initialY);

                        _selection[0, 0] += xChange;
                        _selection[0, 1] += xChange;

                        _selection[1, 0] += yChange;
                        _selection[1, 1] += yChange;

                        _placeDragAction.Undo();
                        _placeDragAction = SetSelection(layer, _selectedTiles, _selection[0, 0], _selection[1, 0]);

                        _initialX = x;
                        _initialY = y;
                    }
                }
            }
            else if (_state == SelectionState.Selecting)
            {
                _state = SelectionState.Idle;
            }
            else if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && (_state == SelectionState.Dragging || _state == SelectionState.Pasted))
            {
                _state = SelectionState.Finished;
            }
            else if (_state == SelectionState.Dragging)
            {
                _state = SelectionState.Pasted;
            }
            else if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && _state == SelectionState.Idle)
            {
                _placeDragAction = FillSelection(layer, tile);

                if (_placeDragAction != null)
                {
                    _state = SelectionState.Finished;
                }
            }
        }

        // Method to return the bounds of the selection.
        public int[,] GetBounds()
        {
            return _selection;
        }

        // Method to return the subarray with the selected values.
        public int[,] GetSelection(TileLayer layer)
        {
            int[,] subArray = null;

            if(_selection != null)
            {
                uint minX = Math.Max(0, (uint)_selection[0, 0]);
                uint minY = Math.Max(0, (uint)_selection[1, 0]);

                uint maxX = Math.Min((uint)layer.GetRows()-1, (uint)_selection[0, 1]);
                uint maxY = Math.Min((uint)layer.GetCols()-1, (uint)_selection[1, 1]);

                subArray = new int[1 + maxX - minX, 1 + maxY - minY];

                for(uint i = 0; i < subArray.GetLength(0); i++)
                {
                    for(uint j = 0; j < subArray.GetLength(1); j++)
                    {
                        subArray[i, j] = layer.GetTile(minX + i, minY + j);
                    }
                }
            }

            return subArray;
        }

        // Method to paste subarray over layer.
        private MultiplaceEditAction SetSelection(TileLayer layer, int[,] subArray, int x, int y)
        {
            List<PlaceEditAction> singlePlacements = new List<PlaceEditAction>();

            int row;
            int col;

            for (int i = 0; i < subArray.GetLength(0); i++)
            {
                for (int j = 0; j < subArray.GetLength(1); j++)
                {
                    row = i + x;
                    col = j + y;

                    if(subArray[i, j] != -1 && row >= 0 && row < layer.GetRows() && col >= 0 && col < layer.GetCols())
                    {
                        singlePlacements.Add(new PlaceEditAction(layer, (uint)row, (uint)col, layer.GetTile((uint)row, (uint)col), subArray[i,j]));

                        layer.SetTile((uint)row, (uint)col, subArray[i, j]);
                    }
                }
            }

            return new MultiplaceEditAction(singlePlacements);
        }

        // Public method for pasting.
        public void Paste(TileLayer layer, int[,] subArray)
        {
            if(_state == SelectionState.Idle || _state == SelectionState.Selecting)
            {
                _state = SelectionState.Pasted;

                _placeDragAction = SetSelection(layer, subArray, 0, 0);

                _selectedTiles = subArray;

                _selection = new int[2, 2];

                _selection[0, 0] = 0;
                _selection[0, 1] = subArray.GetLength(0) - 1;

                _selection[1, 0] = 0;
                _selection[1, 1] = subArray.GetLength(1) - 1;
            }
        }

        // Method to fill selection with single tile
        private MultiplaceEditAction FillSelection(TileLayer layer, int tile)
        {
            MultiplaceEditAction edit = null;

            if (_selection != null)
            {
                List<PlaceEditAction> singlePlacements = new List<PlaceEditAction>();

                int row;
                int col;

                for (int i = _selection[0, 0]; i <= _selection[0, 1]; i++)
                {
                    for (int j = _selection[1, 0]; j <= _selection[1, 1]; j++)
                    {
                        row = i;
                        col = j;

                        if (row >= 0 && row < layer.GetRows() && col >= 0 && col < layer.GetCols()
                            && layer.GetTile((uint)row, (uint)col) != tile)
                        {
                            singlePlacements.Add(new PlaceEditAction(layer, (uint)row, (uint)col, layer.GetTile((uint)row, (uint)col), tile));

                            layer.SetTile((uint)row, (uint)col, tile);
                        }
                    }
                }

                if (singlePlacements.Count > 0)
                {
                    edit = new MultiplaceEditAction(singlePlacements);
                }
            }

            return edit;
        }
    }
}
