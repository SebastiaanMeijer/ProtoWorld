/*
 * 
 * GAPSLABS EXTENDED EDITOR
 * Aram Azhari
 * 
 * Reviewed by Miguel Ramos Carretero
 * Note: This is not used anymore. Look at OSMReaderSQL.cs instead.
 * 
 */

using UnityEngine;
using UnityEditor;
using Aram.OSMParser;
using System.Linq;
using System.Xml;
using System.Collections;

public class OSMReader : Editor
{

    //[MenuItem("Gapslabs GIS Package/OBSOLETE!")]
    static void Create()
    {
        //ImportOSMData();
    }

    [System.Obsolete("This functionality has been moved to the server side. Use other overloads of ImportOSMData()", true)]
    static void ImportOSMData()
    {
        //    Color buildingColor=new Color(0,0,255,255);
        //    OSMSource source=new OSMSource( AramGISGlobalObject.GetGlobalObject().filename);
        //    if (source!=null){

        //        var ways = (from s in source.OsmDocument.Descendants("osm")
        //                    select s.Descendants("way")).Single();

        //        var allNodes = source.OsmDocument.Descendants("osm").Select(i => i.Elements("node")).First();
        //        // All ways
        //        LineDraw draw = LineDraw.CreateInstance<LineDraw>();

        //        int totalWays=ways.Count();
        //        float progress=0f;
        //        foreach (var FirstWay in ways)
        //        {
        //            if (!EditorUtility.DisplayCancelableProgressBar("Importing data","Generating roads and buildings\t"+progress+"/"+totalWays,progress++/totalWays))
        //            {
        //            // First way
        //            // var FirstWay = ways.First();
        //            // Debug.Log(string.Format( "Information about way {0} created by {1}:",
        //            // FirstWay.Attribute("id").Value, FirstWay.Attribute("user").Value));
        //            var WayNodes = from w in FirstWay.Elements("nd")
        //                           select w.Attribute("ref").Value;
        //            var WayTags = from w in FirstWay.Elements("tag")
        //                          select new
        //                              {
        //                                  Key = w.Attribute("k").Value,
        //                                  Value = w.Attribute("v").Value
        //                              };
        ////	        Debug.Log("Tags:");
        ////	        foreach (var tag in WayTags)
        ////	            Debug.Log(string.Format("{0} = {1}", tag.Key, tag.Value));

        //            // A VERY IMPORTANT NOTE:
        //            // Since the way nodes in OSM files are in the correct order, they have to be read in the same order.
        //            // Otherwise, you will get confusing results.
        //            // Since querying is a cartesian product, the selected nodes in the current Way should appear first 
        //            // in the LINQ query to preserve the correct order of the nodes. (ie. From w in WayNodes , before from s in allNodes)
        //            var nodePositions = from w in WayNodes
        //                                from s in allNodes
        //                                where s.Attribute("id").Value == w
        //                                select new GeoPosition( s.Attribute("lat").Value,s.Attribute("lon").Value);
        //    //							select new
        //    //                            {
        //    //                                id = s.Attribute("id").Value,
        //    //                                lat = s.Attribute("lat").Value,
        //    //                                lon = s.Attribute("lon").Value
        //    //                            };
        ////	        Debug.Log("Nodes:");
        //            if (WayTags.Where(i=> i.Key.ToLower()== "landuse"||i.Key.ToLower()== "building"||i.Key.ToLower()== "highway").Count()!=0)
        //                {
        //                    Vector3[] tempPoints=new Vector3[nodePositions.Count()];
        //                    int counter=0;
        //                    Aram.OSMParser.Bounds b =source.Bounds;
        //                    float[] minmaxX=new float[] {-100f,300f};
        //                    float[] minmaxY=new float[] {-100f,300f};
        //                    foreach (var node in nodePositions)
        //                    {
        //                        // GeoPosition g=new GeoPosition(node.lat,node.lon);
        //                        //var result=StandardConverters.ConvertWGS84toECEF_NoAltitude(g);
        //                        var result=StandardConverters.WGS84toXY_SimpleInterpolation(node,b,minmaxX,minmaxY);
        //                        tempPoints[counter]=new Vector3(result[0],0,result[1]);
        //                        counter++;
        //                    }

        //                    var building = WayTags.Where( i => i.Key.ToLower()== "building");
        //                    if (building.Count()!=0)
        //                        draw.Draw(tempPoints, buildingColor,buildingColor,1.2f,1.2f,LineDraw.OSMType.Line,FirstWay.Attribute("id").Value,"Building","Building");
        //                    else
        //                        draw.Draw(tempPoints, Color.red,Color.yellow,1f,1f,LineDraw.OSMType.Line,FirstWay.Attribute("id").Value,null,"Line");
        //                }
        //            }
        //        }
        //        EditorUtility.ClearProgressBar();

    }
}