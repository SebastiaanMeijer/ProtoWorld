/*
 * 
 * VIN VISUALIZATION
 * VVisQuery.cs
 * Miguel Ramos Carretero
 * 
 */

using UnityEngine;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class that defines a list of VVIS queries (for serialization).
/// </summary>
[XmlRoot("vvis_queries")]
public class VVisQueryList
{
    [XmlArrayItem("query")]
    public List<VVisQueryRequest> queries = new List<VVisQueryRequest>();

    public VVisQueryList() { }
}

/// <summary>
/// Class that defines a VVIS query request.
/// </summary>
public class VVisQueryRequest
{
    [XmlAttribute]
    public string name { get; set; }

    [XmlAttribute]
    public string query { get; set; }

    public VVisQueryRequest() { }

    public VVisQueryRequest(string name, string query)
    {
        this.name = name;
        this.query = query;
    }

    public override string ToString()
    {
        return "name: " + name + ", query: " + query;
    }
}

/// <summary>
/// Class that defines the results of a VVIS query and how to visualize it in the scene.
/// </summary>
public class VVISQueryResults
{
    public List<string> queryResults { get; set; }
    public bool visualizeWithGradient { get; set; }
    public Color color { get; set; }
    public bool queryError { get; set; }

    public VVISQueryResults(List<string> queryResults)
    {
        this.queryResults = queryResults;
        visualizeWithGradient = false;
        color = Color.red;
        this.queryError = false;
    }

    public VVISQueryResults(List<string> queryResults, bool visualizeWithGradient)
    {
        this.queryResults = queryResults;
        this.visualizeWithGradient = visualizeWithGradient;
        color = Color.red;
        this.queryError = false;

    }

    public VVISQueryResults(List<string> queryResults, bool visualizeWithGradient, Color color)
    {
        this.queryResults = queryResults;
        this.visualizeWithGradient = visualizeWithGradient;
        this.color = color;
        this.queryError = false;
    }

    public void SetAsQueryError(bool isError)
    {
        this.queryError = isError;
    }
}


