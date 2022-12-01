using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;
using Tile_Mapper;

namespace TileMapper.UI
{

    public class Canvas : GraphicsWindow
    {

        private int _mapHeight, _mapWidth;

        private int _unitSize;

        private TileMap _tMap;

        private TileSelector _ts;

        private MapAction _currentAction;
        private UndoLog _actionLog;

        private int _trueWidth, _trueHeight;
        private float _currentWidth, _currentHeight;

        // Scale = currernt/true.
        private float _scaleX, _scaleY;

        private bool _sizeSet = false;

        private int _windowPadding, _windowPaddingTop;


        // The width and height should be that of the tile map.
        public Canvas(TileSelector ts, TileMap tMap) : base(0, 0)
        {

            this._tMap = tMap;
            this._ts = ts;

            _mapWidth = _tMap.GetRows();
            _mapHeight = _tMap.GetCols();
            _unitSize = _tMap.GetUnitWidth();
            _trueWidth = _mapWidth * _unitSize;
            _trueHeight = _mapHeight * _unitSize;

            _currentAction = new PlaceAction();
            _actionLog = new UndoLog(20);

            this.ResizeRenderTarget(_trueWidth, _trueHeight);
        }

        public override void DrawUI()
        {

            GetWindowPadding();

            // Canvas is 600x600, other window elements are 8x27x8x8 found from GetWindowContentRegionMin.

            if (!_sizeSet)
            {
                ImGui.SetNextWindowSize(new Vector2(_trueWidth + 2 * _windowPadding, _trueHeight + _windowPaddingTop + _windowPadding));
                _sizeSet = true;
            }

            ImGui.Begin("Canvas", ref _open);

            var size = ImGui.GetContentRegionAvail();

            _currentWidth = size.X;
            _currentHeight = size.Y;
            _scaleX = _currentWidth / _trueWidth;
            _scaleY = _currentHeight / _trueHeight;

            //Console.WriteLine(_currentWidth + " " + _currentWidth + " " + _scaleX + " " + _scaleY + " " + _trueWidth + " " + _trueHeight);

            if (ImGui.BeginMainMenuBar())
            {
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
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }

            // Draw target.
            DrawRenderTarget((int)_currentWidth, (int)_currentHeight);

            ImGui.End();
        }

        public override void Update()
        {
            // Determine where mouse is.
            Vector2 windowPos = ImGui.GetWindowPos();
            Vector2 mousePos = ImGui.GetMousePos();

            int x = (int)mousePos.X - (int)windowPos.X - _windowPadding;
            int y = (int)mousePos.Y - (int)windowPos.Y - _windowPaddingTop;

            x = (int)(x / (_unitSize * _scaleX));
            y = (int)(y / (_unitSize * _scaleY));

            //Check if coordinates valid
            if(x >= 0 && x < _tMap.GetRows()
                && y >= 0 && y < _tMap.GetCols())
            {
                _currentAction.Update((uint)x, (uint)y, _tMap.GetCurrentLayer(), _ts.GetTileSelected());

                if(_currentAction.CanGenerate())
                {
                    _actionLog.AddAction(_currentAction.GenerateAction());
                }
            }
        }

        // Draw according to trueSize not current.
        protected override void Draw()
        {

            Raylib.ClearBackground(Color.DARKBLUE);

            // Draw each layer.
            for (int k = 0; k < _tMap.GetLayerCount(); k++)
            {

                TileLayer layer = _tMap.GetLayer(k);
                var set = _tMap.NameToSet(layer.TileSet);

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
                            float scale = (float)_unitSize / set.TileWidth;
                            set.Draw(i * _unitSize, j * _unitSize, (uint)t, scale);
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
                    Raylib.DrawRectangleLinesEx(new Rectangle(i * _unitSize, j * _unitSize, _unitSize, _unitSize), 0.5f, Color.BLACK);
                }
            }
        }

        private void GetWindowPadding()
        {
            var v = ImGui.GetWindowContentRegionMin();
            _windowPadding = (int)v.X;
            _windowPaddingTop = (int)v.Y;
        }

    }
}