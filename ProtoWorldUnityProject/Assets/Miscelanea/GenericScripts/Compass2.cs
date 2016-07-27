/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
public class Compass2 : MonoBehaviour
{
    //For holding the compass textures
    public Texture2D bg;
    public Texture2D bubble;


    //"North" in the game
    //0 for + Z Axis, 90 for + X Axis, etc
    public float north;

    //Where the compass bubble needs to be inside the compass
    public float radius;

    //Where the compass needs to be placed
    public Vector2 center;

    //Size in pixels about how big the compass should be
    public Vector2 compassSize = new Vector2(50,50);
    public Vector2 bubbleSize = new Vector2(50,50);
    public float margin=5f;
	public bool top=true;
	public bool right=true;
    // Use this for initialization
    void Start()
    {
    	if (top && right)
    		center=new Vector2(Screen.width-compassSize.x/2-margin,compassSize.y/2+margin);
        //Set the placement of compass from size and center
        compassRect = new Rect(
            center.x - compassSize.x / 2,
            center.y - compassSize.y / 2,
            compassSize.x,
            compassSize.y);

    }

    private Rect compassRect;
    void OnGUI()
    {
        // Draw background
        if (bg!=null)
        GUI.DrawTexture(compassRect, bg);

        // Draw bubble
        GUI.DrawTexture(new Rect(center.x + x - bubbleSize.x / 2, center.y + y - bubbleSize.y/2, bubbleSize.x, bubbleSize.y), bubble);
    }

    // Update is called once per frame
    float rot, x, y;
    void Update()
    {
    	if (top && right)
    		center=new Vector2(Screen.width-compassSize.x/2-margin,compassSize.y/2+margin);
    	 compassRect = new Rect(
            center.x - compassSize.x / 2,
            center.y - compassSize.y / 2,
            compassSize.x,
            compassSize.y);

        // Note -90 compensation cos north is along 2D Y axis
        rot = (-90 + this.transform.eulerAngles.y - north)* Mathf.Deg2Rad;

        // Bubble position
        x = radius * Mathf.Cos(rot);
        y = radius * Mathf.Sin(rot);
    }
}
