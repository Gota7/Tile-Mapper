using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper {

    public class TileSelector : GraphicsWindow {

        public static int TilesPerRow = 5;
        
        public static int UnitSize = 50;

        public static int TileGap = 20, RowGap = 10;

        int trueWidth, trueHeight;
        float currentWidth, currentHeight;

        float scaleX, scaleY; //scale = currernt/true

        //number of columns
        int rowNum;

        private Tile TileSelcted;

        private int[] tileList;
        
        TileSet set = null; //= new TileSet("./grass.tms");

        bool sizeSet = false;

        public TileSelector() : base(0, 0) { 

            // var dim = set.GetTileDimensions();
            // uint tileNum = dim.Item1*dim.Item2;
            // tileList = new int[tileNum];

            // rowNum = (int)Math.Ceiling((double)tileNum/TilesPerRow);

            // trueWidth = TilesPerRow*UnitSize + (TilesPerRow-1)*TileGap;
            // trueHeight = rowNum*UnitSize + (rowNum-1)*RowGap;
            // this.ResizeRenderTarget(trueWidth, trueHeight);

            // for (uint i = 0, count = 0; i < dim.Item1; i++) {
            //     for (uint j = 0; j < dim.Item2; j++) {
            //         tileList[count] = (int)set.GetID(i,j);
            //         count++;
            //     }
            // }

            TileSelcted = new Tile("", -1);
        }

        public override void DrawUI() {

            if (!sizeSet) {
            ImGui.SetNextWindowSize(new Vector2(trueWidth + 8 + 8, trueHeight + 27 + 8));
            sizeSet = true;
            }

            ImGui.Begin("Tile Selector");

            var size = ImGui.GetContentRegionAvail();

            currentWidth = size.X;
            currentHeight = size.Y;
            scaleX = currentWidth / trueWidth;
            scaleY = currentHeight / trueHeight;

             //TileSelector click, tile selection
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && ImGui.IsWindowHovered()) {

                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 mousePos = ImGui.GetMousePos();

                int x = (int)mousePos.X - (int)windowPos.X - 8;
                int y = (int)mousePos.Y - (int)windowPos.Y - 27;

                x = (int)(x/((UnitSize+TileGap)*scaleX));
                y = (int)(y/((UnitSize+RowGap)*scaleY));

                if (x >= 0 && y>=0) {
                    int tileIndex = x + y*TilesPerRow;
                    TileSelcted.Id = tileList[tileIndex];
                    TileSelcted.TileSet = set.Name;
                }

            }

            DrawRenderTarget((int)currentWidth, (int)currentHeight);

            ImGui.End();
        }

        public override void Update() {

        }

        protected override void Draw() {

            //null check
            if (set == null)
                return;

            Raylib.ClearBackground(Color.DARKBLUE);

            float scale = (float)UnitSize/set.TileWidth;
            int col = 0, row = 0;

            for (int i = 0; i < tileList.Length; i++) {

                set.Draw(row*UnitSize + row*TileGap, col*UnitSize+RowGap*col, (uint)tileList[i], scale);
                
                //draw border around selected tile
                if (TileSelcted.Id == tileList[i])
                    Raylib.DrawRectangleLinesEx(new Rectangle(row*UnitSize + row*TileGap,col*UnitSize+RowGap*col,UnitSize,UnitSize), 2f, Color.BLACK);

                row++;
                if(row >= TilesPerRow) {
                    col++;
                    row = 0;
                }
            }
        }

        public Tile GetTileSelected() {
            return TileSelcted;
        }

        public void ChangeTileSet(TileSet ts) {

        set = ts;

        var dim = set.GetTileDimensions();
        uint tileNum = dim.Item1*dim.Item2;
        tileList = new int[tileNum];

        rowNum = (int)Math.Ceiling((double)tileNum/TilesPerRow);

        trueWidth = TilesPerRow*UnitSize + (TilesPerRow-1)*TileGap;
        trueHeight = rowNum*UnitSize + (rowNum-1)*RowGap;
        this.ResizeRenderTarget(trueWidth, trueHeight);
        sizeSet = false;

        for (uint i = 0, count = 0; i < dim.Item1; i++) {
            for (uint j = 0; j < dim.Item2; j++) {
                tileList[count] = (int)set.GetID(i,j);
                count++;
            }
        }
        
        //uncomment these lines if you want selected Tile to reset when changing TileSets
        // TileSelcted.TileSet = "";
        // TileSelcted.Id = -1;
    }

    }
}