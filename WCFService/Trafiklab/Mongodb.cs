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
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Wrappers;

namespace Trafiklab
{
	/*
	 {
        "_id" : ObjectId("536a268800555f776ef3e6ba"),
        "LatestUpdate" : "2014-05-07T14:31:07.1072301+02:00",
        "xmlns" : "http://www1.sl.se/realtidws/",
        "xmlnsxsi" : "http://www.w3.org/2001/XMLSchema-instance",
        "Buses" : {
                "DpsBus" : [
                        {
                                "TimeTabledDateTime" : "2014-05-07T14:43:48",
                                "TransportMode" : "BUS",
                                "DisplayTime" : "15 min",
                                "ExpectedDateTime" : "2014-05-07T14:46:33",
                                "Destination" : "Lagnö by via Gärdsvik Åsättra",

                                "StopAreaName" : "Gärdsviks vägskäl",
                                "SiteId" : "2770",
                                "LineNumber" : "626",
                                "StopAreaNumber" : "62615"
                        },
                        {
                                "TimeTabledDateTime" : "2014-05-07T14:55:24",
                                "TransportMode" : "BUS",
                                "DisplayTime" : "27 min",
                                "ExpectedDateTime" : "2014-05-07T14:58:09",
                                "Destination" : "Lagnö by via Gärdsvik Åsättra",

                                "StopAreaName" : "Gärdsviks vägskäl",
                                "SiteId" : "2770",
                                "LineNumber" : "626",
                                "StopAreaNumber" : "62615"
                        }
                ]
        },
        "ExecutionTime" : "00:00:00.0468738",
        "xmlnsxsd" : "http://www.w3.org/2001/XMLSchema",
        "Trams" : {

        },
        "Trains" : {

        },
        "Metros" : {

        }
}
	 */
	public class MongodbBusses
	{
		public object _id;
		public string LatestUpdate;
		public string xmlns;
		public string xmlnsxsi;
		public List<Bus> Buses;
	}
	
	public class Bus
	{
		public DpsBus DpsBus;
	}
	public class DpsBus
	{
		public string TimeTabledDateTime;
		public string TransportMode;
		public string DisplayTime;
		public string ExpectedDateTime;
		public string Destination;

		public string StopAreaName;
		public string SiteId;
		public string LineNumber;
		public string StopAreaNumber;
	}
	public class Trams
	{ }
	public class Trains
	{ }
	public class Metros
	{ }

}
