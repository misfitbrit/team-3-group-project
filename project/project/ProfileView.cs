﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static project.PupilDataManager.SharedResources.Types;

namespace project
{
    public partial class ProfileEditView : Form
    {

        public pupilRecords searchForm; // all data from parent form
        public Pupil activeStudent; // stores student data that is currently being accessed
        public String noteContext; // for access of the note editing context (ie add/edit)
        private PupilFileManager Mgr; // instance of pupilfilemanager

        public ProfileEditView()
        {
            InitializeComponent();
        }

        private static void populateNotes(ListBox SearchResults, Pupil activeStudent)
        {
            SearchResults.Items.Clear();
            foreach (Note i_Note in activeStudent.Notes) {
                SearchResults.Items.Add(i_Note.Text + " [" + i_Note.Date + "]");
            }
        }

        private static int compareDates(String selectedDate, String noteDate)
        {
            string[] selectedStrArray = selectedDate.Split('-');
            string[] noteStrArray = noteDate.Split('-');

            int[] selectedIntArray = new int[3];
            int[] noteIntArray = new int[3];

            for (int i = 0; i < 3; i++)
            {
                selectedIntArray[i] = int.Parse(selectedStrArray[i]);
                noteIntArray[i] = int.Parse(noteStrArray[i]);
            }

            // Compare years
            if (selectedIntArray[2] > noteIntArray[2]) return 1;
            if (selectedIntArray[2] < noteIntArray[2]) return -1;
            // Compare months
            if (selectedIntArray[1] > noteIntArray[1]) return 1;
            if (selectedIntArray[1] < noteIntArray[1]) return -1;
            // Compare days
            if (selectedIntArray[0] > noteIntArray[0]) return 1;
            if (selectedIntArray[0] == noteIntArray[0]) return 0;
            if (selectedIntArray[0] < noteIntArray[0]) return -1;

            // if all else fails, which it won't...
            return 0;
        }

        private static Pupil getStudent(PupilFileManager Mgr, string studentName)
        {
            Pupil student;

            // When searching by id, the correct student will always be able to be accessed through students[0]
            List<Pupil> students = Mgr.GetPupilsByProperties(new { Name = studentName }); // Replace with ID, when implemented

            // saves student info globally (locally? global to this form at least. idk how c# works) so it can be accessed by other form events
            student = students[0];
            return student;
        }

        private void ProfileEditView_Load(object sender, EventArgs e)
        {

            ComboBoxContext.SelectedIndex = 0; // set default selection

            Mgr = new PupilFileManager(searchForm.dataDir);

            string[] highlightedField = ((pupilRecords)searchForm).SearchResults.GetItemText(((pupilRecords)searchForm).SearchResults.SelectedItem).Split('(');

            string studentName = highlightedField[0].Trim(); // Gets pupil name from substring array, removing trailing space
            string studentID = highlightedField[1].Trim('(',' ', ')');

            activeStudent = getStudent(Mgr, studentName);

            this.Text = activeStudent.Name + " (" + activeStudent.PupilID + ") - Info";
            LabelStudentName.Text = activeStudent.Name;
            LabelStudentNo.Text = activeStudent.PupilID;
            LabelCompany.Text = activeStudent.Company;

            string groups = "";
            if (activeStudent.A2E) groups += "A2E, ";
            // additional groups go here
            groups = groups.Trim(',', ' ');
            if (groups == "") LabelGroups.Visible = false;
            else LabelGroups.Text = groups;

            populateNotes(SearchResults, activeStudent);


            if(File.Exists($@"{searchForm.dataDir}\Pupil_{studentName}\{activeStudent.ImgRef}")) StudentPhoto.Image = Image.FromFile($@"{searchForm.dataDir}\Pupil_{studentName}\{activeStudent.ImgRef}");
            else if(File.Exists($@"{searchForm.dataDir}\default.jpg")) StudentPhoto.Image = Image.FromFile($@"{searchForm.dataDir}\default.jpg");
            else StudentPhoto.Image = null;

        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {

            // Only display notes from the date selected in the DateTimePicker on clicking this button
            // Dates are formatted in file as: 1-1-2021
            string selectedDate = DateTimePicker.Value.Day.ToString() + "-" + DateTimePicker.Value.Month.ToString() + "-" + DateTimePicker.Value.Year.ToString();

            SearchResults.Items.Clear();

            switch (ComboBoxContext.SelectedIndex)
            {
                case 0:
                    // all notes AFTER specified date
                    foreach (Note i_Note in activeStudent.Notes)
                    {
                        int relative = compareDates(selectedDate, i_Note.Date);
                        if (relative == 1 || relative == 0)
                        {
                            SearchResults.Items.Add(i_Note.Text + " [" + i_Note.Date + "]");
                        }
                    }
                    break;
                case 1:
                    // all notes FROM specified date
                    foreach (Note i_Note in activeStudent.Notes)
                    {
                        int relative = compareDates(selectedDate, i_Note.Date);
                        if (relative == 0)
                        {
                            SearchResults.Items.Add(i_Note.Text);
                        }
                    }
                    break;
                case 2:
                    // all notes BEFORE specified date
                    foreach (Note i_Note in activeStudent.Notes)
                    {
                        int relative = compareDates(selectedDate, i_Note.Date);
                        if (relative == -1 || relative == 0)
                        {
                            SearchResults.Items.Add(i_Note + " [" + i_Note.Date + "]");
                        }
                    }
                    break;
            }

            if (SearchResults.Items.Count == 0) SearchResults.Items.Add("No notes on this date.");

        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {

            // Reload all notes
            populateNotes(SearchResults, activeStudent);

        }

        private void ButtonAddNote_Click(object sender, EventArgs e)
        {
            ProfileAddNote addNote = new ProfileAddNote();
            noteContext = "add"; // lets the next form know that we are adding and not editing a note
            addNote.profileForm = this;
            this.Hide();
            addNote.ShowDialog();

            // then, on close of this form
            activeStudent = getStudent(Mgr, activeStudent.Name);
            this.Show();
            populateNotes(SearchResults, activeStudent);
        }

        private void ButtonEditNote_Click(object sender, EventArgs e)
        {
            if (SearchResults.SelectedIndex != -1)
            {
                ProfileAddNote addNote = new ProfileAddNote();
                noteContext = "edit"; // lets the next form know that we are editing and not adding a note
                addNote.profileForm = this;
                this.Hide();
                addNote.ShowDialog();

                // then, on close of this form
                activeStudent = getStudent(Mgr, activeStudent.Name);
                this.Show();
                populateNotes(SearchResults, activeStudent);
            }
            else
            {
                MessageBox.Show("Please select the note you wish to edit.", "No note selected", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void ButtonEditInfo_Click(object sender, EventArgs e)
        {

        }
    }
}
