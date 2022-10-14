using ImGuiNET;
using TileMapper.Windowing;

namespace WindowTest
{

    // Test window to implement a standard window.
    public class TestWindow : Window
    {
        private int m_Counter = 0;

        public override void DrawUI()
        {
            if (_open && ImGui.Begin("Hello World!", ref _open, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("This is a sample window that does nothing.");
                ImGui.Text("Counter: " + m_Counter);
                if (ImGui.Button("Close"))
                    _open = false;
                ImGui.End();
            }
        }

        public override void Update()
        {
            m_Counter++;
        }

    }

}