using System;

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
			string database = "Stockholm_Plain";

			if(set) {
				database = string.Format("Stockholm_{0}_{1}_{2}", location.ToString(), employment.ToString().ToLower(), commitment == StockholmMatSIMCommitment.Minimum ? "min" : "max");
			}

			// TODO This is hard-coded! This needs to either be somehow set from Unity or changed here to work for the deployment setup.
			return string.Format("Server=127.0.0.1;Port=5432;Database={0};User Id=postgres;Password=test;", database);
		}
	}


	private StockholmMatSIMParameters() {
		location = StockholmMatSIMLocation.Alvsjo;
		employment = StockholmMatSIMEmployment.Plain;
		commitment = StockholmMatSIMCommitment.Minimum;

		set = false;
	}
}
