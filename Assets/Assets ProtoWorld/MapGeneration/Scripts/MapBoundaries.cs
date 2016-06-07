/*
 * 
 * SCENARIO GENERATION
 * MapBoundaries.cs
 * Aram Azhari
 * Miguel Ramos Carretero
 */

using System;
using System.Collections;
using System.Linq;
using System.Xml;
using Aram.OSMParser;
using UnityEngine;

/// <summary>
/// Defines the parameters for generating the map from OSM into Unity.
/// </summary>
[Serializable()]
public class MapBoundaries : MonoBehaviour
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    [HideInInspector]
    public string filename;

    public float OneMeterInUnity3DUnits = 0.5f;

    public double minLat = 0;
    public double maxLat = 1;
    public double minLon = 0;
    public double maxLon = 1;

    [HideInInspector]
    public string Name;

    public float[] minMaxX = new float[2] { 0, 20000f };
    public float[] minMaxY = new float[2] { 0, 40000f };

    [HideInInspector]
    public bool RemoveRedundantPointsOnTheSameLine = true;

    [HideInInspector]
    public float RedundantPointErrorThreshold = 0.001f;

    [HideInInspector]
    public bool SegmentLines = true;

    [HideInInspector]
    public float BuildingLineThickness = 1.2f;

    [HideInInspector]
    public float RoadLineThickness = 1.0f;

    [HideInInspector]
    [Compact]
    public Vector2 Scale = new Vector2(10, 10);

    [HideInInspector]
    public Color BuildingColor;

    [HideInInspector]
    public Color LineColorStart;

    [HideInInspector]
    public Color LineColorEnd;

    [SerializeField()]
    public Vector3 MinPointOnMap;

    public Material BuildingMaterial;
    public Material RoadMaterial;
    public Material CycleWayMaterial;
    public Material FootWayMaterial;
    public Material RailWayMaterial;
    public Material StepsMaterial;
    public Material ResidentialMaterial;
    public Material ServiceMaterial;
    public Material AreaMaterial;
    public Material WaterMaterial;

    public float RoadWidth = 0.1f;
    public float CyclewayWidth = 0.05f;
    public float FootwayWidth = 0.025f;
    public float BuildingHeight = 7.5f;
    public float BuildingMinimumHeight;
    public float BuildingMaximumHeight;

    public AnimationCurve BuildingHeightVariation;

    [HideInInspector]
    [Compact]
    public Vector2 CombinationOptimizationSize = new Vector2(100, 100);

    public bool OverrideDatabaseConnection = false;

    public string serverAddress = "127.0.0.1";
    public string serverPort = "5432";
    public string serverUserId = "postgres";
    public string serverDatabaseName = "GIS";
    public string serverPassword = "test";

    //Using local DB Bounds allows the user to set the database boundaries manually,
    //easing from the need of having the WCF active in play mode.
    public bool UseLocalDatabaseBounds;

    public double dbBoundMinLat;
    public double dbBoundMaxLat;
    public double dbBoundMinLon;
    public double dbBoundMaxLon;

    /// <summary>
    /// Starts the script.
    /// </summary>
    void Start()
    {
        log.Info("Map boundaries : MaxLat " + maxLat + ", MinLat " + minLat + ", MaxLon " + maxLon + ", MinLon " + minLon);

        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        //Initialize the coordinate convertor (need it for running the game!)
        CoordinateConvertor.Initialize(client, this);
    }

    /// <summary>
    /// Gets the boundaries of the scenario defined.
    /// </summary>
    /// <returns></returns>
    public double[] GetBoundsMinMaxLatMinMaxLon()
    {
        return new double[] { this.minLat, this.maxLat, this.minLon, this.maxLon };
    }

    /// <summary>
    /// Gets the connection string overriden from the default one.
    /// </summary>
    /// <returns>String containing the overriden connection string.</returns>
    public string GetOverridenConnectionString()
    {
        return "Server=" + serverAddress + ";Port=" + serverPort + ";Database=" + serverDatabaseName + ";User Id=" + serverUserId + ";Password=" + serverPassword + ";";
    }
}
