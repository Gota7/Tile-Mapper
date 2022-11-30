using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper.UI
{

    public class TSSelector : Window
    {

        // FileName to TileSet, keep tracks of the sets that have been loaded. prevents laoding a set twice.
        private Dictionary<string, TileSet> _fnameToSet = new Dictionary<string, TileSet>();

        // Current tileset.
        public string CurrTileset { get; private set; } = "";

        // Current tileset data.
        public TileSet CurrTilesetData => CurrTileset.Equals("") ? null : _fnameToSet[CurrTileset];

        public TSSelector() : base() {}

        public override void DrawUI()
        {

            ImGui.Begin("Tile Set Selector", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoResize);
            ImGui.Text("Tile Sets:");
            CreateSelectables();
            ImGui.End();
        }

        // Add a tileset to the list. Returns false if tileset exists.
        public bool AddTileset(string path, TileSet tileset)
        {
            if (_fnameToSet.ContainsKey(path)) return false;
            else _fnameToSet.Add(path, tileset);
            return true;
        }

        // Remove a tileset from the list. Returns if it was able to be removed.
        public bool RemoveTileset(string path) {
            if (_fnameToSet.ContainsKey(path)) _fnameToSet.Remove(path);
            else return false;
            if (CurrTileset.Equals(path)) CurrTileset = "";
            return true;
        }

        // TileSet selection creationa and behavior.
        private void CreateSelectables()
        {
            foreach (string fname in _fnameToSet.Keys)
            {
                string tsName = _fnameToSet[fname].Name;
                if (fname.Equals(CurrTileset)) tsName += " *";
                if (ImGui.Selectable(tsName))
                {
                    CurrTileset = fname;
                }
            }
        }
    }
}