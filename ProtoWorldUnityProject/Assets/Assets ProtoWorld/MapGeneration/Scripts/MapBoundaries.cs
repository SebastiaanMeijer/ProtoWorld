/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

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

    public float BuildingLineThickness = 1.2f;

    public float RoadLineThickness = 1.0f;

    //public bool CorrectAspectRatio = false;
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

    [Compact]
    public Vector2 CombinationOptimizationSize = new Vector2(100, 100);

    public bool OverrideDatabaseConnection = false;

    public string serverAddress = "127.0.0.1";
    public string serverPort = "5432";
    public string serverUserId = "postgres";
    public string serverDatabaseName = "GIS";
    public string serverPassword = "test";
    public string commandTimeout = "60";

    //public string OverridenConnectionString;

    //Using local DB Bounds allows the user to set the database boundaries manually,
    //easing from the need of having the WCF active in play mode. -- Miguel R. C.
    public bool UseLocalDatabaseBounds;

    public double dbBoundMinLat;
    public double dbBoundMaxLat;
    public double dbBoundMinLon;
    public double dbBoundMaxLon;

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
        return "Server=" + serverAddress + ";Port=" + serverPort + ";Database=" + serverDatabaseName + ";User Id=" + serverUserId + ";Password=" + serverPassword + ";CommandTimeout=" + commandTimeout + ";";
    }

    /// <summary>
    /// Starts the script.
    /// </summary>    
    void Start()
    {
        log.Info("Map boundaries : MaxLat " + maxLat + ", MinLat " + minLat + ", MaxLon " + maxLon + ", MinLon " + minLon);

        //Initialize the coordinate convertor (need it for running the game!)
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        CoordinateConvertor.Initialize(client, this);
    }
}
