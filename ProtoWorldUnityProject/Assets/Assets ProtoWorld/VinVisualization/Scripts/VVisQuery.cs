/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
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

    [XmlAttribute]
    public string info { get; set; }

    public VVisQueryRequest() { }

    public VVisQueryRequest(string name, string query, string info)
    {
        this.name = name;
        this.query = query;
        this.info = info;
    }

    public override string ToString()
    {
        return "name: " + name + ", query: " + query + ", info: " + info;
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


