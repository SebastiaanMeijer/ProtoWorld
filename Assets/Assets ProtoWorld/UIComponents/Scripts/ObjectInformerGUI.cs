using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GapslabWCFservice;
using System.Linq;
using System.Threading;

//[ExecuteInEditMode()]
public class ObjectInformerGUI : MonoBehaviour
{

	public Rect WindowSize = new Rect(10, 10, 210, 200);

	public float Margin = 10f;
	public BoundsWCF boundsTemp;
	public string WindowName = "Element Inspector GUI";
	private string wcfCon = ServicePropertiesClass.ConnectionPostgreDatabase;
	private RaycastHit hit;
	private Transform currentSelection;
	public Transform GUIObject;
	private UnityEngine.UI.Text UIText;
	private ServiceGapslabsClient client;
	public Transform SUMOAnimator;
	private SUMOSimulationAnimator sumoAnimComponent;
	public bool UseNewGUI = false;
	private string display = "";
	public GUIStyle Style;
	void Start()
	{
		client = ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
		sumoAnimComponent = SUMOAnimator.GetComponent<SUMOSimulationAnimator>();
		Interpolations.MyLog("Object Informer - Init Gapslabs Service");
		if (GUIObject != null)
			UIText = GUIObject.GetComponent<UnityEngine.UI.Text>();

		var go = GameObject.Find("AramGISBoundaries");
		var connection = go.GetComponent<MapBoundaries>();
        wcfCon = connection.OverrideDatabaseConnection ? connection.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
	}
    void OnDestroy()
    {
        if (client!=null)
        client.Close();
    }

    void OnDisable()
    {
        if (client != null)
            client.Close();
    }

	// Update is called once per frame
	void Update()
	{
		if (UseNewGUI)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				//Interpolations.MyLog(hit.transform.name);
				currentSelection = hit.transform;
				if (Input.GetMouseButtonDown(0))
				{
#if UNITY_ANDROID
					display= GetInfoFromCloud();
#endif
#if UNITY_STANDALONE_WIN
					GetInfo();
					
#endif
					//Interpolations.MyLog(display);
				}
			}
			if (UIText != null)
				UIText.text = display;
		}
	}

	void OnGUI()
	{
		if (!UseNewGUI)
		{
			//GUI.color= Color.green;
			WindowSize = SetWindowMargins(WindowSize, Margin);
			// Without dragging
			//GUI.Window(1,WindowSize,DoMyWindow,"Router");
			// With dragging
			WindowSize = GUI.Window(3, WindowSize, DoMyWindow, WindowName);
		}
	}
	Vector2 ScrollRouter;
	void DoMyWindow(int windowID)
	{
		//if (Application.isPlaying)
		if (windowID == 3)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				//Interpolations.MyLog(hit.transform.name);
				currentSelection = hit.transform;
				if (Input.GetMouseButtonDown(0))
				{
#if UNITY_ANDROID
					display= GetInfoFromCloud();
#endif
#if UNITY_STANDALONE_WIN
					GetInfo();
#endif
					//Interpolations.MyLog(display);
				}

			}
			if (UIText != null)
			{
				UIText.text = display;
			}
			//GUI.Label(new Rect(10,20,WindowSize.width-10,WindowSize.height-20),display,Style);
			GUILayout.Label(display);
			GUI.DragWindow(new Rect(0, 0, 10000, WindowSize.height));
		}
	}
	public string OSMApi()
	{
		var url = "http://api.openstreetmap.org/api/0.6/";
		var example = url + "way" + "/1881614";
		// http://api.openstreetmap.org/api/0.6/way/1881614
		return "TODO: Not implemented yet.";
	}
	public string GetInfoFromCloud()
	{
		var ret = "";
		if (!currentSelection.name.Contains("|"))
		{
			return "";
		}
		var o = currentSelection.name.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
		ret += "Type: " + o[0] + "\n";
		ret += "id: " + o[2] + "\n";
		switch (o[1])
		{
			case "0": // Node
				{
					var selectedNode = client.GetNodeInfo(o[2], wcfCon); // TODO
					var nodeTags = client.GetNodeTags(o[2], wcfCon); // TODO
					ret += "Position: " + "Lat=" + selectedNode.lat + " Lon=" + selectedNode.lon + "\n";
					foreach (var tag in nodeTags)
					{
						ret += "Tag info: " + tag.KeyValue[0] + "=" + tag.KeyValue[1] + "\n";
					}
					break;
				}
			case "1": // Line
			case "2": // Polygon
				{
					Interpolations.MyLog(o[2]);
					var WayTags = client.GetWayTags(o[2], wcfCon); // TODO
					foreach (var tag in WayTags)
					{
						ret += "Tag info: " + tag.KeyValue[0] + "=" + tag.KeyValue[1] + "\n";
					}
					//					foreach (var node in way.Nodes)	
					//					{
					//						ret += "Node: "+"ID="+node.id+" Lat="+ node.Position.Lat+" Lon=" + node.Position.Lon+"\n";
					//					}

					break;
				}
			case "3":
				{
					// TODO
					//var selectedPolygon = source.Nodes.Where(i => i.Attribute("id").Value == o[2]).Single();
					ret += "Relations have not been implemented yet.";
					break;
				}
		}
		return ret;
	}
	private bool GettingInfo = false;
	private Thread infoThread;
	public string GetInfo()
	{
		var ret = "";
		if (currentSelection.tag == "Car")
		{
			var timeStep = sumoAnimComponent.CurrentTimeStep;
			var Vehicle = timeStep.Vehicles.Where(i => i.Id == currentSelection.name).Single();
			ret += "Id: " + Vehicle.Id + "\n";
			ret += "Type: " + Vehicle.VehicleType + "\n";
			ret += "Geoposition: " + Vehicle.Latitude + ", " + Vehicle.Longitude + "\n";
			ret += "Position: " + Vehicle.Pos + "\n";
			ret += "Angle: " + Vehicle.Angle + "\n";
			ret += "Speed: " + Vehicle.Speed + "\n";
			ret += "Slope: " + Vehicle.Slope + "\n";
			ret += "Lane: " + Vehicle.Lane;
		}
		else
		{
			if (!currentSelection.name.Contains("|"))
			{
				return "";
			}
			var o = currentSelection.name.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
			ret += "Type: " + o[0] + "\n";
			ret += "id: " + o[2] + "\n";


			Interpolations.MyLog("Starting getting info Thread");

			infoThread = new Thread(new ThreadStart(() =>
			{
				try
				{
					GettingInfo = true;
					display = "Loading information...";
					switch (o[1])
					{
						case "0": // Node
							{
								var selectedNode = client.GetNodeInfo(o[2], wcfCon);
								var nodeTags = client.GetNodeTags(o[2], wcfCon);
								ret += "Position: " + "Lat=" + selectedNode.lat + " Lon=" + selectedNode.lon + "\n";
								foreach (var tag in nodeTags)
								{
									ret += "Tag info: " + tag.KeyValue[0] + "=" + tag.KeyValue[1] + "\n";
								}
								break;
							}
						case "1": // Line
						case "2": // Polygon
							{
								Interpolations.MyLog(o[2]);
								var WayTags = client.GetWayTags(o[2], wcfCon);
								foreach (var tag in WayTags)
								{
									ret += "Tag info: " + tag.KeyValue[0] + "=" + tag.KeyValue[1] + "\n";
								}
								//					foreach (var node in way.Nodes)	
								//					{
								//						ret += "Node: "+"ID="+node.id+" Lat="+ node.Position.Lat+" Lon=" + node.Position.Lon+"\n";
								//					}

								break;
							}
						case "3":
							{
								// TODO
								//var selectedPolygon = source.Nodes.Where(i => i.Attribute("id").Value == o[2]).Single();
								ret += "Relations have not been implemented yet.";
								break;
							}
					}
				}
				catch (ThreadAbortException tae)
				{
 					// Thread was aborted.
				}
				finally
				{
					display = ret;
					GettingInfo = false;
				}
			}));
			if (GettingInfo)
				infoThread.Abort();
			infoThread.Start();
		}
		return ret;
	}
	public static Rect SetWindowMargins(Rect WindowRectangle, float Margin)
	{
		if (WindowRectangle.x < Margin)
			WindowRectangle = new Rect(Margin, WindowRectangle.y, WindowRectangle.width, WindowRectangle.height);
		if (WindowRectangle.x + WindowRectangle.width > Screen.width - Margin)
			WindowRectangle = new Rect(Screen.width - Margin - WindowRectangle.width, WindowRectangle.y, WindowRectangle.width, WindowRectangle.height);
		if (WindowRectangle.y < Margin)
			WindowRectangle = new Rect(WindowRectangle.x, Margin, WindowRectangle.width, WindowRectangle.height);
		if (WindowRectangle.y + WindowRectangle.height > Screen.height - Margin)
			WindowRectangle = new Rect(WindowRectangle.x, Screen.height - Margin - WindowRectangle.height, WindowRectangle.width, WindowRectangle.height);
		return WindowRectangle;
	}

	public static string ParseGameobjectName(string name)
	{
		if (!name.Contains("|"))
		{
			return null;
		}
		var ret = "";
		var o = name.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
		// Type: o[0]
		ret = o[2];
		//switch (o[1])
		//{
		//	case "0": // Node
		//		{
		//			break;
		//		}
		//	case "1": // Line
		//	case "2": // Polygon
		//		{
		//			break;
		//		}
		//	case "3": // Relation
		//		{
		//			break;
		//		}
		//}
		return ret;
	}

}
