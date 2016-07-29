/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿namespace TrafficFileToPWS
{
    partial class TrafficFileToPWS_Form
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
            this.chooseFile_textBox = new System.Windows.Forms.TextBox();
            this.destinationFolder_textBox = new System.Windows.Forms.TextBox();
            this.chooseFile_searchButton = new System.Windows.Forms.Button();
            this.destinationFolder_searchButton = new System.Windows.Forms.Button();
            this.convertToPWS_button = new System.Windows.Forms.Button();
            this.chooseFile_label = new System.Windows.Forms.Label();
            this.destinationFolder_label = new System.Windows.Forms.Label();
            this.message_label = new System.Windows.Forms.Label();
            this.mbPerFile_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mbPerFile_label = new System.Windows.Forms.Label();
            this.outputPrefix_label = new System.Windows.Forms.Label();
            this.outputPrefix_textBox = new System.Windows.Forms.TextBox();
            this.trace_textBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.mbPerFile_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // chooseFile_textBox
            // 
            this.chooseFile_textBox.Location = new System.Drawing.Point(23, 71);
            this.chooseFile_textBox.Name = "chooseFile_textBox";
            this.chooseFile_textBox.Size = new System.Drawing.Size(316, 22);
            this.chooseFile_textBox.TabIndex = 0;
            // 
            // destinationFolder_textBox
            // 
            this.destinationFolder_textBox.Location = new System.Drawing.Point(23, 142);
            this.destinationFolder_textBox.Name = "destinationFolder_textBox";
            this.destinationFolder_textBox.Size = new System.Drawing.Size(316, 22);
            this.destinationFolder_textBox.TabIndex = 1;
            // 
            // chooseFile_searchButton
            // 
            this.chooseFile_searchButton.Location = new System.Drawing.Point(353, 66);
            this.chooseFile_searchButton.Name = "chooseFile_searchButton";
            this.chooseFile_searchButton.Size = new System.Drawing.Size(102, 32);
            this.chooseFile_searchButton.TabIndex = 2;
            this.chooseFile_searchButton.Text = "search...";
            this.chooseFile_searchButton.UseVisualStyleBackColor = true;
            this.chooseFile_searchButton.Click += new System.EventHandler(this.chooseFile_searchButton_Click);
            // 
            // destinationFolder_searchButton
            // 
            this.destinationFolder_searchButton.Location = new System.Drawing.Point(353, 137);
            this.destinationFolder_searchButton.Name = "destinationFolder_searchButton";
            this.destinationFolder_searchButton.Size = new System.Drawing.Size(102, 32);
            this.destinationFolder_searchButton.TabIndex = 3;
            this.destinationFolder_searchButton.Text = "search...";
            this.destinationFolder_searchButton.UseVisualStyleBackColor = true;
            this.destinationFolder_searchButton.Click += new System.EventHandler(this.destinationFolder_searchButton_Click);
            // 
            // convertToPWS_button
            // 
            this.convertToPWS_button.Location = new System.Drawing.Point(158, 265);
            this.convertToPWS_button.Name = "convertToPWS_button";
            this.convertToPWS_button.Size = new System.Drawing.Size(173, 42);
            this.convertToPWS_button.TabIndex = 4;
            this.convertToPWS_button.Text = "Convert to PWS";
            this.convertToPWS_button.UseVisualStyleBackColor = true;
            this.convertToPWS_button.Click += new System.EventHandler(this.convertToPWS_button_Click);
            // 
            // chooseFile_label
            // 
            this.chooseFile_label.AutoSize = true;
            this.chooseFile_label.Location = new System.Drawing.Point(23, 51);
            this.chooseFile_label.Name = "chooseFile_label";
            this.chooseFile_label.Size = new System.Drawing.Size(239, 17);
            this.chooseFile_label.TabIndex = 5;
            this.chooseFile_label.Text = "Choose a FCD or FZP file to convert:";
            // 
            // destinationFolder_label
            // 
            this.destinationFolder_label.AutoSize = true;
            this.destinationFolder_label.Location = new System.Drawing.Point(23, 122);
            this.destinationFolder_label.Name = "destinationFolder_label";
            this.destinationFolder_label.Size = new System.Drawing.Size(254, 17);
            this.destinationFolder_label.TabIndex = 6;
            this.destinationFolder_label.Text = "Choose a destination for the PWS files:";
            // 
            // message_label
            // 
            this.message_label.AutoSize = true;
            this.message_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.message_label.Location = new System.Drawing.Point(14, 321);
            this.message_label.MinimumSize = new System.Drawing.Size(450, 0);
            this.message_label.Name = "message_label";
            this.message_label.Size = new System.Drawing.Size(450, 18);
            this.message_label.TabIndex = 7;
            this.message_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mbPerFile_numericUpDown
            // 
            this.mbPerFile_numericUpDown.Location = new System.Drawing.Point(26, 214);
            this.mbPerFile_numericUpDown.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.mbPerFile_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mbPerFile_numericUpDown.Name = "mbPerFile_numericUpDown";
            this.mbPerFile_numericUpDown.Size = new System.Drawing.Size(111, 22);
            this.mbPerFile_numericUpDown.TabIndex = 8;
            this.mbPerFile_numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mbPerFile_numericUpDown.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // mbPerFile_label
            // 
            this.mbPerFile_label.AutoSize = true;
            this.mbPerFile_label.Location = new System.Drawing.Point(23, 189);
            this.mbPerFile_label.Name = "mbPerFile_label";
            this.mbPerFile_label.Size = new System.Drawing.Size(114, 17);
            this.mbPerFile_label.TabIndex = 9;
            this.mbPerFile_label.Text = "MB per PWS file:";
            // 
            // outputPrefix_label
            // 
            this.outputPrefix_label.AutoSize = true;
            this.outputPrefix_label.Location = new System.Drawing.Point(155, 189);
            this.outputPrefix_label.Name = "outputPrefix_label";
            this.outputPrefix_label.Size = new System.Drawing.Size(93, 17);
            this.outputPrefix_label.TabIndex = 10;
            this.outputPrefix_label.Text = "Output prefix:";
            // 
            // outputPrefix_textBox
            // 
            this.outputPrefix_textBox.Location = new System.Drawing.Point(158, 213);
            this.outputPrefix_textBox.Name = "outputPrefix_textBox";
            this.outputPrefix_textBox.Size = new System.Drawing.Size(297, 22);
            this.outputPrefix_textBox.TabIndex = 11;
            // 
            // trace_textBox
            // 
            this.trace_textBox.Location = new System.Drawing.Point(17, 352);
            this.trace_textBox.Multiline = true;
            this.trace_textBox.Name = "trace_textBox";
            this.trace_textBox.Size = new System.Drawing.Size(447, 141);
            this.trace_textBox.TabIndex = 12;
            // 
            // TrafficFileToPWS_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 507);
            this.Controls.Add(this.trace_textBox);
            this.Controls.Add(this.outputPrefix_textBox);
            this.Controls.Add(this.outputPrefix_label);
            this.Controls.Add(this.mbPerFile_label);
            this.Controls.Add(this.mbPerFile_numericUpDown);
            this.Controls.Add(this.message_label);
            this.Controls.Add(this.destinationFolder_label);
            this.Controls.Add(this.chooseFile_label);
            this.Controls.Add(this.convertToPWS_button);
            this.Controls.Add(this.destinationFolder_searchButton);
            this.Controls.Add(this.chooseFile_searchButton);
            this.Controls.Add(this.destinationFolder_textBox);
            this.Controls.Add(this.chooseFile_textBox);
            this.Name = "TrafficFileToPWS_Form";
            this.Text = "Traffic File To PWS ";
            ((System.ComponentModel.ISupportInitialize)(this.mbPerFile_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chooseFile_textBox;
        private System.Windows.Forms.TextBox destinationFolder_textBox;
        private System.Windows.Forms.Button chooseFile_searchButton;
        private System.Windows.Forms.Button destinationFolder_searchButton;
        private System.Windows.Forms.Button convertToPWS_button;
        private System.Windows.Forms.Label chooseFile_label;
        private System.Windows.Forms.Label destinationFolder_label;
        private System.Windows.Forms.Label message_label;
        private System.Windows.Forms.NumericUpDown mbPerFile_numericUpDown;
        private System.Windows.Forms.Label mbPerFile_label;
        private System.Windows.Forms.Label outputPrefix_label;
        private System.Windows.Forms.TextBox outputPrefix_textBox;
        private System.Windows.Forms.TextBox trace_textBox;

    }
}

