/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using Npgsql;

public class MatsimIO : SimulationIOBase
{
    public override bool CanRead(string connectionString)
    {
        Debug.Log("Not implemented yet");
        return false;
    }

    public override bool CanWrite(string fileName)
    {
        Debug.Log("Not implemented yet");
        return false;
    }

    /// <summary>
    /// First version to read from the DB.
    /// </summary>
    /// <param name="connectionString"></param>
    public override void Read(string connectionString)
    {
        string vehTable = "event_position";
        currentReadingStep = 3600*5;

        try
        {
            NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);

            // TODO It might generate a connection timeout if this queries the whole table,
            // therefore more reasonable to split up the query by eventtime (ex. using a for-loop)...
            string commandString = string.Format(
                "SELECT * FROM {0} WHERE eventtime>={1}" +
                " AND eventtime<{2} ORDER BY eventtime, veh_id",
                vehTable, trafficDB.getNumberOfTimeSteps(), trafficDB.getNumberOfTimeSteps() + readingChunkForMatsim);

            Debug.Log(commandString);

            var dbCommand = new NpgsqlCommand(commandString, dbConn);

            dbConn.Open();
            var dbReader = dbCommand.ExecuteReader();
            List<string> output = new List<string>();
            while (dbReader.Read())
            {
                var veh_id = dbReader["veh_id"].ToString();
                var time = dbReader["eventtime"].ToString();
				var type = dbReader["veh_type"].ToString();
                var x = dbReader["x"].ToString();
                var y = dbReader["y"].ToString();
                var ang = dbReader["deg"].ToString();

                //var str = string.Join(",", new string[] { veh_id, time, x, y, ang });
                //output.Add(str);
                //Debug.Log(str);

                TryAddTimeStep(float.Parse(time));
                InsertVehicle(veh_id, x, y, type, ang);
            }
            if (!dbReader.IsClosed)
            {
                dbReader.Close();
            }

            Debug.Log("Reading chunk completed");

            dbConn.Close();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error while reading from database:" + ex);
        }
    }

    public override void Write(object sender, DoWorkEventArgs e)
    {
        Debug.Log("Not implemented yet");
    }
}
