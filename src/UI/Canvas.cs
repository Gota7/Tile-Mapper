using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper
{

    public class Canvas : GraphicsWindow
    {

        private int _mapHeight, _mapWidth;

        private int _unitSize;

        private TileMap _tMap;

        private TileLayer _currentLayer;

        private TileSelector _ts;

        private TileSet _set = null;

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

            ImGui.Begin("Canvas", ref _open);

            var size = ImGui.GetContentRegionAvail();

            _currentWidth = size.X;
            _currentHeight = size.Y;
            _scaleX = _currentWidth / _trueWidth;
            _scaleY = _currentHeight / _trueHeight;

            //Console.WriteLine(_currentWidth + " " + _currentWidth + " " + _scaleX + " " + _scaleY + " " + _trueWidth + " " + _trueHeight);

            _currentLayer = _tMap.GetCurrentLayer();

            // Canvas click, tile placement.
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {

                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - _windowPadding;
                int y = (int)mousePos.Y - (int)windowPos.Y - _windowPaddingTop;

                //Console.WriteLine(x + " : " + y);

                x = (int)(x / (_unitSize * _scaleX));
                y = (int)(y / (_unitSize * _scaleY));



                try
                {
                    Tile t = _ts.GetTileSelected();
                    _currentLayer.SetTile((uint)x, (uint)y, t.Id, t.TileSet);
                }
                catch (Exception e) { }

            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - 8;
                int y = (int)mousePos.Y - (int)windowPos.Y - 27;

                //Console.WriteLine(x + " : " + y);

                x = (int)(x / (_unitSize * _scaleX));
                y = (int)(y / (_unitSize * _scaleY));

                try
                {
                    _currentLayer.SetTile((uint)x, (uint)y, -1, "");
                }
                catch (Exception e) { }
            }

            // Draw target.
            DrawRenderTarget((int)_currentWidth, (int)_currentHeight);

            ImGui.End();
        }

        public override void Update()
        {
        }

        // Draw according to trueSize not current.
        protected override void Draw()
        {

            Raylib.ClearBackground(Color.DARKBLUE);

            // Draw each layer.
            for (int k = 0; k < _tMap.GetLayerCount(); k++)
            {

                TileLayer layer = _tMap.GetLayer(k);

                for (uint i = 0; i < _mapWidth; i++)
                {
                    for (uint j = 0; j < _mapHeight; j++)
                    {

                        Tile t = layer.GetTile(i, j);

                        _set = _tMap.NameToSet(t.TileSet);

                        // Null check.
                        if (_set == null)
                            continue;

                        if (t.Id != -1)
                        {
                            //Raylib.DrawRectangle(i*_unitSize,j*_unitSize,_unitSize,_unitSize,Color.DARKGREEN);
                            float scale = (float)_unitSize / _set.TileWidth;
                            _set.Draw(i * _unitSize, j * _unitSize, (uint)t.Id, scale);
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