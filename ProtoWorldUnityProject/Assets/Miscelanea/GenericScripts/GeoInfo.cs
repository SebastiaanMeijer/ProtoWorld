/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;
using GapslabWCFservice;

public class GeoInfo : MonoBehaviour
{

    public GameObject locatorArrowA;
    public Vector3 previousPositionA;

    public GameObject locatorArrowB;
    public Vector3 previousPositionB;

    ServiceGapslabsClient client;
    string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase; // .ConnectionDatabase;
    // Use this for initialization
    void Start()
    {
        locatorArrowA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        locatorArrowA.transform.localScale = new Vector3(0.5f, 100, 0.5f);
        locatorArrowA.transform.position = Vector3.zero;
        locatorArrowA.GetComponent<Renderer>().material = Resources.Load("A") as Material;
        locatorArrowA.name = "A";
        Object.DestroyImmediate(locatorArrowA.GetComponent<BoxCollider>());
        previousPositionA = Vector3.zero;

        locatorArrowB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        locatorArrowB.transform.localScale = new Vector3(0.5f, 100, 0.5f);
        locatorArrowB.transform.position = Vector3.zero;
        locatorArrowB.GetComponent<Renderer>().material = Resources.Load("B") as Material;
        Object.DestroyImmediate(locatorArrowB.GetComponent<BoxCollider>());
        locatorArrowB.name = "B";
        previousPositionB = Vector3.zero;

        var go = GameObject.Find("AramGISBoundaries");
        var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;

        client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);

        Interpolations.MyLog("init Router");
        try
        {
            client.InitializeRouter(wcfCon);
        }
        catch (System.TimeoutException timeout)
        {
            Debug.LogException(timeout);
        }
        Interpolations.MyLog("Router initialized successfully.");
    }
    void OnDestroy()
    {
        if (client != null)
            client.Close();
    }
    // Update is called once per frame
    void Update()
    {



    }


    RaycastHit hit;
    void OnGUI()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    locatorArrowB.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                    previousPositionB = locatorArrowB.transform.position;
                }
                else if (Input.GetMouseButton(2))
                {
                    locatorArrowA.transform.position = new Vector3(hit.point.x, 0f, hit.point.z);
                    previousPositionA = locatorArrowA.transform.position;
                }

            }
        //			OsmNodeWCF n1 = new OsmNodeWCF();
        //            n1.id = "none";
        //            n1.order = -1;
        //            n1.lat = 59.374563;
        //            n1.lon = 18.0135727;
        //            OsmNodeWCF n2 = new OsmNodeWCF();
        //            n2.id = "none";
        //            n2.order = -1;
        //            n2.lat = 59.37225;
        //            n2.lon = 18.00733;
        //
        //
        //            var RouterResult = client.RouteUsingDykstra(OsmSharp.Routing.VehicleEnum.Car, n1, n2);
        //			if (RouterResult!=null)
        //				Debug.Log("NOT NULL");
    }


}
