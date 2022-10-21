using Raylib_cs;

namespace TileMapper
{

    // Details of a tileset that change behavior.
    public partial class TileSet : IDisposable
    {

        // Name of the tileset.
        public string Name;

        // How wide each tile is.
        public ushort TileWidth = 0x10;

        // How tall each tile is.
        public ushort TileHeight = 0x10;

        // Padding between each tile horizontally.
        public ushort TilePaddingX = 0;

        // Padding between each tile vertically.
        public ushort TilePaddingY = 0;

        // Initial spacing at the start horizontally.
        public ushort TileInitialSpacingX = 0;

        // Initial spacing at the start vertically.
        public ushort TileInitialSpacingY = 0;

        // Image data extension.
        private string _imageExt;

        // Image data for the tileset.
        private byte[] _imageData;

        // Texture data for the tileset. Convenient to have both so saving the tileset doesn't involve copying data from the GPU.
        private Texture2D _texture;

        // Create a new tileset from an image.
        public TileSet(string imagePath, string name)
        {
            Name = name;
            _imageExt = Path.GetExtension(imagePath);
            _imageData = File.ReadAllBytes(imagePath);
            _texture = Raylib.LoadTexture(imagePath);
        }

        // Change the image of the tileset.
        public void ChangeImage(string imagePath)
        {
            Raylib.UnloadTexture(_texture);
            _imageData = File.ReadAllBytes(imagePath);
            _texture = Raylib.LoadTexture(imagePath);
        }

        // Cleanup all resources used by tileset.
        public void Dispose()
        {
            Raylib.UnloadTexture(_texture);
        }

    }


}