﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MW5.Plugins.Concrete;
using MW5.Plugins.Interfaces;
using MW5.UI.Properties;
using Syncfusion.Windows.Forms.Tools;

namespace MW5.UI.Toolbox
{
    /// <summary>
    /// GisToolbox control
    /// </summary>
    public class Toolbox : SplitContainerAdv, IToolbox 
    {
        // icon indices
        internal const int IconFolder = 0;
        internal const int IconFolderOpen = 1;
        internal const int IconTool = 2;
        
        private TreeView _tree;
        private RichTextBox _textbox;

        public event EventHandler<ToolboxToolEventArgs> ToolClicked;
        public event EventHandler<ToolboxToolEventArgs> ToolSelected;
        public event EventHandler<ToolboxGroupEventArgs> GroupSelected;

        #region Initialization

        /// <summary>
        /// Creates a new instance of GIS toolbox class.
        /// </summary>
        public Toolbox()
        {
            Init();

            AddEventHandlers();

#if STYLE2010
            Style = global::Syncfusion.Windows.Forms.Tools.Enums.Style.Office2007Blue;
#else
            Style = global::Syncfusion.Windows.Forms.Tools.Enums.Style.Mozilla;
#endif
        }

        private void Init()
        {
            Orientation = Orientation.Vertical;
            BorderStyle = BorderStyle.None;

            InitTreeView();

            InitTextBox();

            SplitterDistance = Convert.ToInt32(Height * 0.9);

            InitImageList();
        }

        private void InitTreeView()
        {
            _tree = new TreeView
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill
            };
            Panel1.Controls.Add(_tree);
            Panel1MinSize = 0;
        }

        private void InitTextBox()
        {
            _textbox = new RichTextBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                ScrollBars = RichTextBoxScrollBars.None,
                BackColor = Color.FromKnownColor(KnownColor.Control),
                ReadOnly = true,
                Text = "No tool is selected."
            };

            Panel2.Controls.Add(_textbox);
            Panel2MinSize = 0;
        }

        private void AddEventHandlers()
        {
            _tree.BeforeExpand += GeoprocessingTreeBeforeExpand;
            _tree.BeforeCollapse += GeoprocessingTreeBeforeCollapse;
            _tree.AfterSelect += GeoprocessingTreeAfterSelect;
            _tree.NodeMouseDoubleClick += TreeNodeMouseDoubleClick;
            ToolSelected += GisToolbox_ToolSelected;
            GroupSelected += GisToolbox_GroupSelected;
        }

	    private void InitImageList()
	    {
	        ImageList imageList = new ImageList {ColorDepth = ColorDepth.Depth32Bit};

	        Bitmap bmp = new Bitmap(Resources.img_folder_closed, new Size(16, 16));
		    imageList.Images.Add(bmp);

            bmp = new Bitmap(Resources.img_folder_open, new Size(16, 16));
		    imageList.Images.Add(bmp);

            bmp = new Bitmap(Resources.img_tool, new Size(16, 16));
		    imageList.Images.Add(bmp);

		    _tree.ImageList = imageList;
        }
        #endregion

        #region IGisToolBox Members

        /// <summary>
        /// Returns list of groups located at the top level of toolbox.
        /// </summary>
        public IToolboxGroups Groups
        {
            get 
            {
                return new ToolboxGroups(_tree.Nodes);
            }
        }

        /// <summary>
        /// Returns list of all tools in the toolbox
        /// </summary>
        public IToolCollection Tools
        {
            get { return new ToolCollection(_tree.Nodes); }
        }
        
        /// <summary>
        /// Creates a new tool.
        /// </summary>
        public IGisTool CreateTool(string name, string key, PluginIdentity identity)
        {
            var tool = new GisTool(name, key, identity);
            return tool;
        }

        /// <summary>
        /// Creates a new group.
        /// </summary>
        public IToolboxGroup CreateGroup(string name, string description, PluginIdentity identity)
        {
            return new ToolboxGroup(name, description, identity);
        }

        /// <summary>
        /// Expands all the groups up to the specified level
        /// </summary>
        public void ExpandGroups(int level)
        {
            ExpandGroups(Groups, level);
        }

        /// <summary>
        /// Recursively expans all the child groups up to the specified level
        /// </summary>
        private void ExpandGroups(IToolboxGroups groups, int level)
        {
            foreach (var group in groups)
            {
                group.Expanded = true;
                level--;

                if (level > 0)
                {
                    ExpandGroups(group.SubGroups, level);
                }
            }
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Sets the closed state of folder
        /// </summary>
        private void GeoprocessingTreeBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null) return;

            if (e.Node.ImageIndex != IconFolderOpen) return;

            e.Node.ImageIndex = IconFolder;
            e.Node.SelectedImageIndex = IconFolder;
        }

        /// <summary>
        /// Sets the opened state of folder
        /// </summary>
        private void GeoprocessingTreeBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null) return;

            if (e.Node.ImageIndex != IconFolder) return;

            e.Node.ImageIndex = IconFolderOpen;
            e.Node.SelectedImageIndex = IconFolderOpen;
        }

        /// <summary>
	    /// Fires events, sets the same icons for selected mode as for regular mode
	    /// </summary>
	    private void GeoprocessingTreeAfterSelect(object sender, TreeViewEventArgs e)
	    {
            if (e.Node == null) return;

            e.Node.SelectedImageIndex = e.Node.ImageIndex;
            if ((e.Node.Tag != null)) 
            {
                if (e.Node.ImageIndex == IconTool)
                {
                    var tool = e.Node.Tag as IGisTool;
                    if (tool != null)
                    {
                        FireToolSelected(tool);
                    }
                }
                else
                {
                    var group = e.Node.Tag as IToolboxGroup;
                    if (group != null)
                    {
                        FireGroupSelected(group);
                    }
                }
            }
	    }

        /// <summary>
        /// Generates tool clicked event for plug-ins
        /// </summary>
        private void TreeNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null || e.Node.Tag is IToolboxGroup)
            {
                return;
            }
            
            var tool = e.Node.Tag as IGisTool;
            if (tool != null)
            {
                FireToolClicked(tool);
            }
        }

        private void GisToolbox_GroupSelected(object sender, ToolboxGroupEventArgs e)
        {
            var group = e.Group;

            _textbox.Clear();
            _textbox.Text = group.Name + Environment.NewLine + Environment.NewLine + group.Description;
            _textbox.Select(0, group.Name.Length);
            _textbox.SelectionFont = new Font(Font, FontStyle.Bold);
        }

        private void GisToolbox_ToolSelected(object sender, ToolboxToolEventArgs e)
        {
            var tool = e.Tool;
            _textbox.Clear();
            _textbox.Text = tool.Name + Environment.NewLine + Environment.NewLine + tool.Description;

            if (tool.Name.Length > 0)
            {
                _textbox.Select(0, tool.Name.Length);
                _textbox.SelectionFont = new Font(Font, FontStyle.Bold);
            }
        }

        private void FireToolClicked(IGisTool tool)
        {
            FireEvent(ToolClicked, new ToolboxToolEventArgs(tool));
        }

        private void FireToolSelected(IGisTool tool)
        {
            FireEvent(ToolSelected, new ToolboxToolEventArgs(tool));
        }

        private void FireGroupSelected(IToolboxGroup group)
        {
            FireEvent(GroupSelected, new ToolboxGroupEventArgs(group));
        }

        private void FireEvent<T>(EventHandler<T> handler, T args)
        {
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion
    }
}