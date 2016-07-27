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
//[ExecuteInEditMode()]
public class HUDNodeInfo : MonoBehaviour {

	public Rect WindowSize=new Rect(10,10,210,200);
	public float Margin=10f;
	private GeoInfo geoinfo;
	private MapBoundaries mapBoundaries;
	private BoundsWCF GameBoundaries;
	public BoundsWCF boundsTemp;
	public float[] GameBoundLat;
	public float[] GameBoundLon;
	public Vector3 MinPointOnMap;
	public float[] MinMaxLat;
	public float[] MinMaxLon;
	private string wcfCon=ServicePropertiesClass.ConnectionPostgreDatabase;
	// TODO
	private ServiceGapslabsClient client;
	void Start () {
		client =ServicePropertiesClass.GetGapslabsService(ServicePropertiesClass.ServiceUri);
		geoinfo = transform.GetComponent<GeoInfo>();
		
		var go= GameObject.Find("AramGISBoundaries");
		mapBoundaries=go.GetComponent<MapBoundaries>();
        wcfCon = mapBoundaries.OverrideDatabaseConnection ? mapBoundaries.GetOverridenConnectionString() : ServicePropertiesClass.ConnectionPostgreDatabase;
		
		boundsTemp = client.GetBounds(wcfCon);
		// Temporary - it appears the data for sweden includes some data that go to the north pole and it ruins all interpolations.
		boundsTemp.maxlon=32;
		
		MinMaxLat=new float[2];
		MinMaxLon=new float[2];
		// Setting local values for target boundaries (Openstreetmap database). Used in interpolation as destination boundary.
		MinMaxLat[0]=(float)boundsTemp.minlat;
		MinMaxLat[1]=(float)boundsTemp.maxlat;
		MinMaxLon[0]=(float)boundsTemp.minlon;
		MinMaxLon[1]=(float)boundsTemp.maxlon;
		
		
		
		// Setting local values for 3d world boundaries. Used in interpolation as source boundary
        GameBoundaries = new BoundsWCF();
		GameBoundaries.minlat=mapBoundaries.minMaxX[0];
		GameBoundaries.maxlat=mapBoundaries.minMaxX[1];
		GameBoundaries.minlon=mapBoundaries.minMaxY[0];
		GameBoundaries.maxlon=mapBoundaries.minMaxY[1];
		GameBoundLat=new float[2];
		GameBoundLat[0]=(float)GameBoundaries.minlat;
		GameBoundLat[1]=(float)GameBoundaries.maxlat;
		GameBoundLon=new float[2];
		GameBoundLon[0]=(float)GameBoundaries.minlon;
		GameBoundLon[1]=(float)GameBoundaries.maxlon;
		
		

		
		float[] MinPointOnArea =
			Interpolations.SimpleInterpolation(
				(float)mapBoundaries.minLat,
				(float)mapBoundaries.minLon,
				boundsTemp,
				GameBoundLat,GameBoundLon);
		MinPointOnMap = new Vector3(MinPointOnArea[0],0,MinPointOnArea[1]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI()
	{
		GUI.color= Color.green;
		WindowSize=SetWindowMargins(WindowSize,Margin);
		// Without dragging
		//GUI.Window(1,WindowSize,DoMyWindow,"Router");
		// With dragging
		WindowSize=GUI.Window(1,WindowSize,DoMyWindow,"Router");
	}
	Vector2 ScrollRouter;
	void DoMyWindow(int windowID) 
	{
		if (Application.isPlaying && windowID==1){
		// var result=Interpolations.SimpleInterpolation((float)node.lat,(float)node.lon,boundsTemp/*SelectedArea*/,minmaxX,minmaxY);
		
		float[] ToWorld=Interpolations.SimpleInterpolation(59.339589f,17.9391974f,boundsTemp,GameBoundLat,GameBoundLon);
		ToWorld[0]-=MinPointOnMap.x;
		ToWorld[1]-=MinPointOnMap.z;
		ToWorld[0]*=mapBoundaries.Scale.x;
		ToWorld[1]*=mapBoundaries.Scale.y;
		//float[] p_prime=GamePointToGeoPoint(new Vector3(ToWorld[0],0,ToWorld[1]));
		float[] tempw=ToWorld;
		tempw[0]/=mapBoundaries.Scale.x;
		tempw[1]/=mapBoundaries.Scale.y;
		tempw[0]+=MinPointOnMap.x;
		tempw[1]+=MinPointOnMap.z;
		float[] p_prime=Interpolations.SimpleInterpolation(tempw[0],tempw[1],GameBoundaries,MinMaxLat,MinMaxLon);
			
			
		// float[] PointA = GamePointToGeoPoint(geoinfo.previousPositionA);						
		float[] PointA =new float[] { geoinfo.previousPositionA.x,geoinfo.previousPositionA.z};
		PointA[0]/=mapBoundaries.Scale.x;
		PointA[1]/=mapBoundaries.Scale.y;
		PointA[0]+=MinPointOnMap.x;
		PointA[1]+=MinPointOnMap.z;
		PointA=Interpolations.SimpleInterpolation(PointA[0],PointA[1],GameBoundaries,MinMaxLat,MinMaxLon);
			
		GUI.Label(new Rect(10,20,WindowSize.width,20),"Point A:"+geoinfo.previousPositionA.x+", "+geoinfo.previousPositionA.z);
		GUI.Label(new Rect(10,40,WindowSize.width,20),"Point A\":"+PointA[0]+", "+PointA[1]);
		GUI.Label(new Rect(10,60,WindowSize.width,20),"Point B:"+geoinfo.previousPositionB.x+", "+geoinfo.previousPositionB.z);
		GUI.Label(
			new Rect(10,80,WindowSize.width,20),
			"Real: 59.3395890f,17.9391974f");
		GUI.Label(
			new Rect(10,100,WindowSize.width,20),
			"WORLD:" + ToWorld[0] + ", " + ToWorld[1]);
		GUI.Label(
			new Rect(10,120,WindowSize.width,20),
			"TEST Reverse:" + p_prime[0] + ", " + p_prime[1]);
			
			
		GUI.DragWindow(new Rect(0,0,10000,WindowSize.height));
		}
	}
	public static Rect SetWindowMargins(Rect WindowRectangle,float Margin)
	{
		if (WindowRectangle.x <Margin)
			WindowRectangle=new Rect(Margin,WindowRectangle.y,WindowRectangle.width,WindowRectangle.height);
		if (WindowRectangle.x +WindowRectangle.width>Screen.width-Margin)
			WindowRectangle=new Rect(Screen.width- Margin - WindowRectangle.width ,WindowRectangle.y,WindowRectangle.width,WindowRectangle.height);
		if (WindowRectangle.y<Margin)
			WindowRectangle=new Rect(WindowRectangle.x,Margin,WindowRectangle.width,WindowRectangle.height);
		if (WindowRectangle.y+WindowRectangle.height >Screen.height-Margin)
			WindowRectangle=new Rect(WindowRectangle.x,Screen.height-Margin-WindowRectangle.height,WindowRectangle.width,WindowRectangle.height);
		return WindowRectangle;
	}
	public float[] GamePointToGeoPoint(Vector3 GamePoint)
	{
		
		Vector3 gp=GamePoint;
		gp.x/=mapBoundaries.Scale.x;
		gp.z/=mapBoundaries.Scale.y;
		gp.x+=MinPointOnMap.x;
		gp.z+=MinPointOnMap.z;
		float[] ret = Interpolations.SimpleInterpolation(gp.x,gp.z,GameBoundaries,MinMaxLat,MinMaxLon);
		return ret;
	}
}
