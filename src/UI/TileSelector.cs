using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper
{

    public class TileSelector : GraphicsWindow
    {

        public static int TilesPerRow = 5;

        public static int UnitSize = 50;

        public static int TileGap = 20, RowGap = 10;

        private int _trueWidth, _trueHeight;
        private float _currentWidth, _currentHeight;

        // Scale = currernt/true.
        private float _scaleX, _scaleY;

        // Number of rows.
        private int _rowNum;

        private Tile _TileSelcted;

        private int[] _tileList;

        private TileSet _set = null;

        private bool _sizeSet = false;

        private int _windowPadding, _windowPaddingTop;

        public TileSelector() : base(0, 0)
        {

            // var dim = _set.GetTileDimensions();
            // uint tileNum = dim.Item1*dim.Item2;
            // _tileList = new int[tileNum];

            // _rowNum = (int)Math.Ceiling((double)tileNum/TilesPerRow);

            // _trueWidth = TilesPerRow*UnitSize + (TilesPerRow-1)*TileGap;
            // _trueHeight = _rowNum*UnitSize + (_rowNum-1)*RowGap;
            // this.ResizeRenderTarget(_trueWidth, _trueHeight);

            // for (uint i = 0, count = 0; i < dim.Item1; i++) {
            //     for (uint j = 0; j < dim.Item2; j++) {
            //         _tileList[count] = (int)_set.GetID(i,j);
            //         count++;
            //     }
            // }

            _TileSelcted = new Tile("", -1);
        }

        public override void DrawUI()
        {

            GetWindowPadding();

            if (!_sizeSet)
            {
                ImGui.SetNextWindowSize(new Vector2(_trueWidth + 2*_windowPadding, _trueHeight + _windowPaddingTop + _windowPadding));
                _sizeSet = true;
            }

            ImGui.Begin("Tile Selector");

            var size = ImGui.GetContentRegionAvail();

            _currentWidth = size.X;
            _currentHeight = size.Y;
            _scaleX = _currentWidth / _trueWidth;
            _scaleY = _currentHeight / _trueHeight;

            // TileSelector click, tile selection.
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered())
            {

                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - _windowPadding;
                int y = (int)mousePos.Y - (int)windowPos.Y - _windowPaddingTop;

                x = (int)(x / ((UnitSize + TileGap) * _scaleX));
                y = (int)(y / ((UnitSize + RowGap) * _scaleY));

                if (x >= 0 && y >= 0)
                {
                    int tileIndex = x + y * TilesPerRow;
                    _TileSelcted.Id = _tileList[tileIndex];
                    _TileSelcted.TileSet = _set.Name;
                }

            }

            DrawRenderTarget((int)_currentWidth, (int)_currentHeight);

            ImGui.End();
        }

        public override void Update()
        {

        }

        protected override void Draw()
        {

            // Null check.
            if (_set == null)
                return;

            Raylib.ClearBackground(Color.DARKBLUE);

            float scale = (float)UnitSize / _set.TileWidth;
            int col = 0, row = 0;

            for (int i = 0; i < _tileList.Length; i++)
            {

                _set.Draw(row * UnitSize + row * TileGap, col * UnitSize + RowGap * col, (uint)_tileList[i], scale);

                // Draw border around selected tile.
                if (_TileSelcted.Id == _tileList[i])
                    Raylib.DrawRectangleLinesEx(new Rectangle(row * UnitSize + row * TileGap, col * UnitSize + RowGap * col, UnitSize, UnitSize), 2f, Color.BLACK);

                row++;
                if (row >= TilesPerRow)
                {
                    col++;
                    row = 0;
                }
            }
        }

        public Tile GetTileSelected()
        {
            return _TileSelcted;
        }

        public void ChangeTileSet(TileSet ts)
        {

            _set = ts;

            var dim = _set.GetTileDimensions();
            uint tileNum = dim.Item1 * dim.Item2;
            _tileList = new int[tileNum];

            _rowNum = (int)Math.Ceiling((double)tileNum / TilesPerRow);

            _trueWidth = TilesPerRow * UnitSize + (TilesPerRow - 1) * TileGap;
            _trueHeight = _rowNum * UnitSize + (_rowNum - 1) * RowGap;
            this.ResizeRenderTarget(_trueWidth, _trueHeight);
            _sizeSet = false;

            for (uint i = 0, count = 0; i < dim.Item1; i++)
            {
                for (uint j = 0; j < dim.Item2; j++)
                {
                    _tileList[count] = (int)_set.GetID(i, j);
                    count++;
                }
            }

            // Uncomment these lines if you want selected Tile to reset when changing TileSets.
            // _TileSelcted.TileSet = "";
            // _TileSelcted.Id = -1;
        }

        private void GetWindowPadding()
        {
            var v = ImGui.GetWindowContentRegionMin();
            _windowPadding = (int)v.X;
            _windowPaddingTop = (int)v.Y;
        }

    }
}