using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ExportMatsimFilesToDB
{
    public class MatSimsVehicles
    {
        [XmlElement("vehicleType")]
        public List<VehicleType> vTypes;

        [XmlElement("vehicle")]
        public List<Vehicle> vehicles;

        public static MatSimsVehicles Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                XmlRootAttribute xRoot = new XmlRootAttribute();
                xRoot.ElementName = "vehicleDefinitions";
                xRoot.Namespace = "http://www.matsim.org/files/dtd";
                xRoot.IsNullable = true;

                var serializer = new XmlSerializer(typeof(MatSimsVehicles), xRoot);
                return serializer.Deserialize(stream) as MatSimsVehicles;
            }
        }

        public void Save(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(MatSimsVehicles));
                serializer.Serialize(stream, this);
            }
        }

        public string GetVehicleTypeString(string id)
        {
            return vTypes.Find(t => t.id == id).ToString();
        }


        public string GetVehicleString(string id)
        {
            return vehicles.Find(veh => veh.id == id).ToString();
        }

        internal void ExportToPostgreSQL(string connectionString)
        {
            if (vTypes == null || vTypes.Count == 0)
                return;
            if (vehicles == null || vehicles.Count == 0)
                return;

            string typeTable = "matsimvehtype";
            string vehTable = "matsimvehicle";

            string commandString = string.Format("DROP TABLE IF EXISTS {0};" +
                "CREATE TABLE {0}(id text, seats integer, stands integer );" +
                "DROP TABLE IF EXISTS {1};" +
                "CREATE TABLE {1}(id text, type text);",
                typeTable, vehTable);

            try
            {
                NpgsqlConnection dbConn = new NpgsqlConnection(connectionString);

                var dbCommand = new NpgsqlCommand(commandString, dbConn);

                dbConn.Open();
                dbCommand.ExecuteNonQuery();
                dbConn.Close();

                dbConn.Open();

                int counterVeh = 0;
                int totalVeh = vehicles.Count;

                foreach (var veh in vehicles)
                {
                    Console.Write("\r{0} of {1} vehicles exported", ++counterVeh, totalVeh);

                    var insertString = string.Format("INSERT INTO {0} VALUES ('{1}','{2}');",
                        vehTable, veh.id, veh.type);
                    byte[] bytes = Encoding.Default.GetBytes(insertString);
                    insertString = Encoding.UTF8.GetString(bytes);
                    dbCommand = new NpgsqlCommand(insertString, dbConn);
                    dbCommand.ExecuteNonQuery();
                }
                dbConn.Close();
                Console.WriteLine();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }

    public class VehicleType
    {
        [XmlAttribute]
        public string id;

        public override string ToString()
        {
            //return $"id: {id}";
            return string.Format("id: {0}", id);
        }
    }

    public class Vehicle
    {
        [XmlAttribute]
        public string id;

        [XmlAttribute]
        public string type;

        public override string ToString()
        {
            //return $"id: {id}, type: {type}";
            return string.Format("id: {0}, type: {1}", id, type);

        }
    }

}
