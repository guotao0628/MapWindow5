﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5.Plugins.Concrete;
using MW5.UI.Menu;

namespace MW5.Plugins.Interfaces
{
    internal interface IMenuIndex
    {
        void AddItem(string key, IMenuItem item);
        void Remove(string key);
        IMenuItem GetItem(string key);
        void RemoveItemsForPlugin(PluginIdentity pluginIdentity);
        IEnumerable<IMenuItem> ItemsForPlugin(PluginIdentity pluginIdentity);
        void Clear();
        void SaveMetadata(object key, MenuItemCollectionMetadata metadata);
        MenuItemCollectionMetadata LoadMetadata(object key);
    }
}
