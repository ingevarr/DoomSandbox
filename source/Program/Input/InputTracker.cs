using System.Collections.Generic;

using Veldrid;

namespace Doom.Input 
{
    public sealed class InputTracker : IInputUpdater, IInputTracker
    {
        private readonly HashSet<Key> pressedKeysInThisFrame = new HashSet<Key>();
        private readonly HashSet<Key> pressedKeys = new HashSet<Key>();
        
        private readonly HashSet<MouseButton> pressedMouseButtons = new HashSet<MouseButton>();

        private InputSnapshot currentFrameSnapshot;

        public bool GetMouse(MouseButton mouseButton) => pressedMouseButtons.Contains(mouseButton);
        public bool GetMouseDown(MouseButton mouseButton) => currentFrameSnapshot.IsMouseDown(mouseButton);


        public bool GetKey(Key key) => pressedKeys.Contains(key);
        public bool GetKeyDown(Key key) => pressedKeysInThisFrame.Contains(key);

        public void Update(InputSnapshot inputSnapshot)
        {
            currentFrameSnapshot = inputSnapshot;

            UpdateKeyboard(inputSnapshot.KeyEvents);
            UpdateMouse(inputSnapshot);
        }

        private void UpdateKeyboard(IReadOnlyList<KeyEvent> keyEvents)
        {
            pressedKeysInThisFrame.Clear();
            for (var i = 0; i < keyEvents.Count; i++)
            {
                var keyEvent = keyEvents[i];
                if (keyEvent.Down)
                {
                    if (pressedKeys.Add(keyEvent.Key))
                        pressedKeysInThisFrame.Add(keyEvent.Key);
                }
                else
                    pressedKeys.Remove(keyEvent.Key);
            }
        }

        private void UpdateMouse(InputSnapshot inputSnapshot)
        {
            for (var i = 0; i < inputSnapshot.MouseEvents.Count; i++)
            {
                var mouseEvent = inputSnapshot.MouseEvents[i];
                if(inputSnapshot.IsMouseDown(mouseEvent.MouseButton))
                    pressedMouseButtons.Add(mouseEvent.MouseButton);
                else
                    pressedMouseButtons.Remove(mouseEvent.MouseButton);
            }
        }
    }
}