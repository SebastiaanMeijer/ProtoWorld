using System;
using System.IO;

namespace ExportMatsimFilesToDB
{
    class MatsimContainer
    {
        public MatSimNetwork matSimNetwork;
        public MatSimSchedule matSimSchedule;
        public MatSimsVehicles vehicleDefinition;
        public MatsimEvents matSimEvents;

        public string connectionString;

        public MatsimContainer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void LoadEvents(string path)
        {
            //var eventsPath = Path.Combine(path, "500.events.xml");
            //var eventsPath = Path.Combine(path, "event_sample.xml");
            //matSimEvents = MatsimEvents.Load(eventsPath);

            matSimEvents = MatsimEvents.ReadXml(path);
            //Console.WriteLine("events loaded!");

            //Console.WriteLine(matSimEvents.lines[0]);
            //Console.WriteLine(matSimEvents.lines[1]);

        }

        public void LoadNetwork(string path)
        {
            var networkPath = Path.Combine(path, "network-plain.xml");
            matSimNetwork = MatSimNetwork.Load(networkPath);
            Console.WriteLine("Network loaded!");
        }

        public void LoadSchedule(string path)
        {
            var schedulePath = Path.Combine(path, "transitSchedule.xml");
            matSimSchedule = MatSimSchedule.Load(schedulePath);
            Console.WriteLine("Schedule loaded!");

        }

        public void LoadVehicles(string path)
        {
            var vehiclePath = Path.Combine(path, "vehicles.xml");
            vehicleDefinition = MatSimsVehicles.Load(vehiclePath);
            Console.WriteLine("Vehicles loaded!");

        }

        /// <summary>
        /// Export matsim network to Postgre db.
        /// </summary>
        /// <param name="connectionString"></param>
        public void ExportEventsToPostgre(string connectionString)
        {
            if (matSimEvents == null)
            {
                Console.WriteLine("Network is null or empty");
                return;
            }

            matSimEvents.ExportToPostgreSQL(connectionString);
            Console.WriteLine("Export events done!");
        }

        /// <summary>
        /// Export matsim network to Postgre db.
        /// </summary>
        /// <param name="connectionString"></param>
        public void ExportNetworkToPostgre(string connectionString)
        {
            if (matSimNetwork == null)
            {
                Console.WriteLine("Network is null or empty");
                return;
            }
            matSimNetwork.ExportToPostgreSQL(connectionString);

            Console.WriteLine("Export network done!");
        }

        /// <summary>
        /// Export matsim schedule to Postgre db.    
        /// </summary>
        /// <param name="connectionString">Server={0};Port={1};Database={2};User Id={3};Password={4}</param>
        public void ExportScheduleToPostgre(string connectionString)
        {
            if (matSimSchedule == null)
            {
                Console.WriteLine("Schedule is null or empty");
                return;
            }
            matSimSchedule.ExportToPostgreSQL(connectionString);

            Console.WriteLine("Export schedule done!");
        }

        public void ExportVehiclesToPostgre(string connectionString)
        {
            if (vehicleDefinition == null)
                return;
            vehicleDefinition.ExportToPostgreSQL(connectionString);

            Console.WriteLine("Export vehicles done!");
        }
    }
}
