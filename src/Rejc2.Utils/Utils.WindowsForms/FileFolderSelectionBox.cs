using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utils.WindowsForms
{
	public partial class FileFolderSelectionBox : UserControl
	{
		public FileFolderSelectionBox()
		{
			InitializeComponent();
		}

		public override string Text
		{
			get
			{
				return textBox1.Text;
			}
			set
			{
				textBox1.Text = value;
			}
		}

		public FileFolderSelectionTypes SelectionType { get; set; }

		private void button1_Click(object sender, EventArgs e)
		{
			if ((SelectionType & FileFolderSelectionTypes.Folder) != 0)
			{
				folderBrowserDialog1.SelectedPath = textBox1.Text;
				folderBrowserDialog1.ShowNewFolderButton = ((SelectionType & FileFolderSelectionTypes.Save) != 0);
				var result = folderBrowserDialog1.ShowDialog(this);
				if (result == DialogResult.OK)
				{
					textBox1.Text = folderBrowserDialog1.SelectedPath;
				}
			}
		}
	}
}
