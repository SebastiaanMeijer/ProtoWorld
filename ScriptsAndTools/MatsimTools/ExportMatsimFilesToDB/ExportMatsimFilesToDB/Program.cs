using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportMatsimFilesToDB
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("USAGE:");
                Console.WriteLine();
                Console.WriteLine("ExportMatsimFilesToDB.exe" + " MatsimEventFilePath"
                    + " MatsimInputFolderPath" + " ConnectionString");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("EXAMPLE:");
                Console.WriteLine();
                Console.WriteLine("ExportMatsimFilesToDB.exe"
                    + " C:\\Users\\mrc\\Desktop\\output\\500.event.xml"
                    + " C:\\Users\\mrc\\Desktop\\input"
                    + " \"Server=127.0.0.1;Port=5432;Database=GIS;User Id=postgres;Password=test;\"");
                Console.ForegroundColor = oldcolor;

                return;
            }

            // Get the arguments
            string matsimEventFilePath = args[0];
            string matsimInputFolderPath = args[1];
            string connectionString = args[2];

            // Initialize the Matsim container
            MatsimContainer container = new MatsimContainer(connectionString);

            // Read the event file
            if (File.Exists(matsimEventFilePath))
            {
                Console.WriteLine("Reading event file...");
                container.LoadEvents(matsimEventFilePath);
            }
            else
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The event file path does not exist");
                Console.ForegroundColor = oldcolor;
                return;
            }

            // Read the input files
            if (Directory.Exists(matsimInputFolderPath))
            {
                Console.WriteLine("Reading network file...");
                container.LoadNetwork(matsimInputFolderPath);

                Console.WriteLine("Reading schedule file...");
                container.LoadSchedule(matsimInputFolderPath);

                Console.WriteLine("Reading vehicles file...");
                container.LoadVehicles(matsimInputFolderPath);
            }
            else
            {
                var oldcolor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The input folder path does not exist");
                Console.ForegroundColor = oldcolor;
                return;
            }

            // Export the event file
            Console.WriteLine("Exporting events to DB...");
            container.ExportEventsToPostgre(connectionString);

            // Export the network file
            Console.WriteLine("Exporting network to DB...");
            container.ExportNetworkToPostgre(connectionString);

            // Export the schedule file
            Console.WriteLine("Exporting schedule to DB...");
            container.ExportScheduleToPostgre(connectionString);

            // Export the vehicles file
            Console.WriteLine("Exporting vehicles to DB...");
            container.ExportVehiclesToPostgre(connectionString);
        }
    }
}
