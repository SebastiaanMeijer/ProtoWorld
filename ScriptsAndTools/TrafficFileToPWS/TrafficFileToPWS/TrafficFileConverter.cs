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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace TrafficFileToPWS
{    
    /// <summary>
    /// Controller class for TrafficFileToPWS.
    /// </summary>
    class TrafficFileConverter
    {
        /// <summary>
        /// Converts the given FCD input file into one or several PWS output files.
        /// </summary>
        /// <param name="pathChoosenFile">Path of the FCD input file.</param>
        /// <param name="pathDestinationFolder">Destination folder for the PWS output files.</param>
        /// <param name="outputFilePrefix">Prefix for the output files.</param>
        /// <param name="bytesPerFile">Max. number of bytes per output file.</param>
        /// <param name="sender">Object that represents the background worker calling this method.</param>
        internal void ConvertFromFCD(string pathChoosenFile, string pathDestinationFolder, string outputFilePrefix, long bytesPerFile, object sender)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<float> times = new List<float>();
            List<string> outputFilenames = new List<string>();
            List<string> vehiclesOfCurrentTimeStep = new List<string>();
            int timeStepCounter = 0;
            int firstTimeStepOutputFile = 0;

            FileStream inputFile;

            string pathOutputFile;
            FileStream descriptorOutputFile;
            StreamWriter writer;

            XmlReader reader;
            StreamReader streamReader;
            bool endReached = false;
			
			if(String.IsNullOrWhiteSpace(outputFilePrefix)) {
				outputFilePrefix = Path.GetFileNameWithoutExtension(pathChoosenFile);
			}

			// Open the FCD file and create the XML reader
			inputFile = File.OpenRead(pathChoosenFile);
            streamReader = new StreamReader(inputFile);
            reader = XmlReader.Create(streamReader);

            // Create the first output file
            pathOutputFile = pathDestinationFolder + "\\" + outputFilePrefix + "_" + firstTimeStepOutputFile + ".pws";
            Console.WriteLine("Creating new output file");
            descriptorOutputFile = File.Create(pathOutputFile);
            writer = new StreamWriter(descriptorOutputFile);

            //If readable XML chunk, process data
            try
            {
                while (!endReached)
                {
                    if (reader.CanReadValueChunk)
                    {
                        reader.Read();

                        switch (reader.NodeType)
                        {
                            //Reading element    
                            case XmlNodeType.Element:

                                //Reading timeStep
                                if (reader.Name.Equals("timestep"))
                                {
                                    if (times.Count > 0)
                                    {
                                        WriteNewTimeStep(timeStepCounter, vehiclesOfCurrentTimeStep, writer);
                                        vehiclesOfCurrentTimeStep.Clear();
                                        timeStepCounter++;
                                    }

                                    // Check the size of the current output file and create a new one if needed
                                    FileInfo fInfo = new FileInfo(pathOutputFile);
                                    if (fInfo.Length > bytesPerFile)
                                    {
                                        //Give a final name to the file
                                        outputFilenames.Add(fInfo.Name + " " + firstTimeStepOutputFile + " " + (timeStepCounter - 1));

                                        Console.WriteLine("Creating new output file");

                                        writer.Close();
                                        descriptorOutputFile.Close();
                                        firstTimeStepOutputFile = timeStepCounter;
                                        pathOutputFile = pathDestinationFolder + "\\" + outputFilePrefix + "_" + firstTimeStepOutputFile + ".pws";
                                        descriptorOutputFile = File.Create(pathOutputFile);
                                        writer = new StreamWriter(descriptorOutputFile);
                                    }


                                    worker.ReportProgress(timeStepCounter);
                                    times.Add(float.Parse(reader.GetAttribute("time"), NumberStyles.Any, CultureInfo.InvariantCulture));
                                }

                                //Reading vehicle
                                if (reader.Name.Equals("vehicle"))
                                {
                                    vehiclesOfCurrentTimeStep.Add(reader.GetAttribute("id") + " "
                                        + reader.GetAttribute("x") + " "
                                        + reader.GetAttribute("y") + " "
                                        + reader.GetAttribute("type") + " "
                                        + reader.GetAttribute("angle"));
                                }
                                break;

                            //Reading end element
                            case XmlNodeType.EndElement:

                                if (reader.Name.Equals("fcd-export"))
                                {
                                    // Write the last timestep
                                    WriteNewTimeStep(timeStepCounter, vehiclesOfCurrentTimeStep, writer);

                                    FileInfo fInfo = new FileInfo(pathOutputFile);
                                    outputFilenames.Add(fInfo.Name + " " + firstTimeStepOutputFile + " " + timeStepCounter);

                                    writer.Close();
                                    descriptorOutputFile.Close();
                                    pathOutputFile = pathDestinationFolder + "\\" + outputFilePrefix + ".pws.meta";
                                    descriptorOutputFile = File.Create(pathOutputFile);
                                    writer = new StreamWriter(descriptorOutputFile);
                                    WriteMetaData(times, outputFilenames, writer);
                                    writer.Close();
                                    descriptorOutputFile.Close();

                                    endReached = true;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                if (descriptorOutputFile != null)
                {
                    descriptorOutputFile.Close();
                    descriptorOutputFile.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                }
                if (inputFile != null)
                {
                    inputFile.Close();
                    inputFile.Dispose();
                }
            }

            worker.CancelAsync();
        }

        /// <summary>
        /// Converts the given FZP input file into one or several PWS output files.
        /// </summary>
        /// <param name="pathChoosenFile">Path of the FZP input file.</param>
        /// <param name="pathDestinationFolder">Destination folder for the PWS output files.</param>
        /// <param name="outputFilePrefix">Prefix for the output files.</param>
        /// <param name="bytesPerFile">Max. number of bytes per output file.</param>
        /// <param name="sender">Object that represents the background worker calling this method.</param>
        internal void ConvertFromFZP(string pathChoosenFile, string pathDestinationFolder, string outputFilePrefix, long bytesPerFile, object sender)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<float> times = new List<float>();
            List<string> outputFilenames = new List<string>();
            List<string> vehiclesOfCurrentTimeStep = new List<string>();
            int timeStepCounter = 0;
            int firstTimeStepOutputFile = 0;
            float currentTime = 0.0f;

            string pathOutputFile;
            FileStream descriptorOutputFile;
            StreamWriter writer;
            FileInfo fInfo;
			
			if(String.IsNullOrWhiteSpace(outputFilePrefix)) {
				outputFilePrefix = Path.GetFileNameWithoutExtension(pathChoosenFile);
			}

			// Create a reader for the input file
			FileStream fileStream = File.OpenRead(pathChoosenFile);
            StreamReader reader = new StreamReader(fileStream);

			// Create the first output file
			pathOutputFile = pathDestinationFolder + "\\" + outputFilePrefix + "_" + firstTimeStepOutputFile + ".pws";
            Console.WriteLine("Creating new output file");
            descriptorOutputFile = File.Create(pathOutputFile);
            writer = new StreamWriter(descriptorOutputFile);

            try
            {
                bool startVehicleRead = false;
                var delims = new char[] { ';', ' ' };

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.StartsWith("*"))
                    {
                        continue;
                    }
                    if (line.Contains("$VEHICLE"))
                    {
                        startVehicleRead = true;
                        line = reader.ReadLine();
                        string[] lineWords = line.Split(delims);
                        currentTime = float.Parse(lineWords[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                        times.Add(currentTime);
                    }
                    if (startVehicleRead)
                    {
                        string[] lineWords = line.Split(delims);
                        float time = float.Parse(lineWords[0], NumberStyles.Any, CultureInfo.InvariantCulture);

                        if (currentTime != time)
                        {
                            // Write timestep on file
                            WriteNewTimeStep(timeStepCounter, vehiclesOfCurrentTimeStep, writer);
                            vehiclesOfCurrentTimeStep.Clear();
                            currentTime = time;
                            times.Add(currentTime);
                            timeStepCounter++;
                        }

                        // Check the size of the current output file and create a new one if needed
                        fInfo = new FileInfo(pathOutputFile);
                        if (fInfo.Length > bytesPerFile)
                        {
                            //Give a final name to the file
                            outputFilenames.Add(fInfo.Name + " " + firstTimeStepOutputFile + " " + (timeStepCounter - 1));

                            Console.WriteLine("Creating new output file");

                            writer.Close();
                            descriptorOutputFile.Close();
                            firstTimeStepOutputFile = timeStepCounter;
                            pathOutputFile = pathDestinationFolder + "\\" + outputFilePrefix + "_" + firstTimeStepOutputFile + ".pws";
                            descriptorOutputFile = File.Create(pathOutputFile);
                            writer = new StreamWriter(descriptorOutputFile);
                        }

                        worker.ReportProgress(timeStepCounter);

                        vehiclesOfCurrentTimeStep.Add(lineWords[1] + " "
                            + lineWords[2] + " "
                            + lineWords[3] + " "
                            + "VEH_FZP" + " "
                            + ConvertFZPAngle(float.Parse(lineWords[8], NumberStyles.Any, CultureInfo.InvariantCulture)));
                    }
                }

                // Write the last timestep
                WriteNewTimeStep(timeStepCounter, vehiclesOfCurrentTimeStep, writer);

                fInfo = new FileInfo(pathOutputFile);
                outputFilenames.Add(fInfo.Name + " " + firstTimeStepOutputFile + " " + timeStepCounter);

                writer.Close();
                descriptorOutputFile.Close();
                pathOutputFile = pathDestinationFolder + "\\" + outputFilePrefix + "_" + ".pws.meta";
                descriptorOutputFile = File.Create(pathOutputFile);
                writer = new StreamWriter(descriptorOutputFile);
                WriteMetaData(times, outputFilenames, writer);
                writer.Close();
                descriptorOutputFile.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                if (descriptorOutputFile != null)
                {
                    descriptorOutputFile.Close();
                    descriptorOutputFile.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Converts a FZP angle into the PWS system.
        /// </summary>
        /// <param name="angle">FZP angle.</param>
        /// <returns>Float with the equivalent PWS angle.</returns>
        private float ConvertFZPAngle(float angle)
        {
            var converted = 180f - angle;
            if (converted < 0)
                converted += 360f;
            return converted;
        }

        /// <summary>
        /// Writes the metadata file for the PWS output. 
        /// </summary>
        /// <param name="times">Full list of times from the traffic simulation.</param>
        /// <param name="outputFilenames">Full list of the output filenames and its first and last timestep.</param>
        /// <param name="writer">Writer of the output file.</param>
        private void WriteMetaData(List<float> times, List<string> outputFilenames, StreamWriter writer)
        {
            Console.WriteLine("Writing meta file");

            string timesMeta = "TIMES ";

            for (int i = 0; i < times.Count; i++)
                timesMeta = timesMeta + times[i] + " ";

            writer.WriteLine(timesMeta);

            for (int i = 0; i < outputFilenames.Count; i++)
            {
                writer.WriteLine("FILENAME " + outputFilenames[i]);
            }
        }

        /// <summary>
        /// Writes a new timestep in an PWS output file. 
        /// </summary>
        /// <param name="currentTimeStep">Timestep to write.</param>
        /// <param name="vehiclesOfCurrentTimeStep">List of vehicles in the timestep.</param>
        /// <param name="writer">Writer of the output file.</param>
        private void WriteNewTimeStep(int currentTimeStep, List<string> vehiclesOfCurrentTimeStep, StreamWriter writer)
        {
            string line = currentTimeStep + ";" + vehiclesOfCurrentTimeStep.Count + ";";

            for (int i = 0; i < vehiclesOfCurrentTimeStep.Count; i++)
            {
                line = line + vehiclesOfCurrentTimeStep[i] + ";";
            }

            writer.WriteLine(line);
        }
    }
}