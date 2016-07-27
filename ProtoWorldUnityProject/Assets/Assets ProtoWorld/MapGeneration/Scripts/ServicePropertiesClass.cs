/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
//using UnityEditor;
using Aram.OSMParser;
using System.Linq;
using System.Xml;
using System.Collections;
//using System.Data.OleDb;
using System.ServiceModel;

/// <summary>
/// The main WCF binding class. Here, the connection parameters to the WCF services are defined.
/// </summary>
public class ServicePropertiesClass : ScriptableObject
{
	/// <summary>
	/// Gets the  net tcp binding. Note that the WCf service must be set accordingly and using this class directly does not guarantee it to work.
	/// </summary>
    /// <returns>Returns the <see cref="NetTcpBinding"/> to be used with <see cref="ServiceGapslabsClient"/> </returns>
    public static NetTcpBinding GetTCPBinding()
    {
        NetTcpBinding b = new NetTcpBinding();
        b.SendTimeout = new System.TimeSpan(1, 0, 0);
        b.ReceiveTimeout = new System.TimeSpan(1, 15, 0);
        b.OpenTimeout = new System.TimeSpan(1, 15, 0);
        b.MaxBufferPoolSize = 10485760;
        b.MaxBufferSize = 10485760;
        b.MaxReceivedMessageSize = 10485760;
        return b;
    }
    /// <summary>
    /// Gets the basic http binding. Note that the WCf service must be set accordingly and using this class directly does not guarantee it to work.
    /// </summary>
    /// <returns>Returns the <see cref="BasicHttpBinding"/> binding to be used with <see cref="ServiceGapslabsClient"/> </returns>
	public static BasicHttpBinding GetBinding()
	{
		BasicHttpBinding b=new BasicHttpBinding();
		b.SendTimeout= new System.TimeSpan(1,0,0);
		b.ReceiveTimeout=new System.TimeSpan(1,15,0);
		b.OpenTimeout=new System.TimeSpan(1,15,0);
		b.MaxBufferPoolSize=10485760;
		b.MaxBufferSize=10485760;
		b.MaxReceivedMessageSize=10485760; 
		return b;
	}
	public static string ServiceUri = "http://localhost:53869/ServiceGapslabs.svc";
	public static string ServiceUriLocal="http://localhost/GapslabGIS/ServiceGapslabs.svc";
	public static string ServiceUriDevelopment="http://localhost:53869/ServiceGapslabs.svc";
	
	public static string ConnectionDatabase = "Server=localhost\\SQLEXPRESS;Database=GIStest;User Id=gapslabuser;Password=test;";
    
    // IMPORTANT: Change the ConnectionPostgreDatabase string from the object "AramGISBoundaries":
    //      - Check parameter "Override Database Connection" to true
    //      - Write the desire connection string in "Overriden Connection String"
    // -- Miguel R. C.
    public static string ConnectionPostgreDatabase = "Server=127.0.0.1;Port=5432;Database=GIS_Stockholm;User Id=postgres;Password=test;";
    //public static string ConnectionPostgreDatabase = "Server=127.0.0.1;Port=5432;Database=GIS_Paris;User Id=postgres;Password=test;";
    //public static string ConnectionPostgreDatabase = "Server=127.0.0.1;Port=5432;Database=GIS_Parc_des_Princes;User Id=postgres;Password=test;";
    //public static string ConnectionPostgreDatabase = "Server=127.0.0.1;Port=5432;Database=GIS_Vatican;User Id=postgres;Password=test;";
    
    public static string connPostGreSqlParis = "Server=127.0.0.1;Port=5432;Database=GIS_Paris;User Id=postgres;Password=test;";
	// 
	public static string ConnectionPostgreDatabaseTestForDistribution = "Server=127.0.0.1;Port=5432;Database=TestForDistribution;User Id=postgres;Password=test;";
    /// <summary>
    /// Initializes and returns an instance of <see cref="ServiceGapslabsClient"/>.
    /// </summary>
    /// <returns>returns an instance of <see cref="ServiceGapslabsClient"/>.</returns>
    /// <seealso cref="GetGapslabsService(string ServiceUri)"/>
	public static ServiceGapslabsClient GetGapslabsService()
	{
		ServiceGapslabsClient client=new ServiceGapslabsClient(GetBinding(),new EndpointAddress( ServiceUri));
		return client;
	}
    /// <summary>
    /// Initializes and returns an instance of <see cref="ServiceGapslabsClient"/> given the Endpoint service uri.
    /// </summary>
    /// <returns>returns an instance of <see cref="ServiceGapslabsClient"/>.</returns>
    /// <seealso cref="GetGapslabsService()"/>
	public static ServiceGapslabsClient GetGapslabsService(string ServiceUri)
	{
        ServiceGapslabsClient client;
        try
        {
            client = new ServiceGapslabsClient(GetBinding(), new EndpointAddress(ServiceUri));
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);
            client = new ServiceGapslabsClient(GetBinding(), new EndpointAddress(ServiceUri));
            return client;
        }
        
        //ServiceGapslabsClient client = new ServiceGapslabsClient(GetTCPBinding(), new EndpointAddress(ServiceUri));
		return client;
	}
}