﻿namespace project
{
    partial class pupilRecords
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
            this.label1 = new System.Windows.Forms.Label();
            this.SearchBar = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.CheckBoxA2E = new System.Windows.Forms.CheckBox();
            this.SearchResults = new System.Windows.Forms.ListBox();
            this.ViewButton = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Pupil Records Program";
            // 
            // SearchBar
            // 
            this.SearchBar.Location = new System.Drawing.Point(12, 60);
            this.SearchBar.Name = "SearchBar";
            this.SearchBar.Size = new System.Drawing.Size(695, 20);
            this.SearchBar.TabIndex = 1;
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(713, 58);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 2;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            // 
            // CheckBoxA2E
            // 
            this.CheckBoxA2E.AutoSize = true;
            this.CheckBoxA2E.Location = new System.Drawing.Point(12, 90);
            this.CheckBoxA2E.Name = "CheckBoxA2E";
            this.CheckBoxA2E.Size = new System.Drawing.Size(170, 17);
            this.CheckBoxA2E.TabIndex = 3;
            this.CheckBoxA2E.Text = "Registered for Able to Enable?";
            this.CheckBoxA2E.UseVisualStyleBackColor = true;
            // 
            // SearchResults
            // 
            this.SearchResults.FormattingEnabled = true;
            this.SearchResults.Location = new System.Drawing.Point(12, 119);
            this.SearchResults.Name = "SearchResults";
            this.SearchResults.Size = new System.Drawing.Size(776, 290);
            this.SearchResults.TabIndex = 4;
            // 
            // ViewButton
            // 
            this.ViewButton.Location = new System.Drawing.Point(713, 415);
            this.ViewButton.Name = "ViewButton";
            this.ViewButton.Size = new System.Drawing.Size(75, 23);
            this.ViewButton.TabIndex = 5;
            this.ViewButton.Text = "View";
            this.ViewButton.UseVisualStyleBackColor = true;
            this.ViewButton.Click += new System.EventHandler(this.ViewButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(713, 86);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 6;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.ViewButton);
            this.Controls.Add(this.SearchResults);
            this.Controls.Add(this.CheckBoxA2E);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.SearchBar);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Pupil Records Program";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SearchBar;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.CheckBox CheckBoxA2E;

        /// Set listbox to public so highlighted entry can be accessed by different forms
        public System.Windows.Forms.ListBox SearchResults; 

        private System.Windows.Forms.Button ViewButton;
        private System.Windows.Forms.Button ResetButton;
    }
}
