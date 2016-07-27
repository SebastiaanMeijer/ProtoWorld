/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using Aram.OSMParser;
using System.Linq;
using System.Xml;
using System.Collections;
using System.Data.OleDb;
using System.ServiceModel;
using OsmParserTestApplication.ServiceReference1;

namespace OsmParserTestApplication
{
    public class ServicePropertiesClass
    {

        public static BasicHttpBinding GetBinding()
        {
            BasicHttpBinding b = new BasicHttpBinding();
            b.SendTimeout = new System.TimeSpan(0, 5, 0);
            b.MaxBufferPoolSize = 10485760;
            b.MaxBufferSize = 10485760;
            b.MaxReceivedMessageSize = 10485760;
            return b;
        }
        public static string ServiceUri = "http://localhost:53869/ServiceGapslabs.svc";
        public static string ConnectionDatabase = "Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";
        public static string ConnectionPostgreDatabase = "Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;";
        public static ServiceGapslabsClient GetGapslabsService()
        {
            ServiceGapslabsClient client = new ServiceGapslabsClient(GetBinding(), new EndpointAddress(ServiceUri));
            return client;
        }
        public static ServiceGapslabsClient GetGapslabsService(string ServiceUri)
        {
            ServiceGapslabsClient client = new ServiceGapslabsClient(GetBinding(), new EndpointAddress(ServiceUri));
            return client;
        }
    }
    public class Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
    }
}