﻿namespace ConsoleIO
{
    public sealed class SelectorMenu<T> : MenuBase<SelectorMenu<T>>
        where T : MenuBase<T>
    {
        public SelectorMenu(Action<ConsoleMenuSettings> settings) : base(settings)
        {

        }
    }
}