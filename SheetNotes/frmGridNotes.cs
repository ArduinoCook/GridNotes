using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SheetNotes
{
    public partial class frmGridNotes : Form
    {
        private string existingNotes = "";

        public frmGridNotes()
        {
            InitializeComponent();
        }

        private int existingNotesLength = 0;
        private void FRMSheetNotes_Load(object sender, EventArgs e)
        {
            string filePath = Path.Combine(
     Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
     "Sheet Notes.txt");

            if (File.Exists(filePath))
            {
                existingNotes = File.ReadAllText(filePath);

                richTextBox2.Text = existingNotes;
            }
            else
            {
                existingNotes = "";
                richTextBox2.Text = "";
            }

            richTextBox2.SelectionStart = 0;
            richTextBox2.SelectionLength = 0;
            richTextBox2.Focus();
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Sheet Notes.txt");

                string textToSave = richTextBox2.Text;

                File.WriteAllText(filePath, textToSave);

                MessageBox.Show("Saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving: " + ex.Message,
                    "Save Error");
            }
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveTabNames()
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Grid Notes Tabs.txt");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (TabPage tab in tabGrid.TabPages)
                {
                    writer.WriteLine(tab.Text);
                }
            }
        }

        private void AddTab_Click(object sender, EventArgs e)
        {
            TabPage newTab = new TabPage("New Tab");

            RichTextBox newRichTextBox = new RichTextBox();
            newRichTextBox.Dock = DockStyle.Fill;

            newTab.Controls.Add(newRichTextBox);

            tabGrid.TabPages.Add(newTab);

            tabGrid.SelectedTab = newTab;

            // Save the new tab
            SaveTabNames();
        }

        private void btnRenameTab_Click(object sender, EventArgs e)
        {
            if (tabGrid.SelectedTab == null)
                return;

            string currentName = tabGrid.SelectedTab.Text;

            using (Form renameForm = new Form())
            {
                renameForm.Text = "Rename Tab";
                renameForm.Size = new Size(350, 150);
                renameForm.StartPosition = FormStartPosition.CenterParent;
                renameForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                renameForm.MaximizeBox = false;
                renameForm.MinimizeBox = false;

                Label label = new Label();
                label.Text = "Enter new tab name:";
                label.Location = new Point(15, 15);
                label.AutoSize = true;

                TextBox textBox = new TextBox();
                textBox.Text = currentName;
                textBox.Location = new Point(15, 40);
                textBox.Width = 300;

                Button btnOK = new Button();
                btnOK.Text = "OK";
                btnOK.DialogResult = DialogResult.OK;
                btnOK.Location = new Point(155, 75);

                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.Location = new Point(240, 75);

                renameForm.Controls.Add(label);
                renameForm.Controls.Add(textBox);
                renameForm.Controls.Add(btnOK);
                renameForm.Controls.Add(btnCancel);

                renameForm.AcceptButton = btnOK;
                renameForm.CancelButton = btnCancel;

                textBox.SelectAll();
                textBox.Focus();

                if (renameForm.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        tabGrid.SelectedTab.Text = textBox.Text.Trim();

                        // Save the renamed tab
                        SaveTabNames();
                    }
                }
            }
        }

        private void btnDeleteTab_Click(object sender, EventArgs e)
        {
            if (tabGrid.SelectedTab == null)
                return;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this tab?",
                "Delete Tab",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                tabGrid.TabPages.Remove(tabGrid.SelectedTab);

                // Save the new tab list
                SaveTabNames();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Sheet Notes.txt");

                // Clear the text file
                File.WriteAllText(filePath, string.Empty);

                // Clear the RichTextBox
                richTextBox2.Clear();

                MessageBox.Show("All notes have been cleared.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error clearing notes: " + ex.Message,
                    "Clear Error");
            }
        }

        private void frmGridNotes_Load(object sender, EventArgs e)
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Grid Notes Tabs.txt");

            if (File.Exists(filePath))
            {
                string[] tabNames = File.ReadAllLines(filePath);

                foreach (string tabName in tabNames)
                {
                    if (string.IsNullOrWhiteSpace(tabName))
                        continue;

                    // Don't add the two original tabs again
                    if (tabName == "Notes" || tabName == "VBA Code")
                        continue;

                    TabPage newTab = new TabPage(tabName);

                    RichTextBox newRichTextBox = new RichTextBox();
                    newRichTextBox.Dock = DockStyle.Fill;

                    newTab.Controls.Add(newRichTextBox);

                    tabGrid.TabPages.Add(newTab);
                }
            }
        }
    }
}