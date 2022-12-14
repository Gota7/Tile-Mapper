using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;
using rlImGui_cs;
using System.Reflection.Emit;

namespace TileMapper.UI
{

    public class Canvas : GraphicsWindow
    {

        private int _mapHeight, _mapWidth;

        public TileMap TileMap { get; private set; }

        private TileSelector _ts;

        private MapAction _currentAction;
        private UndoLog _actionLog;

        private int[,] _copiedSubarray;

        private int _trueWidth, _trueHeight;
        private float _currentWidth, _currentHeight;

        // Scale = currernt/true.
        private float _scaleX, _scaleY;

        private bool _sizeSet = false;

        private static int _windowPadding, _windowPaddingTop;
        private string _windowName = "";
        private static int _newTileMapNumber = 1;

        private ushort _resizeX;
        private ushort _resizeY;

        private FileDialog _fd = null;


        // The width and height should be that of the tile map.
        public Canvas(TileSelector ts, TileMap tMap) : base(0, 0)
        {

            this.TileMap = tMap;
            this._ts = ts;
            ResetSize();
            GetWindowPadding();

            _currentAction = new PlaceAction();
            _copiedSubarray = null;
        }


        private void ResetSize()
        {
            _mapWidth = _resizeX = TileMap.GetRows();
            _mapHeight = _resizeY = TileMap.GetCols();
            _trueWidth = _mapWidth * TileMap.TileWidth;
            _trueHeight = _mapHeight * TileMap.TileHeight;
            this.ResizeRenderTarget(_trueWidth, _trueHeight);

            _actionLog = new UndoLog(20);
        }

        public override unsafe void DrawUI()
        {

            // Canvas is 600x600, other window elements are 8x27x8x8 found from GetWindowContentRegionMin.

            if (!_sizeSet)
            {
                ImGui.SetNextWindowSize(new Vector2(_trueWidth + 2 * _windowPadding, _trueHeight + _windowPaddingTop * 2));
                _sizeSet = true;
            }

            string windowName = Path.GetFileNameWithoutExtension(TileMap.Path);
            if (windowName.Equals("")) windowName = _windowName;
            if (windowName.Equals("")) windowName = "New Tilemap " + _newTileMapNumber++;
            _windowName = windowName;
            if (ImGui.Begin(windowName, ImGuiWindowFlags.MenuBar))
            {

                // Menu bar.

                if (ImGui.BeginMenuBar())
                {

                    if (ImGui.BeginMenu("File"))
                    {
                        if (ImGui.MenuItem("Save")) Save();
                        if (ImGui.MenuItem("Save As")) SaveAs();
                        if (ImGui.MenuItem("Close")) Close();
                        if (ImGui.MenuItem("Delete")) Delete();

                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Edit"))
                    {
                        if (ImGui.MenuItem("Undo"))
                        {
                            // Interupting to ensure undo will not mess with the action.
                            _currentAction.Interrupt();

                            if (_currentAction.CanGenerate())
                            {
                                _actionLog.AddAction(_currentAction.GenerateAction());
                            }
                            _actionLog.Undo();
                        }
                        if (ImGui.MenuItem("Redo"))
                        {
                            _currentAction.Interrupt();

                            if (_currentAction.CanGenerate())
                            {
                                _actionLog.AddAction(_currentAction.GenerateAction());
                            }
                            _actionLog.Redo();
                        }

                        if(ImGui.MenuItem("Copy") && _currentAction is SelectAction)
                        {
                            _copiedSubarray = ((SelectAction)_currentAction).GetSelection(TileMap.GetCurrentLayer());
                        }
                        if(ImGui.MenuItem("Paste") && _copiedSubarray != null)
                        {
                            _currentAction.Interrupt();

                            if (_currentAction.CanGenerate())
                            {
                                _actionLog.AddAction(_currentAction.GenerateAction());
                            }

                            SelectAction newAction = new SelectAction();
                            newAction.Paste(TileMap.GetCurrentLayer(),_copiedSubarray);
                            _currentAction = newAction;
                        }

                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Actions"))
                    {
                        MapAction newAction = null;

                        if (ImGui.RadioButton("Place", _currentAction is PlaceAction)) newAction = new PlaceAction();
                        if (ImGui.RadioButton("Select", _currentAction is SelectAction)) newAction = new SelectAction();
                        if (ImGui.RadioButton("Fill", _currentAction is FillAction)) newAction = new FillAction();
                        if (ImGui.RadioButton("Pipet", _currentAction is PipetAction)) newAction = new PipetAction(_ts);

                        if(newAction != null)
                        {
                            _currentAction.Interrupt();

                            if (_currentAction.CanGenerate())
                            {
                                _actionLog.AddAction(_currentAction.GenerateAction());
                            }

                            _currentAction = newAction;
                        }

                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();

                    if (ImGui.BeginTabBar("Tabs", ImGuiTabBarFlags.Reorderable))
                    {

                        // Tilemap properties.
                        if (ImGui.BeginTabItem("Properties"))
                        {
                            fixed (ushort* ptr = &TileMap.TileWidth) if (ImGui.InputScalar("Tile Width", ImGuiDataType.U16, (nint)ptr))
                                {
                                    if (TileMap.TileWidth == 0) TileMap.TileWidth = 1; // Divide by 0 error otherwise.
                                    ResetSize();
                                }
                            rlImGui.Tooltip("Width of each tile in pixels.");
                            fixed (ushort* ptr = &TileMap.TileHeight) if (ImGui.InputScalar("Tile Height", ImGuiDataType.U16, (nint)ptr))
                                {
                                    if (TileMap.TileHeight == 0) TileMap.TileHeight = 1; // Divide by 0 error otherwise.
                                    ResetSize();
                                }
                            rlImGui.Tooltip("Height of each tile in pixels.");
                            fixed (ushort* ptr = &_resizeX) if (ImGui.InputScalar("Width", ImGuiDataType.U16, (nint)ptr))
                                    if (_resizeX == 0) _resizeX = 1;
                            fixed (ushort* ptr = &_resizeY) if (ImGui.InputScalar("Height", ImGuiDataType.U16, (nint)ptr))
                                    if (_resizeY == 0) _resizeY = 1;
                            if (ImGui.Button("Resize"))
                            {
                                TileMap.Resize(_resizeX, _resizeY);
                                ResetSize();
                            }
                            rlImGui.Tooltip("Resize the tile map to the new width and height provided.");
                            ImGui.EndTabItem();
                        }

                        // Layer tab.
                        if (ImGui.BeginTabItem("Layers"))
                        {
                            if (TileMap.GetCurrentLayerIndex() != -1) ImGui.InputText("Tileset", ref TileMap.GetCurrentLayer().TileSet, 5000);
                            if (ImGui.BeginTable("Tilesets", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.NoSavedSettings | ImGuiTableFlags.Borders))
                            {
                                for (int i = 0; i < TileMap.GetLayerCount(); i++)
                                {
                                    ImGui.TableNextRow();
                                    ImGui.TableNextColumn();
                                    bool curr = TileMap.GetCurrentLayerIndex() == i;
                                    if (ImGui.Selectable(TileMap.GetLayer(i).TileSet + (curr ? " *" : "") + "##" + i))
                                    {
                                        TileMap.SetCurrentLayer(i);
                                    }
                                    ImGui.TableNextColumn();
                                    if (i > 0)
                                    {
                                        if (ImGui.Button("Move Up##" + i)) TileMap.SwapLayers(i - 1, i);
                                        ImGui.SameLine();
                                    }
                                    if (i < TileMap.GetLayerCount() - 1)
                                    {
                                        if (ImGui.Button("Move Down##" + i)) TileMap.SwapLayers(i, i + 1);
                                        ImGui.SameLine();
                                    }
                                    if (ImGui.Button("-##" + i))
                                    {
                                        TileMap.DeleteLayer(i);
                                        break;
                                    }
                                }
                                ImGui.EndTable();
                            }
                            if (ImGui.Button("Add Layer"))
                            {
                                TileMap.AddLayer("TilesetHere");
                            }
                            ImGui.EndTabItem();
                        }

                        var currPos = ImGui.GetCursorPos();

                        // Canvas.
                        if (ImGui.BeginTabItem("Canvas"))
                        {

                            var size = ImGui.GetContentRegionAvail();

                            _currentWidth = size.X;
                            _currentHeight = size.Y;
                            _scaleX = _currentWidth / _trueWidth;
                            _scaleY = _currentHeight / _trueHeight;

                            //Console.WriteLine(_currentWidth + " " + _currentWidth + " " + _scaleX + " " + _scaleY + " " + _trueWidth + " " + _trueHeight);

                            if (ImGui.IsWindowFocused() && TileMap.GetCurrentLayer() != null)
                            {

                                Vector2 windowPos = ImGui.GetWindowPos();
                                Vector2 mousePos = ImGui.GetMousePos();

                                int x = (int)(mousePos.X - windowPos.X - currPos.X);
                                int y = (int)(mousePos.Y - windowPos.Y - currPos.Y);

                                x = (int)(x / (TileMap.TileWidth * _scaleX));
                                y = (int)(y / (TileMap.TileHeight * _scaleY));

                                //Check if coordinates valid
                                if (x >= 0 && x < TileMap.GetRows()
                                    && y >= 0 && y < TileMap.GetCols())
                                {
                                    try
                                    {
                                        _currentAction.Update((uint)x, (uint)y, TileMap.GetCurrentLayer(), _ts.GetTileSelected());

                                        if (_currentAction.CanGenerate())
                                        {
                                            _actionLog.AddAction(_currentAction.GenerateAction());
                                        }
                                    }
                                    catch { }
                                }

                            }

                            // Draw target.
                            ImGui.InvisibleButton("NoDrag", new Vector2(_currentWidth, _currentHeight));
                            ImGui.SetCursorPos(currPos); // Prevent dragging with invisible button and put cursor back in place.
                            DrawRenderTarget((int)_currentWidth, (int)_currentHeight);
                            ImGui.EndTabItem();

                        }

                    }
                    ImGui.EndTabBar();
                }
                ImGui.End();
            }
            DoSaveAs();
        }

        // Draw according to trueSize not current.
        protected override void Draw()
        {

            Raylib.ClearBackground(Color.DARKBLUE);

            // Draw each layer.
            for (int k = 0; k < TileMap.GetLayerCount(); k++)
            {

                TileLayer layer = TileMap.GetLayer(k);
                var set = TileMap.NameToSet(layer.TileSet);

                for (uint i = 0; i < _mapWidth; i++)
                {
                    for (uint j = 0; j < _mapHeight; j++)
                    {

                        int t = layer.GetTile(i, j);

                        // Null check.
                        if (set == null)
                            continue;

                        if (t != -1)
                        {
                            //Raylib.DrawRectangle(i*_unitSize,j*_unitSize,_unitSize,_unitSize,Color.DARKGREEN);
                            float scaleX = (float)TileMap.TileWidth / set.TileWidth;
                            float scaleY = (float)TileMap.TileHeight / set.TileHeight;
                            set.Draw(i * TileMap.TileWidth, j * TileMap.TileHeight, (uint)t, scaleX, scaleY);
                        }
                    }
                }
            }

            DrawTileBorders();
        }

        // Draws the tile borders.
        private void DrawTileBorders()
        {
            for (uint i = 0; i < _mapWidth; i++)
            {
                for (uint j = 0; j < _mapHeight; j++)
                {
                    // Draw tile borders.
                    Raylib.DrawRectangleLinesEx(new Rectangle(i * TileMap.TileWidth, j * TileMap.TileHeight, TileMap.TileWidth, TileMap.TileHeight), 0.5f, Color.BLACK);
                }
            }

            // Draw border of selection.
            if (_currentAction is SelectAction)
            {
                int[,] bounds = ((SelectAction)_currentAction).GetBounds();

                if (bounds != null)
                {
                    int minX = Math.Max(0, bounds[0, 0]);
                    int minY = Math.Max(0, bounds[1, 0]);

                    int maxX = Math.Min(TileMap.GetRows() - 1, bounds[0, 1]);
                    int maxY = Math.Min(TileMap.GetCols() - 1, bounds[1, 1]);

                    float x = minX * TileMap.TileWidth;
                    float y = minY * TileMap.TileHeight;

                    float width = (1 + maxX - minX) * TileMap.TileWidth;
                    float height = (1 + maxY - minY) * TileMap.TileHeight;

                    Raylib.DrawRectangleLinesEx(new Rectangle(x, y, width, height), 1f, Color.RED);
                }
            }
        }

        private void GetWindowPadding()
        {
            var v = ImGui.GetStyle().WindowPadding + ImGui.GetStyle().FramePadding;
            _windowPadding = (int)v.X;
            _windowPaddingTop = (int)v.Y;
        }

        private void Save()
        {
            if (TileMap.Path.Equals("")) SaveAs();
            else TileMap.Save();
        }

        private void SaveAs()
        {
            _fd = new FileDialog("Canvas", FileDialogMode.SaveFile, "Tile-Mapper Tilemap|*.tmm");
        }

        private void DoSaveAs()
        {
            if (_fd != null)
            {
                _fd.DrawUI();
                if (!_fd.Open && !_fd.SelectedItem.Equals(""))
                {
                    TileMap.Save(_fd.SelectedItem);
                    _fd = null;
                } else if (!_fd.Open) _fd = null;
            }
        }

        // Delete the map.
        public void Delete()
        {
            if (File.Exists(TileMap.Path)) File.Delete(TileMap.Path);
            Close();
        }

    }
}