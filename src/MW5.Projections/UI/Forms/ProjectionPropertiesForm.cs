﻿// -------------------------------------------------------------------------------------------
// <copyright file="ProjectionPropertiesForm.cs" company="MapWindow OSS Team - www.mapwindow.org">
//  MapWindow OSS Team - 2015
// </copyright>
// -------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MW5.Api.Concrete;
using MW5.Api.Enums;
using MW5.Api.Interfaces;
using MW5.Plugins.Interfaces.Projections;
using MW5.Shared;
using MW5.UI.Forms;

namespace MW5.Projections.UI.Forms
{
    /// <summary>
    /// Displays dialog with projection properties. Allows modification of the projection dialogs.
    /// </summary>
    public partial class ProjectionPropertiesForm : MapWindowForm
    {
        private static int _lastTabIndex;
        private readonly ICoordinateSystem _coordinateSystem;

        private readonly IProjectionDatabase _database;

        // At least one dialect was either added or removed
        private bool _dialectsChanged;
        private ISpatialReference _projection;

        // well-known coordinate system (in case one was passed)

        /// <summary>
        /// Creates a new instance of ProjectionViewer class
        /// </summary>
        public ProjectionPropertiesForm(ICoordinateSystem projection, IProjectionDatabase database)
        {
            InitializeComponent();

            _database = database;
            _coordinateSystem = projection;
            tabControl1.SelectedIndex = _lastTabIndex;

            listView1.MouseDoubleClick += (s, e) => EditProjection();

            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            Init();
        }

        /// <summary>
        /// Creates a new instance of ProjectionViewer class
        /// </summary>
        public ProjectionPropertiesForm(ISpatialReference projection)
        {
            InitializeComponent();

            _projection = projection;
            _projectionMap1.Visible = false;
            linkLabel1.Visible = false;
            tabControl1.SelectedIndex = _lastTabIndex;

            // those aren't avaialable for unidentified projection
            tabControl1.TabPages.Remove(tabMap);
            tabControl1.TabPages.Remove(tabDialects);

            Init();
        }

        /// <summary>
        /// Shows information about selected projection
        /// </summary>
        public void ShowProjection(ICoordinateSystem projection)
        {
            if (projection == null) throw new NullReferenceException("projection");

            txtName.Text = projection.Name;
            txtCode.Text = projection.Code.ToString();

            _projection = new SpatialReference();
            if (!_projection.ImportFromEpsg(projection.Code))
            {
                // usupported projection
            }
            else
            {
                projectionTextBox1.ShowProjection(_projection.ExportToWkt());

                _projectionMap1.DrawCoordinateSystem(projection);
                _projectionMap1.ZoomToCoordinateSystem(projection);

                txtProj4.Text = _projection.ExportToProj4();

                txtAreaName.Text = projection.AreaName;
                txtRemarks.Text = projection.Remarks;
                txtScope.Text = projection.Scope;
            }

            // showing dialects
            if (_coordinateSystem != null)
            {
                _database.ReadDialects(_coordinateSystem);

                for (int i = 0; i < _coordinateSystem.Dialects.Count; i++)
                {
                    string s = _coordinateSystem.Dialects[i];
                    var item = listView1.Items.Add((i + 1).ToString());
                    UpdateDialectString(item, s);
                }

                if (listView1.Items.Count > 0)
                {
                    listView1.Items[0].Selected = true;
                }
            }
        }

        /// <summary>
        /// Shows information about unrecognized projection
        /// </summary>
        public void ShowProjection(ISpatialReference projection)
        {
            if (projection == null)
            {
                throw new NullReferenceException("Geoprojection wasn't passed");
            }

            _projection = projection;

            txtName.Text = projection.Name == "" ? "None" : projection.Name;
            txtCode.Text = "None";

            if (!projection.IsEmpty)
            {
                projectionTextBox1.ShowProjection(_projection.ExportToWkt());
                txtProj4.Text = _projection.ExportToProj4();

                txtAreaName.Text = "Not defined";
                txtScope.Text = "Not defined";
                txtRemarks.Text = "Unrecognized projection";
            }
        }

        /// <summary>
        /// Edits projection string. Returns true if editing took place
        /// </summary>
        private void EditProjection()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string text = listView1.SelectedItems[0].SubItems[2].Text;

                // building the list of available dialects
                var list = new List<string>();
                foreach (ListViewItem item in listView1.Items)
                {
                    if (!item.Selected)
                    {
                        list.Add(item.SubItems[2].Text);
                    }
                }

                using (var form = new EnterProjectionForm(_coordinateSystem, list, _database))
                {
                    form.textBox1.Text = text;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        listView1.SelectedItems[0].SubItems[2].Text = form.textBox1.Text;
                    }
                }
            }
        }

        private void Init()
        {
            LoadMapPreviewSettings();

            if (_coordinateSystem != null)
            {
                ShowProjection(_coordinateSystem);
            }
            else
            {
                ShowProjection(_projection);
            }

            FormClosed += (s, e) => _lastTabIndex = tabControl1.SelectedIndex;
        }

        private void LoadMapPreviewSettings()
        {
            _projectionMap1.LoadStateFromExeName(Application.ExecutablePath);
            _projectionMap1.ZoomBar.Visible = false;
            _projectionMap1.ScalebarVisible = false;
            _projectionMap1.ShowCoordinates = CoordinatesDisplay.None;
            _projectionMap1.ShowVersionNumber = false;
            _projectionMap1.TileProvider = TileProvider.None;
            _projectionMap1.ZoomBehavior = ZoomBehavior.Default;
        }

        private void SaveDialects()
        {
            _coordinateSystem.Dialects.Clear();
            foreach (ListViewItem item in listView1.Items)
            {
                _coordinateSystem.Dialects.Add(item.SubItems[2].Text);
            }
            _database.SaveDialects(_coordinateSystem);

            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Displays a single projection string
        /// </summary>
        /// <param name="item">Listview item</param>
        /// <param name="projection">String to display</param>
        private void UpdateDialectString(ListViewItem item, string projection)
        {
            var projTest = new SpatialReference();
            string projType = projTest.ImportFromProj4(projection) ? "proj4" : "WKT";

            item.SubItems.Add(projType);
            item.SubItems.Add(projection);
        }

        /// <summary>
        /// Adds a dialect formulation to the list
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            var list = listView1.Items.Cast<ListViewItem>().Select(item => item.SubItems[2].Text);
            using (var form = new EnterProjectionForm(_coordinateSystem, list, _database))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string s = (listView1.Items.Count + 1).ToString();
                    var item = listView1.Items.Add(s);
                    UpdateDialectString(item, form.textBox1.Text);
                    item.Selected = true;
                    _dialectsChanged = true;
                }
            }
        }

        /// <summary>
        /// Copies WRT string to clipboard
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            projectionTextBox1.SelectAll();
            projectionTextBox1.Copy();
            projectionTextBox1.SelectionLength = 0;
        }

        /// <summary>
        /// Saving dialects if needed
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_coordinateSystem != null && _dialectsChanged)
            {
                SaveDialects();
            }
        }

        /// <summary>
        /// Removes a dialect string from the list
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                listView1.Items.Remove(listView1.SelectedItems[0]);
                _dialectsChanged = true;
            }
        }

        private void buttonAdv2_Click(object sender, EventArgs e)
        {
            EditProjection();
        }

        /// <summary>
        /// Shows spatialreference.org page for the projection
        /// </summary>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "http://spatialreference.org/ref/epsg/" + txtCode.Text + "/";
            PathHelper.OpenUrl(url);
        }

        /// <summary>
        /// Shows string for the selected projection
        /// </summary>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = listView1.SelectedItems.Count > 0;
            txtDialect.ShowProjection(listView1.SelectedItems.Count > 0 ? listView1.SelectedItems[0].SubItems[2].Text : "");
        }

        /// <summary>
        /// Shows map control on the second tab only
        /// </summary>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _projectionMap1.Visible = tabControl1.SelectedIndex == 1;
        }
    }
}