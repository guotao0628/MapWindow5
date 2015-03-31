﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MW5.Plugins;
using MW5.Plugins.Concrete;
using MW5.Plugins.Interfaces;
using Syncfusion.Windows.Forms.Tools;

namespace MW5.UI.Menu
{
    internal class StatusBar: IStatusBar
    {
        private readonly StatusStripEx _bar;
        private readonly IMenuIndex _menuIndex;
        private ToolStripItem _progressMessage;
        private ToolStripProgressBar _progressBar;

        public const string ProgressMsg = "statusProgressMsg";
        public const string ProgressBar = "statusProgressBar";

        internal StatusBar(object bar, IMenuIndex menuIndex, PluginIdentity identity)
        {
            _bar = bar as StatusStripEx;
            _menuIndex = menuIndex;

            if (_bar == null) throw new ArgumentNullException("bar");
            if (_menuIndex == null) throw new ArgumentNullException("menuIndex");
            
            _bar.Tag = new MenuItemMetadata(identity, "statusbar");
        }

        public string Name { get; set; }

        public IStatusItemCollection Items
        {
            get { return new StatusItemCollection(_bar.Items, _menuIndex, true); }
        }

        IMenuItemCollection IToolbar.Items
        {
            get { return Items; }
        }

        public bool Visible
        {
            get { return _bar.Visible; }
            set { _bar.Visible = value; }
        }

        public object Tag
        {
            get { return Metadata.Tag; }
            set { Metadata.Tag = value; }
        }

        public string Key
        {
            get { return Metadata.Key; }
        }

        private MenuItemMetadata Metadata
        {
            get
            {
                var metadata = _bar.Tag as MenuItemMetadata;
                if (metadata == null)
                {
                    throw new ApplicationException("Tag must have an instance of MenuItemMetadata class.");
                }
                return metadata;
            }
        }

        public ToolbarDockState DockState
        {
            get { return ToolbarDockState.Bottom; }
            set { ; }
        }

        public PluginIdentity PluginIdentity
        {
            get { return Metadata.PluginIdentity; }
        }

        public void Update()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].BeginGroup && i > 0)
                {
                    var item = _bar.Items[i - 1] as IStatusItem;
                    if (item != null)
                    {
                        item.EndOfGroup = true;
                    }
                    else
                    {
                        var stripItem = _bar.Items[i - 1] as ToolStripStatusLabel;
                        if (stripItem != null)
                        {
                            stripItem.BorderSides = ToolStripStatusLabelBorderSides.Right;
                        }
                    }
                }
            }
        }

        private bool FindProgressBar()
        {
            if (_progressMessage != null && _progressBar != null)
            {
                return true;
            }
            
            var item = FindItem(ProgressBar, PluginIdentity.Default);
            if (item != null)
            {
                _progressBar = item.GetInternalObject() as ToolStripProgressBar;
            }

            item = FindItem(ProgressMsg, PluginIdentity.Default);
            if (item != null)
            {
                _progressMessage = item.GetInternalObject() as ToolStripItem;
            }

            return _progressMessage != null && _progressBar != null;
        }

        public void ShowProgress(string message, int percent)
        {
            if (!FindProgressBar())
            {
                return;
            }

            _progressMessage.Text = message;
            _progressBar.Value = percent;
            if (!_progressMessage.Visible)
            {
                _progressMessage.Visible = true;
                _progressBar.Visible = true;
                _bar.Refresh();
            }
        }

        public void HideProgress()
        {
            if (!FindProgressBar())
            {
                return;
            }

            _progressMessage.Visible = false;
            _progressBar.Visible = false;
        }

        public IMenuItem FindItem(string key, PluginIdentity identity)
        {
            return _menuIndex.GetItem(identity.GetUniqueKey(key));
        }

        public bool AlignNewItemsRight
        {
            get
            {
                var coll = Items as StatusItemCollection;
                if (coll != null)
                {
                    return coll.AlignRight;
                }
                return false;
            }
            set
            {
                var coll = Items as StatusItemCollection;
                if (coll != null)
                {
                    coll.AlignRight = value;
                }
            }
        }
    }
}
