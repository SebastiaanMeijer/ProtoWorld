using UnityEngine;
using System.Collections;
using System.Xml;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System;
using System.ComponentModel;

public class SumoIO : SimulationIOBase
{

    public override bool CanRead(string fileName)
    {
        if (System.IO.Path.GetExtension(fileName).ToLower().Equals("xml"))
            return true;
        else
            return false;
    }

    public override bool CanWrite(string fileName)
    {
        //TODO
        return false;
    }

    public void ReadXml(XmlReader reader)
    {
        try
        {
            float time;
            //Listening
            while (!shouldStop)
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
                                TryAddTimeStep(time);
                                //trafficDB.InsertNewTimeStep(time);
                            }

                            //Reading vehicle
                            if (reader.Name.Equals("vehicle"))
                            {
                                InsertVehicle(reader.GetAttribute("id"),
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
            Debug.Log("Thread for ReadXml aborted");
            return;
        }
        catch (Exception e)
        {
            Debug.Log("Error in ReadXml\n" + e.ToString());
            return;
        }
    }

    /// <summary>
    /// Listen to Sumo FCD from local port.
    /// </summary>
    /// <param name="port"></param>
    public override void Read(int port)
    {
        XmlReader reader;
        TcpListener sumoListener;
        Socket sumoSocket;
        Stream streamSocket;

        try
        {
            //Start the listener
            UnityEngine.Debug.Log("Starting FCD Listener (SumoIO)...");
            sumoListener = new TcpListener(IPAddress.Any, port);
            sumoListener.Start();
            UnityEngine.Debug.Log("Listening in port " + port + "...\n");

            //Open a socket to receive data from SUMO 
            sumoSocket = sumoListener.AcceptSocket();

            //Create a xml reader to read from the stream of the socket
            streamSocket = new NetworkStream(sumoSocket);
            reader = XmlReader.Create(streamSocket);

            //Start reading xml.
            ReadXml(reader);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("Error trying to listen from SUMO, is the server running?" + e.ToString());
            return;
        }
        finally
        {
            // No need to set shouldStop = true; otherwise it will cause errors in Sumo.
            // No need to close reader or socket, otherwise it will cause errors in Sumo.
            UnityEngine.Debug.Log("Stream closed by Sumo.");
        }
    }

    /// <summary>
    /// Read Sumo FCD from file.
    /// </summary>
    /// <param name="fileName"></param>
    public override void Read(string fileName)
    {
        FileStream fileStream = null;
        StreamReader streamReader = null;
        XmlReader reader = null;

        try
        {

            UnityEngine.Debug.Log("Opening FCD file...");

            fileStream = File.OpenRead(fileName);
            streamReader = new StreamReader(fileStream);
            reader = XmlReader.Create(streamReader);
            UnityEngine.Debug.Log("Reading FCD File...");

            //Start reading xml.
            ReadXml(reader);

        }
        catch
        {
            UnityEngine.Debug.Log("Error trying to open the file");
            return;
        }
        finally
        {
            // Just in case, make sure the thread stops reading.
            shouldStop = true;

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
            UnityEngine.Debug.Log("Stream closed in SumoIO.");
        }

    }

    public override void Write(object sender, DoWorkEventArgs e)
    {
        if (trafficDB == null)
            return;
        TimeStepTDB tdb;
        using (XmlWriter writer = XmlWriter.Create((string)e.Argument))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("fcd-export");

            for (int i = 0; i < trafficDB.getNumberOfTimeSteps(); i++)
            {
                tdb = trafficDB.GetTimeStep(i);
                writer.WriteStartAttribute("timestep", "time", null);
                writer.WriteString(tdb.time.ToString());
                writer.WriteEndAttribute();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }


}
