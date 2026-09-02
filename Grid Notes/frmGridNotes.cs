
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SheetNotes
{
    public partial class frmGridNotes : Form
    {
        private bool hasLoadedTabs = false;

        public frmGridNotes()
        {
            InitializeComponent();
        }


        // ============================================================
        // FIND THE RICHTEXTBOX ON A TAB
        // ============================================================

        private RichTextBox FindRichTextBox(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is RichTextBox richTextBox)
                    return richTextBox;

                RichTextBox found = FindRichTextBox(control);

                if (found != null)
                    return found;
            }

            return null;
        }


        // ============================================================
        // SAVE THE CURRENT TAB
        // ============================================================

        private void SaveCurrentTabText()
        {
            if (tabGrid.SelectedTab == null)
                return;

            string tabName = tabGrid.SelectedTab.Text;

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                tabName + ".txt");

            RichTextBox richTextBox =
                FindRichTextBox(tabGrid.SelectedTab);

            if (richTextBox == null)
                return;

            File.WriteAllText(filePath, richTextBox.Text);
        }


        // ============================================================
        // LOAD A TAB'S SAVED TEXT
        // ============================================================

        private void LoadTabText(TabPage tab)
        {
            string tabName = tab.Text;

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                tabName + ".txt");

            if (!File.Exists(filePath))
                return;

            RichTextBox richTextBox =
                FindRichTextBox(tab);

            if (richTextBox == null)
                return;

            richTextBox.Text = File.ReadAllText(filePath);
        }


        // ============================================================
        // SAVE TAB NAMES
        // ============================================================

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


        // ============================================================
        // LOAD SAVED TABS
        // ============================================================

        private void LoadSavedTabs()
        {
            // Prevent the tabs from being loaded more than once.
            if (hasLoadedTabs)
                return;

            hasLoadedTabs = true;

            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Grid Notes Tabs.txt");

            if (!File.Exists(filePath))
                return;

            string[] tabNames = File.ReadAllLines(filePath);

            foreach (string tabName in tabNames)
            {
                if (string.IsNullOrWhiteSpace(tabName))
                    continue;

                // Don't add the two original tabs again.
                if (tabName == "Notes" || tabName == "VBA Code")
                    continue;

                TabPage newTab = new TabPage(tabName);

                RichTextBox newRichTextBox = new RichTextBox();
                newRichTextBox.Dock = DockStyle.Fill;

                newTab.Controls.Add(newRichTextBox);

                tabGrid.TabPages.Add(newTab);

                // Load this tab's saved text.
                LoadTabText(newTab);
            }
        }


        // ============================================================
        // FORM LOAD
        // ============================================================

        private void frmGridNotes_Load(object sender, EventArgs e)
        {
            // Load the original Notes tab
            LoadTabText(tabPage2);

            // Load the original VBA Code tab
            LoadTabText(tabPage4);

            // Load any additional tabs created by the user
            LoadSavedTabs();
        }


        // ============================================================
        // OLD FORM LOAD EVENT
        // ============================================================
        //
        // This is kept because your Designer may still be connected
        // to the old event name. It now simply loads the saved tabs.
        //

        


        // ============================================================
        // SAVE BUTTON
        // ============================================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCurrentTabText();

                MessageBox.Show(
                    "Saved successfully.",
                    "Save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error saving: " + ex.Message,
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ============================================================
        // CLOSE BUTTON
        // ============================================================

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }


        // ============================================================
        // ADD TAB
        // ============================================================

        private void AddTab_Click(object sender, EventArgs e)
        {
            TabPage newTab = new TabPage("New Tab");

            RichTextBox newRichTextBox = new RichTextBox();
            newRichTextBox.Dock = DockStyle.Fill;

            newTab.Controls.Add(newRichTextBox);

            tabGrid.TabPages.Add(newTab);

            tabGrid.SelectedTab = newTab;

            // Save the new tab name.
            SaveTabNames();
        }


        // ============================================================
        // RENAME TAB
        // ============================================================

        private void btnRenameTab_Click(object sender, EventArgs e)
        {
            if (tabGrid.SelectedTab == null)
                return;

            string oldName = tabGrid.SelectedTab.Text;

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
                textBox.Text = oldName;
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
                    string newName = textBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(newName))
                        return;

                    if (newName == oldName)
                        return;

                    string documentsPath =
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyDocuments);

                    string oldFilePath =
                        Path.Combine(documentsPath, oldName + ".txt");

                    string newFilePath =
                        Path.Combine(documentsPath, newName + ".txt");

                    // Rename the text file if it exists.
                    if (File.Exists(oldFilePath))
                    {
                        if (File.Exists(newFilePath))
                        {
                            MessageBox.Show(
                                "A text file with that tab name already exists.",
                                "Rename Tab",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                            return;
                        }

                        File.Move(oldFilePath, newFilePath);
                    }

                    // Rename the tab.
                    tabGrid.SelectedTab.Text = newName;

                    // Save the updated tab list.
                    SaveTabNames();
                }
            }
        }


        // ============================================================
        // DELETE TAB
        // ============================================================

        private void btnDeleteTab_Click(object sender, EventArgs e)
        {
            if (tabGrid.SelectedTab == null)
                return;

            string tabName = tabGrid.SelectedTab.Text;

            // Don't allow the original tabs to be deleted.
            if (tabName == "Notes" || tabName == "VBA Code")
            {
                MessageBox.Show(
                    "The original tabs cannot be deleted.",
                    "Delete Tab",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete the '" + tabName + "' tab?",
                "Delete Tab",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string documentsPath =
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments);

                string filePath =
                    Path.Combine(documentsPath, tabName + ".txt");

                // Delete the tab's text file.
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Remove the tab.
                tabGrid.TabPages.Remove(tabGrid.SelectedTab);

                // Save the updated tab list.
                SaveTabNames();
            }
        }


        // ============================================================
        // CLEAR BUTTON
        // ============================================================

        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabGrid.SelectedTab == null)
                    return;

                string tabName = tabGrid.SelectedTab.Text;

                RichTextBox richTextBox =
                    FindRichTextBox(tabGrid.SelectedTab);

                if (richTextBox == null)
                    return;

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to clear all text in '" +
                    tabName + "'?",
                    "Clear Notes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    return;

                string filePath = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments),
                    tabName + ".txt");

                // Clear the RichTextBox.
                richTextBox.Clear();

                // Clear the corresponding text file.
                File.WriteAllText(filePath, string.Empty);

                MessageBox.Show(
                    "'" + tabName + "' has been cleared.",
                    "Clear Notes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error clearing notes: " + ex.Message,
                    "Clear Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

