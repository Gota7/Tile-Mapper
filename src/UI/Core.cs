using ImGuiNET;
using TileMapper.Windowing;

namespace TileMapper.UI
{

    // Main editor core.
    public class Core : Window
    {

        // Tileset selector related.
        private TSSelector _selector = new TSSelector();
        private bool _selectorShown = true;

        // Tile set editor.
        private TSEditor _tsEditor = null;

        // Open file dialogs.
        private FileDialog _fileDialog = null;
        private int _fileDialogMode = 0; // 0 - Main window open file. 1 - Tileset image. 2 - Tileset path.
        private string _fileDialogImageTmp;

        // Tile selecting.
        private TileSelector _tsSelector = new TileSelector();
        private string _lastTileSet = "";

        public void DoDraw()
        {
            if (_tsEditor != null) _tsEditor.DoDraw();
            _tsSelector.DoDraw();
        }

        public override void DrawUI()
        {

            // Main menu items.
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Tileset"))
                    {
                        _fileDialog = new FileDialog("TileMapper", FileDialogMode.OpenFile, "Image|*.png;*.jpg;*.gif");
                        _fileDialogMode = 1;
                    }
                    if (ImGui.MenuItem("New Tilemap"))
                    {
                    }
                    if (ImGui.MenuItem("Open")) OpenFiles();
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo"))
                    {
                    }
                    if (ImGui.MenuItem("Redo"))
                    {
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Tool"))
                {
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("View"))
                {
                    ImGui.Checkbox("Tileset Selector", ref _selectorShown);
                    ImGui.EndMenu();
                }
                if (ImGui.MenuItem("About"))
                {
                    // TODO!!!
                }
                ImGui.EndMainMenuBar();
            }

            // File-dialog popups.
            if (_fileDialog != null)
            {
                _fileDialog.DrawUI();
                if (!_fileDialog.Open && !_fileDialog.SelectedItem.Equals(""))
                {
                    string item = _fileDialog.SelectedItem;
                    _fileDialog = null;
                    switch (_fileDialogMode)
                    {
                        case 0: OpenFileCallback(item); break;
                        case 1: NewTileSetImage(item); break;
                        case 2: NewTileSet(_fileDialogImageTmp, item); break;
                    }
                } else if (!_fileDialog.Open) _fileDialog = null;
            }

            // Draw other UIs.
            if (_tsEditor != null && _tsEditor.CurrTileSet != null) _tsEditor.DrawUI();
            if (_selectorShown) _selector.DrawUI();
            _tsSelector.DrawUI();

        }

        public override void Update()
        {

            // Initialize needed handlers (can't do this in constructors because &this doesn't exist until end of constructor).
            if (_tsEditor == null) _tsEditor = new TSEditor(DrawTilesetEditorMenu, this);

            // Update other UIs.
            _selector.Update();
            if (!_lastTileSet.Equals(_selector.CurrTileset)) _tsSelector.ChangeTileSet(_selector.CurrTilesetData);
            _tsEditor.CurrTileSet = _selector.CurrTilesetData;
            _lastTileSet = _selector.CurrTileset;
            _tsEditor.Update();
            _tsSelector.Update();

        }

        public override void Close()
        {

            // Close if needed.
            _selector.Close();

        }

        // Open a file.
        private void OpenFiles()
        {
            _fileDialog = new FileDialog("TileMapper", FileDialogMode.OpenFile, "Tile-Mapper Files|*.tms;*.tmm");
            _fileDialogMode = 0;
        }

        // Callback for file dialog to open.
        private void OpenFileCallback(string item)
        {
            if (item.EndsWith(".tms")) OpenTileSet(item);
            else if (item.EndsWith(".tmm")) OpenTileMap(item);
        }

        // Launch next stage of tileset creator.
        private void NewTileSetImage(string image)
        {
            _fileDialogImageTmp = image;
            _fileDialog = new FileDialog("TileMapper", FileDialogMode.SaveFile, "Tile-Mapper Tileset|*.tms");
            _fileDialogMode = 2;
        }

        // Create a new tileset.
        private void NewTileSet(string image, string path)
        {
            TileSet ts = new TileSet(image, "New Tileset");
            if (_selector.AddTileset(path, ts)) ts.Save(path); // Only save if successful in adding.
        }

        // Open a tileset.
        private void OpenTileSet(string tms)
        {
            _selector.AddTileset(tms, new TileSet(tms));
        }

        // Open a tilemap.
        private void OpenTileMap(string tmm)
        {
            // TODO!!!
        }

        // Draw the menu for the tileset editor.
        private static void DrawTilesetEditorMenu(Core core) {
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.MenuItem("Save"))
                {
                    core._selector.CurrTilesetData.Save(core._selector.CurrTileset);
                }
                if (ImGui.MenuItem("Close"))
                {
                    core._selector.RemoveTileset(core._selector.CurrTileset);
                }
                if (ImGui.MenuItem("Delete"))
                {
                    string currTileset = core._selector.CurrTileset;
                    core._selector.RemoveTileset(currTileset);
                    File.Delete(currTileset);
                }
                ImGui.EndMenuBar();
            }
        }

    }

}