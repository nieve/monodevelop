
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Xml.Formatting
{
	internal partial class XmlFormattingPolicyPanelWidget
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.VBox boxScopes;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView listView;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.VBox vbox4;
		private global::Gtk.Label labelScopes;
		private global::Gtk.Table tableScopes;
		private global::MonoDevelop.Components.PropertyGrid.PropertyGrid propertyGrid;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Button buttonAdvanced;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Xml.Formatting.XmlFormattingPolicyPanelWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.Xml.Formatting.XmlFormattingPolicyPanelWidget";
			// Container child MonoDevelop.Xml.Formatting.XmlFormattingPolicyPanelWidget.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.boxScopes = new global::Gtk.VBox ();
			this.boxScopes.Name = "boxScopes";
			this.boxScopes.Spacing = 6;
			// Container child boxScopes.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.listView = new global::Gtk.TreeView ();
			this.listView.CanFocus = true;
			this.listView.Name = "listView";
			this.listView.HeadersVisible = false;
			this.GtkScrolledWindow.Add (this.listView);
			this.boxScopes.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.boxScopes [this.GtkScrolledWindow]));
			w2.Position = 0;
			// Container child boxScopes.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.buttonAdd = new global::Gtk.Button ();
			this.buttonAdd.CanFocus = true;
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.UseStock = true;
			this.buttonAdd.UseUnderline = true;
			this.buttonAdd.Label = "gtk-add";
			this.hbox2.Add (this.buttonAdd);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.buttonAdd]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.buttonRemove = new global::Gtk.Button ();
			this.buttonRemove.CanFocus = true;
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.UseStock = true;
			this.buttonRemove.UseUnderline = true;
			this.buttonRemove.Label = "gtk-remove";
			this.hbox2.Add (this.buttonRemove);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.buttonRemove]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			this.boxScopes.Add (this.hbox2);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.boxScopes [this.hbox2]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.hbox1.Add (this.boxScopes);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.boxScopes]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.labelScopes = new global::Gtk.Label ();
			this.labelScopes.Name = "labelScopes";
			this.labelScopes.Xalign = 0F;
			this.labelScopes.LabelProp = global::Mono.Unix.Catalog.GetString ("Enter one or several xpath expressions to which this format applies:");
			this.vbox4.Add (this.labelScopes);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.labelScopes]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.tableScopes = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
			this.tableScopes.Name = "tableScopes";
			this.tableScopes.RowSpacing = ((uint)(6));
			this.tableScopes.ColumnSpacing = ((uint)(6));
			this.vbox4.Add (this.tableScopes);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.tableScopes]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.propertyGrid = new global::MonoDevelop.Components.PropertyGrid.PropertyGrid ();
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ShowToolbar = false;
			this.propertyGrid.ShowHelp = false;
			this.vbox4.Add (this.propertyGrid);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.propertyGrid]));
			w9.Position = 2;
			this.hbox1.Add (this.vbox4);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox4]));
			w10.Position = 1;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w11.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.buttonAdvanced = new global::Gtk.Button ();
			this.buttonAdvanced.CanFocus = true;
			this.buttonAdvanced.Name = "buttonAdvanced";
			this.buttonAdvanced.UseUnderline = true;
			this.buttonAdvanced.Label = global::Mono.Unix.Catalog.GetString ("Advanced Settings");
			this.hbox3.Add (this.buttonAdvanced);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.buttonAdvanced]));
			w12.Position = 0;
			w12.Expand = false;
			w12.Fill = false;
			this.vbox2.Add (this.hbox3);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox3]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
			this.buttonAdvanced.Clicked += new global::System.EventHandler (this.OnButtonAdvancedClicked);
		}
	}
}
