using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;
using TileMapper.Windowing;

namespace TileMapper.UI
{

    // Delegate type for drawing tileset editor menu.
    public delegate void TSEditorDrawMenuFunc(Core core);

    // An editor for tilesets.
    public class TSEditor : GraphicsWindow
    {

        // Default size if null.
        const int DEFAULT_SIZE = 500;

        // Current tileset.
        public TileSet CurrTileSet = null;

        // Scale of the tileset.
        private float _scale = 1.0f;

        // Last size.
        private Vector2 _lastSize = new Vector2(DEFAULT_SIZE, DEFAULT_SIZE);

        // For selecting files.
        private FileDialog _fd = null;

        // Callback for executing the menu.
        private TSEditorDrawMenuFunc _drawMenuFunc;
        private Core _core;

        // Make a new tileset editor.
        public TSEditor(TSEditorDrawMenuFunc drawMenuFunc, Core core) : base(DEFAULT_SIZE, DEFAULT_SIZE)
        {
            _drawMenuFunc = drawMenuFunc;
            _core = core;
        }

        public override unsafe void DrawUI()
        {
            Vector2 size = CurrTileSet == null ? new Vector2(DEFAULT_SIZE, DEFAULT_SIZE) : CurrTileSet.TextureSize;
            size *= _scale;
            if (ImGui.Begin("Tilset Editor", ImGuiWindowFlags.AlwaysHorizontalScrollbar | ImGuiWindowFlags.AlwaysVerticalScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.MenuBar))
            {
                if (CurrTileSet != null)
                {
                    _drawMenuFunc(_core);
                    ImGui.InputText("Name", ref CurrTileSet.Name, 10000);
                    rlImGui.Tooltip("Internal name of the tile to be associated with layers.");
                    fixed (ushort* ptr = &CurrTileSet.TileWidth) ImGui.InputScalar("Tile Width", ImGuiDataType.U16, (nint)ptr);
                    rlImGui.Tooltip("Width of each tile in pixels.");
                    fixed (ushort* ptr = &CurrTileSet.TileHeight) ImGui.InputScalar("Tile Height", ImGuiDataType.U16, (nint)ptr);
                    rlImGui.Tooltip("Height of each tile in pixels.");
                    fixed (ushort* ptr = &CurrTileSet.TilePaddingX) ImGui.InputScalar("Tile Padding X", ImGuiDataType.U16, (nint)ptr);
                    rlImGui.Tooltip("Padding between each tile horizontally in pixels.");
                    fixed (ushort* ptr = &CurrTileSet.TilePaddingY) ImGui.InputScalar("Tile Padding Y", ImGuiDataType.U16, (nint)ptr);
                    rlImGui.Tooltip("Padding between each tile vertically in pixels.");
                    fixed (ushort* ptr = &CurrTileSet.TileInitialSpacingX) ImGui.InputScalar("Tile Initial Spacing X", ImGuiDataType.U16, (nint)ptr);
                    rlImGui.Tooltip("Padding to begin the tile set with from the left in pixels.");
                    fixed (ushort* ptr = &CurrTileSet.TileInitialSpacingY) ImGui.InputScalar("Tile Initial Spacing Y", ImGuiDataType.U16, (nint)ptr);
                    rlImGui.Tooltip("Padding to begin the tile set with from the top in pixels.");
                    if (ImGui.Button("Change Image")) ChangeImage();
                    ImGui.InputFloat("View Scale", ref _scale, 0.1f, 0.5f);
                }
                DrawRenderTarget((int)size.X, (int)size.Y);
                ImGui.End();
            }
            if (_fd != null) // Check for any images selected.
            {
                _fd.DrawUI();
                if (!_fd.Open && !_fd.SelectedItem.Equals(""))
                {
                    ChangeImage(_fd.SelectedItem);
                    _fd = null;
                }
            }
        }

        protected override void Draw()
        {
            if (CurrTileSet == null)
            {
                Raylib.ClearBackground(Color.BLACK);
            }
            else
            {
                Raylib.DrawTexture(CurrTileSet.Texture, 0, 0, Color.WHITE);
                int currX = CurrTileSet.TileInitialSpacingX;
                int currY = CurrTileSet.TileInitialSpacingY;
                var dims = CurrTileSet.GetTileDimensions();
                for (uint i = 0; i <= dims.Item1; i++)
                {
                    Raylib.DrawLine(currX, 0, currX, (int)_lastSize.Y, Color.RED);
                    currX += CurrTileSet.TileWidth;
                    if (CurrTileSet.TilePaddingX != 0) Raylib.DrawLine(currX, 0, currX, (int)_lastSize.Y, Color.RED);
                    currX += CurrTileSet.TilePaddingX;
                }
                for (uint i = 0; i <= dims.Item2; i++)
                {
                    Raylib.DrawLine(0, currY, (int)_lastSize.X, currY, Color.RED);
                    currY += CurrTileSet.TileHeight;
                    if (CurrTileSet.TilePaddingY != 0) Raylib.DrawLine(0, currY, (int)_lastSize.X, currY, Color.RED);
                    currY += CurrTileSet.TilePaddingY;
                }
            }
        }

        public override void Update()
        {
            Vector2 size = CurrTileSet == null ? new Vector2(DEFAULT_SIZE, DEFAULT_SIZE) : CurrTileSet.TextureSize;
            if (size != _lastSize)
            {
                ResizeRenderTarget((int)size.X, (int)size.Y);
                _lastSize = size;
            }
        }

        private void ChangeImage()
        {
            _fd = new FileDialog("TSEditor", FileDialogMode.OpenFile, "Images|*.png;*.jpg;*.gif");
        }

        private void ChangeImage(string image)
        {
            if (CurrTileSet != null) CurrTileSet.ChangeImage(image);
        }

    }

}