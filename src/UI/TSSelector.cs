using TileMapper.Windowing;
using Raylib_cs;
using ImGuiNET;
using System.Numerics;

namespace TileMapper {

    public class TSSelector : Window {

        //stores the TileSet names' and their paths
        private List<String> tileSets = new List<string>();

        private String tsPath;

        private int currentLayer;

        private TileSelector ts;

        private TileMap tMap;

        //FileName to TileSet, keep tracks of the sets that have been loaded. prevents laoding a set twice
        private Dictionary<String, TileSet> fnameToSet;

        //give path of folder containing TileSets
        public TSSelector(String tsPath, TileSelector ts, TileMap tMap) {

            this.ts = ts;
            this.tMap = tMap;
            this.tsPath = tsPath;
            fnameToSet = new Dictionary<string, TileSet>();

            var test = Directory.EnumerateFiles(tsPath);
            foreach (String s in test) {
                tileSets.Add(Path.GetFileName(s));
            }

            currentLayer = 0;
        }

        public override void DrawUI() {

            ImGui.Begin("TileSet Selector");
            
            ImGui.BeginTabBar("t");

            if (ImGui.BeginTabItem("TileSets")) {
                CreateSelectables();
                ImGui.EndTabItem();
            }


            if (ImGui.BeginTabItem("Layers")) {
                CreateLayers();

                if (ImGui.Button("add layer")) {
                    tMap.AddLayer();
                }
                ImGui.EndTabItem();
            }


            ImGui.EndTabBar();



            ImGui.End();
        }

        public override void Update() {

        }

        //TileSet selection creationa and behavior
        private void CreateSelectables() {
            
            foreach (String fname in tileSets) {

                String path = tsPath + "/" + fname;
                
                if (ImGui.Selectable(fname)) {
                    if (!fnameToSet.ContainsKey(fname)) {
                        TileSet set = new TileSet(path);
                        fnameToSet.Add(fname, set);
                        ts.ChangeTileSet(set);
                        tMap.AddTileSet(set);
                    } else {
                        TileSet set = fnameToSet[fname];
                        ts.ChangeTileSet(set);
                    }
                    
                }
            }
        }

        //Layer Selections
        private void CreateLayers() {

            for (int i = 0; i < tMap.GetLayerCount(); i++) {

                String layerName = (i == currentLayer) ? "Layer " + i + "*": "Layer " + i;

                if (ImGui.Selectable(layerName)) {
                    tMap.SetCurrentLayer(i);
                    currentLayer = i;
                }
            }
        }
    }
}