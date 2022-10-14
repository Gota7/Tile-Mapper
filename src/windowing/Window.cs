namespace TileMapper.Windowing
{

    // General graphics window.
    public abstract class Window
    {

        // Specifies if the window is open internally.
        // This has to be private and not public get and private set due to it needing to be passed by reference.
        protected bool _open = true;

        // If a window is open.
        public bool Open => _open;

        // Draw graphics components of the window.
        public virtual void DrawUI() {}

        // Update logic for the window.
        public virtual void Update() {}

        // Close the window, freeing any resources as needed.
        public virtual void Close()
        {
            _open = false;
        }

    }

}