using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;
using TileMapper.Windowing;

namespace WindowTest
{

    // Test graphics window to implement a graphics window.
    public class TestGraphicsWindow : GraphicsWindow
    {

        // Last size for reference.
        private Vector2 _lastSize = Vector2.Zero;

        // If to resize the canvas on window resize.
        private bool _resizeCanvas = false;

        // Set only if canvas needs to resize.
        private Vector2? _newCanvasSize;

        public TestGraphicsWindow() : base(100, 50) {}

        public override void DrawUI()
        {
            if (_open && ImGui.Begin("Demo Graphics!", ref _open))
            {

                // Get how much room is available.
                ImGui.Checkbox("Resize Canvas On Window Resize", ref _resizeCanvas);
                var size = ImGui.GetContentRegionAvail();

                // Draw target.
                DrawRenderTarget((int)size.X, (int)size.Y);

                // Resize if allowed to. Make sure to do this after drawing so we draw the properly sized version.
                if (_resizeCanvas && !size.Equals(_lastSize)) _newCanvasSize = size;
                _lastSize = size;
                ImGui.End();

            }
        }

        protected override void Draw()
        {
            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawText("Hello World!", 5, 5, 12, Color.RAYWHITE);
        }

        public override void Update()
        {
            if (_newCanvasSize.HasValue)
            {
                ResizeRenderTarget((int)_newCanvasSize.Value.X, (int)_newCanvasSize.Value.Y);
                _newCanvasSize = null;
            }
        }

    }

}