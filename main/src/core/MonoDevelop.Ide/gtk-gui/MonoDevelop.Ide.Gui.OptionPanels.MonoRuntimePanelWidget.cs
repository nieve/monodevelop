
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	internal partial class MonoRuntimePanelWidget
	{
		private global::Gtk.VBox vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.HBox hbox2;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TreeView tree;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Button buttonDefault;
		private global::Gtk.Button buttonAdd;
		private global::Gtk.Button buttonRemove;
		private global::Gtk.Label labelRunning;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Ide.Gui.OptionPanels.MonoRuntimePanelWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.Ide.Gui.OptionPanels.MonoRuntimePanelWidget";
			// Container child MonoDevelop.Ide.Gui.OptionPanels.MonoRuntimePanelWidget.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.WidthRequest = 500;
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("If you have a parallel installation of Mono you can register it here, so you can " +
					"use it for building and running projects. The <b>default runtime</b> is the .NET" +
					" runtime to be used for building and running applications when none is specifica" +
					"lly selected.");
			this.label1.UseMarkup = true;
			this.label1.Wrap = true;
			this.vbox1.Add (this.label1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.tree = new global::Gtk.TreeView ();
			this.tree.CanFocus = true;
			this.tree.Name = "tree";
			this.GtkScrolledWindow.Add (this.tree);
			this.hbox2.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.GtkScrolledWindow]));
			w3.Position = 0;
			// Container child hbox2.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.buttonDefault = new global::Gtk.Button ();
			this.buttonDefault.CanFocus = true;
			this.buttonDefault.Name = "buttonDefault";
			this.buttonDefault.UseUnderline = true;
			this.buttonDefault.Label = global::Mono.Unix.Catalog.GetString ("Set as Default");
			this.vbox2.Add (this.buttonDefault);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.buttonDefault]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.buttonAdd = new global::Gtk.Button ();
			this.buttonAdd.CanFocus = true;
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.UseStock = true;
			this.buttonAdd.UseUnderline = true;
			this.buttonAdd.Label = "gtk-add";
			this.vbox2.Add (this.buttonAdd);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.buttonAdd]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.buttonRemove = new global::Gtk.Button ();
			this.buttonRemove.CanFocus = true;
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.UseStock = true;
			this.buttonRemove.UseUnderline = true;
			this.buttonRemove.Label = "gtk-remove";
			this.vbox2.Add (this.buttonRemove);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.buttonRemove]));
			w6.Position = 2;
			w6.Expand = false;
			w6.Fill = false;
			this.hbox2.Add (this.vbox2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox2]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox1.Add (this.hbox2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox2]));
			w8.Position = 1;
			// Container child vbox1.Gtk.Box+BoxChild
			this.labelRunning = new global::Gtk.Label ();
			this.labelRunning.Name = "labelRunning";
			this.labelRunning.Xalign = 0F;
			this.labelRunning.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			this.vbox1.Add (this.labelRunning);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.labelRunning]));
			w9.Position = 2;
			w9.Expand = false;
			w9.Fill = false;
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.buttonDefault.Clicked += new global::System.EventHandler (this.OnButtonDefaultClicked);
			this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
		}
	}
}
