using Raylib_cs;
using rlImGui_cs;

namespace TileMapper.Windowing
{

    // Window that renders to a texture that can be drawn.
    // For understanding how to use the RenderTarget property see: https://github.com/raylib-extras/rlImGui-cs/blob/07d3387ba3ed5b8d025218ed0a3b61eb1a373f5d/editor_example/ImageViewerWindow.cs#L56
    public abstract class GraphicsWindow : Window
    {

        // Render target to draw to.
        protected RenderTexture2D RenderTarget;

        // Create a new graphics window.
        public GraphicsWindow(int renderTargetWidth, int renderTargetHeight)
        {
            RenderTarget = Raylib.LoadRenderTexture(renderTargetWidth, renderTargetHeight);
        }

        // Draw the render target. Call this in your UI code.
        public void DrawRenderTarget(int width, int height)
        {
            Rectangle viewRect = new Rectangle();
            viewRect.x = 0;
            viewRect.y = 0;
            viewRect.width = RenderTarget.texture.width;
            viewRect.height = -RenderTarget.texture.height;
            rlImGui.ImageRect(RenderTarget.texture, width, height, viewRect);
        }

        // Resize the render target. Do this if the window has been resized and you wish the target to be expanded.
        public void ResizeRenderTarget(int newWidth, int newHeight)
        {
            Raylib.UnloadRenderTexture(RenderTarget);
            RenderTarget = Raylib.LoadRenderTexture(newWidth, newHeight);
        }

        // Draw what is needed to the render target.
        protected abstract void Draw();

        // Handle drawing.
        public void DoDraw()
        {
            Raylib.BeginTextureMode(RenderTarget);
            Draw();
            Raylib.EndTextureMode();
        }

        // Close the graphics window.
        public override void Close()
        {

            // We have to make sure to free texture resources from the GPU and properly close the window.
            Raylib.UnloadRenderTexture(RenderTarget);
            base.Close();

        }

    }

}