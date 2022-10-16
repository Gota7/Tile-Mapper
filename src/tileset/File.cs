using System.Text;
using Raylib_cs;

namespace TileMapper
{

    // Stores tiles from an image.
    public partial class TileSet
    {

        // Load a tileset from a file path and load all data required.
        public unsafe TileSet(string filePath)
        {

            // Create binary reader input stream.
            using (FileStream fileIn = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader r = new BinaryReader(fileIn))
                {

                    // Read generic properties.
                    if (!new string(r.ReadChars(3)).Equals("TMS"))
                    {
                        throw new Exception("Can not load tileset " + filePath + ". Not a Tile-Mapper Tileset.");
                    }
                    byte version = r.ReadByte();
                    if (version != 0) throw new Exception("Can not load tileset " + filePath + ". Unsupported format version " + version);
                    Name = r.ReadString();
                    TileWidth = r.ReadUInt16();
                    TileHeight = r.ReadUInt16();
                    TilePaddingX = r.ReadUInt16();
                    TilePaddingY = r.ReadUInt16();
                    TileInitialSpacingX = r.ReadUInt16();
                    TileInitialSpacingY = r.ReadUInt16();

                    // Read the image data.
                    _imageExt = r.ReadString();
                    _imageData = r.ReadBytes(r.ReadInt32());
                    byte[] extBytes = Encoding.ASCII.GetBytes(_imageExt);
                    fixed (byte* fileData = _imageData)
                    {
                        fixed (byte* ext = extBytes)
                        {
                            Image img = Raylib.LoadImageFromMemory((sbyte*)ext, fileData, _imageData.Length);
                            _texture = Raylib.LoadTextureFromImage(img);
                            Raylib.UnloadImage(img);
                        }
                    }

                }
            }

        }

        // Save the tileset to a given path.
        public void Save(string filePath)
        {

            // Create binary writer output stream.
            using (FileStream fileOut = new FileStream(filePath, FileMode.Create))
            {
                using (BinaryWriter w = new BinaryWriter(fileOut))
                {

                    // Save generic properties. TMT signature for Tile-Mapper Set, version 0.
                    w.Write("TMS".ToCharArray());
                    w.Write((byte)0);
                    w.Write(Name);
                    w.Write(TileWidth);
                    w.Write(TileHeight);
                    w.Write(TilePaddingX);
                    w.Write(TilePaddingY);
                    w.Write(TileInitialSpacingX);
                    w.Write(TileInitialSpacingY);

                    // Write the image.
                    w.Write(_imageExt);
                    w.Write(_imageData.Length);
                    w.Write(_imageData);

                }
            }

        }

    }

}