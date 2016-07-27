/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace SumoTrafficGenerator
{
    public partial class SumoGenTraffic_Form : Form
    {
        public SumoGenTraffic_Form()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler for generateTraffic_button 
        /// </summary>
        private void generateTraffic_button_Click(object sender, EventArgs e)
        {
            string minLat, minLon, maxLat, maxLon, periodicity;
            float aux;

            // Parse the OSM coordinates
            if (!float.TryParse(minLat_textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out aux) || 
                !float.TryParse(minLon_textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out aux) || 
                !float.TryParse(maxLat_textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out aux) || 
                !float.TryParse(maxLon_textBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out aux))
            {
                ShowMessage("failing in parsing, some parameters are not valid", true);
            }
            else
            {
                // Change commas for points (for compatibility safety with batch scripts) 
                minLat = minLat_textBox.Text.Replace(",", ".");
                minLon = minLon_textBox.Text.Replace(",", ".");
                maxLat = maxLat_textBox.Text.Replace(",", ".");
                maxLon = maxLon_textBox.Text.Replace(",", ".");
                
                if (density_trackBar.Value == 0)
                    periodicity = "2";
                else
                    periodicity = (1 / (float)density_trackBar.Value).ToString().Replace(",",".");
                                
                // Call the traffic generator
                generateTraffic_control(minLat, minLon, maxLat, maxLon, periodicity);
            }
        }

        /// <summary>
        /// Controller that generates the files needed to run an instance of SUMO with the parameters given.
        /// </summary>
        /// <param name="minLat">min latitude of the simulation boundaries.</param>
        /// <param name="minLon">min longitude of the simulation boundaries.</param>
        /// <param name="maxLat">max latitude of the simulation boundaries.</param>
        /// <param name="maxLon">max longitude of the simulation boundaries.</param>
        /// <param name="carDensity">density of cars (in a 1-10 range).</param>
        private void generateTraffic_control(string minLat, string minLon, string maxLat, string maxLon, string periodicity)
        {
            string currentDirectory, destinationDirectory;
            FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Diagnostics.Process sumoBatchProcess;

            // Clean message before starting
            ShowMessage("", false);

            try
            {
                // Get the SUMO_HOME environment variable                
                currentDirectory = Directory.GetCurrentDirectory();

                // Ask for a location to generate the files
                folderBrowserDialog.Description = "Select a directory to generate the SUMO files";
                folderBrowserDialog.ShowNewFolderButton = true;
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    destinationDirectory = folderBrowserDialog.SelectedPath;
                else
                    return;

                ShowMessage("Generating traffic...", false);

                // Disable the button to prevent nested calls
                EnableGenButton(false);

                // Add config files and start script to location given
                System.IO.File.Copy(currentDirectory + "/files/start.bat",
                    destinationDirectory + "/start.bat", true);

                System.IO.File.Copy(currentDirectory + "/files/map.local.sumocfg",
                    destinationDirectory + "/map.local.sumocfg", true);

                System.IO.File.Copy(currentDirectory + "/files/map.gui.sumocfg",
                    destinationDirectory + "/map.gui.sumocfg", true);

                // Run the script gensumofiles.bat in location given
                sumoBatchProcess = System.Diagnostics.Process.Start(currentDirectory + "/files/gensumofiles.bat ", minLat + " " + minLon + " " + maxLat + " " + maxLon + " " + periodicity + " " + destinationDirectory);

                // Set the handler for when the script ends
                sumoBatchProcess.EnableRaisingEvents = true;
                sumoBatchProcess.Exited += new EventHandler(BatchProcessHasExited);
            }
            catch (System.Exception e)
            {
                ShowMessage(e.Message, true);
                EnableGenButton(true);
            }
        }

        private void BatchProcessHasExited(object sender, EventArgs e)
        {
            MessageBox.Show("Sumo Traffic Generator completed. Check map.log for more details about the results.");
            ShowMessage("Process completed", false);
            EnableGenButton(true);
        }

        private void ShowMessage(string p, bool isError)
        {
            if (message_label.InvokeRequired)
                this.Invoke(new ShowMessageCallback(ShowMessage), new object[] { p, isError });
            else
            {
                message_label.Text = p;
                message_label.Visible = true;

                if (isError)
                    message_label.ForeColor = Color.Red;
                else
                    message_label.ForeColor = Color.Green;
            }
        }

        private void EnableGenButton(bool isEnable)
        {
            if (generateTraffic_button.InvokeRequired)
                this.Invoke(new EnableGenButtonCallback(EnableGenButton), new object[] { isEnable });
            else
                generateTraffic_button.Enabled = isEnable;
        }

        // Delegates that allow for asynchronous calls. Used to enable thread-safe calls.
        delegate void ShowMessageCallback(string p, bool isError);
        delegate void EnableGenButtonCallback(bool isEnable);
    }
}
