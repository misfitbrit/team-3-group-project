﻿namespace project
{
    partial class ProfileAddNote
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextBoxNote = new System.Windows.Forms.TextBox();
            this.ButtonAddNote = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxNote
            // 
            this.TextBoxNote.Location = new System.Drawing.Point(12, 12);
            this.TextBoxNote.Multiline = true;
            this.TextBoxNote.Name = "TextBoxNote";
            this.TextBoxNote.Size = new System.Drawing.Size(513, 121);
            this.TextBoxNote.TabIndex = 0;
            // 
            // ButtonAddNote
            // 
            this.ButtonAddNote.Location = new System.Drawing.Point(450, 139);
            this.ButtonAddNote.Name = "ButtonAddNote";
            this.ButtonAddNote.Size = new System.Drawing.Size(75, 23);
            this.ButtonAddNote.TabIndex = 1;
            this.ButtonAddNote.Text = "Add Note";
            this.ButtonAddNote.UseVisualStyleBackColor = true;
            this.ButtonAddNote.Click += new System.EventHandler(this.ButtonAddNote_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(12, 139);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 2;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ProfileAddNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 178);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonAddNote);
            this.Controls.Add(this.TextBoxNote);
            this.Name = "ProfileAddNote";
            this.Text = "[StudentName] - Add Note";
            this.Load += new System.EventHandler(this.ProfileAddNote_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxNote;
        private System.Windows.Forms.Button ButtonAddNote;
        private System.Windows.Forms.Button ButtonCancel;
    }
}