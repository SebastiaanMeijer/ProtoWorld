using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SEStar))]
public class SEStarEditor : Editor
{
    /// <summary>
    /// Inspector button of the Unity Editor to calculate coordinate difference.
    /// </summary>
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Calculate coordinate difference"))
            CalculateCoordinateDifference();

        DrawDefaultInspector();
    }

    /// <summary>
    /// Calculates the coordinate difference between SeStar and the Unity scene. 
    /// </summary>
    private void CalculateCoordinateDifference()
    {
        var sestar = target as SEStar;
        var go = GameObject.Find("AramGISBoundaries");
        var mapboundaries = go.GetComponent<MapBoundaries>();
        ServiceGapslabsClient client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        CoordinateConvertor.Initialize(client, mapboundaries);

        sestar.CalculatedUnityReference = CoordinateConvertor.LatLonToVector3(sestar.ReferenceLatitudeLongitude.x, sestar.ReferenceLatitudeLongitude.y);

        if (sestar.OverrideReferenceOffsetByAnObject)
            sestar.CalculatedDifference = sestar.ReferenceOffsetObject.position;
        else
            sestar.CalculatedDifference = sestar.CalculatedUnityReference - sestar.ReferenceSEStarCartesian.ThalesCartesianToUnity();

        EditorUtility.SetDirty(target);
    }
}
