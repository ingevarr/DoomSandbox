using Veldrid;

namespace Doom.Input 
{
    public interface IInputUpdater
    {
        void Update(InputSnapshot inputSnapshot);
    }
}