namespace ConsoleIO
{
    public sealed class MenuMessage : MenuItem
    {
        internal MenuMessage(string text, Action<MenuItemSettings>? settings) : base(text, null)
        {
            if (settings is null) return;

            var itemSettings = new MenuItemSettings();
            settings(itemSettings);      // Invoke the setup delegate
            ApplySettings(itemSettings); // Apply the derived settings to the menu item
        }
    }
}