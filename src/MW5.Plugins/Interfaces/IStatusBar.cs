﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5.Plugins.Concrete;
using MW5.UI.Menu;

namespace MW5.Plugins.Interfaces
{
    public interface IStatusBar: IToolbar
    {
        void ShowProgress(string message, int percent);
        void HideProgress();
        IMenuItem FindItem(string key, PluginIdentity identity);
        bool AlignNewItemsRight { get; set; }
        new IStatusItemCollection Items { get; }
    }
}
