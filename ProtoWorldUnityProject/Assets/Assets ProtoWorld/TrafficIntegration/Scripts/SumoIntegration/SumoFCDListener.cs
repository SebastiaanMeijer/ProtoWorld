/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * SUMO COMMUNICATION
 * SumoFCDListener.cs
 * Miguel Ramos Carretero
 * Johnson Ho
 * 
 */

using UnityEngine;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;

/// <summary>
/// Listens and processes data from the FCD output of SUMO when the simulation is running.
/// Alternatively, it reads from an FCD file containing information from a preprocessed simulation.
/// </summary>
public class SumoFCDListener : MonoBehaviour
{
    [HideInInspector]
    public int port = 3654;

    [HideInInspector]
    public TrafficIntegrationData trafficDB;

    [HideInInspector]
    public bool readFromFCDFile = false;

    [HideInInspector]
    public bool readFromSumoFCDOutput = false;

    Thread thread;
    Stream streamSocket;
    Socket sumoSocket;
    TcpListener sumoListener;
    private float time;
    private string pathFCD;

    TrafficIntegrationController trafficContr;

    /// <summary>
    /// This is kept for backward compatibility with the old module.
    /// </summary>
    SumoConfig conf;

    /// <summary>
    /// Set this to true to stop the thread to read the FCD-File.
    /// </summary>
    [HideInInspector]
    public volatile bool _shouldStop = false;

    /// <summary>
    /// Creates a thread for listening the FCD output of SUMO when the script is awaked. 
    /// </summary>
    /// <seealso cref="ListeningSumoFCD"/>
    void Awake()
    {
        trafficContr = FindObjectOfType<TrafficIntegrationController>();

        if (trafficContr != null)
        {
            port = trafficContr.remotePortForListener;
            pathFCD = trafficContr.pathSumoFCDFile;
            readFromFCDFile = (trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoFCDFile);
            readFromSumoFCDOutput = (trafficContr.typeOfIntegration == TrafficIntegrationController.TypeOfIntegration.SumoLiveIntegration);
        }
        else
        {
            // running SUMO with old module:
            conf = this.GetComponentInParent<SumoConfig>();
            if (conf != null)
            {
                port = conf.remotePortForListener;
                pathFCD = conf.FCDFilePath;
                readFromFCDFile = conf.integrateSumo && conf.simulateFromFCDFile;
                readFromSumoFCDOutput = conf.integrateSumo && !conf.simulateFromFCDFile;
            }
        }

        trafficDB = FindObjectOfType<TrafficIntegrationData>();
        
        _shouldStop = false;

        //Create a thread for the listener/reader
        if (readFromSumoFCDOutput)
        {
            ThreadStart ts = new ThreadStart(ListeningSumoFCD);
            thread = new Thread(ts);
            thread.Start();
            UnityEngine.Debug.Log("Thread for SUMO FCD listening created");
        }
        else if (readFromFCDFile)
        {
            ////Create a thread for the listener
            //ThreadStart ts = new ThreadStart(ReadingFileFCD);
            //thread = new Thread(ts);
            //thread.Start();
            //UnityEngine.Debug.Log("Thread for FILE FCD listening created");
            UnityEngine.Debug.Log("SumoFCDListener = obsolete. Please use SimulationReader");
        }
    }

    /// <summary>
    /// Auxiliar method for threading. 
    /// Creates a TCP listener to receive data from the FCD output from SUMO. Reads and parses
    /// the XML data containing all the information of the simulation (timesteps and vehicles), 
    /// adding it to the <see cref="trafficDB"/>.
    /// </summary>
    /// <seealso cref="SumoTrafficDB"/>
    public void ListeningSumoFCD()
    {
        XmlReader reader;

        try
        {
            //Start the listener
            UnityEngine.Debug.Log("Starting FCD Listener...");
            sumoListener = new TcpListener(IPAddress.Any, port);
            sumoListener.Start();
            UnityEngine.Debug.Log("Listening in port " + port + "...\n");

            //Open a socket to receive data from SUMO 
            sumoSocket = sumoListener.AcceptSocket();

            //Create a xml reader to read from the stream of the socket
            streamSocket = new NetworkStream(sumoSocket);
            reader = XmlReader.Create(streamSocket);
        }
        catch
        {
            UnityEngine.Debug.Log("Error trying to listen from SUMO, is the server running?");
            return;
        }

        try
        {
            //Listening
            while (!_shouldStop)
            {
                //If readable XML chunk, process data
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
                                time = float.Parse(reader.GetAttribute("time"));
                                trafficDB.InsertNewTimeStep(time);
                            }

                            //Reading vehicle
                            if (reader.Name.Equals("vehicle"))
                            {
                                trafficDB.InsertVehicle(reader.GetAttribute("id"),
                                        reader.GetAttribute("x"),
                                        reader.GetAttribute("y"),
                                        reader.GetAttribute("type"),
                                        reader.GetAttribute("angle")
                                        );
                            }
                            break;

                        //Reading end element
                        case XmlNodeType.EndElement:

                            if (reader.Name.Equals("fcd-export"))
                            {
                                UnityEngine.Debug.Log("End of the file reached\n");
                                return;
                            }
                            break;
                    }
                }
            }
        }
        catch (ThreadAbortException)
        {
            UnityEngine.Debug.Log("Thread for listening aborted");
            return;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("Error while listening!\n" + e.ToString());
            return;
        }
    }

    /// <summary>
    /// Auxiliar method for threading. 
    /// Reads data from the FCD file and parses it to the trafficDB. 
    /// adding it to the <see cref="trafficDB"/>.
    /// </summary>
    /// <seealso cref="SumoTrafficDB"/>
    public void ReadingFileFCD()
    {
        XmlReader reader;
        FileStream fileStream;
        StreamReader streamReader;

        if (File.Exists(pathFCD) == true)
        {
            try
            {
                UnityEngine.Debug.Log("Opening FCD file...");

                fileStream = File.OpenRead(pathFCD);
                streamReader = new StreamReader(fileStream);
                reader = XmlReader.Create(streamReader);

                UnityEngine.Debug.Log("Reading FCD File...");
            }
            catch
            {
                UnityEngine.Debug.Log("Error trying to open the file");
                return;
            }

            try
            {
                //Listening
                while (!_shouldStop)
                {
                    //If readable XML chunk, process data
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
                                    time = float.Parse(reader.GetAttribute("time"));
                                    trafficDB.InsertNewTimeStep(time);
                                }

                                //Reading vehicle
                                if (reader.Name.Equals("vehicle"))
                                {
                                    trafficDB.InsertVehicle(reader.GetAttribute("id"),
                                            reader.GetAttribute("x"),
                                            reader.GetAttribute("y"),
                                            reader.GetAttribute("type"),
                                            reader.GetAttribute("angle")
                                            );
                                }
                                break;

                            //Reading end element
                            case XmlNodeType.EndElement:

                                if (reader.Name.Equals("fcd-export"))
                                {
                                    UnityEngine.Debug.Log("FCD File fully loaded in memory\n");
                                    return;
                                }
                                break;
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                UnityEngine.Debug.Log("Thread for listening aborted");
                return;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Error while listening!\n" + e.ToString());
                return;
            }
            finally
            {
                _shouldStop = true;

                if (reader != null)
                    reader.Close();

                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }

                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader.Dispose();
                }
                UnityEngine.Debug.Log("Streams closed in SumoFCDListener.");
            }

        }
        else
        {
            UnityEngine.Debug.Log("FCD File not found, reading aborted");
            return;
        }
    }

    /// <summary>
    /// Releases the resources when the object containing the script is being destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (streamSocket != null)
        {
            streamSocket.Close();
            streamSocket.Dispose();
        }
        if (sumoSocket != null)
        {
            sumoSocket.Close();
        }
        if (thread != null)
        {
            _shouldStop = true;
            thread.Abort();
        }
    }

    /// <summary>
    /// When application quits, aborts the thread for listening. 
    /// </summary>
    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Application quitting! Abort FCD Listener");
        _shouldStop = true;
        if (thread != null)
            thread.Abort();
        return;
    }
}
