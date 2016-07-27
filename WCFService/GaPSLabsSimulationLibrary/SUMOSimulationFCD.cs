/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

namespace GaPSlabsSimulationLibrary
{
    [Serializable]
    public class SUMOSimulationFCD : SimulationBase
    {
        public override string Name { get; set; }
        public override string Description { get; set; }
        public override TimeStep[] TimeStep { get; set; }
        public String FCDElementId = "fcd-export";
        public void LoadFromXML(String filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                List<TimeStep> timeStepList = new List<GaPSlabsSimulationLibrary.TimeStep>();
                TimeStep t = new TimeStep();
                List<VehicleBase> VehicleList = new List<VehicleBase>();
                int counter = 0;
                while (reader.Read())
                {
                    if (reader.Name == "timestep")
                    {
                        var time = reader.GetAttribute("time");
                        if (time != null)
                        {
                            t.time = float.Parse(reader.GetAttribute("time"), CultureInfo.InvariantCulture);
                            VehicleList = new List<VehicleBase>();
                        }
                        else // The end tag
                        {
                            t.Vehicles = VehicleList.ToArray();
                            t.index = counter++;
                            timeStepList.Add(t);
                            t = new TimeStep();
                        }
                    }
                    else if (reader.Name == "vehicle")
                    {
                        VehicleFCD v = new VehicleFCD();
                        v.Id = reader.GetAttribute("id");
                        v.VehicleType = reader.GetAttribute("type") == "DEFAULT_VEHTYPE" ? VehicleType.Default : VehicleType.Default;
                        v.Latitude = float.Parse(reader.GetAttribute("y"), CultureInfo.InvariantCulture);
                        v.Longitude = float.Parse(reader.GetAttribute("x"), CultureInfo.InvariantCulture);
                        v.Slope = float.Parse(reader.GetAttribute("slope"), CultureInfo.InvariantCulture);
                        v.Angle = float.Parse(reader.GetAttribute("angle"), CultureInfo.InvariantCulture);
                        v.Speed = float.Parse(reader.GetAttribute("speed"), CultureInfo.InvariantCulture);
                        v.Pos = float.Parse(reader.GetAttribute("pos"), CultureInfo.InvariantCulture);
                        v.Lane = reader.GetAttribute("lane");
                        VehicleList.Add(v);
                    }
                }
                TimeStep = timeStepList.ToArray();
            }
        }
        [Obsolete("This method cannot handle large xml files. Please use LoadFromXML() instead.",false)]
        public void LoadFromXMLSmallFile(String filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(filename));
            XmlElement root = doc.DocumentElement;
            var iterator = root.GetEnumerator();
            List<TimeStep> timeStepList = new List<GaPSlabsSimulationLibrary.TimeStep>();
            int counter = 0;
            while (iterator.MoveNext())
            {
                var currentTimeStep = iterator.Current as XmlElement;
                TimeStep t = new TimeStep();
                t.time = float.Parse(currentTimeStep.GetAttribute("time"), CultureInfo.InvariantCulture);
                var VehicleIterator = currentTimeStep.GetEnumerator();
                List<VehicleBase> VehicleList = new List<VehicleBase>();
                while (VehicleIterator.MoveNext())
                {
                    var currentVehicle = VehicleIterator.Current as XmlElement;
                    VehicleFCD v = new VehicleFCD();
                    v.Id = currentVehicle.GetAttribute("id");
                    v.VehicleType = currentVehicle.GetAttribute("type") == "DEFAULT_VEHTYPE" ? VehicleType.Default : VehicleType.Default;
                    v.Latitude = float.Parse(currentVehicle.GetAttribute("y"), CultureInfo.InvariantCulture);
                    v.Longitude = float.Parse(currentVehicle.GetAttribute("x"), CultureInfo.InvariantCulture);
                    v.Slope = float.Parse(currentVehicle.GetAttribute("slope"), CultureInfo.InvariantCulture);
                    v.Angle = float.Parse(currentVehicle.GetAttribute("angle"), CultureInfo.InvariantCulture);
                    v.Speed = float.Parse(currentVehicle.GetAttribute("speed"), CultureInfo.InvariantCulture);
                    v.Pos = float.Parse(currentVehicle.GetAttribute("pos"), CultureInfo.InvariantCulture);
                    v.Lane = currentVehicle.GetAttribute("lane");
                    VehicleList.Add(v);
                }
                t.Vehicles = VehicleList.ToArray();
                t.index = counter++;
                timeStepList.Add(t);
            }
            TimeStep = timeStepList.ToArray();
        }
        public void LoadFromCSV(String filename)
        {
            Name = "iMobility data feed";
            Description = "This is the FCD taxi data for two weeks, retrieved from http://imobilitylab.se/Download_files/stkhlm-taxi.tar.gz";
            List<TimeStep> timeStepList = new List<GaPSlabsSimulationLibrary.TimeStep>();
            List<VehicleFCD> vehicleList = new List<VehicleFCD>();
            using (StreamReader reader = new StreamReader(filename))
            {

                int counter = 0;
                // Skip the header line
                // vehicle_id,date,lon,lat
                reader.ReadLine();
                var currentLine = reader.ReadLine().Trim().Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var currentTimestep = DateTime.Parse(currentLine[1]);
                var oldTimestep = currentTimestep;
                VehicleFCD v = new VehicleFCD();
                v.Id = currentLine[0];
                v.Latitude = float.Parse(currentLine[3], CultureInfo.InvariantCulture);
                v.Longitude = float.Parse(currentLine[2], CultureInfo.InvariantCulture);
                v.VehicleType = VehicleType.Default;
                vehicleList.Add(v);
                TimeStep t = new TimeStep();
                t.iMobilityTime = currentTimestep;
                t.index = counter++;


                while (!reader.EndOfStream)
                {
                    currentLine = reader.ReadLine().Trim().Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    currentTimestep = DateTime.Parse(currentLine[1]);
                    if (oldTimestep != currentTimestep) // A new timestep has arrived, create that.
                    {
                        t.Vehicles = vehicleList.ToArray();
                        timeStepList.Add(t);
                        t = new TimeStep();
                        t.iMobilityTime = currentTimestep;
                        t.index = counter++;
                        oldTimestep = currentTimestep;
                        vehicleList = new List<VehicleFCD>();
                    }
                    v = new VehicleFCD();
                    v.Id = currentLine[0];
                    v.Latitude = float.Parse(currentLine[3], CultureInfo.InvariantCulture);
                    v.Longitude = float.Parse(currentLine[2], CultureInfo.InvariantCulture);
                    v.VehicleType = VehicleType.Default;
                    vehicleList.Add(v);
                }

                // The last timestep in the list
                t.Vehicles = vehicleList.ToArray();
                timeStepList.Add(t);
            }
            TimeStep = timeStepList.ToArray();
        }
    }
}
