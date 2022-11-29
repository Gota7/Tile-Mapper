using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper
{

    public class TSSelector : Window
    {

        // Stores the TileSet names' and their paths.
        private List<String> _tileSets = new List<string>();

        private String _tsPath;

        private TileSelector _ts;

        private TileMap _tMap;

        // FileName to TileSet, keep tracks of the sets that have been loaded. prevents laoding a set twice.
        private Dictionary<String, TileSet> _fnameToSet;

        // Give path of folder containing TileSets.
        public TSSelector(string tsPath, TileSelector ts, TileMap tMap)
        {

            this._ts = ts;
            this._tMap = tMap;
            this._tsPath = tsPath;
            _fnameToSet = new Dictionary<string, TileSet>();

            var test = Directory.EnumerateFiles(_tsPath);
            foreach (String s in test)
            {
                _tileSets.Add(Path.GetFileName(s));
            }

            _tMap.SetCurrentLayer(0);
        }

        public override void DrawUI()
        {

            ImGui.Begin("TileSet Selector");

            ImGui.BeginTabBar("t");

            if (ImGui.BeginTabItem("TileSets"))
            {
                CreateSelectables();
                ImGui.EndTabItem();
            }


            if (ImGui.BeginTabItem("Layers"))
            {
                CreateLayers();

                if (ImGui.Button("Add layer"))
                {
                    _tMap.AddLayer(_tileSets[0]); // TODO: FIX!!!
                    _tMap.SetCurrentLayer(_tMap.GetLayerCount() - 1);
                }
                ImGui.EndTabItem();
            }


            ImGui.EndTabBar();



            ImGui.End();
        }

        public override void Update()
        {

        }

        // TileSet selection creationa and behavior.
        private void CreateSelectables()
        {

            foreach (String fname in _tileSets)
            {

                String path = _tsPath + "/" + fname;

                if (ImGui.Selectable(fname))
                {
                    if (!_fnameToSet.ContainsKey(fname))
                    {
                        TileSet set = new TileSet(path);
                        _fnameToSet.Add(fname, set);
                        _ts.ChangeTileSet(set);
                        _tMap.AddTileSet(set);
                    }
                    else
                    {
                        TileSet set = _fnameToSet[fname];
                        _ts.ChangeTileSet(set);
                    }

                }
            }
        }

        // Layer Selections.
        private void CreateLayers()
        {

            for (int i = 0; i < _tMap.GetLayerCount(); i++)
            {

                String layerName = (i == _tMap.GetCurrentLayerIndex()) ? "Layer " + i + "*" : "Layer " + i;

                if (ImGui.Selectable(layerName))
                {
                    _tMap.SetCurrentLayer(i);
                }
            }
        }
    }
}