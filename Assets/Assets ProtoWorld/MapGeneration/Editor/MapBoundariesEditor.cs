using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapBoundaries))]
public class MapBoundariesEditor : Editor
{
    static string wcfCon;

    public override void OnInspectorGUI()
    {
        var t = target as MapBoundaries;

        if (GUILayout.Button("Calculate boundaries"))
        {
            ChooseConnection();
            ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
            Debug.Log(wcfCon);
            var b = client.GetBounds(wcfCon);
            Debug.Log("bound maxlon " + b.maxlon);
            var latDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.maxlat, b.minlon) * 1000;
            var lonDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.minlat, b.maxlon) * 1000;
            //var latDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.maxLat, t.minLon) * 1000;
            //var lonDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.minLat, t.maxLon) * 1000;
            Debug.Log("Latitude in kilometers: " + latDist + " ,in float: " + (float)latDist);
            Debug.Log("Longitude in kilometers: " + lonDist + " ,in float: " + (float)lonDist);
            var maxX = (float)latDist;
            var maxY = (float)lonDist;
            t.minMaxX[1] = maxX;
            t.minMaxY[1] = maxY;

            //Get the DB boundaries, so they can be used locally in play mode (without the need of WCF) -- Miguel R. C.
            var databaseBounds = client.GetBounds(wcfCon);
            t.dbBoundMinLat = databaseBounds.minlat;
            t.dbBoundMaxLat = databaseBounds.maxlat;
            t.dbBoundMinLon = databaseBounds.minlon;
            t.dbBoundMaxLon = databaseBounds.maxlon;

            EditorUtility.SetDirty(target);
        }

        //if (GUILayout.Button("Calculate boundaries"))
        //{
        //    ChooseConnection();
        //    ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        //    Debug.Log(wcfCon);
        //    var b = client.GetBounds(wcfCon);
        //    Debug.Log("bound maxlon " + b.maxlon);
        //    var latDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.maxlat, b.minlon) * 1000;
        //    var lonDist = CoordinateConvertor.GeoDistance(b.minlat, b.minlon, b.minlat, b.maxlon) * 1000;
        //    //var latDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.maxLat, t.minLon) * 1000;
        //    //var lonDist = CoordinateConvertor.GeoDistance(t.minLat, t.minLon, t.minLat, t.maxLon) * 1000;
        //    Debug.Log("Latitude in kilometers: " + latDist + " ,in float: " + (float)latDist);
        //    Debug.Log("Longitude in kilometers: " + lonDist + " ,in float: " + (float)lonDist);
        //    var maxX = (float)latDist;
        //    var maxY = (float)lonDist;
        //    t.minMaxX[1] = maxX;
        //    t.minMaxY[1] = maxY;
        //    EditorUtility.SetDirty(target);
        //}

        ////Get the DB boundaries, so they can be used locally in play mode (without the need of WCF) -- Miguel R. C.
        //if (GUILayout.Button("Get DB boundaries"))
        //{
        //    ChooseConnection();
        //    ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
        //    Debug.Log(wcfCon);
        //    var databaseBounds = client.GetBounds(wcfCon);
        //    t.dbBoundMinLat = databaseBounds.minlat;
        //    t.dbBoundMaxLat = databaseBounds.maxlat;
        //    t.dbBoundMinLon = databaseBounds.minlon;
        //    t.dbBoundMaxLon = databaseBounds.maxlon;
        //    EditorUtility.SetDirty(target);
        //}

        DrawDefaultInspector();
        // base.OnInspectorGUI();
    }

    public static string ChooseConnection()
    {
        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
        return wcfCon;
    }
}
