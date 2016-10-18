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
using System.IO;
using System.Xml;
using System.Globalization;

namespace TrafficFileToPWS
{
    /// <summary>
    /// Form class for TrafficFileToPWS.
    /// </summary>
    public partial class TrafficFileToPWS_Form : Form
    {
        /// <summary>
        /// Property that keeps the path of the choosen file.
        /// </summary>
        public string pathChoosenFile
        {
            get
            {
                return chooseFile_textBox.Text;
            }
            set
            {
                chooseFile_textBox.Text = value;
            }
        }

        /// <summary>
        /// Property that keeps the path of the destination folder.
        /// </summary>
        public string pathDestinationFolder
        {
            get
            {
                return destinationFolder_textBox.Text;
            }
            set
            {
                destinationFolder_textBox.Text = value;
            }
        }

        /// <summary>
        /// Property that keeps the prefix for the output files.
        /// </summary>
        public string outputFilePrefix
        {
            get
            {
                return outputPrefix_textBox.Text;
            }
            set
            {
                outputPrefix_textBox.Text = value;
            }
        }

        /// <summary>
        /// Property that keeps the maximum number of bytes per output file.
        /// </summary>
        public long bytesPerFile
        {
            get
            {
                return (long) mbPerFile_numericUpDown.Value * 1024 * 1024;
            }
            set
            {
                mbPerFile_numericUpDown.Value = value * 1024 * 1024;
            }
        }

        /// <summary>
        /// Property that keep the trace text of the form.
        /// </summary>
        public string traceText
        {
            get 
            { 
                return trace_textBox.Text; 
            }
            set 
            { 
                trace_textBox.Text = trace_textBox.Text + value + Environment.NewLine; 
            }
        }

        /// <summary>
        /// Form initializer.
        /// </summary>
        public TrafficFileToPWS_Form()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Click handler chooseFile_searchButton.
        /// </summary>
        private void chooseFile_searchButton_Click(object sender, EventArgs e)
        {
            ChooseFileDialog();
        }

        /// <summary>
        /// Click handler destinationFolder_searchButton.
        /// </summary>
        private void destinationFolder_searchButton_Click(object sender, EventArgs e)
        {
            ChooseDestinationFolder();
        }

        /// <summary>
        /// Click handler convertToPWS_button.
        /// </summary>
        private void convertToPWS_button_Click(object sender, EventArgs e)
        {
            // Check that the input is correct
            if (!File.Exists(pathChoosenFile) || !Directory.Exists(pathDestinationFolder))
            {
                ShowMessage("The path of the file or folder is not valid", true);
                traceText = "The path of the file or folder is not valid";
            }
            else
            {
                // Create a new background worker
                BackgroundWorker bgWorker = new BackgroundWorker();
                bgWorker.WorkerReportsProgress = true;
				bgWorker.WorkerSupportsCancellation = true;

                // Check the format of the file
                string extension = Path.GetExtension(pathChoosenFile);

                if (extension == ".fcd")
                {
                    ShowMessage("Converting FCD to PWS format...", false);
                    traceText = "Converting FCD to PWS format...";
                    bgWorker.DoWork += new DoWorkEventHandler(bgWorker_ConvertFCDFile);

                }
                else if (extension == ".fzp")
                {
                    ShowMessage("Converting FZP to PWS format...", false);
                    traceText = "Converting FZP to PWS format...";
                    bgWorker.DoWork += new DoWorkEventHandler(bgWorker_ConvertFZPFile);
                }
                else
                {
                    ShowMessage("File extension of the input file is not valid", true);
                    traceText = "File extension of the input file is not valid";
                    return;
                }

                // Set the events for the background worker
                bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_ConvertCompleted);
                bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ConvertProgressChanged);

                // Disable the button to prevent nested calls
                EnableConverterButton(false);

                // Debug messages for the parameters
                Console.WriteLine("Input file: " + pathChoosenFile + "\n");
                traceText = "Input file: " + pathChoosenFile;
                Console.WriteLine("Destination folder: " + pathDestinationFolder);
                traceText = "Destination folder: " + pathDestinationFolder;
                Console.WriteLine("Prefix: " + outputFilePrefix);
                traceText = "Prefix: " + outputFilePrefix;
                Console.WriteLine("Bytes per file: " + bytesPerFile);
                traceText = "Bytes per file: " + bytesPerFile;

                // Start the conversion operation with the background worker
                bgWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Background worker Do Work event for converting FCD files. 
        /// </summary>
        private void bgWorker_ConvertFCDFile(object sender, DoWorkEventArgs e)
        {
            TrafficFileConverter traffConverter = new TrafficFileConverter();
            traffConverter.ConvertFromFCD(pathChoosenFile, pathDestinationFolder, outputFilePrefix, bytesPerFile, sender);
        }

        /// <summary>
        /// Background worker Do Work event for converting FZP files. 
        /// </summary>
        private void bgWorker_ConvertFZPFile(object sender, DoWorkEventArgs e)
        {
            TrafficFileConverter traffConverter = new TrafficFileConverter();
            traffConverter.ConvertFromFZP(pathChoosenFile, pathDestinationFolder, outputFilePrefix, bytesPerFile, sender);
        }

        /// <summary>
        /// Background worker Progress Changed event for converting files. 
        /// </summary>
        private void bgWorker_ConvertProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ShowMessage("Converting timestep " + e.ProgressPercentage, false);
        }

        /// <summary>
        /// Background worker Completed Work event for converting files. 
        /// </summary>
        private void bgWorker_ConvertCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowMessage("Conversion completed", false);
            EnableConverterButton(true);
        }

        /// <summary>
        /// Chooses the input file for convertion. 
        /// </summary>
        private void ChooseFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "FCD Files (*.fcd)|*.fcd|FZPFiles (*.fzp)|*.fzp";
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                pathChoosenFile = dialog.FileName;
            }
        }

        /// <summary>
        /// Chooses the destination folder for the output convertion. 
        /// </summary>
        private void ChooseDestinationFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                pathDestinationFolder = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Shows a message on the form. 
        /// </summary>
        /// <param name="p">Message.</param>
        /// <param name="isError">True if it is an error message.</param>
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

        /// <summary>
        /// Enables/disables the converter button. 
        /// </summary>
        /// <param name="isEnable">True for enabling.</param>
        private void EnableConverterButton(bool isEnable)
        {
            if (convertToPWS_button.InvokeRequired)
                this.Invoke(new EnableGenButtonCallback(EnableConverterButton), new object[] { isEnable });
            else
                convertToPWS_button.Enabled = isEnable;
        }

        // Delegates that allow for asynchronous calls. Used to enable thread-safe calls.
        delegate void ShowMessageCallback(string p, bool isError);
        delegate void EnableGenButtonCallback(bool isEnable);
    }
}
