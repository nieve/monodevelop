
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Deployment.Linux
{
	public partial class DotDesktopViewWidget
	{
		private global::Gtk.Notebook notebook;
		private global::Gtk.ScrolledWindow scrolledwindow1;
		private global::Gtk.VBox vbox4;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Label label9;
		private global::Gtk.ComboBox comboType;
		private global::Gtk.HSeparator hseparator4;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Label label1;
		private global::Gtk.ComboBox comboLocales;
		private global::Gtk.Button buttonNewLocale;
		private global::Gtk.Table table6;
		private global::Gtk.Entry entryComment;
		private global::Gtk.Entry entryGenericName;
		private global::Gtk.Entry entryIcon;
		private global::Gtk.Entry entryName;
		private global::Gtk.Label label10;
		private global::Gtk.Label label11;
		private global::Gtk.Label label13;
		private global::Gtk.Label label20;
		private global::Gtk.HSeparator hseparator2;
		private global::Gtk.Table tableCommand;
		private global::Gtk.CheckButton checkTerminal;
		private global::Gtk.Entry entryExec;
		private global::Gtk.Entry entryPath;
		private global::Gtk.Entry entryTryExec;
		private global::Gtk.Label label14;
		private global::Gtk.Label label15;
		private global::Gtk.Label label16;
		private global::Gtk.HBox boxUrl;
		private global::Gtk.Label label18;
		private global::Gtk.Entry entryUrl;
		private global::Gtk.Label label7;
		private global::Gtk.VBox boxMenu;
		private global::Gtk.CheckButton checkShowInMenu;
		private global::Gtk.HSeparator hseparator5;
		private global::Gtk.VBox boxCategories;
		private global::Gtk.Label label17;
		private global::Gtk.HBox hbox6;
		private global::Gtk.ScrolledWindow scrolledwindow2;
		private global::Gtk.TreeView treeCategories;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Button buttonAddCategories;
		private global::Gtk.Button buttonRemoveCategory;
		private global::Gtk.HSeparator hseparator6;
		private global::Gtk.Label label3;
		private global::Gtk.RadioButton radioAlwaysShow;
		private global::Gtk.RadioButton radioOnlyShowIn;
		private global::Gtk.RadioButton radioNotShowIn;
		private global::Gtk.Frame frame1;
		private global::Gtk.Alignment GtkAlignment4;
		private global::Gtk.TreeView treeEnvs;
		private global::Gtk.Label label2;
		private global::Gtk.Table tableMimeTypes;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Label label4;
		private global::Gtk.ScrolledWindow scrolledwindow3;
		private global::Gtk.TreeView treeMimeTypes;
		private global::Gtk.VBox vbox5;
		private global::Gtk.Button buttonAddMimeType;
		private global::Gtk.Button buttonRemoveMimeType;
		private global::Gtk.Label label8;
		private global::Gtk.HBox hbox4;
		private global::Gtk.ScrolledWindow scrolledwindow4;
		private global::Gtk.TreeView treeEntries;
		private global::Gtk.VBox vbox7;
		private global::Gtk.Button buttonAddEntry;
		private global::Gtk.Button buttonRemoveEntry;
		private global::Gtk.Label label5;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Deployment.Linux.DotDesktopViewWidget
			global::Stetic.BinContainer.Attach (this);
			this.CanFocus = true;
			this.Name = "MonoDevelop.Deployment.Linux.DotDesktopViewWidget";
			// Container child MonoDevelop.Deployment.Linux.DotDesktopViewWidget.Gtk.Container+ContainerChild
			this.notebook = new global::Gtk.Notebook ();
			this.notebook.CanFocus = true;
			this.notebook.Name = "notebook";
			this.notebook.CurrentPage = 0;
			this.notebook.TabPos = ((global::Gtk.PositionType)(0));
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			// Container child scrolledwindow1.Gtk.Container+ContainerChild
			global::Gtk.Viewport w1 = new global::Gtk.Viewport ();
			w1.CanFocus = true;
			w1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child GtkViewport.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 12;
			this.vbox4.BorderWidth = ((uint)(12));
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 0F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Desktop Entry Type:");
			this.hbox2.Add (this.label9);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.label9]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.comboType = global::Gtk.ComboBox.NewText ();
			this.comboType.AppendText (global::Mono.Unix.Catalog.GetString ("Application"));
			this.comboType.AppendText (global::Mono.Unix.Catalog.GetString ("Link"));
			this.comboType.AppendText (global::Mono.Unix.Catalog.GetString ("Directory"));
			this.comboType.Name = "comboType";
			this.comboType.Active = 0;
			this.hbox2.Add (this.comboType);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.comboType]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			this.vbox4.Add (this.hbox2);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox2]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hseparator4 = new global::Gtk.HSeparator ();
			this.hseparator4.CanFocus = true;
			this.hseparator4.Name = "hseparator4";
			this.vbox4.Add (this.hseparator4);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hseparator4]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Show strings for locale:");
			this.hbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label1]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.comboLocales = global::Gtk.ComboBox.NewText ();
			this.comboLocales.Name = "comboLocales";
			this.hbox3.Add (this.comboLocales);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.comboLocales]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.buttonNewLocale = new global::Gtk.Button ();
			this.buttonNewLocale.CanFocus = true;
			this.buttonNewLocale.Name = "buttonNewLocale";
			this.buttonNewLocale.Label = global::Mono.Unix.Catalog.GetString ("New locale...");
			this.hbox3.Add (this.buttonNewLocale);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.buttonNewLocale]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			this.vbox4.Add (this.hbox3);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox3]));
			w9.Position = 2;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.table6 = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.table6.CanFocus = true;
			this.table6.Name = "table6";
			this.table6.RowSpacing = ((uint)(6));
			this.table6.ColumnSpacing = ((uint)(6));
			// Container child table6.Gtk.Table+TableChild
			this.entryComment = new global::Gtk.Entry ();
			this.entryComment.CanFocus = true;
			this.entryComment.Name = "entryComment";
			this.entryComment.IsEditable = true;
			this.entryComment.InvisibleChar = '●';
			this.table6.Add (this.entryComment);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table6 [this.entryComment]));
			w10.TopAttach = ((uint)(2));
			w10.BottomAttach = ((uint)(3));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.entryGenericName = new global::Gtk.Entry ();
			this.entryGenericName.CanFocus = true;
			this.entryGenericName.Name = "entryGenericName";
			this.entryGenericName.IsEditable = true;
			this.entryGenericName.InvisibleChar = '●';
			this.table6.Add (this.entryGenericName);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table6 [this.entryGenericName]));
			w11.TopAttach = ((uint)(1));
			w11.BottomAttach = ((uint)(2));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.entryIcon = new global::Gtk.Entry ();
			this.entryIcon.CanFocus = true;
			this.entryIcon.Name = "entryIcon";
			this.entryIcon.IsEditable = true;
			this.entryIcon.InvisibleChar = '●';
			this.table6.Add (this.entryIcon);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table6 [this.entryIcon]));
			w12.TopAttach = ((uint)(3));
			w12.BottomAttach = ((uint)(4));
			w12.LeftAttach = ((uint)(1));
			w12.RightAttach = ((uint)(2));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.WidthChars = 50;
			this.entryName.InvisibleChar = '●';
			this.table6.Add (this.entryName);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table6 [this.entryName]));
			w13.LeftAttach = ((uint)(1));
			w13.RightAttach = ((uint)(2));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 0F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Icon:");
			this.table6.Add (this.label10);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table6 [this.label10]));
			w14.TopAttach = ((uint)(3));
			w14.BottomAttach = ((uint)(4));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 0F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Generic name:");
			this.table6.Add (this.label11);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table6 [this.label11]));
			w15.TopAttach = ((uint)(1));
			w15.BottomAttach = ((uint)(2));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.Xalign = 0F;
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("Comment:");
			this.table6.Add (this.label13);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table6 [this.label13]));
			w16.TopAttach = ((uint)(2));
			w16.BottomAttach = ((uint)(3));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table6.Gtk.Table+TableChild
			this.label20 = new global::Gtk.Label ();
			this.label20.Name = "label20";
			this.label20.Xalign = 0F;
			this.label20.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table6.Add (this.label20);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table6 [this.label20]));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox4.Add (this.table6);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.table6]));
			w18.Position = 3;
			w18.Expand = false;
			w18.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.vbox4.Add (this.hseparator2);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hseparator2]));
			w19.Position = 4;
			w19.Expand = false;
			w19.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.tableCommand = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.tableCommand.Name = "tableCommand";
			this.tableCommand.RowSpacing = ((uint)(6));
			this.tableCommand.ColumnSpacing = ((uint)(6));
			// Container child tableCommand.Gtk.Table+TableChild
			this.checkTerminal = new global::Gtk.CheckButton ();
			this.checkTerminal.CanFocus = true;
			this.checkTerminal.Name = "checkTerminal";
			this.checkTerminal.Label = global::Mono.Unix.Catalog.GetString ("Run in terminal");
			this.checkTerminal.DrawIndicator = true;
			this.tableCommand.Add (this.checkTerminal);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.checkTerminal]));
			w20.TopAttach = ((uint)(3));
			w20.BottomAttach = ((uint)(4));
			w20.RightAttach = ((uint)(2));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableCommand.Gtk.Table+TableChild
			this.entryExec = new global::Gtk.Entry ();
			this.entryExec.CanFocus = true;
			this.entryExec.Name = "entryExec";
			this.entryExec.IsEditable = true;
			this.entryExec.InvisibleChar = '●';
			this.tableCommand.Add (this.entryExec);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.entryExec]));
			w21.LeftAttach = ((uint)(1));
			w21.RightAttach = ((uint)(2));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableCommand.Gtk.Table+TableChild
			this.entryPath = new global::Gtk.Entry ();
			this.entryPath.CanFocus = true;
			this.entryPath.Name = "entryPath";
			this.entryPath.IsEditable = true;
			this.entryPath.InvisibleChar = '●';
			this.tableCommand.Add (this.entryPath);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.entryPath]));
			w22.TopAttach = ((uint)(2));
			w22.BottomAttach = ((uint)(3));
			w22.LeftAttach = ((uint)(1));
			w22.RightAttach = ((uint)(2));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableCommand.Gtk.Table+TableChild
			this.entryTryExec = new global::Gtk.Entry ();
			this.entryTryExec.CanFocus = true;
			this.entryTryExec.Name = "entryTryExec";
			this.entryTryExec.IsEditable = true;
			this.entryTryExec.InvisibleChar = '●';
			this.tableCommand.Add (this.entryTryExec);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.entryTryExec]));
			w23.TopAttach = ((uint)(1));
			w23.BottomAttach = ((uint)(2));
			w23.LeftAttach = ((uint)(1));
			w23.RightAttach = ((uint)(2));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableCommand.Gtk.Table+TableChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xalign = 0F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("Command:");
			this.tableCommand.Add (this.label14);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.label14]));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableCommand.Gtk.Table+TableChild
			this.label15 = new global::Gtk.Label ();
			this.label15.Name = "label15";
			this.label15.Xalign = 0F;
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("Test exe:");
			this.tableCommand.Add (this.label15);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.label15]));
			w25.TopAttach = ((uint)(1));
			w25.BottomAttach = ((uint)(2));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableCommand.Gtk.Table+TableChild
			this.label16 = new global::Gtk.Label ();
			this.label16.Name = "label16";
			this.label16.Xalign = 0F;
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("Working path:");
			this.tableCommand.Add (this.label16);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.tableCommand [this.label16]));
			w26.TopAttach = ((uint)(2));
			w26.BottomAttach = ((uint)(3));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox4.Add (this.tableCommand);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.tableCommand]));
			w27.Position = 5;
			w27.Expand = false;
			w27.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.boxUrl = new global::Gtk.HBox ();
			this.boxUrl.Name = "boxUrl";
			this.boxUrl.Spacing = 6;
			// Container child boxUrl.Gtk.Box+BoxChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.Xalign = 0F;
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("Url:");
			this.boxUrl.Add (this.label18);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.boxUrl [this.label18]));
			w28.Position = 0;
			w28.Expand = false;
			w28.Fill = false;
			// Container child boxUrl.Gtk.Box+BoxChild
			this.entryUrl = new global::Gtk.Entry ();
			this.entryUrl.CanFocus = true;
			this.entryUrl.Name = "entryUrl";
			this.entryUrl.IsEditable = true;
			this.entryUrl.InvisibleChar = '●';
			this.boxUrl.Add (this.entryUrl);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.boxUrl [this.entryUrl]));
			w29.Position = 1;
			this.vbox4.Add (this.boxUrl);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.boxUrl]));
			w30.Position = 6;
			w30.Expand = false;
			w30.Fill = false;
			w1.Add (this.vbox4);
			this.scrolledwindow1.Add (w1);
			this.notebook.Add (this.scrolledwindow1);
			// Notebook tab
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Header");
			this.notebook.SetTabLabel (this.scrolledwindow1, this.label7);
			this.label7.ShowAll ();
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.boxMenu = new global::Gtk.VBox ();
			this.boxMenu.Name = "boxMenu";
			this.boxMenu.Spacing = 6;
			this.boxMenu.BorderWidth = ((uint)(12));
			// Container child boxMenu.Gtk.Box+BoxChild
			this.checkShowInMenu = new global::Gtk.CheckButton ();
			this.checkShowInMenu.CanFocus = true;
			this.checkShowInMenu.Name = "checkShowInMenu";
			this.checkShowInMenu.Label = global::Mono.Unix.Catalog.GetString ("Show in desktop menu");
			this.checkShowInMenu.DrawIndicator = true;
			this.boxMenu.Add (this.checkShowInMenu);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.checkShowInMenu]));
			w34.Position = 0;
			w34.Expand = false;
			w34.Fill = false;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.hseparator5 = new global::Gtk.HSeparator ();
			this.hseparator5.Name = "hseparator5";
			this.boxMenu.Add (this.hseparator5);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.hseparator5]));
			w35.Position = 1;
			w35.Expand = false;
			w35.Fill = false;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.boxCategories = new global::Gtk.VBox ();
			this.boxCategories.Name = "boxCategories";
			this.boxCategories.Spacing = 6;
			// Container child boxCategories.Gtk.Box+BoxChild
			this.label17 = new global::Gtk.Label ();
			this.label17.Name = "label17";
			this.label17.Xalign = 0F;
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("Menu categories:");
			this.boxCategories.Add (this.label17);
			global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.boxCategories [this.label17]));
			w36.Position = 0;
			w36.Expand = false;
			w36.Fill = false;
			// Container child boxCategories.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.scrolledwindow2 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow2.HeightRequest = 150;
			this.scrolledwindow2.CanFocus = true;
			this.scrolledwindow2.Name = "scrolledwindow2";
			this.scrolledwindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow2.Gtk.Container+ContainerChild
			this.treeCategories = new global::Gtk.TreeView ();
			this.treeCategories.CanFocus = true;
			this.treeCategories.Name = "treeCategories";
			this.scrolledwindow2.Add (this.treeCategories);
			this.hbox6.Add (this.scrolledwindow2);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.scrolledwindow2]));
			w38.Position = 0;
			// Container child hbox6.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.CanFocus = true;
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.buttonAddCategories = new global::Gtk.Button ();
			this.buttonAddCategories.CanFocus = true;
			this.buttonAddCategories.Name = "buttonAddCategories";
			this.buttonAddCategories.UseStock = true;
			this.buttonAddCategories.UseUnderline = true;
			this.buttonAddCategories.Label = "gtk-add";
			this.vbox3.Add (this.buttonAddCategories);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.buttonAddCategories]));
			w39.Position = 0;
			w39.Expand = false;
			w39.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.buttonRemoveCategory = new global::Gtk.Button ();
			this.buttonRemoveCategory.CanFocus = true;
			this.buttonRemoveCategory.Name = "buttonRemoveCategory";
			this.buttonRemoveCategory.UseStock = true;
			this.buttonRemoveCategory.UseUnderline = true;
			this.buttonRemoveCategory.Label = "gtk-remove";
			this.vbox3.Add (this.buttonRemoveCategory);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.buttonRemoveCategory]));
			w40.Position = 1;
			w40.Expand = false;
			w40.Fill = false;
			this.hbox6.Add (this.vbox3);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.vbox3]));
			w41.Position = 1;
			w41.Expand = false;
			w41.Fill = false;
			this.boxCategories.Add (this.hbox6);
			global::Gtk.Box.BoxChild w42 = ((global::Gtk.Box.BoxChild)(this.boxCategories [this.hbox6]));
			w42.Position = 1;
			// Container child boxCategories.Gtk.Box+BoxChild
			this.hseparator6 = new global::Gtk.HSeparator ();
			this.hseparator6.CanFocus = true;
			this.hseparator6.Name = "hseparator6";
			this.boxCategories.Add (this.hseparator6);
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(this.boxCategories [this.hseparator6]));
			w43.Position = 2;
			w43.Expand = false;
			w43.Fill = false;
			this.boxMenu.Add (this.boxCategories);
			global::Gtk.Box.BoxChild w44 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.boxCategories]));
			w44.Position = 2;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.CanFocus = true;
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Select the environments that should display this desktop entry:");
			this.boxMenu.Add (this.label3);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.label3]));
			w45.Position = 3;
			w45.Expand = false;
			w45.Fill = false;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.radioAlwaysShow = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Always show"));
			this.radioAlwaysShow.CanFocus = true;
			this.radioAlwaysShow.Name = "radioAlwaysShow";
			this.radioAlwaysShow.Active = true;
			this.radioAlwaysShow.DrawIndicator = true;
			this.radioAlwaysShow.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.boxMenu.Add (this.radioAlwaysShow);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.radioAlwaysShow]));
			w46.Position = 4;
			w46.Expand = false;
			w46.Fill = false;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.radioOnlyShowIn = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Only show in the following environments:"));
			this.radioOnlyShowIn.CanFocus = true;
			this.radioOnlyShowIn.Name = "radioOnlyShowIn";
			this.radioOnlyShowIn.DrawIndicator = true;
			this.radioOnlyShowIn.Group = this.radioAlwaysShow.Group;
			this.boxMenu.Add (this.radioOnlyShowIn);
			global::Gtk.Box.BoxChild w47 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.radioOnlyShowIn]));
			w47.Position = 5;
			w47.Expand = false;
			w47.Fill = false;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.radioNotShowIn = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Not show in the following environments:"));
			this.radioNotShowIn.CanFocus = true;
			this.radioNotShowIn.Name = "radioNotShowIn";
			this.radioNotShowIn.DrawIndicator = true;
			this.radioNotShowIn.Group = this.radioAlwaysShow.Group;
			this.boxMenu.Add (this.radioNotShowIn);
			global::Gtk.Box.BoxChild w48 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.radioNotShowIn]));
			w48.Position = 6;
			w48.Expand = false;
			w48.Fill = false;
			// Container child boxMenu.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.treeEnvs = new global::Gtk.TreeView ();
			this.treeEnvs.CanFocus = true;
			this.treeEnvs.Name = "treeEnvs";
			this.GtkAlignment4.Add (this.treeEnvs);
			this.frame1.Add (this.GtkAlignment4);
			this.boxMenu.Add (this.frame1);
			global::Gtk.Box.BoxChild w51 = ((global::Gtk.Box.BoxChild)(this.boxMenu [this.frame1]));
			w51.Position = 7;
			w51.Expand = false;
			w51.Fill = false;
			this.notebook.Add (this.boxMenu);
			global::Gtk.Notebook.NotebookChild w52 = ((global::Gtk.Notebook.NotebookChild)(this.notebook [this.boxMenu]));
			w52.Position = 1;
			// Notebook tab
			this.label2 = new global::Gtk.Label ();
			this.label2.CanFocus = true;
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Menu entry");
			this.notebook.SetTabLabel (this.boxMenu, this.label2);
			this.label2.ShowAll ();
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.tableMimeTypes = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.tableMimeTypes.Name = "tableMimeTypes";
			this.tableMimeTypes.RowSpacing = ((uint)(6));
			this.tableMimeTypes.ColumnSpacing = ((uint)(6));
			this.tableMimeTypes.BorderWidth = ((uint)(12));
			// Container child tableMimeTypes.Gtk.Table+TableChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("MIME types supported by this application:");
			this.hbox1.Add (this.label4);
			global::Gtk.Box.BoxChild w53 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label4]));
			w53.Position = 0;
			w53.Expand = false;
			w53.Fill = false;
			this.tableMimeTypes.Add (this.hbox1);
			global::Gtk.Table.TableChild w54 = ((global::Gtk.Table.TableChild)(this.tableMimeTypes [this.hbox1]));
			w54.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableMimeTypes.Gtk.Table+TableChild
			this.scrolledwindow3 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow3.CanFocus = true;
			this.scrolledwindow3.Name = "scrolledwindow3";
			this.scrolledwindow3.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow3.Gtk.Container+ContainerChild
			this.treeMimeTypes = new global::Gtk.TreeView ();
			this.treeMimeTypes.CanFocus = true;
			this.treeMimeTypes.Name = "treeMimeTypes";
			this.scrolledwindow3.Add (this.treeMimeTypes);
			this.tableMimeTypes.Add (this.scrolledwindow3);
			global::Gtk.Table.TableChild w56 = ((global::Gtk.Table.TableChild)(this.tableMimeTypes [this.scrolledwindow3]));
			w56.TopAttach = ((uint)(1));
			w56.BottomAttach = ((uint)(2));
			// Container child tableMimeTypes.Gtk.Table+TableChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.buttonAddMimeType = new global::Gtk.Button ();
			this.buttonAddMimeType.CanFocus = true;
			this.buttonAddMimeType.Name = "buttonAddMimeType";
			this.buttonAddMimeType.UseStock = true;
			this.buttonAddMimeType.UseUnderline = true;
			this.buttonAddMimeType.Label = "gtk-add";
			this.vbox5.Add (this.buttonAddMimeType);
			global::Gtk.Box.BoxChild w57 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.buttonAddMimeType]));
			w57.Position = 0;
			w57.Expand = false;
			w57.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.buttonRemoveMimeType = new global::Gtk.Button ();
			this.buttonRemoveMimeType.CanFocus = true;
			this.buttonRemoveMimeType.Name = "buttonRemoveMimeType";
			this.buttonRemoveMimeType.UseStock = true;
			this.buttonRemoveMimeType.UseUnderline = true;
			this.buttonRemoveMimeType.Label = "gtk-remove";
			this.vbox5.Add (this.buttonRemoveMimeType);
			global::Gtk.Box.BoxChild w58 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.buttonRemoveMimeType]));
			w58.Position = 1;
			w58.Expand = false;
			w58.Fill = false;
			this.tableMimeTypes.Add (this.vbox5);
			global::Gtk.Table.TableChild w59 = ((global::Gtk.Table.TableChild)(this.tableMimeTypes [this.vbox5]));
			w59.TopAttach = ((uint)(1));
			w59.BottomAttach = ((uint)(2));
			w59.LeftAttach = ((uint)(1));
			w59.RightAttach = ((uint)(2));
			w59.XOptions = ((global::Gtk.AttachOptions)(4));
			this.notebook.Add (this.tableMimeTypes);
			global::Gtk.Notebook.NotebookChild w60 = ((global::Gtk.Notebook.NotebookChild)(this.notebook [this.tableMimeTypes]));
			w60.Position = 2;
			// Notebook tab
			this.label8 = new global::Gtk.Label ();
			this.label8.CanFocus = true;
			this.label8.Name = "label8";
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Mime types");
			this.notebook.SetTabLabel (this.tableMimeTypes, this.label8);
			this.label8.ShowAll ();
			// Container child notebook.Gtk.Notebook+NotebookChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			this.hbox4.BorderWidth = ((uint)(12));
			// Container child hbox4.Gtk.Box+BoxChild
			this.scrolledwindow4 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow4.CanFocus = true;
			this.scrolledwindow4.Name = "scrolledwindow4";
			this.scrolledwindow4.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child scrolledwindow4.Gtk.Container+ContainerChild
			this.treeEntries = new global::Gtk.TreeView ();
			this.treeEntries.CanFocus = true;
			this.treeEntries.Name = "treeEntries";
			this.scrolledwindow4.Add (this.treeEntries);
			this.hbox4.Add (this.scrolledwindow4);
			global::Gtk.Box.BoxChild w62 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.scrolledwindow4]));
			w62.Position = 0;
			// Container child hbox4.Gtk.Box+BoxChild
			this.vbox7 = new global::Gtk.VBox ();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			// Container child vbox7.Gtk.Box+BoxChild
			this.buttonAddEntry = new global::Gtk.Button ();
			this.buttonAddEntry.CanFocus = true;
			this.buttonAddEntry.Name = "buttonAddEntry";
			this.buttonAddEntry.UseStock = true;
			this.buttonAddEntry.UseUnderline = true;
			this.buttonAddEntry.Label = "gtk-add";
			this.vbox7.Add (this.buttonAddEntry);
			global::Gtk.Box.BoxChild w63 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.buttonAddEntry]));
			w63.Position = 0;
			w63.Expand = false;
			w63.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.buttonRemoveEntry = new global::Gtk.Button ();
			this.buttonRemoveEntry.CanFocus = true;
			this.buttonRemoveEntry.Name = "buttonRemoveEntry";
			this.buttonRemoveEntry.UseStock = true;
			this.buttonRemoveEntry.UseUnderline = true;
			this.buttonRemoveEntry.Label = "gtk-remove";
			this.vbox7.Add (this.buttonRemoveEntry);
			global::Gtk.Box.BoxChild w64 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.buttonRemoveEntry]));
			w64.Position = 1;
			w64.Expand = false;
			w64.Fill = false;
			this.hbox4.Add (this.vbox7);
			global::Gtk.Box.BoxChild w65 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.vbox7]));
			w65.Position = 1;
			w65.Expand = false;
			w65.Fill = false;
			this.notebook.Add (this.hbox4);
			global::Gtk.Notebook.NotebookChild w66 = ((global::Gtk.Notebook.NotebookChild)(this.notebook [this.hbox4]));
			w66.Position = 3;
			// Notebook tab
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Other entries");
			this.notebook.SetTabLabel (this.hbox4, this.label5);
			this.label5.ShowAll ();
			this.Add (this.notebook);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.comboType.Changed += new global::System.EventHandler (this.OnComboTypeChanged);
			this.comboLocales.Changed += new global::System.EventHandler (this.OnComboLocalesChanged);
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.entryIcon.Changed += new global::System.EventHandler (this.OnEntryIconChanged);
			this.entryGenericName.Changed += new global::System.EventHandler (this.OnEntryGenericNameChanged);
			this.entryComment.Changed += new global::System.EventHandler (this.OnEntryCommentChanged);
			this.entryTryExec.Changed += new global::System.EventHandler (this.OnEntryTryExecChanged);
			this.entryPath.Changed += new global::System.EventHandler (this.OnEntryPathChanged);
			this.entryExec.Changed += new global::System.EventHandler (this.OnEntryExecChanged);
			this.checkTerminal.Clicked += new global::System.EventHandler (this.OnCheckTerminalClicked);
			this.entryUrl.Changed += new global::System.EventHandler (this.OnEntryUrlChanged);
			this.checkShowInMenu.Clicked += new global::System.EventHandler (this.OnCheckShowInMenuClicked);
			this.buttonAddCategories.Clicked += new global::System.EventHandler (this.OnButtonAddCategoriesClicked);
			this.buttonRemoveCategory.Clicked += new global::System.EventHandler (this.OnButtonRemoveCategoryClicked);
			this.radioAlwaysShow.Clicked += new global::System.EventHandler (this.OnRadioAlwaysShowClicked);
			this.radioOnlyShowIn.Clicked += new global::System.EventHandler (this.OnRadioOnlyShowInClicked);
			this.radioNotShowIn.Clicked += new global::System.EventHandler (this.OnRadioNotShowInClicked);
			this.buttonAddMimeType.Clicked += new global::System.EventHandler (this.OnButtonAddMimeTypeClicked);
			this.buttonRemoveMimeType.Clicked += new global::System.EventHandler (this.OnButtonRemoveMimeTypeClicked);
			this.buttonAddEntry.Clicked += new global::System.EventHandler (this.OnButtonAddEntryClicked);
			this.buttonRemoveEntry.Clicked += new global::System.EventHandler (this.OnButtonRemoveEntryClicked);
		}
	}
}
