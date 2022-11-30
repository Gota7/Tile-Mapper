using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TileMapper.Windowing;
using Num = System.Numerics;

namespace TileMapper.UI
{

    /// <summary>
    /// File dialog.
    /// </summary>
    public class FileDialog : Window
    {

        /// <summary>
        /// Max file dialog name.
        /// </summary>
        public const int MAX_FILE_DIALOG_NAME_BUFFER = 2048;

        /// <summary>
        /// Filters.
        /// </summary>
        public List<Tuple<string, string[]>> Filters { get; private set; } = new List<Tuple<string, string[]>>();

        /// <summary>
        /// Filter texts.
        /// </summary>
        public string[] FilterTexts { get; private set; }

        /// <summary>
        /// Filter index.
        /// </summary>
        public int FilterIndex;

        /// <summary>
        /// Mode to select file.
        /// </summary>
        public FileDialogMode Mode;

        /// <summary>
        /// Name of the file within the path.
        /// </summary>
        public string FileName;

        /// <summary>
        /// Path.
        /// </summary>
        public string Path = Directory.GetCurrentDirectory();

        /// <summary>
        /// Selected path/folder.
        /// </summary>
        public string SelectedItem = "";

        /// <summary>
        /// Name of the window.
        /// </summary>
        private string _name = "Open File:";

        /// <summary>
        /// If to open up the popup.
        /// </summary>
        private bool _openPopup = true;

        /// <summary>
        /// Create a new file dialog.
        /// </summary>
        /// <param name="dialogClass">Dialog class, used for re-using the same path.</param>
        /// <param name="mode">Dialog mode.</param>
        /// <param name="filter">File filter.</param>
        /// <param name="defaultFileName">Default file name.</param>
        /// <param name="defaultPath">Default file path.</param>
        public FileDialog(string dialogClass, FileDialogMode mode, string filter, string defaultFileName = "", string defaultPath = "")
        {

            // Set vars.
            switch (mode)
            {
                case FileDialogMode.OpenFile:
                    _name = "Open File:";
                    break;
                case FileDialogMode.SaveFile:
                    _name = "Save File:";
                    break;
                case FileDialogMode.Directory:
                    _name = "Select Directory:";
                    break;
            }
            Mode = mode;
            FileName = defaultFileName;

            // Filters if not dir mode.
            if (Mode != FileDialogMode.Directory)
            {

                // Split the filter.
                string[] strs = filter.Split('|');

                // Load the filters.
                Filters = new List<Tuple<string, string[]>>();
                FilterTexts = new string[strs.Length / 2];

                // Make sure the filter is legal.
                if (strs.Length == 0 || strs.Length % 2 != 0)
                {
                    throw new Exception("Bad File Filter!");
                }

                // Temp values.
                string filterText = "";
                string description = "";
                string[] extensions;

                // Get filters.
                for (int i = 0; i < strs.Length; i++)
                {

                    // Description.
                    if (i % 2 == 0)
                    {
                        description = strs[i];
                        filterText = description + " (";
                    }

                    // Extensions.
                    else
                    {

                        // Get extensions.
                        extensions = strs[i].Split(';');

                        // Add to filters.
                        Filters.Add(new Tuple<string, string[]>(description, extensions));

                        // Filter text.
                        filterText += strs[i] + ")";
                        FilterTexts[i / 2] = filterText;

                    }

                }

            }

        }

        /// <summary>
        /// Draw the layout.
        /// <summary>
        public override void DrawUI()
        {
            if (_openPopup)
            {
                ImGui.OpenPopup(_name);
                _openPopup = false;
            }
            if (ImGui.BeginPopupModal(_name))
            {
                DrawLayout();
                ImGui.EndPopup();
            }
        }

        /// <summary>
        /// Draw the layout.
        /// </summary>
        private void DrawLayout()
        {

            // Sizes.
            float width = ImGui.GetWindowWidth();
            float height = ImGui.GetWindowHeight();

            // Set the selected item to the filename.
            if (FileName != "" || Mode == FileDialogMode.Directory)
            {
                SelectedItem = Path + "/" + FileName;
            }

            // Get the path.
            Path = Path.Replace("\\", "/");

            // Path elements.
            string[] pathElements = Path.Split('/');
            if (pathElements.Length > 0)
            {

                // First element.
                //  if (ImGui.Button(pathElements[0] + "##Path0")) {
                //      if (pathElements.Length > 1) {
                //          Path = Path.Substring(0, Path.IndexOf('/'));
                //          if (Mode == FileDialogMode.Directory) {
                //              SelectedItem = Path;
                //          }
                //      }
                //  } //  Just have the programs folder show.

                // Other elements.
                for (int i = 1; i < pathElements.Length; i++)
                {

                    // Same line.
                    ImGui.SameLine();

                    // Get button.
                    if (ImGui.Button(pathElements[i] + "##Path" + i))
                    {
                        if (i != pathElements.Length - 1)
                        {
                            Path = Path.Substring(0, GetNthIndex(Path, '/', i + 1));
                            if (Mode == FileDialogMode.Directory)
                            {
                                SelectedItem = Path;
                            }
                        }
                    }

                }

            }

            // Folder and file select.
            ImGui.BeginChild("##FileDialogFileList", new Num.Vector2(width - ImGui.GetStyle().WindowPadding.X / 2, height - ImGui.GetTextLineHeight() * 3 - ImGui.GetStyle().FramePadding.Y * 8 - ImGui.GetStyle().WindowPadding.Y * 2 - ImGui.GetStyle().ItemSpacing.Y * 6));

            // Catch access denied.
            try
            {

                // Folder names.
                List<string> names = new List<string>();
                // if (Path.Contains('/')) { names.Add("[Dir] .."); }
                var pathNames = Directory.EnumerateDirectories(Path.Contains('/') ? Path : Path + "/");
                pathNames = pathNames.OrderBy(x => x);
                foreach (var p in pathNames)
                {
                    names.Add("[Dir] " + p.Replace("\\", "/").Split('/').Last());
                }

                // File names.
                if (Mode != FileDialogMode.Directory)
                {
                    var fileNames = ApplyFilter(Directory.EnumerateFiles(Path.Contains('/') ? Path : Path + "/").ToList());
                    foreach (var f in fileNames)
                    {
                        // names.Add("[File] " + f.Replace("\\", "/").Split('/').Last());
                        names.Add(f.Replace("\\", "/").Split('/').Last());
                    }
                }

                // Show the folder and file names.
                ImGui.PushItemWidth(width - ImGui.GetStyle().WindowPadding.X / 2);
                for (int i = 0; i < names.Count; i++)
                {

                    // Add the item.
                    if (ImGui.Selectable(names[i] + "##DirFile" + i))
                    {

                        // Directory.
                        if (names[i].StartsWith("[Dir]"))
                        {

                            // Get the dir name.
                            string dir = names[i].Substring(names[i].IndexOf(' ') + 1, names[i].Length - names[i].IndexOf(' ') - 1);

                            // Open the dir.
                            bool openDir = false;

                            // Not directory select mode, so allow openening it.
                            if (Mode != FileDialogMode.Directory)
                            {
                                openDir = true;
                            }
                            else
                            {

                                // Open directory if it was selected before.
                                if (SelectedItem.Equals(Path + "/" + dir))
                                {
                                    openDir = true;
                                }

                                // Append path.
                                SelectedItem = Path + "/" + dir;

                            }

                            // Open dir.
                            if (openDir)
                            {

                                // Remove filename.
                                FileName = "";
                                SelectedItem = "";

                                // Backwards.
                                if (names[i].Equals("[Dir] .."))
                                {

                                    // Substring it.
                                    Path = Path.Substring(0, Path.LastIndexOf("/"));
                                    if (Mode == FileDialogMode.Directory)
                                    {
                                        SelectedItem = Path;
                                    }

                                }
                                else
                                {

                                    // Add to path.
                                    Path += "/" + dir;

                                }

                            }

                        }

                        // File.
                        else
                        {

                            // Get the file name.
                            // string file = names[i].Substring(names[i].IndexOf(' ') + 1, names[i].Length - names[i].IndexOf(' ') - 1);
                            string file = names[i];

                            // It is the selected item, ok it.
                            if (SelectedItem.Equals(Path + "/" + file))
                            {

                                // It's ok.
                                _open = false;

                            }

                            // Select the item.
                            else
                            {

                                // Set selected item and file name.
                                SelectedItem = Path + "/" + file;
                                FileName = file;

                            }

                        }

                    }

                }
                ImGui.PopItemWidth();

            }
            catch { ImGui.Selectable("Access Denied!"); }

            // End folder and file select.
            ImGui.EndChild();

            // Get width of max combo-box.
            float comboWidth = 0;

            // If dir mode, there are is no combo width.
            if (Mode != FileDialogMode.Directory)
            {

                comboWidth = ImGui.CalcTextSize(FilterTexts[0]).X;
                for (int i = 1; i < FilterTexts.Length; i++)
                {
                    float w = ImGui.CalcTextSize(FilterTexts[i]).X;
                    if (w > comboWidth)
                    {
                        comboWidth = w;
                    }
                }

                // Add stuff to width.
                comboWidth += ImGui.GetStyle().FramePadding.X + 25;

            }

            // Get file name.
            if (SelectedItem.Contains('/'))
            {
                if (Directory.Exists(SelectedItem) && Mode == FileDialogMode.Directory)
                {
                    FileName = SelectedItem.Substring(SelectedItem.LastIndexOf('/') + 1, SelectedItem.Length - SelectedItem.LastIndexOf('/') - 1);
                }
                if (File.Exists(SelectedItem) && Mode != FileDialogMode.Directory)
                {
                    FileName = System.IO.Path.GetFileName(SelectedItem);
                }
            }

            // Make sure it's not a folder within a folder.
            if (Path.Equals(SelectedItem))
            {
                FileName = "";
            }

            // File name.
            ImGui.Text(Mode == FileDialogMode.Directory ? "Directory Name: " : "File Name: ");
            ImGui.SameLine();
            float width2 = ImGui.GetContentRegionAvail().X - comboWidth - ImGui.GetStyle().WindowPadding.X;
            ImGui.PushItemWidth(width2);
            ImGui.InputText("##FileName", ref FileName, MAX_FILE_DIALOG_NAME_BUFFER);
            ImGui.PopItemWidth();

            // Don't do filters if dir.
            if (Mode != FileDialogMode.Directory)
            {

                // Filters.
                ImGui.SameLine();
                ImGui.PushItemWidth(comboWidth);
                ImGui.Combo("##Filters", ref FilterIndex, FilterTexts, FilterTexts.Length);
                ImGui.PopItemWidth();

            }

            // Cancel.
            if (ImGui.Button("Cancel"))
            {
                _open = false;
            }

            // Ok.
            ImGui.SameLine();
            if (ImGui.Button("Ok") && SelectedItem != "")
            {

                // Make sure file/ exists to open.
                if ((File.Exists(SelectedItem) && Mode == FileDialogMode.OpenFile) || Mode != FileDialogMode.OpenFile)
                {

                    // Create folder if needed.
                    if (Mode == FileDialogMode.Directory && !Directory.Exists(SelectedItem))
                    {
                        Directory.CreateDirectory(SelectedItem);
                    }

                    // Append extension if needed.
                    if (Mode != FileDialogMode.Directory)
                    {
                        if (ApplyFilter(new List<string>() { SelectedItem }).Count < 1)
                        {
                            SelectedItem += Filters[0].Item2[0].Replace("*", "");
                        }
                    }

                    // Set ok.
                    _open = false;

                }
            }

        }

        /// <summary>
        /// Get the Nth index of a character.
        /// </summary>
        /// <param name="s">String.</param>
        /// <param name="t">Char to search for.</param>
        /// <param name="n">Index number for the character.</param>
        /// <returns>The index of the char the Nth time.</returns>
        public int GetNthIndex(string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Apply the current filter.
        /// </summary>
        /// <param name="fileNames">File names.</param>
        /// <returns>Sting list with filters applied.</returns>
        public List<string> ApplyFilter(List<string> fileNames)
        {

            // New filtered list.
            List<string> filtered = new List<string>();

            // Go through each name.
            foreach (string f in fileNames)
            {

                // Add.
                bool add = false;

                // Find match.
                foreach (string ext in Filters[FilterIndex].Item2)
                {

                    // Get ending and start.
                    string s = ext.Replace("*", "");
                    string start = s.Split('.')[0];
                    string end = s.Split('.')[1];

                    // Definite name.
                    bool def = !ext.Contains("*");

                    // Def.
                    if (def)
                    {
                        if (f.Equals(ext)) { add = true; }
                    }
                    else
                    {
                        if (f.StartsWith(start) && f.ToLower().EndsWith(end)) { add = true; }
                    }

                }

                // Add to filtered.
                if (add)
                {
                    filtered.Add(f);
                }

            }

            // Filtered.
            return filtered;

        }

    }

}
