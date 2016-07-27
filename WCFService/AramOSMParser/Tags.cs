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
using System.Threading.Tasks;

namespace Aram.OSMParser
{
    /// <summary>
    /// The class that defines the values for the openstreetmap tags.
    /// </summary>
    public static class Tags
    {
        /// <summary>
        /// The class that defines the values for the "Highway" tag.
        /// </summary>
        public static class Highways
        {
            /// <summary>
            /// higyway=motorway_link
            /// </summary>
            public readonly static string motorway_link = "motorway_link";
            /// <summary>
            /// higyway=minor
            /// </summary>
            public readonly static string minor = "minor";
            /// <summary>
            /// higyway=tertiary_link
            /// </summary>
            public readonly static string tertiary_link = "tertiary_link";
            /// <summary>
            /// higyway=moved:cycleway
            /// </summary>
            public readonly static string moved_cycleway = "moved:cycleway";
            /// <summary>
            /// higyway=bridleway
            /// </summary>
            public readonly static string bridleway = "bridleway";
            /// <summary>
            /// higyway=construction
            /// </summary>
            public readonly static string construction = "construction";
            /// <summary>
            /// higyway=primary_link
            /// </summary>
            public readonly static string primary_link = "primary_link";
            /// <summary>
            /// higyway=sr
            /// </summary>
            public readonly static string sr = "sr";
            /// <summary>
            /// higyway=pedestrian
            /// </summary>
            public readonly static string pedestrian = "pedestrian";
            /// <summary>
            /// higyway=disused
            /// </summary>
            public readonly static string disused = "disused";
            /// <summary>
            /// higyway=planned
            /// </summary>
            public readonly static string planned = "planned";
            /// <summary>
            /// higyway=hallway
            /// </summary>
            public readonly static string hallway = "hallway";
            /// <summary>
            /// higyway=proposal
            /// </summary>
            public readonly static string proposal = "proposal";
            /// <summary>
            /// higyway=services
            /// </summary>
            public readonly static string services = "services";
            /// <summary>
            /// higyway=elevator
            /// </summary>
            public readonly static string elevator = "elevator";
            /// <summary>
            /// higyway=path
            /// </summary>
            public readonly static string path = "path";
            /// <summary>
            /// higyway=unclassified
            /// </summary>
            public readonly static string unclassified = "unclassified";
            /// <summary>
            /// higyway=ford
            /// </summary>
            public readonly static string ford = "ford";
            /// <summary>
            /// higyway=trunk_link
            /// </summary>
            public readonly static string trunk_link = "trunk_link";
            /// <summary>
            /// higyway=driveway
            /// </summary>
            public readonly static string driveway = "driveway";
            /// <summary>
            /// higyway=path;footway
            /// </summary>
            public readonly static string path_footway = "path;footway";
            /// <summary>
            /// higyway=old
            /// </summary>
            public readonly static string old = "old";
            /// <summary>
            /// higyway=escalator
            /// </summary>
            public readonly static string escalator = "escalator";
            /// <summary>
            /// higyway=street_lamp
            /// </summary>
            public readonly static string street_lamp = "street_lamp";
            /// <summary>
            /// higyway=secondary
            /// </summary>
            public readonly static string secondary = "secondary";
            /// <summary>
            /// higyway=tertiary
            /// </summary>
            public readonly static string tertiary = "tertiary";
            /// <summary>
            /// higyway=proposed
            /// </summary>
            public readonly static string proposed = "proposed";
            /// <summary>
            /// higyway=cycleway
            /// </summary>
            public readonly static string cycleway = "cycleway";
            /// <summary>
            /// higyway=closed:service
            /// </summary>
            public readonly static string closed_service = "closed:service";
            /// <summary>
            /// higyway=track
            /// </summary>
            public readonly static string track = "track";
            /// <summary>
            /// higyway=turning_circle
            /// </summary>
            public readonly static string turning_circle = "turning_circle";
            /// <summary>
            /// higyway=rest_area
            /// </summary>
            public readonly static string rest_area = "rest_area";
            /// <summary>
            /// higyway=raceway
            /// </summary>
            public readonly static string raceway = "raceway";
            /// <summary>
            /// higyway=residential
            /// </summary>
            public readonly static string residential = "residential";
            /// <summary>
            /// higyway=public_transport
            /// </summary>
            public readonly static string public_transport = "public_transport";
            /// <summary>
            /// higyway=service
            /// </summary>
            public readonly static string service = "service";
            /// <summary>
            /// higyway=bus_guideway
            /// </summary>
            public readonly static string bus_guideway = "bus_guideway";
            /// <summary>
            /// higyway=crossing
            /// </summary>
            public readonly static string crossing = "crossing";
            /// <summary>
            /// higyway=motorway
            /// </summary>
            public readonly static string motorway = "motorway";
            /// <summary>
            /// higyway=abandoned
            /// </summary>
            public readonly static string abandoned = "abandoned";
            /// <summary>
            /// higyway=steps
            /// </summary>
            public readonly static string steps = "steps";
            /// <summary>
            /// higyway=living_street
            /// </summary>
            public readonly static string living_street = "living_street";
            /// <summary>
            /// higyway=historic
            /// </summary>
            public readonly static string historic = "historic";
            /// <summary>
            /// higyway=trunk
            /// </summary>
            public readonly static string trunk = "trunk";
            /// <summary>
            /// higyway=secondary_link
            /// </summary>
            public readonly static string secondary_link = "secondary_link";
            /// <summary>
            /// higyway=road
            /// </summary>
            public readonly static string road = "road";
            /// <summary>
            /// higyway=primary
            /// </summary>
            public readonly static string primary = "primary";
            /// <summary>
            /// higyway=platform
            /// </summary>
            public readonly static string platform = "platform";
            /// <summary>
            /// higyway=footway
            /// </summary>
            public readonly static string footway = "footway";
            /// <summary>
            /// higyway=bus_stop
            /// </summary>
            public readonly static string bus_stop = "bus_stop";
            /// <summary>
            /// higyway=trail
            /// </summary>
            public readonly static string trail = "trail";
            /// <summary>
            /// higyway=dismantled
            /// </summary>
            public readonly static string dismantled = "dismantled";
            /// <summary>
            /// higyway=conveyor
            /// </summary>
            public readonly static string conveyor = "conveyor";

        }
    }
}