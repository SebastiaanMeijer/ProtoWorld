/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 *
 * Stockholm MatSIM integration.
 * 
 * Berend Wouda
 * 
 */

using System.Globalization;

/// <summary>
/// This singleton remains in memory while scenes are loading, carrying the selected simulation parameters to the scene being loaded.
/// </summary>
public class StockholmMatSIMParameters {
	private static StockholmMatSIMParameters instance;

	public static StockholmMatSIMParameters Instance {
		get {
			if(instance == null) {
				instance = new StockholmMatSIMParameters();
			}

			return instance;
		}
	}


	private StockholmMatSIMLocation location;
	private StockholmMatSIMHour hour;
	private StockholmMatSIMEmployment employment;
	private StockholmMatSIMCommitment commitment;

	private bool set;


	public StockholmMatSIMLocation Location {
		get {
			return location;
		}
		set {
			location = value;

			set = true;
		}
	}
	
	public StockholmMatSIMHour Hour {
		get {
			return hour;
		}
		set {
			hour = value;

			set = true;
		}
	}

	public StockholmMatSIMEmployment Employment {
		get {
			return employment;
		}
		set {
			employment = value;

			set = true;
		}
	}

	public StockholmMatSIMCommitment Commitment {
		get {
			return commitment;
		}
		set {
			commitment = value;

			set = true;
		}
	}


	public bool Set {
		get {
			return set;
		}
	}


	public string ConnectionString {
		get {
			// This isn't really necessary but you can use it to start in an unselectable simulation like the plain Stockholm simulation.
			// Do note that the start time is still the one from the constructor.
			//string database = "StockholmPlainSQLScriptTest";
			string database = "stockholm_alvsjo_employed_max";
			string server = "127.0.0.1";
			string port = "5432";
			string userID = "postgres";
			//string password = "test";
			string password = "postgres";

			if(set) {
				database = string.Format("stockholm_{0}_{1}_{2}", location.ToString().ToLower(CultureInfo.InvariantCulture), employment.ToString().ToLower(CultureInfo.InvariantCulture), commitment == StockholmMatSIMCommitment.Minimum ? "min" : "max");
			}

			// This is hard-coded! This needs to be changed here to work for the deployment setup.
			return string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", server, port, database, userID, password);
		}
	}


	private StockholmMatSIMParameters() {
		location = StockholmMatSIMLocation.Alvsjo;
		hour = StockholmMatSIMHour.Hour7;
		employment = StockholmMatSIMEmployment.Employed;
		commitment = StockholmMatSIMCommitment.Maximum;

		set = false;
	}
}
