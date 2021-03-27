﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static project.PupilDataManager.SharedResources.Types;

namespace project
{
    public partial class ProfileAddNote : Form
    {

        public ProfileEditView profileForm;
        private String initialNote; // for editing notes
        private String initialDate;
        private DbPupilDataManager Mgr;

        public ProfileAddNote()
        {
            InitializeComponent();
        }

        // WINDOW CONTROL BAR

        // allows for window dragging
        // https://stackoverflow.com/a/1592899
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void PanelWindowControls_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void PanelWindowClose_MouseHover(object sender, EventArgs e)
        {
            PanelWindowClose.BackColor = Color.FromArgb(255, 210, 211, 213);
        }

        private void PanelWindowClose_MouseLeave(object sender, EventArgs e)
        {
            PanelWindowClose.BackColor = Color.FromArgb(255, 230, 231, 233);
        }

        private void PanelWindowClose_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            FadeEffect.FadeOut(this, 100, new Action(() => this.Close()));
        }

        private void PanelWindowMinimise_MouseHover(object sender, EventArgs e)
        {
            PanelWindowMinimise.BackColor = Color.FromArgb(255, 210, 211, 213);
        }

        private void PanelWindowMinimise_MouseLeave(object sender, EventArgs e)
        {
            PanelWindowMinimise.BackColor = Color.FromArgb(255, 230, 231, 233);
        }

        private void PanelWindowMinimise_MouseDown(object sender, MouseEventArgs e)
        {
            FadeEffect.FadeOut(this, 100, new Action(() =>
            this.WindowState = FormWindowState.Minimized
            ));
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized) FadeEffect.FadeIn(this, 100);
        }

        // FORM CODE

        private void ProfileAddNote_Load(object sender, EventArgs e)
        {

            VisualThemes.ToDarkTheme(this);
            FadeEffect.FadeIn(this, 100);

            Mgr = profileForm.searchForm.Mgr; // loads DBPupilDataManager

            // Changes window title
            switch (profileForm.noteContext)
            {
                case "add":
                    this.Text = profileForm.activeStudent.Name + " - Add Note";
                    break;
                case "edit":
                    this.Text = profileForm.activeStudent.Name + " - Edit Note";

                    // split text of selected item
                    string[] selectedItem = ((ProfileEditView)profileForm).SearchResults.GetItemText(((ProfileEditView)profileForm).SearchResults.SelectedItem).Split('[');

                    // founder.Remove(founder.Length - 1, 1);
                    initialNote = selectedItem[0].Remove(selectedItem[0].Length - 1, 1);
                    initialDate = selectedItem[1].Trim(' ', ']');

                    TextBoxNote.Text = initialNote;
                    break;
            }
        }

        private void ButtonAddNote_Click(object sender, EventArgs e)
        {

            if (TextBoxNote.Text.Trim() != "")
            {
                // I'd really like to make newlines possible here, but it doesn't seem like that's gonna happen :(
                string newNote = TextBoxNote.Text.Replace("\r", " ").Replace("\n", "").Trim();

                switch (profileForm.noteContext)
                {
                    case "add":
                        string currentDate = DateTime.Now.ToString("d-M-yyyy");
                        profileForm.activeStudent.Notes.Add(new Note(currentDate, newNote));

                        break;
                    case "edit":
                        // iterates through notes and overwrites initial note (probably could be done better)
                        bool Found = false;
                        foreach (Note i_Note in profileForm.activeStudent.Notes) if (i_Note.Text == initialNote)
                            {
                                i_Note.Text = newNote;
                                Found = true;
                                break;
                            }
                        if (!Found) throw new Exception("The specified note wasn't found.");
                        break;
                }

                profileForm.activeStudent.LastAccess = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.s"); // updates last accessed time

                // Write new info to db
                Mgr.WritePupilData(profileForm.activeStudent);
                SystemSounds.Beep.Play();

                FadeEffect.FadeOut(this, 100, new Action(() => this.Close()));
            }
            else
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Please ensure note text is not empty.", "Invalid Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            FadeEffect.FadeOut(this, 100, new Action(() => this.Close()));
        }
    }
}
