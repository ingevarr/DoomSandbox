using Veldrid;

namespace Doom.Input 
{
    public interface IInputTracker
    {
        bool GetMouseDown(MouseButton mouseButton);
        bool GetMouse(MouseButton mouseButton);
        bool GetKey(Key key);
        bool GetKeyDown(Key key);
    }
}