using Npgsql;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Xml;

namespace MatSIMPlansFileScoreParser {
	/// <summary>
	/// Quick-and-dirty plans file score parser.
	/// </summary>
	class Program {
		const string scoresTableName = "scores";
		const int numberOfStatementsPerTransaction = 1000;

		static void Main(string[] args) {
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			if(args.Length != 2) {
				Console.WriteLine("Example usage: MatSIMPlansFileScoreParser.exe 1000.plans.xml[.gz] \"Server=127.0.0.1;Port=5432;Database=Stockholm;User Id=postgres;Password=test;\"");

				return;
			}

			string path = args[0];
			string connectionString = args[1];

			Stream stream = new FileStream(path, FileMode.Open);

			if(path.EndsWith(".gz")) {
				stream = new GZipStream(stream, CompressionMode.Decompress);
			}

			try {
				Console.WriteLine("Parsing plans file scores...");

				NpgsqlConnection connection = new NpgsqlConnection(connectionString);

				string createTableString = string.Format("DROP TABLE IF EXISTS {0}; CREATE TABLE {0} (pid integer, score real);", scoresTableName);
				NpgsqlCommand createTableCommand = new NpgsqlCommand(createTableString, connection);

				connection.Open();
				createTableCommand.ExecuteNonQuery();
				connection.Close();

				connection.Open();

				NpgsqlTransaction transaction = null;

				bool isPerson = false;
				int id = -1;

				int counter = 0;
				int numberOfStatements = 0;

				Console.Write("\r{0} scores parsed", counter);

				using(XmlReader reader = new XmlTextReader(stream)) {
					while(reader.Read()) {
						switch(reader.NodeType) {
							case XmlNodeType.Element:
								if(reader.Name.Equals("person")) {
									isPerson = int.TryParse(reader.GetAttribute("id"), out id);
								}
								else if(reader.Name.Equals("plan")) {
									if(isPerson) {
										if(reader.GetAttribute("selected") == "yes") {
											float score;
											if(float.TryParse(reader.GetAttribute("score"), out score)) {
												if(transaction == null) {
													transaction = connection.BeginTransaction();
												}

												string insertIntoString = string.Format("INSERT INTO {0} (pid, score) VALUES ({1}, {2});", scoresTableName, id, score);
												NpgsqlCommand insertIntoCommand = new NpgsqlCommand(insertIntoString, connection);

												insertIntoCommand.ExecuteNonQuery();

												counter += 1;
												numberOfStatements += 1;
											}
										}
									}
								}
								break;
							case XmlNodeType.EndElement:
								if(reader.Name.Equals("person")) {
									isPerson = false;
								}
								break;
						}

						if(numberOfStatements == numberOfStatementsPerTransaction) {
							transaction.Commit();
							transaction.Dispose();
							transaction = null;

							numberOfStatements = 0;

							Console.Write("\r{0} scores parsed", counter);
						}
					}
				}

				if(transaction != null) {
					transaction.Commit();
					transaction.Dispose();

					Console.Write("\r{0} scores parsed", counter);
				}

				Console.WriteLine();

				connection.Close();
				
				Console.WriteLine("Plans file scores parsed!");
			}
			catch(Exception exception) {
				Console.WriteLine(exception);
			}
		}
	}
}
