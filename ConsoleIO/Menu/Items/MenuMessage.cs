namespace ConsoleIO
{
    public sealed class MenuMessage : MenuItem
    {
        // TODO: Should not inherit from MenuItem
        //       as it does not share common properties and does not
        //       need to be selectable and have an action

        internal MenuMessage(string text, Action<MenuItemSettings>? settings) : base(null!, text, null)
        {
            if (settings is null) return;

            var itemSettings = new MenuItemSettings();

            // Invoke the setup delegate
            settings(itemSettings);

            // Apply the derived settings to the menu item
            ApplySettings(itemSettings);
        }
    }
}