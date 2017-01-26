using System;

/// <summary>
/// This singleton remains in memory while scenes are loading, carrying the selected simulation parameters to the scene being loaded.
/// </summary>
public class StockholmMatSIMParameters {
	private static StockholmMatSIMParameters instance;

	public static StockholmMatSIMParameters getInstance() {
		if(instance == null) {
			instance = new StockholmMatSIMParameters();
		}

		return instance;
	}


	public StockholmMatSIMLocation location {
		get;
		set;
	}

	public StockholmMatSIMEmployment employment {
		get;
		set;
	}

	public StockholmMatSIMCommitment commitment {
		get;
		set;
	}


	private StockholmMatSIMParameters() {
		location = StockholmMatSIMLocation.Alvsjo;
		employment = StockholmMatSIMEmployment.Plain;
		commitment = StockholmMatSIMCommitment.Minimum;
	}
}
