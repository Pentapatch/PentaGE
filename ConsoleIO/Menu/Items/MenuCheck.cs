namespace ConsoleIO
{
    public sealed class MenuCheck : MenuItem
    {
        public bool Checked { get; set; } = false;

        public MenuCheck(ConsoleMenu owner, string text, Action<MenuCheckSettings>? settings, Action? action = null) : base(owner, text, action)
        {
            if (settings is null) return;

            var itemSettings = new MenuCheckSettings();
            settings(itemSettings);      // Invoke the setup delegate
            ApplySettings(itemSettings); // Apply the derived settings to the menu item
        }

        protected override void ApplySettings(MenuItemSettings settings)
        {
            // Apply base settings
            base.ApplySettings(settings);

            // Apply derived settings
            if (settings is MenuCheckSettings derrivedSettings)
            {
                Checked = derrivedSettings.Checked; 
            }
        }
    }
}