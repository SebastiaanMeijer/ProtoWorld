using UnityEngine;
using System.Collections;
using GapslabWCFservice;

/// <summary>
/// A class that convert the OpenStreetMaps geopositions system to Unity 3d space and vice versa.
/// </summary>
public static class CoordinateConvertor
{
    /// <summary>
    /// The gis bounds in the postgre database that contains openstreetmaps
    /// </summary>
    public static BoundsWCF databaseBounds;
    /// <summary>
    /// Minimum and maximum X value of Unity world space that is calculated from databasebounds
    /// </summary>
    public static float[] minmaxX;
    /// <summary>
    /// Minimum and maximum Y value of Unity world space that is calculated from databasebounds
    /// </summary>
    public static float[] minmaxY;
    /// <summary>
    /// Minimum reference point on map
    /// </summary>
    public static Vector3 MinPointOnMap;
    /// <summary>
    /// Default database connection string
    /// </summary>
    static string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase;
    /// <summary>
    /// Direction fix for coordinates.
    /// </summary>
    private static int direction = -1; // Correct direction.
    /// <summary>
    /// the map boundary object that is under AramGISBoundaries gameobject.
    /// </summary>
    private static MapBoundaries mapboundary;
    /// <summary>
    /// True when the convertor is initialized.
    /// </summary>
    public static bool isInitialized = false;
    
    /// <summary>
    /// Initializes the convertor using the WCF service and the mapboundaries object.
    /// </summary>
    /// <param name="client">The GaPSLabs WCF webservice</param>
    /// <param name="mapboundaries">The AramGISBoundaries component.</param>
    public static void Initialize(ServiceGapslabsClient client, MapBoundaries mapboundaries)
    {
        BoundsWCF SelectedArea = new BoundsWCF();
        mapboundary = mapboundaries;
        SelectedArea.minlat = mapboundaries.minLat;
        SelectedArea.maxlat = mapboundaries.maxLat;
        SelectedArea.minlon = mapboundaries.minLon;
        SelectedArea.maxlon = mapboundaries.maxLon;
        minmaxX = mapboundaries.minMaxX;
        minmaxY = mapboundaries.minMaxY;

        //if (mapboundaries.CorrectAspectRatio)
        //{
        //    var aspectRatio = System.Math.Abs(SelectedArea.maxlat - SelectedArea.minlat) / System.Math.Abs(SelectedArea.maxlon - SelectedArea.minlon);
        //    minmaxY[1] = (float)(minmaxX[1] / aspectRatio);
        //}

        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;

        //If UseLocalDatabaseBounds, WCF won't be used (for portability) -- Miguel R. C.
        if (mapboundaries.UseLocalDatabaseBounds)
        {
            databaseBounds = new BoundsWCF() { minlat = mapboundaries.dbBoundMinLat, 
                                               maxlat = mapboundaries.dbBoundMaxLat, 
                                               minlon = mapboundaries.dbBoundMinLon,
                                               maxlon = mapboundaries.dbBoundMaxLon
            };
        }
        else
        {
            databaseBounds = client.GetBounds(wcfCon);
        }

        float[] MinPointOnArea = SimpleInterpolation((float)SelectedArea.minlat, (float)SelectedArea.minlon, databaseBounds, minmaxX, minmaxY);
        MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);
        isInitialized = true;
    }

    /// <summary>
    /// Convert latitude/longitude to unity 3d coordinates
    /// </summary>
    /// <param name="Lat">The latitude value</param>
    /// <param name="Lon">The longitude value</param>
    /// <returns>Returns the 3d position</returns>
    public static Vector3 LatLonToVector3(double Lat, double Lon)
    {
        //Debug.Log("databaseBounds: maxlon " + databaseBounds.maxlon + ", maxlat " + databaseBounds.maxlat + ", minlon " + databaseBounds.minlon + ", minlat " + databaseBounds.minlat);
        //Debug.Log("minmaxX " + minmaxX[0] + "," + minmaxX[1] + "minmaxY " + minmaxY[0] + "," + minmaxY[1]);
        //Debug.Log("MinPointOnMap " + MinPointOnMap.x + ", " + MinPointOnMap.y + ", " + MinPointOnMap.z);

        var result = SimpleInterpolation((float)Lat, (float)Lon, databaseBounds, minmaxX, minmaxY);
        var ret = new Vector3(direction * (float)result[0], 0, (float)result[1]) - MinPointOnMap;
        Vector3 scale = new Vector3(mapboundary.Scale.x, 1, mapboundary.Scale.y);
        ret.Scale(scale);
        return ret;
    }
    /// <summary>
    /// Convert Thales SE-Star's coordinate into unity coordinate
    /// </summary>
    /// <param name="thales">The position in Thales SE-star system.</param>
    /// <returns>The position in unity 3d system</returns>
    public static Vector3 ThalesCartesianToUnity(this Vector3 thales)
    {
        return new Vector3(-thales.x, thales.z, -thales.y);
    }
    /// <summary>
    /// Converts Thales cartesian to unity cartesians. Note: Not used anymore.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="CartX"></param>
    /// <param name="CartY"></param>
    /// <param name="CartZ"></param>
    /// <param name="CartDirX"></param>
    /// <param name="CartDirY"></param>
    /// <param name="CartDirZ"></param>
    [System.Obsolete("This is not used anymore",true)]
    public static void ThalesCartesianToUnity(ref Transform t, float CartX, float CartY, float CartZ, float CartDirX, float CartDirY, float CartDirZ)
    {
        //Latitude/Longitude : (48,8390602 ; 2,2519118)
        //Cartesian (-175 ; -282,7 ; 0,09992)
        // In Cartesian: X forward, Y left and Z up
        // In Unity: X forward, Y up and Z right
        Vector3 pos = new Vector3(-CartX, CartZ, -CartY);
        Vector3 dir = new Vector3(CartDirX, CartDirY, CartDirZ);
        Vector3 modifier = new Vector3(-90.0f, 90.0f, 0.0f);
        t.position = pos;
        t.LookAt(pos);
        t.Rotate(modifier);
    }
    /// <summary>
    /// Rotation fix for SE-Star coordinate system.
    /// </summary>
    public static Vector3 thalesRotationModifier = new Vector3(-90, 90, 0);
    /// <summary>
    /// Converts Latitude / Longitude / Elevation to unity 3d coordinates.
    /// </summary>
    /// <param name="Lat">The latitude</param>
    /// <param name="Lon">The longitude</param>
    /// <param name="height">The height or elevation value.</param>
    /// <returns>Coordinates in unity 3d space</returns>
    public static Vector3 LatLonToVector3(double Lat, double Lon, float height)
    {
        var result = SimpleInterpolation((float)Lat, (float)Lon, databaseBounds, minmaxX, minmaxY);
        var ret = new Vector3(direction * (float)result[0], height, (float)result[1]) - MinPointOnMap;
        Vector3 scale = new Vector3(mapboundary.Scale.x, 1, mapboundary.Scale.y);
        ret.Scale(scale);
        return ret;
    }

    public static Vector3 LatLonToVector3(double Lat, double Lon, float height, MapBoundaries mb)
    {
        var databaseBounds = new BoundsWCF()
        {
            minlat = mb.dbBoundMinLat,
            maxlat = mb.dbBoundMaxLat,
            minlon = mb.dbBoundMinLon,
            maxlon = mb.dbBoundMaxLon
        };

        float[] MinPointOnArea = SimpleInterpolation((float)mb.minLat, (float)mb.minLon, databaseBounds, mb.minMaxX, mb.minMaxY);
        var MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);

        var result = SimpleInterpolation((float)Lat, (float)Lon, databaseBounds, mb.minMaxX, mb.minMaxY);
        var ret = new Vector3(direction * (float)result[0], height, (float)result[1]) - MinPointOnMap;
        Vector3 scale = new Vector3(mb.Scale.x, 1, mb.Scale.y);
        ret.Scale(scale);
        return ret;
    }

    /// <summary>
    /// Converts a unity 3d space position to latitude/longitude
    /// </summary>
    /// <param name="position">Unity position</param>
    /// <returns>WGS84 [latitude, longitude] </returns>
    public static float[] Vector3ToLatLon(Vector3 position)
    {
        Vector3 reversescale = new Vector3(1f / mapboundary.Scale.x, 1, 1f / mapboundary.Scale.y);
        position.Scale(reversescale);
        position = position + MinPointOnMap;
        position = new Vector3(direction * position.x, 0, position.z);
        var calclat = Interpolations.linear(position.x, minmaxX[0], minmaxX[1], (float)databaseBounds.minlat, (float)databaseBounds.maxlat);
        var calclon = Interpolations.linear(position.z, minmaxY[0], minmaxY[1], (float)databaseBounds.minlon, (float)databaseBounds.maxlon);
        return new float[] { calclat, calclon };
    }

    /// <summary>
    /// Converts a unity 3d space position to latitude/longitude given the MapBoundaries object.
    /// </summary>
    /// <param name="position">Unity position.</param>
    /// <param name="mb">MapBoundaries object.</param>
    /// <returns>WGS84 [latitude, longitude].</returns>
    public static float[] Vector3ToLatLon(Vector3 position, MapBoundaries mb)
    {
        var databaseBounds = new BoundsWCF()
        {
            minlat = mb.dbBoundMinLat,
            maxlat = mb.dbBoundMaxLat,
            minlon = mb.dbBoundMinLon,
            maxlon = mb.dbBoundMaxLon
        };

        float[] MinPointOnArea = SimpleInterpolation((float)mb.minLat, (float)mb.minLon, databaseBounds, mb.minMaxX, mb.minMaxY);
        var MinPointOnMap = new Vector3(direction * MinPointOnArea[0], 0, MinPointOnArea[1]);

        Vector3 reversescale = new Vector3(1f / mb.Scale.x, 1, 1f / mb.Scale.y);
        position.Scale(reversescale);
        position = position + MinPointOnMap;
        position = new Vector3(direction * position.x, 0, position.z);
        var calclat = Interpolations.linear(position.x, mb.minMaxX[0], mb.minMaxX[1], (float)mb.dbBoundMinLat, (float)mb.dbBoundMaxLat);
        var calclon = Interpolations.linear(position.z, mb.minMaxY[0], mb.minMaxY[1], (float)mb.dbBoundMinLon, (float)mb.dbBoundMaxLon);
        return new float[] { calclat, calclon };
    }

    /// <summary>
    /// A linear interpolation
    /// </summary>
    /// <param name="PositionLat">Latitude value</param>
    /// <param name="PositionLon">Longitude value</param>
    /// <param name="Boundings">Source bounds</param>
    /// <param name="MinMaxX">Target minimum maximum X</param>
    /// <param name="MinMaxY">Target minimum maximum Y</param>
    /// <returns>X and Y in unity coordinates</returns>
    public static float[] SimpleInterpolation(float PositionLat, float PositionLon, GapslabWCFservice.BoundsWCF Boundings, float[] MinMaxX, float[] MinMaxY)
    {
        //Debug.Log("This is the info from simple interpolation: " + PositionLat + ", " + PositionLon + ", " + Boundings + ", " + MinMaxX + ", " + MinMaxY);
        // pixelY = ((targetLat - minLat) / (maxLat - minLat)) * (maxYPixel - minYPixel)
        // pixelX = ((targetLon - minLon) / (maxLon - minLon)) * (maxXPixel - minXPi.xel)
        float Y = (float)((PositionLon - Boundings.minlon) / (Boundings.maxlon - Boundings.minlon) * (MinMaxY[1] - MinMaxY[0]));
        float X = (float)((PositionLat - Boundings.minlat) / (Boundings.maxlat - Boundings.minlat) * (MinMaxX[1] - MinMaxX[0]));
        return new float[] { X, Y };
    }

    /// <summary>
    /// This routine calculates the distance between two geo points (given the latitude/longitude of those points).
    /// </summary>
    /// <param name="lat1">Latitude 1</param>
    /// <param name="lon1">Longitude 1</param>
    /// <param name="lat2">Latitude 2</param>
    /// <param name="lon2">Longitude 2</param>
    /// <param name="unit">the unit you desire for results where: 
    /// <para>'M' is statute miles</para>
    /// <para>'K' is kilometers (default)</para>
    /// <para>'N' is nautical miles</para>
    /// </param>
    /// <returns>Returns the value in the targeted unity.</returns>
    public static double GeoDistance(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
    {
        double theta = lon1 - lon2;
        double dist = System.Math.Sin(deg2rad(lat1)) * System.Math.Sin(deg2rad(lat2)) + System.Math.Cos(deg2rad(lat1)) * System.Math.Cos(deg2rad(lat2)) * System.Math.Cos(deg2rad(theta));
        dist = System.Math.Acos(dist);
        dist = rad2deg(dist);
        dist = dist * 60 * 1.1515;
        if (unit == 'K')
        {
            dist = dist * 1.609344;
        }
        else if (unit == 'N')
        {
            dist = dist * 0.8684;
        }
        return (dist);
    }

    /// <summary>
    /// Converts decimal degrees to radians 
    /// </summary>
    /// <param name="deg">The value in degrees</param>
    /// <returns>Returns the value in radians.</returns>
    /// <seealso cref="rad2deg"/>
    private static double deg2rad(double deg)
    {
        return (deg * System.Math.PI / 180.0);
    }

    /// <summary>
    /// Converts radians to decimal degrees
    /// </summary>
    /// <param name="rad">The value in radians</param>
    /// <returns>Returns the value in degrees.</returns>
    /// <seealso cref="deg2rad"/>
    private static double rad2deg(double rad)
    {
        return (rad / System.Math.PI * 180.0);
    }
}