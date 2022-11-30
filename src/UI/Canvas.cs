using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper.UI
{

    public class Canvas : GraphicsWindow
    {

        private int _mapHeight, _mapWidth;

        private int _unitSize;

        public TileMap TileMap { get; private set; }

        private TileSelector _ts;

        private int _trueWidth, _trueHeight;
        private float _currentWidth, _currentHeight;

        // Scale = currernt/true.
        private float _scaleX, _scaleY;

        private bool _sizeSet = false;

        private int _windowPadding, _windowPaddingTop;


        // The width and height should be that of the tile map.
        public Canvas(TileSelector ts, TileMap tMap) : base(0, 0)
        {

            this.TileMap = tMap;
            this._ts = ts;

            _mapWidth = TileMap.GetRows();
            _mapHeight = TileMap.GetCols();
            _unitSize = TileMap.GetUnitWidth();
            _trueWidth = _mapWidth * _unitSize;
            _trueHeight = _mapHeight * _unitSize;

            this.ResizeRenderTarget(_trueWidth, _trueHeight);
        }

        public override void DrawUI()
        {

            GetWindowPadding();

            // Canvas is 600x600, other window elements are 8x27x8x8 found from GetWindowContentRegionMin.

            if (!_sizeSet)
            {
                ImGui.SetNextWindowSize(new Vector2(_trueWidth + 2*_windowPadding, _trueHeight + _windowPaddingTop + _windowPadding));
                _sizeSet = true;
            }

            if (ImGui.Begin(Path.GetFileNameWithoutExtension(TileMap.Path), ref _open))
            {

                if (ImGui.BeginTabBar("Tabs", ImGuiTabBarFlags.Reorderable))
                {

                    // Layer tab.
                    if (ImGui.BeginTabItem("Layers"))
                    {
                        ImGui.InputText("Tileset", ref TileMap.GetCurrentLayer().TileSet, 5000);
                        for (int i = 0; i < TileMap.GetLayerCount(); i++)
                        {
                            bool curr = TileMap.GetCurrentLayerIndex() == i;
                            if (ImGui.Selectable(TileMap.GetLayer(i).TileSet + (curr ? " *" : "") + "##" + i))
                            {
                                TileMap.SetCurrentLayer(i);
                            }
                        }
                        ImGui.EndTabItem();
                    }

                    // Canvas.
                    if (ImGui.BeginTabItem("Canvas"))
                    {

                        var size = ImGui.GetContentRegionAvail();

                        _currentWidth = size.X;
                        _currentHeight = size.Y;
                        _scaleX = _currentWidth / _trueWidth;
                        _scaleY = _currentHeight / _trueHeight;

                        //Console.WriteLine(_currentWidth + " " + _currentWidth + " " + _scaleX + " " + _scaleY + " " + _trueWidth + " " + _trueHeight);

                        // Canvas click, tile placement.
                        var currPos = ImGui.GetCursorPos();
                        if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && ImGui.IsWindowFocused())
                        {

                            Vector2 windowPos = ImGui.GetWindowPos();
                            Vector2 mousePos = ImGui.GetMousePos();

                            int x = (int)mousePos.X - (int)windowPos.X - _windowPadding;
                            int y = (int)mousePos.Y - (int)windowPos.Y - _windowPaddingTop;
                            if (x >= 0 && y >= 0 && x < _currentWidth && y < _currentHeight)
                            {

                                //Console.WriteLine(x + " : " + y);

                                x = (int)(x / (_unitSize * _scaleX));
                                y = (int)(y / (_unitSize * _scaleY));

                                try
                                {
                                    int t = _ts.GetTileSelected();
                                    TileMap.GetCurrentLayer().SetTile((uint)x, (uint)y, t);
                                }
                                catch { }
                            }

                        }

                        if (ImGui.IsMouseDown(ImGuiMouseButton.Right) && ImGui.IsWindowFocused())
                        {
                            Vector2 windowPos = ImGui.GetWindowPos();
                            Vector2 mousePos = ImGui.GetMousePos();

                            int x = (int)mousePos.X - (int)windowPos.X - _windowPadding;
                            int y = (int)mousePos.Y - (int)windowPos.Y - _windowPaddingTop;
                            if (x >= 0 && y >= 0 && x < _currentWidth && y < _currentHeight)
                            {

                                //Console.WriteLine(x + " : " + y);

                                x = (int)(x / (_unitSize * _scaleX));
                                y = (int)(y / (_unitSize * _scaleY));

                                try
                                {
                                    TileMap.GetCurrentLayer().SetTile((uint)x, (uint)y, -1);
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
                    ImGui.EndTabBar();
                }
                ImGui.End();
            }
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

        public override void Close() {
            base.Close();
        }

    }
}