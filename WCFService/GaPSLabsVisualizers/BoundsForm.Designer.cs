/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿namespace GaPSlabsVisualizers
{
    partial class BoundsForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxLatMin = new System.Windows.Forms.TextBox();
            this.textBoxLonMin = new System.Windows.Forms.TextBox();
            this.textBoxLonMax = new System.Windows.Forms.TextBox();
            this.textBoxLatMax = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(87, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 200);
            this.panel1.TabIndex = 0;
            // 
            // textBoxLatMin
            // 
            this.textBoxLatMin.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxLatMin.Location = new System.Drawing.Point(112, 57);
            this.textBoxLatMin.Name = "textBoxLatMin";
            this.textBoxLatMin.ReadOnly = true;
            this.textBoxLatMin.Size = new System.Drawing.Size(150, 22);
            this.textBoxLatMin.TabIndex = 1;
            // 
            // textBoxLonMin
            // 
            this.textBoxLonMin.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxLonMin.Location = new System.Drawing.Point(12, 161);
            this.textBoxLonMin.Name = "textBoxLonMin";
            this.textBoxLonMin.ReadOnly = true;
            this.textBoxLonMin.Size = new System.Drawing.Size(150, 22);
            this.textBoxLonMin.TabIndex = 3;
            // 
            // textBoxLonMax
            // 
            this.textBoxLonMax.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxLonMax.Location = new System.Drawing.Point(207, 161);
            this.textBoxLonMax.Name = "textBoxLonMax";
            this.textBoxLonMax.ReadOnly = true;
            this.textBoxLonMax.Size = new System.Drawing.Size(150, 22);
            this.textBoxLonMax.TabIndex = 4;
            // 
            // textBoxLatMax
            // 
            this.textBoxLatMax.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxLatMax.Location = new System.Drawing.Point(112, 272);
            this.textBoxLatMax.Name = "textBoxLatMax";
            this.textBoxLatMax.ReadOnly = true;
            this.textBoxLatMax.Size = new System.Drawing.Size(150, 22);
            this.textBoxLatMax.TabIndex = 5;
            // 
            // BoundsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 350);
            this.Controls.Add(this.textBoxLatMax);
            this.Controls.Add(this.textBoxLonMax);
            this.Controls.Add(this.textBoxLonMin);
            this.Controls.Add(this.textBoxLatMin);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BoundsForm";
            this.Text = "Bounds";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.TextBox textBoxLatMin;
        public System.Windows.Forms.TextBox textBoxLonMin;
        public System.Windows.Forms.TextBox textBoxLonMax;
        public System.Windows.Forms.TextBox textBoxLatMax;
    }
}