
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Refactoring.Rename
{
	public partial class RenameItemDialog
	{
		private global::Gtk.VBox vbox;
		private global::Gtk.HBox hbox;
		private global::Gtk.Label labelNewName;
		private global::Gtk.Entry entry;
		private global::Gtk.CheckButton renameFileFlag;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Image imageWarning;
		private global::Gtk.Label labelWarning;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonPreview;
		private global::Gtk.Button buttonOk;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Refactoring.Rename.RenameItemDialog
			this.Name = "MonoDevelop.Refactoring.Rename.RenameItemDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Rename {0}");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.BorderWidth = ((uint)(6));
			// Internal child MonoDevelop.Refactoring.Rename.RenameItemDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox = new global::Gtk.VBox ();
			this.vbox.Name = "vbox";
			this.vbox.Spacing = 6;
			this.vbox.BorderWidth = ((uint)(6));
			// Container child vbox.Gtk.Box+BoxChild
			this.hbox = new global::Gtk.HBox ();
			this.hbox.Name = "hbox";
			this.hbox.Spacing = 6;
			// Container child hbox.Gtk.Box+BoxChild
			this.labelNewName = new global::Gtk.Label ();
			this.labelNewName.Name = "labelNewName";
			this.labelNewName.LabelProp = global::Mono.Unix.Catalog.GetString ("New na_me:");
			this.labelNewName.UseUnderline = true;
			this.hbox.Add (this.labelNewName);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox [this.labelNewName]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox.Gtk.Box+BoxChild
			this.entry = new global::Gtk.Entry ();
			this.entry.CanFocus = true;
			this.entry.Name = "entry";
			this.entry.IsEditable = true;
			this.entry.InvisibleChar = '●';
			this.hbox.Add (this.entry);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox [this.entry]));
			w3.Position = 1;
			this.vbox.Add (this.hbox);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox [this.hbox]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox.Gtk.Box+BoxChild
			this.renameFileFlag = new global::Gtk.CheckButton ();
			this.renameFileFlag.CanFocus = true;
			this.renameFileFlag.Name = "renameFileFlag";
			this.renameFileFlag.Label = global::Mono.Unix.Catalog.GetString ("Rename file that contains public class");
			this.renameFileFlag.Active = true;
			this.renameFileFlag.DrawIndicator = true;
			this.renameFileFlag.UseUnderline = true;
			this.vbox.Add (this.renameFileFlag);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox [this.renameFileFlag]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.imageWarning = new global::Gtk.Image ();
			this.imageWarning.Name = "imageWarning";
			this.imageWarning.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-apply", global::Gtk.IconSize.Button);
			this.hbox1.Add (this.imageWarning);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.imageWarning]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.labelWarning = new global::Gtk.Label ();
			this.labelWarning.Name = "labelWarning";
			this.hbox1.Add (this.labelWarning);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.labelWarning]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox.Add (this.hbox1);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox [this.hbox1]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			w1.Add (this.vbox);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Internal child MonoDevelop.Refactoring.Rename.RenameItemDialog.ActionArea
			global::Gtk.HButtonBox w10 = this.ActionArea;
			w10.Name = "dialog1_ActionArea";
			w10.Spacing = 10;
			w10.BorderWidth = ((uint)(5));
			w10.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonCancel]));
			w11.Expand = false;
			w11.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonPreview = new global::Gtk.Button ();
			this.buttonPreview.CanFocus = true;
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.UseUnderline = true;
			this.buttonPreview.Label = global::Mono.Unix.Catalog.GetString ("_Preview");
			this.AddActionWidget (this.buttonPreview, 0);
			global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonPreview]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w13 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonOk]));
			w13.Position = 2;
			w13.Expand = false;
			w13.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 365;
			this.DefaultHeight = 154;
			this.labelNewName.MnemonicWidget = this.entry;
			this.renameFileFlag.Hide ();
			this.Hide ();
		}
	}
}
