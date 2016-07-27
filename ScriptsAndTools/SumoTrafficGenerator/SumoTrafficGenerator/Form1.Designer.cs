/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿namespace SumoTrafficGenerator
{
    partial class SumoGenTraffic_Form
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
            this.generateTraffic_button = new System.Windows.Forms.Button();
            this.minLat_textBox = new System.Windows.Forms.TextBox();
            this.minLon_textBox = new System.Windows.Forms.TextBox();
            this.maxLat_textBox = new System.Windows.Forms.TextBox();
            this.maxLon_textBox = new System.Windows.Forms.TextBox();
            this.osmCoord_label = new System.Windows.Forms.Label();
            this.density_label = new System.Windows.Forms.Label();
            this.message_label = new System.Windows.Forms.Label();
            this.density_trackBar = new System.Windows.Forms.TrackBar();
            this.low_label = new System.Windows.Forms.Label();
            this.high_label = new System.Windows.Forms.Label();
            this.minLon_label = new System.Windows.Forms.Label();
            this.minLat_label = new System.Windows.Forms.Label();
            this.maxLat_label = new System.Windows.Forms.Label();
            this.maxLon_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.density_trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // generateTraffic_button
            // 
            this.generateTraffic_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.generateTraffic_button.Location = new System.Drawing.Point(195, 331);
            this.generateTraffic_button.Name = "generateTraffic_button";
            this.generateTraffic_button.Size = new System.Drawing.Size(225, 37);
            this.generateTraffic_button.TabIndex = 0;
            this.generateTraffic_button.Text = "Generate traffic";
            this.generateTraffic_button.UseVisualStyleBackColor = true;
            this.generateTraffic_button.Click += new System.EventHandler(this.generateTraffic_button_Click);
            // 
            // minLat_textBox
            // 
            this.minLat_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLat_textBox.Location = new System.Drawing.Point(251, 148);
            this.minLat_textBox.Name = "minLat_textBox";
            this.minLat_textBox.Size = new System.Drawing.Size(110, 24);
            this.minLat_textBox.TabIndex = 1;
            this.minLat_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // minLon_textBox
            // 
            this.minLon_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLon_textBox.Location = new System.Drawing.Point(96, 106);
            this.minLon_textBox.Name = "minLon_textBox";
            this.minLon_textBox.Size = new System.Drawing.Size(110, 24);
            this.minLon_textBox.TabIndex = 2;
            this.minLon_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // maxLat_textBox
            // 
            this.maxLat_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxLat_textBox.Location = new System.Drawing.Point(251, 67);
            this.maxLat_textBox.Name = "maxLat_textBox";
            this.maxLat_textBox.Size = new System.Drawing.Size(110, 24);
            this.maxLat_textBox.TabIndex = 3;
            this.maxLat_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // maxLon_textBox
            // 
            this.maxLon_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxLon_textBox.Location = new System.Drawing.Point(401, 106);
            this.maxLon_textBox.Name = "maxLon_textBox";
            this.maxLon_textBox.Size = new System.Drawing.Size(110, 24);
            this.maxLon_textBox.TabIndex = 4;
            this.maxLon_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // osmCoord_label
            // 
            this.osmCoord_label.AutoSize = true;
            this.osmCoord_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.osmCoord_label.Location = new System.Drawing.Point(25, 29);
            this.osmCoord_label.Name = "osmCoord_label";
            this.osmCoord_label.Size = new System.Drawing.Size(163, 20);
            this.osmCoord_label.TabIndex = 5;
            this.osmCoord_label.Text = "OSM Coordinates:";
            // 
            // density_label
            // 
            this.density_label.AutoSize = true;
            this.density_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.density_label.Location = new System.Drawing.Point(25, 192);
            this.density_label.Name = "density_label";
            this.density_label.Size = new System.Drawing.Size(111, 20);
            this.density_label.TabIndex = 6;
            this.density_label.Text = "Car density:";
            // 
            // message_label
            // 
            this.message_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.message_label.ForeColor = System.Drawing.Color.Red;
            this.message_label.Location = new System.Drawing.Point(11, 386);
            this.message_label.Name = "message_label";
            this.message_label.Size = new System.Drawing.Size(592, 20);
            this.message_label.TabIndex = 8;
            this.message_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.message_label.Visible = false;
            // 
            // density_trackBar
            // 
            this.density_trackBar.Location = new System.Drawing.Point(83, 240);
            this.density_trackBar.Name = "density_trackBar";
            this.density_trackBar.Size = new System.Drawing.Size(455, 56);
            this.density_trackBar.TabIndex = 9;
            this.density_trackBar.Value = 5;
            // 
            // low_label
            // 
            this.low_label.AutoSize = true;
            this.low_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.low_label.Location = new System.Drawing.Point(83, 278);
            this.low_label.Name = "low_label";
            this.low_label.Size = new System.Drawing.Size(31, 18);
            this.low_label.TabIndex = 10;
            this.low_label.Text = "low";
            // 
            // high_label
            // 
            this.high_label.AutoSize = true;
            this.high_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.high_label.Location = new System.Drawing.Point(503, 279);
            this.high_label.Name = "high_label";
            this.high_label.Size = new System.Drawing.Size(35, 18);
            this.high_label.TabIndex = 11;
            this.high_label.Text = "high";
            // 
            // minLon_label
            // 
            this.minLon_label.AutoSize = true;
            this.minLon_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLon_label.Location = new System.Drawing.Point(119, 85);
            this.minLon_label.Name = "minLon_label";
            this.minLon_label.Size = new System.Drawing.Size(61, 18);
            this.minLon_label.TabIndex = 12;
            this.minLon_label.Text = "min Lon";
            // 
            // minLat_label
            // 
            this.minLat_label.AutoSize = true;
            this.minLat_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minLat_label.Location = new System.Drawing.Point(277, 127);
            this.minLat_label.Name = "minLat_label";
            this.minLat_label.Size = new System.Drawing.Size(56, 18);
            this.minLat_label.TabIndex = 13;
            this.minLat_label.Text = "min Lat";
            // 
            // maxLat_label
            // 
            this.maxLat_label.AutoSize = true;
            this.maxLat_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxLat_label.Location = new System.Drawing.Point(277, 46);
            this.maxLat_label.Name = "maxLat_label";
            this.maxLat_label.Size = new System.Drawing.Size(60, 18);
            this.maxLat_label.TabIndex = 14;
            this.maxLat_label.Text = "max Lat";
            // 
            // maxLon_label
            // 
            this.maxLon_label.AutoSize = true;
            this.maxLon_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxLon_label.Location = new System.Drawing.Point(423, 85);
            this.maxLon_label.Name = "maxLon_label";
            this.maxLon_label.Size = new System.Drawing.Size(65, 18);
            this.maxLon_label.TabIndex = 15;
            this.maxLon_label.Text = "max Lon";
            // 
            // SumoGenTraffic_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 415);
            this.Controls.Add(this.maxLon_label);
            this.Controls.Add(this.maxLat_label);
            this.Controls.Add(this.minLat_label);
            this.Controls.Add(this.minLon_label);
            this.Controls.Add(this.high_label);
            this.Controls.Add(this.low_label);
            this.Controls.Add(this.minLat_textBox);
            this.Controls.Add(this.maxLon_textBox);
            this.Controls.Add(this.density_trackBar);
            this.Controls.Add(this.maxLat_textBox);
            this.Controls.Add(this.minLon_textBox);
            this.Controls.Add(this.message_label);
            this.Controls.Add(this.density_label);
            this.Controls.Add(this.osmCoord_label);
            this.Controls.Add(this.generateTraffic_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "SumoGenTraffic_Form";
            this.Text = "Sumo Traffic Generator";
            ((System.ComponentModel.ISupportInitialize)(this.density_trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generateTraffic_button;
        private System.Windows.Forms.TextBox minLat_textBox;
        private System.Windows.Forms.TextBox minLon_textBox;
        private System.Windows.Forms.TextBox maxLat_textBox;
        private System.Windows.Forms.TextBox maxLon_textBox;
        private System.Windows.Forms.Label osmCoord_label;
        private System.Windows.Forms.Label density_label;
        private System.Windows.Forms.Label message_label;
        private System.Windows.Forms.TrackBar density_trackBar;
        private System.Windows.Forms.Label low_label;
        private System.Windows.Forms.Label high_label;
        private System.Windows.Forms.Label minLon_label;
        private System.Windows.Forms.Label minLat_label;
        private System.Windows.Forms.Label maxLat_label;
        private System.Windows.Forms.Label maxLon_label;
    }
}

