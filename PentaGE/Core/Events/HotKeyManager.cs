using PentaGE.Common;
using Serilog;

namespace PentaGE.Core.Events
{
    internal sealed class HotKeyManager
    {
        private ModifierKey _modifiers = ModifierKey.None;
        private Dictionary<Key, ModifierKey> _entries;

        internal void KeyPressed(Key key, ModifierKey mods)
        {
            // Add the modifier key to the current modifiers
            _modifiers |= mods;

            // Handle the key

            // (Temporary) Log the result
            LogResult();
        }

        internal void KeyReleased(Key key)
        {
            // Remove the modifier key from the current modifiers
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                _modifiers &= ~ModifierKey.Shift;
            }
            else if (key == Key.LeftControl || key == Key.RightControl)
            {
                _modifiers &= ~ModifierKey.Control;
            }
            else if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                _modifiers &= ~ModifierKey.Alt;
            }
            else if (key == Key.LeftSuper || key == Key.RightSuper)
            {
                _modifiers &= ~ModifierKey.Super;
            }

            // (Temporary) Log the result
            LogResult();
        }

        private void LogResult()
        {
            Log.Information("Modifiers: {Modifiers}", _modifiers);
        }

    }
}