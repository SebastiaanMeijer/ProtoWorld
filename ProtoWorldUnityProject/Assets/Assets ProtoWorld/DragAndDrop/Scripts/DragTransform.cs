/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿/*
 * 
 * DRAG AND DROP SYSTEM 
 * DragTransform.cs
 * Furkan Sonmez
 * Miguel Ramos Carretero
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

class DragTransform : MonoBehaviour
{
    //This is the child of the object. This will show the arrow for the rotation.
    public Transform rotationCanvas;

    //This is the object control panel on screen space, this Transform will be assigned automatically at the awake function
    public Transform ObjectControlPanel;

    //These are events on which functions can be called.
    public UnityEvent m_Selected;
    public UnityEvent m_Deselected;
    public UnityEvent m_Dropped;
    public UnityEvent m_Moved;
    public UnityEvent m_Rotated;
    public UnityEvent m_Deleted;

    //These are booleans to check what the states of the object are. These bools are necessary for different if statements 
    public bool dropped = false;
    public bool dropping = false;
    public bool dragging = false;
    public bool justDragged = true;
    public bool clicked = false;
    public bool selected = false;
    public bool deleting = false;
    public bool ClickingOnChild = false;

    //public bool justInstantiated = true;
    private float distance;

    //this is a public float. This float should be changed to half the height of this object so that the object doesnt get dropped in the floor, but on the floor 
    public float hoveringOverY;

    //this is a public Vector3. The y of this Vector3 should be changed to half the height of this object so that the object doesnt get dropped in the floor, but on the floor 
    private Vector3 hoveringover = Vector3.zero;

    //This is a Vector3 to store the nulVector
    //private Vector3 nulVector;

    //these floats are needed to determine whether the player is just clicking the object or if he's gonna drag it
    public float StartMouseX = 0;
    public float StartMouseY = 0;

    //this float holds the rotation of the object in the Y
    public float rotationY = 0;

    //this float is to change the smoothness at which the rotation happens
    public float smooth = 10.0F;

    //this is the amount at which the object rotates
    public float rotationSpeed = 60.0F;

    //This float stores the distance between the click position and the object itself
    //float distanceClickAndSelf;
    //public float distanceToDeselect = 10f;

    //These are the start and hightlightcolors of the object for when its selected or not. The highlightcolor can be set from the inspector
    private Color startcolor = Color.magenta;
    public Color highLightColor;

    //This reference controls the concurrence between dragging the scene in multitouch controller and dragging objects.
    private MultitouchController multitouch;

    // Logger
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private LoggerAssembly loggerAssembly;

    void Awake()
    {
        loggerAssembly = FindObjectOfType<LoggerAssembly>();

        multitouch = FindObjectOfType<MultitouchController>();
    }

    void Start()
    {
        //change the layer of the gameObject to DroppedObjects layer
        gameObject.layer = LayerMask.NameToLayer("DroppedObjects");

        //change the layer of all the childs of this object also
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<Transform>().gameObject.layer = LayerMask.NameToLayer("DroppedObjects");
        }

        if (this.gameObject.GetComponent<Renderer>() == null)
        {
            Debug.LogError("NO RENDERER");
            gameObject.AddComponent<MeshRenderer>();
        }

        //the rotationCanvas gets assigned at the awake function. The child of the transform will be assigned
        //Ofcourse the rotationcanvas must be the child of the object
        rotationCanvas = GetComponentInChildren<Transform>().FindChild("RotateToCamera");

        //The startcolor of the object is being assigned to the private Color startcolor
        //if (this.GetComponent<Renderer>().material.color != null)
        startcolor = GetComponent<Renderer>().material.color;

        // this code only applies to newly added objects. So objects that were added before the simulation will not be affected
        if (rayHitPositionClass.gameStartedBool == true)
        {
            //this will put the newly instantiated objects into the parent: InstantiatedObjects, to change the parent ,you have to change the string in the Find
            transform.parent = GameObject.Find("InstantiatedObjects").transform;

            //the state dragging will automatically be true when a new object gets dragged into the game
            dragging = true;
            GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color + highLightColor;
        }
        else
        {
            //The rotationCanvas is inactive when spawned
            //rotationCanvas.gameObject.SetActive (false);
        }

        //Assign ObjectControlPanel to the public Transform objectControlPanel(If there is one)
        GameObject ocp = GameObject.Find("ObjectControlPanelUI");
        if (ocp != null)
            ObjectControlPanel = ocp.transform;

        if (m_Dropped == null)
            m_Dropped = new UnityEvent();

        hoveringover.y = hoveringOverY;

        //this.gameObject.SetActive(false);
        //this.gameObject.SetActive (true);
    }

    //these functions are used to check if the user is clicking on the child or not
    public void goDeselect()
    {
        ClickingOnChild = false;
    }
    public void dontDeselect()
    {
        ClickingOnChild = true;
    }

    public void rotatedFunction()
    {
        m_Rotated.Invoke();
    }

    //this code is only for after you have added an object, be it before the simulation or during the simulation
    void OnMouseDown()
    {
        //this bool will check that the object is indeed clicked
        clicked = true;

        dropping = true;
        //Store the first x and y position of the mouse, this is to check if the user will drag the object or just click on it
        StartMouseX = Input.mousePosition.x;
        StartMouseY = Input.mousePosition.y;

        //The object is now selected 
        //selected = true;
        //Change the color of the object to the highlighted color
        //if (GetComponent<Renderer>().material.color != null)
        GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color + highLightColor;

        //The selected events invoker
        m_Selected.Invoke();

        //This will invoke the function Selected from the ObjectData class in this gameObject
        ObjectData od = this.GetComponent<ObjectData>();
        if (od != null)
            od.Selected();
    }

    //this code is only for after you have added an object, be it before the simulation or during the simulation
    void OnMouseUp()
    {
        clicked = false;

        //if the mouseposition is inside of the Drag and drop panel when releasing the mousebutton while dragging the object, the object gets deleted
        //When the Drag and drop panel is collapsed, only the small part of the panel will be the area where the user can drop objects onto to delete it
        if (GameObject.Find("DragAndDropList") == null)
        {
            if (Input.mousePosition.x > (SmallDeleteObjectClass.SxPosition - (SmallDeleteObjectClass.Swidth / 2)) &&
               Input.mousePosition.x < (SmallDeleteObjectClass.SxPosition + (SmallDeleteObjectClass.Swidth / 2)) &&
               Input.mousePosition.y < (SmallDeleteObjectClass.SyPosition + (SmallDeleteObjectClass.Sheight / 2)) && Input.mousePosition.y > (SmallDeleteObjectClass.SyPosition - (SmallDeleteObjectClass.Sheight / 2)))
            {
                //If the object is dropped in the title panel it gets destroyed
                deleting = true;
                LogDestroy();
                Destroy(gameObject);
            }
        }
        else
        {
            //this part is for the full panel area
            if (Input.mousePosition.x > (DeleteObjectClass.xPosition - (DeleteObjectClass.width / 2)) &&
               Input.mousePosition.x < (DeleteObjectClass.xPosition + (DeleteObjectClass.width / 2)) &&
               Input.mousePosition.y < (DeleteObjectClass.yPosition + (DeleteObjectClass.height / 2)) && Input.mousePosition.y > (DeleteObjectClass.yPosition - (DeleteObjectClass.height / 2)))
            {
                //If the object is dropped in the panel it gets destroyed
                deleting = true;
                LogDestroy();
                Destroy(gameObject);
            }
        }

        //if the mouse did not move while clicking, it wasnt a drag so the rotationCanvas will become either active of deactive
        if (dragging == false)
        {

            //if the object is already active, deactivate and vice versa
            if (rotationCanvas != null && rotationCanvas.gameObject.activeSelf == true)
            {

                //deactivate the RotateToCamera child
                rotationCanvas.gameObject.SetActive(false);
                //The object is no longer selected
                selected = false;
                //change the color of the object back to the original color
                //if (GetComponent<Renderer>().material.color != null)
                GetComponent<Renderer>().material.color = startcolor;

                //The Deselected events invoker
                m_Deselected.Invoke();

                this.GetComponent<ObjectData>().Deselected();
            }
            else
            {

                selected = true;


                //Activate the RotateToCamera child
                if (rotationCanvas != null)
                    rotationCanvas.gameObject.SetActive(true);



                //The object is now selected 
                //selected = true;
                //Change the color of the object to the highlighted color
                //if (GetComponent<Renderer>().material.color != null)
                GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color + highLightColor;

                //The selected events invoker
                m_Selected.Invoke();



                //This will invoke the function Selected from the ObjectData class in this gameObject
                ObjectData od = this.GetComponent<ObjectData>();
                if (od != null)
                    od.Selected();
            }
        }

        //when you have moved the object and you drop it, the dropped event gets called again
        if (dragging == true)
        {
            //The Dropped events invoker
            m_Dropped.Invoke();
            dropping = false;
        }

        //If the user leaves the mousebutton the object is no longer dragging(if it was)
        dragging = false;

        //justInstantiated = false;
        //this object is no longer being clicked
        clicked = false;
    }

    void Update()
    {
        // If the object was selected and the user press the key Delete, remove the object from the scene
        if (selected && Input.GetKey(KeyCode.Delete))
            m_Deleted.Invoke();

        //if you have clicked the object and you are not(yet) dragging, it will check further
        if (clicked == true && dragging == false)
        {
            // check if the mouse has left either the first mouseposition x or y
            if ((StartMouseX != Input.mousePosition.x || StartMouseY != Input.mousePosition.y) && clicked == true)
            {

                //dropped = false;
                //the mouseposition has changed from the startposition so the user is now dragging
                dragging = true;

                //The Moved events invoker
                m_Moved.Invoke();
            }
        }

        //if the user is clicking
        if (rayHitPositionClass.dragging)
        {
            // check if the user is dragging this object and check if this object is selected and check if its not just clicked either
            if (dragging == false && selected && clicked != true)
            {
                //now the click was not on the object itself, check if there is a gameObject called ObjectControlPanel
                if (ObjectControlPanel != null)
                {
                    //put the values of the RectTransform into the RectTransform rtOCP(RectTransform Object Control Panel)
                    RectTransform rtOCP = ObjectControlPanel.gameObject.GetComponent<RectTransform>();
                    RectTransform rtOCBP = ObjectControlPanel.Find("ButtonPanel").GetComponent<RectTransform>();
                    RectTransform rtOCS = ObjectControlPanel.Find("SliderPanel").GetComponent<RectTransform>();

                    //if the mouse is clicking(or dragging) inside of this ObjectControlPanel do not deselect
                    if (Input.mousePosition.x > (rtOCP.position.x - (rtOCP.rect.width / 2)) &&
                       Input.mousePosition.x < (rtOCP.position.x + (rtOCP.rect.width / 2)) &&
                       Input.mousePosition.y < (rtOCP.position.y + (rtOCP.rect.height / 2)) &&
                       Input.mousePosition.y > (rtOCP.position.y - (rtOCP.rect.height / 2) - (rtOCBP.rect.height) - rtOCS.rect.height))
                    {

                        //then dont deselect
                        //Debug.Log ("NOT Deselectiing");
                    }
                    else
                    {
                        //if the click or drag is not inside of this panel then check if the click or drag was on the child of the object(RotateToCamera)
                        if (ClickingOnChild == false && selected == true)
                        {
                            //Debug.Log ("Deselectiing");
                            //if that is not true either, deselect the object, set selected to false
                            selected = false;

                            //The Deselected events invoker
                            m_Deselected.Invoke();

                            this.GetComponent<ObjectData>().Deselected();
                        }
                    }
                }
            }
        }

        //if selected is no longer true
        if (selected == false)
        {

            //change the rotationCanvas to inactive
            if (rotationCanvas)
                rotationCanvas.gameObject.SetActive(false);

            //change the highlightedcolor to the startcolor
            //if (GetComponent<Renderer>().material.color != null)
            GetComponent<Renderer>().material.color = startcolor;

        }


        // since OnMouseUp cannot be called when you add a new object, because the OnMouseDown has never happened for this object(since its a new object) this is used
        if (rayHitPositionClass.dragging != true && justDragged == true)
        {
            dragging = false;
            clicked = false;

            if (dropped == false)
            {
                //The Dropped events invoker
                m_Dropped.Invoke();
                dropped = true;
            }

            //if the mouseposition is inside of the Drag and drop panel when releasing the mousebutton while dragging the object, the object gets deleted
            //When the Drag and drop panel is collapsed, only the small part of the panel will be the area where the user can drop objects onto to delete it
            if (GameObject.Find("DragAndDropList") == null)
            {
                if (Input.mousePosition.x > (SmallDeleteObjectClass.SxPosition - (SmallDeleteObjectClass.Swidth / 2)) &&
                   Input.mousePosition.x < (SmallDeleteObjectClass.SxPosition + (SmallDeleteObjectClass.Swidth / 2)) &&
                   Input.mousePosition.y < (SmallDeleteObjectClass.SyPosition + (SmallDeleteObjectClass.Sheight / 2)) &&
                   Input.mousePosition.y > (SmallDeleteObjectClass.SyPosition - (SmallDeleteObjectClass.Sheight / 2)))
                {
                    //If the object is dropped in the title panel it gets destroyed
                    Destroy(gameObject);
                }
            }
            else
            {
                //this part is for the full panel area
                if (Input.mousePosition.x > (DeleteObjectClass.xPosition - (DeleteObjectClass.width / 2)) &&
                   Input.mousePosition.x < (DeleteObjectClass.xPosition + (DeleteObjectClass.width / 2)) &&
                   Input.mousePosition.y < (DeleteObjectClass.yPosition + (DeleteObjectClass.height / 2)) &&
                   Input.mousePosition.y > (DeleteObjectClass.yPosition - (DeleteObjectClass.height / 2)))
                {
                    //If the object is dropped in the panel it gets destroyed
                    Destroy(gameObject);
                }
            }
            //now since the object has been dropped once, it is no longer justDragged(for the first time)
            justDragged = false;
        }

        ////The Moved events invoker
        //m_Moved.Invoke ();
        //if the dragging bool is true for the object than change the position of the transform to the hitLocation of the mouseRay + a certain amount above the y position
        if (dragging == true)
        {
            //the hoveringover Vector3 can be changed from the object Inspector for different objects. This is important so the object is not spawned under the ground.
            //the hoveringover must be half of the height of the object
            transform.position = rayHitPositionClass.hitLocation + hoveringover;
            BlockMultiTouch(true);
        }
    }

    /// <summary>
    /// Auxiliar method to make the drag and drop concurrent with the multitouch controller.
    /// </summary>
    /// <param name="value">True if multitouch should be blocked.</param>
    public void BlockMultiTouch(bool value)
    {
        if (multitouch != null)
            multitouch.BlockNavigation(value);
    }

    /// <summary>
    /// Log the position of the dropped object in Lat-Lon coordinates.
    /// </summary>
    public void LogPositionInLatLon()
    {
        if (!deleting)
        {
            float[] latLon = CoordinateConvertor.Vector3ToLatLon(this.transform.position);

            Debug.Log(this.name + " placed at " + latLon[0] + ", " + latLon[1]);

            if (loggerAssembly != null && loggerAssembly.logDragAndDrop)
                log.Info(string.Format("{0} placed at {1}, {2}", this.name, latLon[0].ToString(), latLon[1].ToString()));
        }
    }

    public void LogDestroy()
    {
        Debug.Log(this.name + " deleted");

        if (loggerAssembly != null && loggerAssembly.logDragAndDrop)
            log.Info(string.Format("{0} deleted", this.name));
    }
}