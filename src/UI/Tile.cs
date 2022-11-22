namespace TileMapper {

    public class Tile {
        
        // Name of the TileSet this tile comes from.
        public String TileSet {get; set;}

        public int Id {get; set;}

        public Tile(String tileSet, int id) {
            this.TileSet = tileSet;
            this.Id = id;
        }
    }

}
