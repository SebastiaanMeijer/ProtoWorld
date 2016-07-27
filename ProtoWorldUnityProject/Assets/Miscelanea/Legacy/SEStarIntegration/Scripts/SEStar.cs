/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * 
 * SESTAR INTEGRATION
 * SEStar.cs
 * Miguel Ramos Carretero
 * Aram Azhari
 * 
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Connector.Messages.SEStar;

/// <summary>
/// Script to control the communication Unity-SEStar.
/// </summary>
public class SEStar : MonoBehaviour
{
    /// -----------------------------------------------------------------------
    ///  VARIABLE DEFINITIONS
    /// -----------------------------------------------------------------------
    /// 
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    // General options for the simulation: 
    public bool useConfFile = false;
    public bool integrateSEStar = true;
    public bool awarenessOfSumoCars = true;
    public bool useLocalAddress = true;
    public float SEStarCameraFarPlane = 200;

    //TCP for SmartObjects:
    public string sestarAddress_ = "127.0.0.1";
    public int sestarPort_ = 6112;
    private bool connected_ = false;
    private Thread tcpThread_ = null;
    private System.Object tcpLock_ = new System.Object();
    private TcpClient sestarClient_ = null;
    private NetworkStream sestarStream_ = null;
    private List<MessageSEStar> msgsIn_ = new List<MessageSEStar>();
    private List<MessageSEStar> msgsOut_ = new List<MessageSEStar>();
    private uint idReceiver_ = 0;
    private uint idSender_ = 256;

    //Data structure to store SmartObjects:
    public Dictionary<uint, GameObject> smartObjects_ = new Dictionary<uint, GameObject>();

    //UDP for SyntheticEntities:
    public int basePortSyntheticEntities_ = 10120;
    private bool isUDPCameraActive = false;
    private Thread udpThreadSyntheticEntities_;
    private UdpClient udpClientSyntheticEntities_ = null;
    private System.Object udpLockSyntheticEntities_ = new System.Object();
    private int[] numSyntheticEntities_ = new int[1];
    private Message3DVIA[] syntheticEntities_ = new Message3DVIA[10000];
    private bool newSyntheticEntitiesData_ = false;

    // Data structure to store SyntheticEntities:
    private Dictionary<uint, GameObject> actorsSyntheticEntities_ = new Dictionary<uint, GameObject>();

    // Parameters to enhance SyntheticEntities:
    private ObjectPool<GameObject> ActorPool = new ObjectPool<GameObject>();
    private Dictionary<uint, uint> actorsGender_ = new Dictionary<uint, uint>();
    private List<string> actorsTemplate_ = new List<string>();

    //UDP for Vehicles (This feature is not being used yet)
    [HideInInspector]
    public int basePortVehicles_ = 10122;
    private Thread udpThreadVehicles_;
    private UdpClient udpClientVehicles_ = null;
    private System.Object udpLockVehicles_ = new System.Object();
    private int[] numVehicles_ = new int[1];
    private Message3DVIA[] vehicles_ = new Message3DVIA[10000];
    private bool newVehicleData_ = false;

    // Data structure to store Vehicles:
    private Dictionary<uint, GameObject> actorsVehicles_ = new Dictionary<uint, GameObject>();

    // Pedestrian types and which types must be visualized in Unity:
    public string[] pedestrianTypes;
    public string[] pedestrianVisualizationFilter;
    private Dictionary<string, int> numberOfPedestriansCreatedOfType = new Dictionary<string, int>();
    private Dictionary<string, int> numberOfPedestriansDeletedOfType = new Dictionary<string, int>();

    // Parameters used to match Unity with SEStar:
    [HideInInspector]
    public string UnityCameraName = "UnityCamera";
    [HideInInspector]
    public Vector2 ReferenceLatitudeLongitude = new Vector2(48.8390602f, 2.2519118f);
    [HideInInspector]
    public Vector3 ReferenceSEStarCartesian = new Vector3(-175f, -282.7f, 0.09992f);
    [HideInInspector]
    public Vector3 CalculatedUnityReference;
    [HideInInspector]
    public Vector3 CalculatedDifference;
    [HideInInspector]
    public bool OverrideReferenceOffsetByAnObject = true;
    [HideInInspector]
    public int UnityCameraId = 0;
    public Transform ReferenceOffsetObject;
    public Transform originPoint;

    // Parameters for speed factor:
    [Range(1.0F, 32.0F)]
    private float speedFactor_ = 1.0F;
    private float prevSpeedFactor_ = 1.0F;

    // KPI information:
    private int numberOfSyntheticEntitiesSpawned = 0;
    private int numberOfSyntheticEntitiesDeleted = 0;

    // Parameters to refresh the UDP camera:
    public float UDPCameraUpdateFrequency = 0.1f;
    public float UnitySeStarFovOffset = 20;
    private float UDPCameraRefreshTime = 0;
    private Vector3 oldUDPCameraPosition;
    private Vector3 oldUDPCameraRotation;
    private float oldUDPCameraFov;

    /// -----------------------------------------------------------------------
    ///  STRUCTURE DEFINITIONS
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Enumerate that defines the state of the simulation.
    /// </summary>
    public enum SimulationState
    {
        Paused,
        Playing,
        Stopped
    };

    /// <summary>
    /// Struct that defines a parsed message received from SEStar.
    /// </summary>
    private struct Message3DVIA
    {
        public uint id_;
        public float posX;
        public float posY;
        public float posZ;
        public float oriX;
        public float oriY;
        public float oriZ;
        public int lifeStatus_;
        public float instantVelocity_;
    };

    /// -----------------------------------------------------------------------
    ///  SCRIPT ACTIONS 
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Actions to perform when awaking (before starting the visualization).
    /// </summary>
    void Awake()
    {
        //IPAddress trial = HandleStartScene.sestarAddress;

        string path = Application.dataPath + "/confSEStar.cfg";

        // Use the confSEStar.cgf in order to make the project portable -- Miguel R. C.
        if (File.Exists(path) == true && useConfFile)
        {
            Debug.Log("Reading confSEStar.cfg...");
            FileStream fs = File.OpenRead(path);
            StreamReader sr = new StreamReader(fs);
            string[] split;
            string line;

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                split = line.Split(new Char[] { ' ' });

                if (split[0].Equals("integrateSEStar"))
                {
                    if (split[1].Equals("false"))
                        integrateSEStar = false;
                    else
                        integrateSEStar = true;
                }
                else if (split[0].Equals("tcpAddress"))
                {
                    if (!split[1].Equals("localhost"))
                    {
                        useLocalAddress = false;
                        sestarAddress_ = split[1];
                    }
                    else
                        useLocalAddress = true;
                }
                else if (split[0].Equals("tcpPort"))
                {
                    if (!split[1].Equals(null))
                        sestarPort_ = Convert.ToInt32(split[1]);
                    else
                        sestarPort_ = 6112; // Default port
                }
                else if (split[0].Equals("udpPort"))
                {
                    if (!split[1].Equals(null))
                        basePortSyntheticEntities_ = Convert.ToInt32(split[1]);
                    else
                        basePortSyntheticEntities_ = 10120; // Default port
                }
                else if (split[0].Equals("integrateSUMO"))
                {
                    if (split[1].Equals("false"))
                        awarenessOfSumoCars = false;
                    else
                        awarenessOfSumoCars = true;
                }
            }

            sr.Close();
        }
    }

    /// <summary>
    /// Actions to perform when starting (at the beginning of the visualization).
    /// </summary>
    void Start()
    {
        if (integrateSEStar)
        {
            log.Info("SEStar script has started");

            if (useLocalAddress)
                sestarAddress_ = "127.0.0.1";

            // Application will keep running even if it is in background
            Application.runInBackground = true;

            // Load the resources into tmpPersos that have a script SEStarPedestrian
            UnityEngine.Object[] tmpPersos = Resources.LoadAll("SEStarSyntheticEntities", typeof(SEStarSyntheticEntity));

            foreach (UnityEngine.Object tmpPerso in tmpPersos)
            {
                if (tmpPerso.name.EndsWith("_f") || tmpPerso.name.EndsWith("_m") || tmpPerso.name.EndsWith("_g"))
                {
                    // Fills the actors template
                    Debug.Log("Adding tmpPersos " + tmpPerso.name);
                    actorsTemplate_.Add(tmpPerso.name);
                }
            }

            connected_ = true;

            // Creates the TCP thread to connect with the SEStar client
            tcpThread_ = new Thread(() => tcpThread());
            tcpThread_.IsBackground = true;
            tcpThread_.Start();
            Thread.Sleep(10);

            // Creates the UDP thread to receive the SyntheticEntities
            udpThreadSyntheticEntities_ = new Thread(() => udpThreadSyntheticEntities());
            udpThreadSyntheticEntities_.IsBackground = true;
            udpThreadSyntheticEntities_.Start();
            Thread.Sleep(10);

            //Creates the UDP thread to receive the Vehicles
            udpThreadVehicles_ = new Thread(() => udpThreadVehicles());
            udpThreadVehicles_.IsBackground = true;
            udpThreadVehicles_.Start();
            Thread.Sleep(10);

            // Initialize controlling the camera for getting pedestrians
            UDPCameraRefreshTime = Time.time;
            oldUDPCameraPosition = Camera.main.transform.position;
            oldUDPCameraRotation = Camera.main.transform.rotation.eulerAngles;
            oldUDPCameraFov = Camera.main.fieldOfView;
            CreateNewSEStarCamera();
            //this.transform.GetComponent<SeStarCameraController>().Activate();

            // Initialize the counters for the different types of pedestrians
            foreach (string type in pedestrianTypes)
            {
                numberOfPedestriansCreatedOfType[type] = 0;
                numberOfPedestriansDeletedOfType[type] = 0;
                //Debug.Log("Counters created for type " + type + ": " + numberOfPedestriansOfEachTypeCreated[type] + ", " + numberOfPedestriansOfEachTypeDeleted[type]);
            }
        }
    }

    /// <summary>
    /// Actions to perform when updating (called once per frame). 
    /// </summary>
    void Update()
    {
        if (connected_ == true)
        {
            // Lock to manage the SyntheticEntities received from the UDP thread
            lock (udpLockSyntheticEntities_)
            {
                // Get the new SyntheticEntities 
                if (newSyntheticEntitiesData_ == true)
                {
                    newSyntheticEntitiesData_ = false;
                    bool find = false;

                    // For each SyntheticEntity actor that is already in the data structure,  check if it is in the last data received
                    foreach (KeyValuePair<uint, GameObject> actor in actorsSyntheticEntities_)
                    {
                        for (uint i = 0; i < numSyntheticEntities_[0]; i++)
                        {
                            if (syntheticEntities_[i].id_ == actor.Key)
                            {
                                // Found: It is in the data
                                find = true;
                                break;
                            }
                        }
                        if (find == false)
                        {
                            // Not found: It is not in the data, so set to not active.
                            actor.Value.SetActive(false);
                        }
                        find = false;
                    }

                    // For each SyntheticEntity in the last data received...
                    for (uint i = 0; i < numSyntheticEntities_[0]; i++)
                    {
                        // If the SyntheticEntity is not in the data structure of SyntheticEntity actors, add it
                        if (!actorsSyntheticEntities_.ContainsKey(syntheticEntities_[i].id_))
                        {
                            CreateUnitySyntheticEntity(syntheticEntities_[i].id_);
                            actorsSyntheticEntities_[syntheticEntities_[i].id_].name = "SyntheticEntities" + syntheticEntities_[i].id_.ToString();
                        }

                        // Position the SyntheticEntity actor in its position
                        actorsSyntheticEntities_[syntheticEntities_[i].id_].transform.position = new Vector3(-syntheticEntities_[i].posX, syntheticEntities_[i].posZ - 0.10f, -syntheticEntities_[i].posY);

                        // If SyntheticEntity has lifeStatus 0, transform its position
                        if (syntheticEntities_[i].lifeStatus_ == 0)
                        {
                            Vector3 dir = new Vector3(syntheticEntities_[i].oriX, syntheticEntities_[i].oriY, syntheticEntities_[i].oriZ);
                            Vector3 pos1 = new Vector3(syntheticEntities_[i].posX, syntheticEntities_[i].posY, syntheticEntities_[i].posZ) + (dir * 100.0f);
                            Vector3 pos2 = new Vector3(-pos1.x, pos1.z, -pos1.y);
                            actorsSyntheticEntities_[syntheticEntities_[i].id_].transform.LookAt(pos2);
                        }

                        // Tweaks to put the actor in correct position -- Added by Aram Azhari
                        actorsSyntheticEntities_[syntheticEntities_[i].id_].transform.RotateAround(Vector3.zero, Vector3.up, 90f);
                        actorsSyntheticEntities_[syntheticEntities_[i].id_].transform.position = actorsSyntheticEntities_[syntheticEntities_[i].id_].transform.position + CalculatedDifference;

                        //UpdateAnimationInUnitySyntheticEntity(syntheticEntities_[i].id_, speed, alive, rand);

                        // Activate the SyntheticEntity actor according to the visualization filter (if any)
                        if (pedestrianVisualizationFilter.Length > 0)
                        {
                            string actorType = actorsSyntheticEntities_[syntheticEntities_[i].id_].GetComponent<SEStarSyntheticEntity>().pedestrianType;

                            foreach (string type in pedestrianVisualizationFilter)
                            {
                                if (actorType == type)
                                {
                                    actorsSyntheticEntities_[syntheticEntities_[i].id_].SetActive(true);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            actorsSyntheticEntities_[syntheticEntities_[i].id_].SetActive(true);
                        }

                    }
                }
            }

            // Lock to manage the Vehicles received from the UDP thread
            lock (udpLockVehicles_)
            {
                // Get the new Vehicles 
                if (newVehicleData_ == true)
                {
                    newVehicleData_ = false;
                    bool find = false;

                    // For each Vehicle actor that is already in the data structure,  check if it is in the last data received
                    foreach (KeyValuePair<uint, GameObject> actor in actorsVehicles_)
                    {
                        for (uint i = 0; i < numVehicles_[0]; i++)
                        {
                            if (vehicles_[i].id_ == actor.Key)
                            {
                                // Found: It is in the data
                                find = true;
                                break;
                            }
                        }
                        if (find == false)
                        {
                            // Not found: It is not in the data, so set to not active.
                            actor.Value.SetActive(false);
                        }
                        find = false;
                    }

                    // For each Vehicle in the last data received...
                    for (uint i = 0; i < numVehicles_[0]; i++)
                    {
                        // If the Vehicle is not in the data structure of Vehicle actors, add it
                        if (!actorsVehicles_.ContainsKey(vehicles_[i].id_))
                        {
                            actorsVehicles_[vehicles_[i].id_] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            actorsVehicles_[vehicles_[i].id_].name = "Vehicle" + vehicles_[i].id_.ToString();
                            actorsVehicles_[vehicles_[i].id_].transform.localScale = new Vector3(2.0f, 1.5F, 4.0F);
                        }

                        // Position the Vehicle actor in its correct position
                        actorsVehicles_[vehicles_[i].id_].transform.position = new Vector3(-vehicles_[i].posX, vehicles_[i].posZ - 0.10f, -vehicles_[i].posY);

                        // If Vehicle has lifeStatus 0, transform its position
                        if (vehicles_[i].lifeStatus_ == 0)
                        {
                            Vector3 dir = new Vector3(vehicles_[i].oriX, vehicles_[i].oriY, vehicles_[i].oriZ);
                            Vector3 pos1 = new Vector3(vehicles_[i].posX, vehicles_[i].posY, vehicles_[i].posZ) + (dir * 100.0f);
                            Vector3 pos2 = new Vector3(-pos1.x, pos1.z, -pos1.y);
                            actorsVehicles_[vehicles_[i].id_].transform.LookAt(pos2);
                        }

                        // Activate the Vehicle actor
                        actorsVehicles_[vehicles_[i].id_].SetActive(true);
                    }
                }
            }

            // Lock to receive the data from the TCP thread
            lock (tcpLock_)
            {
                // Check the speed factor parameter and inform SEStar if it has changed
                if (!Mathf.Approximately(speedFactor_, prevSpeedFactor_))
                {
                    prevSpeedFactor_ = speedFactor_;
                    MessageParameter[] ps = new MessageParameter[24];
                    ps[0].float_ = 1.0F;
                    ps[1].float_ = speedFactor_;
                    MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_SET_TIMER_PROPPERTIES, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);
                    msgsOut_.Add(msgOut);
                }

                // Process each message received from SEStar
                foreach (MessageSEStar msg in msgsIn_)
                {
                    switch (msg.type)
                    {
                        case MessageTypes.TMSG_REGISTER_APPLICATION:
                            {
                                Debug.Log("Received: MessageTypes.TMSG_REGISTER_APPLICATION");
                                idReceiver_ = msg.idReceiver;
                                MessageParameter[] ps = new MessageParameter[24];
                                ps[0].int_ = (int)ExternalApplicationTypes.TEA_BUSINESSSTUDIO;
                                MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_REGISTER_APPLICATION, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);
                                msgsOut_.Add(msgOut);
                                break;
                            }
                        case MessageTypes.TMSG_APPLICATION_COMMAND:
                            {
                                // Check the kind of command received
                                switch ((CommandeTypes)msg.parameters[2].unsigned_)
                                {
                                    case CommandeTypes.CT_REGISTER:
                                        {
                                            switch ((ObjectTypes)msg.parameters[0].unsigned_)
                                            {
                                                case ObjectTypes.OT_TRASH:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_TRASH");
                                                        CreateUnitySmartObject(msg, "Poubelle");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_BARRIER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_BARRIER");
                                                        CreateUnitySmartObject(msg, "Metal_Barrier");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_ESCALATOR:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_ESCALATOR");
                                                        CreateUnitySmartObject(msg, "Escalator");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_GATE:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_GATE");
                                                        CreateUnitySmartObject(msg, "Gate");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_TICKETBARRIER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_TICKETBARRIER");
                                                        CreateUnitySmartObject(msg, "TicketBarrier");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_EXITBARRIER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_EXITBARRIER");
                                                        CreateUnitySmartObject(msg, "ExitBarrier");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_CAMERA:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_CAMERA");
                                                        CreateUnitySmartObject(msg, "Pinhole");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_OTHER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes.OT_OTHER");
                                                        CreateUnitySmartObject(msg, "other");
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        Debug.LogWarning("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_REGISTER, ObjectTypes not registered:" + msg.parameters[0].unsigned_);
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case CommandeTypes.CT_REGISTER_END:
                                        {
                                            Debug.Log("Received: CommandeTypes.CT_REGISTER_END");
                                            break;
                                        }
                                    case CommandeTypes.CT_REGISTER_SMARTOBJECT:
                                        {
                                            Debug.Log("Received: CommandeTypes.CT_REGISTER_SMARTOBJECT");
                                            switch ((ObjectTypes)msg.parameters[0].unsigned_)
                                            {
                                                default:
                                                    {
                                                        Debug.Log("I got " + msg.parameters[1].unsigned_);
                                                        CreateUnitySmartObject(msg, "other", (int)msg.parameters[3].unsigned_);
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case CommandeTypes.CT_REGISTER_SMARTOBJECT_POSITION:
                                        {
                                            Debug.Log("Received: CommandeTypes.CT_REGISTER_SMARTOBJECT_POSITION");
                                            UpdateUnitySmartObject(msg);
                                            break;
                                        }
                                    case CommandeTypes.CT_SMARTOBJECT_UPDATE_POS_ORI:
                                        {
                                            switch ((ObjectTypes)msg.parameters[0].unsigned_)
                                            {
                                                case ObjectTypes.OT_CAMERA:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_SMARTOBJECT_UPDATE_POS_ORI, ObjectTypes.OT_CAMERA");

                                                        // Update the Unity camera with the values received from SEStar camera
                                                        if ((ObjectTypes)msg.parameters[0].unsigned_ == ObjectTypes.OT_CAMERA)
                                                        {
                                                            Vector3 pos = new Vector3(-msg.parameters[3].float_, msg.parameters[5].float_, -msg.parameters[4].float_);
                                                            Camera.main.transform.position = pos;
                                                            Vector3 dir = new Vector3(msg.parameters[6].float_, msg.parameters[7].float_, msg.parameters[8].float_);
                                                            Vector3 pos1 = new Vector3(msg.parameters[3].float_, msg.parameters[4].float_, msg.parameters[5].float_) + (dir * 100.0f);
                                                            Vector3 pos2 = new Vector3(-pos1.x, pos1.z, -pos1.y);
                                                            Camera.main.transform.LookAt(pos2);
                                                            Camera.main.fieldOfView = msg.parameters[9].float_ * 3.0f / 4.0f;
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_SMARTOBJECT_UPDATE_POS_ORI, ObjectTypes not registered:" + msg.parameters[0].unsigned_);
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    case CommandeTypes.CT_CREATE_SYNTHETICENTITY:
                                        {
                                            Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CREATE_SYNTHETICENTITY");

                                            // If the SyntheticEntity is not in the data structure of SyntheticEntity actors, add it
                                            if (!actorsSyntheticEntities_.ContainsKey(msg.parameters[1].unsigned_))
                                            {
                                                CreateUnitySyntheticEntity(msg.parameters[1].unsigned_);
                                                actorsSyntheticEntities_[msg.parameters[1].unsigned_].name = "SyntheticEntities" + msg.parameters[1].unsigned_.ToString();
                                            }

                                            // Set the type of the pedestrian
                                            uint stringSize = msg.parameters[4].unsigned_;
                                            string pedType = GetString(ref msg.parameters, 5, 16, (int)stringSize);
                                            actorsSyntheticEntities_[msg.parameters[1].unsigned_].GetComponent<SEStarSyntheticEntity>().pedestrianType = pedType;

                                            // Update the counters
                                            if (numberOfPedestriansCreatedOfType.ContainsKey(pedType))
                                            {
                                                numberOfPedestriansCreatedOfType[pedType]++;
                                                //Debug.Log("Number of pedestrians of type " + pedType + " created: " + numberOfPedestriansOfEachTypeCreated[pedType]);
                                            }

                                            //id of the new entity  = msg.parameters[1].unsigned_
                                            //id of the creator of the new entity =  msg.parameters[3].unsigned_
                                            //size of model name of the new entity msg.parameters[4].unsigned_
                                            //model name of the new entity msg.parameters[5 ... 16].char_

                                            break;
                                        }
                                    case CommandeTypes.CT_DELETE_SYNTHETICENTITY:
                                        {
                                            Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_DELETE_SYNTHETICENTITY");

                                            // Remove a SyntheticEntity actor that is no longer in the SEStar simulation
                                            if (actorsSyntheticEntities_.ContainsKey(msg.parameters[1].unsigned_))
                                            {
                                                string pedType = actorsSyntheticEntities_[msg.parameters[1].unsigned_].GetComponent<SEStarSyntheticEntity>().pedestrianType;

                                                if (numberOfPedestriansDeletedOfType.ContainsKey(pedType))
                                                {
                                                    numberOfPedestriansDeletedOfType[pedType]++;
                                                    //Debug.Log("Number of pedestrians of type " + pedType + " deleted: " + numberOfPedestriansDeletedOfType[pedType]);
                                                }

                                                DeleteUnitySyntheticEntity(msg.parameters[1].unsigned_);
                                            }

                                            break;
                                        }
                                    case CommandeTypes.CT_DELETE_VEHICLE:
                                        {
                                            Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_DELETE_VEHICLE");

                                            // Remove a Vehicle actor that is no longer in the SEStar simulation
                                            if (actorsVehicles_.ContainsKey(msg.parameters[1].unsigned_))
                                            {
                                                actorsVehicles_.Remove(msg.parameters[1].unsigned_);
                                            }
                                            break;
                                        }
                                    case CommandeTypes.CT_CHANGE_STATE:
                                        {
                                            string type = GetString(ref msg.parameters, 3, 15);

                                            Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CHANGE_STATE");

                                            switch ((ObjectTypes)msg.parameters[0].unsigned_)
                                            {
                                                case ObjectTypes.OT_CAMERA:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CHANGE_STATE, ObjectTypes.OT_CAMERA");
                                                        break;
                                                    }
                                                case ObjectTypes.OT_TICKETBARRIER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CHANGE_STATE, ObjectTypes.OT_TICKETBARRIER");

                                                        // Added by Aram Azhari
                                                        if (type.Contains("Open"))
                                                        {
                                                            if (smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>() == null)
                                                                smartObjects_[msg.parameters[1].unsigned_].AddComponent<TicketBarrierControl>();
                                                            smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>().ChangeBarrier(true);
                                                        }
                                                        else if (type.Contains("Closed"))
                                                        {
                                                            if (smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>() == null)
                                                                smartObjects_[msg.parameters[1].unsigned_].AddComponent<TicketBarrierControl>();
                                                            smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>().ChangeBarrier(false);
                                                        }
                                                        break;
                                                    }
                                                case ObjectTypes.OT_EXITBARRIER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CHANGE_STATE, ObjectTypes.OT_EXITBARRIER");

                                                        // Added by Aram Azhari
                                                        if (type.Contains("Open"))
                                                        {
                                                            if (smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>() == null)
                                                                smartObjects_[msg.parameters[1].unsigned_].AddComponent<TicketBarrierControl>();
                                                            smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>().ChangeBarrier(true);
                                                        }
                                                        else if (type.Contains("Closed"))
                                                        {
                                                            if (smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>() == null)
                                                                smartObjects_[msg.parameters[1].unsigned_].AddComponent<TicketBarrierControl>();
                                                            smartObjects_[msg.parameters[1].unsigned_].GetComponent<TicketBarrierControl>().ChangeBarrier(false);
                                                        }
                                                        break;
                                                    }
                                                case ObjectTypes.OT_OTHER:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CHANGE_STATE, ObjectTypes.OT_OTHER");

                                                        if (smartObjects_.ContainsKey(msg.parameters[1].unsigned_))
                                                        {
                                                            var controller = smartObjects_[msg.parameters[1].unsigned_].GetComponent<SEStarChangeStateController>();
                                                            if (controller != null)
                                                                controller.OnStateChange(type);
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes.CT_CHANGE_STATE, ObjectTypes not registered:" + msg.parameters[0].unsigned_);
                                                        break;
                                                    }
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            Debug.Log("Received: MessageTypes.TMSG_APPLICATION_COMMAND, CommandeTypes not registered:" + msg.parameters[2].unsigned_);
                                            break;
                                        }
                                }
                                break;
                            }
                        default:
                            {
                                Debug.Log("Received: MessageTypes not registered:" + msg.type);
                                break;
                            }
                    }
                }

                // Clear from the list all the messages that have been received
                msgsIn_.Clear();

                // Send all the messages waiting in the queue to be sent
                foreach (MessageSEStar msg in msgsOut_)
                    sendMsg(msg);

                // Clear from the list all the messages that have been sent
                msgsOut_.Clear();
            }

            //Refresh the UDP camera position
            if (Time.time - UDPCameraRefreshTime > UDPCameraUpdateFrequency)
            {
                Debug.Log("Updating UDP camera position");
                if (oldUDPCameraPosition != Camera.main.transform.position
                    || oldUDPCameraRotation != Camera.main.transform.rotation.eulerAngles
                    || oldUDPCameraFov != Camera.main.fieldOfView)
                {
                    UpdateSEStarCamera(Camera.main.transform.position, 
                        Camera.main.transform.rotation.eulerAngles,
                        Mathf.Clamp(Camera.main.fieldOfView + UnitySeStarFovOffset, 
                        1, 
                        179));

                    UDPCameraRefreshTime = Time.time;
                    oldUDPCameraPosition = Camera.main.transform.position;
                    oldUDPCameraRotation = Camera.main.transform.rotation.eulerAngles;
                    oldUDPCameraFov = Camera.main.fieldOfView;
                }
            }
        }
    }

    /// -----------------------------------------------------------------------
    ///  FUNCTIONALITIES SE-STAR -> UNITY 
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Creates a new SyntheticEntity in the scene.
    /// </summary>
    /// <param name="id">Id of the actor.</param>
    /// <param name="rand">Random system.</param>
    void CreateUnitySyntheticEntity(uint id)
    {
        System.Random rand = new System.Random();

        byte[] values = new byte[1];
        rand.NextBytes(values);
        int rr = values[0] % actorsTemplate_.Count;
        if (actorsTemplate_[rr] != null)
        {
            Debug.Log("Actor: " + actorsTemplate_[rr]);
            var pedestrian = ActorPool.Pop();
            if (pedestrian == null)
            {
                pedestrian = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(Resources.Load("SEStarSyntheticEntities/" + actorsTemplate_[rr]));
                pedestrian.transform.parent = this.transform.FindChild("SyntheticEntities").transform;
                numberOfSyntheticEntitiesSpawned++;
            }

            if (awarenessOfSumoCars)
            {
                pedestrian.AddComponent<PedestrianObjectAwareness>();
                Debug.Log("Adding SUMO awareness");
            }

            actorsSyntheticEntities_[id] = pedestrian;

            if (actorsTemplate_[rr].Contains("_f"))
                actorsGender_[id] = 0;
            else if (actorsTemplate_[rr].Contains("_m"))
                actorsGender_[id] = 1;
            else
                actorsGender_[id] = 2;
        }
        else
        {
            Debug.LogError("There is an error in finding a model for a pedestrian randomly");
        }
    }

    /// <summary>
    /// Delete an actor (pedestrian) in the scene.
    /// </summary>
    /// <param name="id">Id of the actor.</param>
    void DeleteUnitySyntheticEntity(uint id)
    {
        if (actorsSyntheticEntities_.ContainsKey(id))
        {
            actorsSyntheticEntities_[id].SetActive(false);
            UnityEngine.GameObject.Destroy(actorsSyntheticEntities_[id]);
            actorsSyntheticEntities_.Remove(id);
        }

        if (actorsGender_.ContainsKey(id))
        {
            actorsGender_.Remove(id);
        }
    }

    /// <summary>
    /// Updates the animation of a certain actor in the scene. 
    /// This method was modified from the original (below) by Aram Azhari.
    /// </summary>
    /// <param name="id">Id of the actor.</param>
    /// <param name="instantVelocity">Instant velocity of the actor.</param>
    /// <param name="alive">Is alive?</param>
    /// <param name="rand">Random system.</param>
    void UpdateAnimationInUnitySyntheticEntity(uint id, float instantVelocity, bool alive, System.Random rand)
    {
        if (alive == true)
        {
            if (actorsGender_[id] == 0)
            {
                if (instantVelocity > 2.0f)
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 2);

                }
                else if (instantVelocity < 0.1f)
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 0);

                }
                else
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 0.5f);

                }
            }
            else if (actorsGender_[id] == 1)
            {
                if (instantVelocity > 2.0f)
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 2);

                }
                else if (instantVelocity < 0.1f)
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 0);

                }
                else
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 0.5f);

                }
            }
            else
            {
                if (instantVelocity > 2.0f)
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 2);

                }
                else if (instantVelocity < 0.1f)
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 0);

                }
                else
                {
                    actorsSyntheticEntities_[id].GetComponent<AIControllerWithLOD>().SetAnimationParameter("Speed", 0.5f);

                }
            }
        }
        else
        {
            // TODO: Implement die animation. Note: Is this neccesary? (by Miguel R.C)
            /*
            if (actorsGender_[id] == 0)
            {
                if (!(actors_[id].GetComponent<Animation>().IsPlaying("breakdown1_f")) && !(actors_[id].GetComponent<Animation>().IsPlaying("breakdown2_f")))
                {
                    byte[] values = new byte[1];
                    rand.NextBytes(values);
                    if ((values[0]) % 2 == 0)
                        actors_[id].GetComponent<Animation>().Play("breakdown1_f");
                    else
                        actors_[id].GetComponent<Animation>().Play("breakdown2_f");
                    foreach (AnimationState state in actors_[id].GetComponent<Animation>())
                    {
                        state.speed = 1.0F;
                    }
                }
            }
            else if (actorsGender_[id] == 1)
            {
                if (!(actors_[id].GetComponent<Animation>().IsPlaying("breakdown1_m")) && !(actors_[id].GetComponent<Animation>().IsPlaying("breakdown2_m")))
                {
                    byte[] values = new byte[1];
                    rand.NextBytes(values);
                    if ((values[0]) % 2 == 0)
                        actors_[id].GetComponent<Animation>().Play("breakdown1_m");
                    else
                        actors_[id].GetComponent<Animation>().Play("breakdown2_m");
                    foreach (AnimationState state in actors_[id].GetComponent<Animation>())
                    {
                        state.speed = 1.0F;
                    }
                }
            }
            else
            {
                if (!(actors_[id].GetComponent<Animation>().IsPlaying("breakdown1_g")) && !(actors_[id].GetComponent<Animation>().IsPlaying("breakdown2_g")))
                {
                    byte[] values = new byte[1];
                    rand.NextBytes(values);
                    if ((values[0]) % 2 == 0)
                        actors_[id].GetComponent<Animation>().Play("breakdown1_g");
                    else
                        actors_[id].GetComponent<Animation>().Play("breakdown2_g");
                    foreach (AnimationState state in actors_[id].GetComponent<Animation>())
                    {
                        state.speed = 1.0F;
                    }
                }
            }
            */
        }
    }

    /// <summary>
    /// Creates a new smart object in the scene.
    /// </summary>
    /// <param name="msgIn">SEStar message with the smart object information (?).</param>
    /// <param name="actorName">Name of the actor (smart object).</param>
    /// <param name="nameSize">Name size (?).</param>
    void CreateUnitySmartObject(MessageSEStar msgIn, string actorName, int nameSize = -1)
    {
        if (actorName != "other")
        {
            string resourceName = actorName;

            UnityEngine.Object resource = Resources.Load("SEStarSmartObjects/" + resourceName);
            if (resource != null)
            {
                UnityEngine.GameObject newActor = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(resource);
                newActor.transform.parent = this.transform.FindChild("SmartObjects").transform;

                // Set the parameters to the smart object
                SEStarSmartObject uSmartObject;
                if ((uSmartObject = newActor.GetComponent<SEStarSmartObject>()) != null)
                {
                    Debug.Log("New SmartObject:  " + actorName);
                    uSmartObject.objectName = actorName;
                    uSmartObject.objectType = msgIn.parameters[0].unsigned_;
                    uSmartObject.objectId = msgIn.parameters[1].unsigned_;
                }

                byte[] chars = new byte[(23 - 4) * 4];
                int rank = 0;

                for (uint j = 4; j < 23; ++j)
                {
                    chars[rank++] = msgIn.parameters[j].char1_;
                    chars[rank++] = msgIn.parameters[j].char2_;
                    chars[rank++] = msgIn.parameters[j].char3_;
                    chars[rank++] = msgIn.parameters[j].char4_;
                }
                if (nameSize != -1)
                {
                    for (int i = nameSize; i < chars.Length; i++)
                    {
                        chars[i] = 0;
                    }
                }

                string type = System.Text.UTF8Encoding.ASCII.GetString(chars);
                if (type.IndexOf('#') != -1)
                    type = type.Substring(0, type.IndexOf('#'));
                newActor.name = type;
                smartObjects_[msgIn.parameters[1].unsigned_] = newActor;
            }
            else
            {
                Debug.LogWarning("FIXME There is no graphic resource for " + actorName);
            }
        }
        // What are the other possibilities?
        else
        {
            byte[] chars = new byte[(23 - 4) * 4];
            int rank = 0;
            for (uint j = 4; j < 23; ++j)
            {
                chars[rank++] = msgIn.parameters[j].char1_;
                chars[rank++] = msgIn.parameters[j].char2_;
                chars[rank++] = msgIn.parameters[j].char3_;
                chars[rank++] = msgIn.parameters[j].char4_;
            }
            if (nameSize != -1)
            {
                for (int i = nameSize; i < chars.Length; i++)
                {
                    chars[i] = 0;
                }
            }

            string type = System.Text.UTF8Encoding.ASCII.GetString(chars);

            if (type.IndexOf('#') != -1)
                type = type.Substring(0, type.IndexOf('#'));

            string objectname = type;

            if (type.StartsWith(UnityCameraName))
            {
                UnityCameraId = (int)msgIn.parameters[1].unsigned_;
                MessageParameter[] ps = new MessageParameter[24];
                ps[1].unsigned_ = (uint)UnityCameraId;
                ps[2].unsigned_ = (uint)CommandeTypes.CT_CAMERA_SET_NETWORK_CAMERA;

                MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);

                sendMsg(msgOut);
                isUDPCameraActive = true;
            }

            try
            {
                if (type.StartsWith("UnityCamera"))
                {
                    UnityEngine.Object resource = Resources.Load("SEStarSmartObjects/Pinhole");
                    if (resource != null)
                    {
                        UnityEngine.GameObject newActor = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(resource);
                        newActor.transform.parent = this.transform.FindChild("SmartObjects").transform;
                        smartObjects_[msgIn.parameters[1].unsigned_] = newActor;

                        // Set the parameters to the smart object
                        SEStarSmartObject uSmartObject;
                        if ((uSmartObject = newActor.GetComponent<SEStarSmartObject>()) != null)
                        {
                            Debug.Log("New SmartObject:  " + "UnityCamera");
                            uSmartObject.objectName = "UnityCamera";
                            uSmartObject.objectType = msgIn.parameters[0].unsigned_;
                            uSmartObject.objectId = msgIn.parameters[1].unsigned_;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("FIXME There is no graphic resource for Pinhole");
                    }
                }
                else
                {
                    if (objectname.ToLower().StartsWith("roadblock") || objectname.ToLower().StartsWith("road block"))
                    {
                        UnityEngine.Object resource = Resources.Load("SEStarSmartObjects/RoadBlock");
                        if (resource != null)
                        {
                            UnityEngine.GameObject newActor = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(resource);
                            newActor.transform.parent = this.transform.FindChild("SmartObjects").transform;
                            //var controller = newActor.GetComponent<SEStarRoadBlockController>();
                            //controller.Id = msgIn.parameters[1].unsigned_;
                            //controller.state = "Undefined";
                            //controller.sestar = this;
                            smartObjects_[msgIn.parameters[1].unsigned_] = newActor;

                            // Set the parameters to the smart object
                            SEStarSmartObject uSmartObject;
                            if ((uSmartObject = newActor.GetComponent<SEStarSmartObject>()) != null)
                            {
                                Debug.Log("New SmartObject:  " + "road block");
                                uSmartObject.objectName = "Road Block";
                                uSmartObject.objectType = msgIn.parameters[0].unsigned_;
                                uSmartObject.objectId = msgIn.parameters[1].unsigned_;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("FIXME There is no graphic resource for RoadBlock");
                        }
                    }
                    else
                    {
                        string resourceName = objectname;
                        //Check the resource name depending on the actorName parameter
                        if (objectname.StartsWith("Exit Gate"))
                        {
                            resourceName = "Gate";
                        }
                        else if (objectname.StartsWith("Security Check"))
                        {
                            resourceName = "Security Check";
                        }
                        else if (objectname.StartsWith("Spawner"))
                        {
                            resourceName = "Spawner";
                        }
                        else if (objectname.StartsWith("Park Barrier") || objectname.StartsWith("Waiting Spot"))
                        {
                            resourceName = "Default";
                        }

                        UnityEngine.Object resource = Resources.Load("SEStarSmartObjects/" + resourceName);
                        if (resource != null)
                        {
                            UnityEngine.GameObject newActor = (UnityEngine.GameObject)UnityEngine.GameObject.Instantiate(resource);
                            Debug.Log("New SmartObject:  " + objectname);
                            newActor.transform.parent = this.transform.FindChild("SmartObjects").transform;
                            smartObjects_[msgIn.parameters[1].unsigned_] = newActor;

                            // Set the parameters to the smart object
                            SEStarSmartObject uSmartObject;
                            if ((uSmartObject = newActor.GetComponent<SEStarSmartObject>()) != null)
                            {
                                uSmartObject.objectName = objectname;
                                uSmartObject.objectType = msgIn.parameters[0].unsigned_;
                                uSmartObject.objectId = msgIn.parameters[1].unsigned_;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("FIXME There is no graphic resource for " + objectname);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }
    }

    /// <summary>
    /// Updates a smart object in the scene. 
    /// </summary>
    /// <param name="msgIn"></param>
    void UpdateUnitySmartObject(MessageSEStar msgIn)
    {
        if (!smartObjects_.ContainsKey(msgIn.parameters[1].unsigned_))
            return;

        Vector3 pos = new Vector3(-msgIn.parameters[3].float_, msgIn.parameters[5].float_ - 0.10f, -msgIn.parameters[4].float_);
        Vector3 ori = new Vector3(msgIn.parameters[6].float_, msgIn.parameters[7].float_, msgIn.parameters[8].float_);
        Vector3 scale = new Vector3(msgIn.parameters[10].float_, msgIn.parameters[11].float_, msgIn.parameters[9].float_);

        smartObjects_[msgIn.parameters[1].unsigned_].transform.position = pos;

        Vector3 dir = ori;
        Vector3 pos1 = new Vector3(msgIn.parameters[3].float_, msgIn.parameters[4].float_, msgIn.parameters[5].float_) + (dir * 100.0f);
        Vector3 pos2 = new Vector3(-pos1.x, pos1.z, -pos1.y);
        smartObjects_[msgIn.parameters[1].unsigned_].transform.LookAt(pos2);
        smartObjects_[msgIn.parameters[1].unsigned_].transform.localScale = new Vector3(scale.x*1.9f, scale.y*1.5f, scale.z);

        // Added by Aram Azhari:
        smartObjects_[msgIn.parameters[1].unsigned_].transform.RotateAround(Vector3.zero, Vector3.up, 90f);
        smartObjects_[msgIn.parameters[1].unsigned_].transform.position = smartObjects_[msgIn.parameters[1].unsigned_].transform.position + CalculatedDifference;
    }

    /// -----------------------------------------------------------------------
    ///  FUNCTIONALITIES UNITY -> SE-STAR
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Initializes the control of the SEStar camera (?).
    /// </summary>
    public void CreateNewSEStarCamera()
    {
        if (connected_ == true)
        {
            Debug.Log("Call to InitializeControllingSEStarCamera");

            MessageParameter[] ps = new MessageParameter[24];
            ps[1].unsigned_ = 0;
            ps[2].unsigned_ = (uint)CommandeTypes.CT_SPAWN_SMARTOBJECT;

            //Vector3 unityVector = Dummy.position;

            //var ToUnity = SEStarToUnityCoordinate(sestarVector, Vector3.forward, Vector3.one);
            var res = UnityCoordinateToSEStar(originPoint.position, originPoint.rotation.eulerAngles, Vector3.one);

            ps[3].float_ = res[0].x;
            ps[4].float_ = res[0].y;
            ps[5].float_ = res[0].z;
            ps[6].float_ = res[1].x;
            ps[7].float_ = res[1].y;
            ps[8].float_ = res[1].z;

            ps[9].float_ = 1;
            ps[10].float_ = 1;
            ps[11].float_ = 1;
            ps[12].bool_ = false;

            UnityCameraName = "UnityCamera" + Guid.NewGuid().ToString().Replace("-", "").ToLower().Substring(0, 5);
            var name = System.Text.UTF8Encoding.ASCII.GetBytes(UnityCameraName + "$Pinhole");
            var charChunks = name.Length / 4 + 1;
            var remainder = name.Length % 4;
            var idx = 13;
            if (charChunks == 1)
                FillChars(ref ps[idx], name);
            else for (int l = 0; l < charChunks; l++)
                {
                    if (l != charChunks - 1)
                        FillChars(ref ps[idx++], name.SubArray(4 * l, 4));
                    else
                        FillChars(ref ps[idx++], name.SubArray(4 * l, remainder));
                }

            MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);
            lock (tcpLock_)
            {
                sendMsg(msgOut);
            }
        }
    }

    /// <summary>
    /// Changes the state of an object (actor?) in the scene.
    /// </summary>
    /// <param name="objectId">Id of the object.</param>
    /// <param name="status">New status of the object.</param>
    public void ChangeSEStarObjectState(uint objectId, string status)
    {
        Debug.Log("Changing state of smartObject " + objectId);

        MessageParameter[] ps = new MessageParameter[24];
        ps[1].unsigned_ = objectId;
        ps[2].unsigned_ = (uint)CommandeTypes.CT_CHANGE_STATE;
        SetString(ref ps, 3, status);

        MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);

        lock (tcpLock_)
        {
            Debug.Log("Sending message out to SEStar...");
            sendMsg(msgOut);
        }
    }

    /// <summary>
    /// Changes the variable of an object in the scene.
    /// </summary>
    /// <param name="objectId">Id of the object.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="variableName">Name of the variable to change.</param>
    /// <param name="variableValue">New value for the variable.</param>
    public void ChangeSEStarObjectVariable(uint objectId, uint objectType, string variableName, int variableValue)
    {
        Debug.Log("Changing variable of smartObject " + objectId);

        MessageParameter[] ps = new MessageParameter[24];
        ps[0].unsigned_ = objectType;
        ps[1].unsigned_ = objectId;
        ps[2].unsigned_ = (uint)CommandeTypes.CT_CHANGE_VARIABLE_FLOAT;
        SetString(ref ps, 3, variableName + "@" + variableValue.ToString());

        MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);

        lock (tcpLock_)
        {
            Debug.Log("Sending message out to SEStar...");
            sendMsg(msgOut);
        }
    }

    /// <summary>
    /// Changes the state of the SEStar simulation. 
    /// </summary>
    /// <param name="state">New state for the simulation (Paused, Playing or Stopped).</param>
    public void ChangeSEStarSimulationState(SimulationState state)
    {
        MessageParameter[] ps = new MessageParameter[24];

        MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_PAUSE_SIMULATION, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);
        switch (state)
        {
            case SimulationState.Paused:
                {
                    msgOut.type = MessageTypes.TMSG_PAUSE_SIMULATION;
                    break;
                }
            case SimulationState.Playing:
                {
                    msgOut.type = MessageTypes.TMSG_PLAY_SIMULATION;
                    break;
                }
            case SimulationState.Stopped:
                {
                    msgOut.type = MessageTypes.TMSG_STOP_SIMULATION;
                    break;
                }
        }

        lock (tcpLock_)
        {
            sendMsg(msgOut);
        }
    }

    /// <summary>
    /// Updates the SEStar camera from Unity. 
    /// </summary>
    /// <param name="cameraPosition">Position coordinates of the camera.</param>
    /// <param name="cameraRotation">Rotation coordinates of the camera.</param>
    /// <param name="fov">FOV of the camera.</param>
    /// <param name="farPlane">Far plane (40 by default).</param>
    public void UpdateSEStarCamera(Vector3 cameraPosition, Vector3 cameraRotation, float fov)
    {
        if (isUDPCameraActive)
        {
            MessageParameter[] ps = new MessageParameter[24];
            ps[1].unsigned_ = (uint)UnityCameraId;
            ps[2].unsigned_ = (uint)CommandeTypes.CT_SPAWN_SMARTOBJECT;
            var res = UnityCoordinateToSEStar(cameraPosition, cameraRotation, Vector3.one);
            ps[3].float_ = res[0].x;
            ps[4].float_ = res[0].y;
            ps[5].float_ = res[0].z;
            ps[6].float_ = res[1].x;
            ps[7].float_ = res[1].y;
            ps[8].float_ = res[1].z;
            ps[9].float_ = 1;
            ps[10].float_ = 1;
            ps[11].float_ = 1;
            MessageSEStar msgOut = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, ps);

            MessageParameter[] psFov = new MessageParameter[24];
            psFov[1].unsigned_ = (uint)UnityCameraId;
            psFov[2].unsigned_ = (uint)CommandeTypes.CT_CHANGE_VARIABLE_FLOAT;
            SetString(ref psFov, 3, "horizontalfov@" + fov);
            MessageSEStar msgOut2 = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, psFov);

            MessageParameter[] psFarPane = new MessageParameter[24];
            psFarPane[1].unsigned_ = (uint)UnityCameraId;
            psFarPane[2].unsigned_ = (uint)CommandeTypes.CT_CHANGE_VARIABLE_FLOAT;
            SetString(ref psFarPane, 3, "plan_far@" + SEStarCameraFarPlane);
            MessageSEStar msgOut3 = new MessageSEStar(MessageTypes.TMSG_APPLICATION_COMMAND, (MessageTypes)ExternalApplicationModes.MEA_BOTH, idSender_, idReceiver_, psFarPane);

            lock (tcpLock_)
            {
                sendMsg(msgOut);
                sendMsg(msgOut2);
                sendMsg(msgOut3);
            }
        }
    }

    /// -----------------------------------------------------------------------
    ///  AUXILIARY METHODS 
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Changes the speed factor parameter according to the parameter given.
    /// </summary>
    /// <param name="speedFactor">New speed factor.</param>
    public void ChangeSpeedFactor(float speedFactor)
    {
        speedFactor_ = speedFactor;
    }

    /// <summary>
    /// Converts SEStar coordinates to Unity coordinates. 
    /// First overload. 
    /// </summary>
    /// <param name="pos">Position coordinates.</param>
    /// <param name="ori">Origin coordinates.</param>
    /// <param name="scale">Scale coordinates.</param>
    /// <returns></returns>
    public Vector3[] SEStarToUnityCoordinate(Vector3 pos, Vector3 ori, Vector3 scale)
    {

        Vector3 npos = new Vector3(-pos.x, pos.z, -pos.y);
        Vector3 nori = new Vector3(ori.x, ori.y, ori.z);

        Vector3 dir = ori;
        Vector3 pos1 = pos + dir * 100;
        Vector3 pos2 = new Vector3(-pos1.x, pos1.z, -pos1.y);
        GameObject g = new GameObject("dummy");
        g.transform.position = npos;
        g.transform.LookAt(pos2);

        // Added by Aram Azhari:
        g.transform.RotateAround(Vector3.zero, Vector3.up, 90f);
        g.transform.position = g.transform.position + CalculatedDifference;

        var calPos = g.transform.position;
        var calRot = g.transform.rotation.eulerAngles;
        UnityEngine.Object.DestroyImmediate(g);
        return new Vector3[] { calPos, calRot, Vector3.one };
    }

    /// <summary>
    /// Converts SEStar coordinates to Unity coordinates. 
    /// Second overload. 
    /// </summary>
    /// <param name="pos">Position coordinates.</param>
    /// <param name="ori">Origin coordinates.</param>
    /// <param name="scale">Scale coordinates.</param>
    /// <param name="objectToUpdate">Object to update (?).</param>
    public void SEStarToUnityCoordinate(Vector3 pos, Vector3 ori, Vector3 scale, ref Transform objectToUpdate)
    {

        Vector3 npos = new Vector3(-pos.x, pos.z, -pos.y);
        Vector3 nori = new Vector3(ori.x, ori.y, ori.z);

        Vector3 dir = ori;
        Vector3 pos1 = pos + dir * 100;
        Vector3 pos2 = new Vector3(-pos1.x, pos1.z, -pos1.y);
        objectToUpdate.position = npos;
        objectToUpdate.LookAt(pos2);


        // Added by Aram Azhari
        objectToUpdate.RotateAround(Vector3.zero, Vector3.up, 90f);
        objectToUpdate.position = objectToUpdate.position + CalculatedDifference;
    }

    /// <summary>
    /// Converts Unity to SEStar coordinates.
    /// First overload.
    /// </summary>
    /// <param name="pos">Position coordinates.</param>
    /// <param name="ori">Origin coordinates.</param>
    /// <param name="scale">Scale coordinates.</param>
    /// <returns></returns>
    public Vector3[] UnityCoordinateToSEStar(Vector3 pos, Vector3 ori, Vector3 scale)
    {
        GameObject go = new GameObject("Camera SPAWN");
        Transform temp = go.transform;
        temp.position = pos;
        temp.rotation = Quaternion.Euler(ori);
        temp.localScale = scale;
        temp.position = temp.position - CalculatedDifference;
        temp.RotateAround(Vector3.zero, Vector3.up, -90);

        var lookatDirection = temp.forward;
        lookatDirection = new Vector3(-lookatDirection.x, -lookatDirection.z, lookatDirection.y);
        temp.position = new Vector3(-temp.position.x, -temp.position.z, temp.position.y);
        UnityEngine.Object.Destroy(go, 0.1f);
        //Debug.Log("Camera: " + temp.position + "\t" + lookatDirection);
        return new Vector3[] { temp.position, lookatDirection, Vector3.one };
    }

    /// <summary>
    /// Converts Unity to SEStar coordinates.
    /// First overload.
    /// </summary>
    /// <param name="buffer">Transform buffer (?).</param>
    /// <returns></returns>
    public Vector3[] UnityCoordinateToSEStar(ref Transform buffer)
    {
        buffer.position = buffer.position - CalculatedDifference;
        buffer.RotateAround(Vector3.zero, Vector3.up, -90);

        var lookatDirection = buffer.forward;
        lookatDirection = new Vector3(-lookatDirection.x, -lookatDirection.z, lookatDirection.y);
        buffer.position = new Vector3(-buffer.position.x, -buffer.position.z, buffer.position.y);
        //Debug.Log("Camera: " + buffer.position + "\t" + lookatDirection);
        return new Vector3[] { buffer.position, lookatDirection, Vector3.one };
    }

    /// <summary>
    /// Sets a string inside a MessageParameter of a SEStarMessage. 
    /// (Auxiliar method to handle SEStarMessages).
    /// </summary>
    /// <param name="ms">Message parameter to be set.</param>
    /// <param name="startingIndex">Starting index (?).</param>
    /// <param name="value">String value to be set.</param>
    public void SetString(ref MessageParameter[] ms, int startingIndex, string value)
    {
        var name = System.Text.UTF8Encoding.ASCII.GetBytes(value);
        var charChunks = name.Length / 4 + 1;
        var remainder = name.Length % 4;
        var idx = startingIndex;
        if (charChunks == 1)
            FillChars(ref ms[idx], name);
        else for (int l = 0; l < charChunks; l++)
            {
                if (l != charChunks - 1)
                    FillChars(ref ms[idx++], name.SubArray(4 * l, 4));
                else
                    FillChars(ref ms[idx++], name.SubArray(4 * l, remainder));
            }
    }

    /// <summary>
    /// Gets a string from inside a MessageParameter of a SEStarMessage.
    /// (Auxiliar method to handle SEStarMessages).
    /// </summary>
    /// <param name="ms">Message parameter to be set.</param>
    /// <param name="startingIndex">Starting index (?).</param>
    /// <param name="endingIndex">Ending index (?).</param>
    /// <param name="Size">Size (-1 by default) (?).</param>
    /// <returns>String with the parameter value requested.</returns>
    public string GetString(ref MessageParameter[] ms, int startingIndex, int endingIndex, int Size = -1)
    {
        byte[] chars = new byte[52];
        int rank = 0;
        for (int j = startingIndex; j < endingIndex; ++j)
        {
            chars[rank++] = ms[j].char1_;
            chars[rank++] = ms[j].char2_;
            chars[rank++] = ms[j].char3_;
            chars[rank++] = ms[j].char4_;
        }
        if (Size != -1)
        {
            for (int i = Size; i < chars.Length; i++)
            {
                chars[i] = 0;
            }

            //Get the string and cut the chars that are not part of it
            string str = System.Text.UTF8Encoding.ASCII.GetString(chars);
            return str.Substring(0, Size);
        }
        else
        {
            string str = System.Text.UTF8Encoding.ASCII.GetString(chars);
            return str;
        }
    }

    /// <summary>
    /// Fills the chars inside a MessageParameter of a SEStarMessage. 
    /// (Auxiliar method to handle SEStarMessages).
    /// </summary>
    /// <param name="ms">Message parameter that contains the chars to be filled.</param>
    /// <param name="chars">List of chars.</param>
    public void FillChars(ref MessageParameter ms, byte[] chars)
    {
        if (chars.Length == 0)
            return;
        else if (chars.Length == 1)
            ms.char1_ = chars[0];
        else if (chars.Length == 2)
        {
            ms.char1_ = chars[0];
            ms.char2_ = chars[1];
        }
        else if (chars.Length == 3)
        {
            ms.char1_ = chars[0];
            ms.char2_ = chars[1];
            ms.char3_ = chars[2];
        }
        else if (chars.Length == 4)
        {
            ms.char1_ = chars[0];
            ms.char2_ = chars[1];
            ms.char3_ = chars[2];
            ms.char4_ = chars[3];
        }
    }

    /// <summary>
    /// Transforms a byte array into a string. 
    /// Auxiliar method. 
    /// </summary>
    /// <param name="bytes">Array of bytes.</param>
    /// <returns></returns>
    public string PrintBytes(Byte[] bytes)
    {
        string msg = "";

        for (int i = 0; i < bytes.Length; i++)
        {
            msg = String.Concat(msg, bytes[i]);
        }

        return msg;
    }

    /// <summary>
    /// Gets the total number of SyntheticEntities deleted in SEStar since the beginning of the simulation. 
    /// If type is given, it will return only the number of SyntheticEntities of that type.
    /// </summary> 
    /// <param name="type">Type of the SyntheticEntities deleted (if any).</param>
    /// <returns>Number of SyntheticEntities deleted.</returns>
    public int GetNumberOfSyntheticEntitiesDeleted(string type = null)
    {
        if (type != null && numberOfPedestriansDeletedOfType.ContainsKey(type))
        {
            return numberOfPedestriansDeletedOfType[type];
        }
        else
        {
            int sum = 0;

            foreach (string key in pedestrianTypes)
            {
                sum += numberOfPedestriansDeletedOfType[key];
            }

            return sum;
        }
    }

    /// <summary>
    /// Gets the total number of SyntheticEntities created in SEStar since the beginning of the simulation. 
    /// If type is given, it will return only the number of SyntheticEntities of that type.
    /// </summary> 
    /// <param name="type">Type of the SyntheticEntities created (if any).</param>
    /// <returns>Number of SyntheticEntities created.</returns>
    public int GetNumberOfSyntheticEntitiesCreated(string type = null)
    {
        if (type != null && numberOfPedestriansCreatedOfType.ContainsKey(type))
        {
            return numberOfPedestriansCreatedOfType[type];
        }
        else
        {
            int sum = 0;

            foreach (string key in pedestrianTypes)
            {
                sum += numberOfPedestriansCreatedOfType[key];
            }

            return sum;
        }
    }

    /// <summary>
    /// Gets the total number of SyntheticEntities that are currently part of the simulation (i.e. created and not deleted yet). 
    /// If type is given, it will return only the number of SyntheticEntities of that type.
    /// </summary> 
    /// <param name="type">Type of the SyntheticEntities created (if any).</param>
    /// <returns>Number of SyntheticEntities created.</returns>
    public int GetNumberOfSyntheticEntitiesInSimulation(string type = null)
    {
        if (type != null && numberOfPedestriansCreatedOfType.ContainsKey(type) && numberOfPedestriansDeletedOfType.ContainsKey(type))
        {
            return numberOfPedestriansCreatedOfType[type] - numberOfPedestriansDeletedOfType[type];
        }
        else
        {
            int sum = 0;

            foreach (string key in pedestrianTypes)
            {
                sum += numberOfPedestriansCreatedOfType[key] - numberOfPedestriansDeletedOfType[key];
            }

            return sum;
        }
    }

    /// -----------------------------------------------------------------------
    ///  TCP/UDP COMMUNICATION HANDLERS 
    /// -----------------------------------------------------------------------

    /// <summary>
    /// Thread for the TCP connection with SEStar.
    /// </summary>
    public void tcpThread()
    {
        Debug.Log("tcpThread : Begin");

        //Try to connect to SEStar client via TCP
        while (sestarClient_ == null)
        {
            try
            {
                sestarClient_ = new TcpClient(sestarAddress_, sestarPort_);
            }
            catch (SocketException se)
            {
                Debug.Log("Error when trying to create tcpClient: " + se.Message + se.StackTrace);
                Thread.Sleep(3000);
            }
        }

        // Create a stream for the TCP connection
        sestarStream_ = sestarClient_.GetStream();
        int bytesRead;

        // Read the messages sent by SEStar via TCP while connected
        while (connected_ && sestarClient_.Connected)
        {
            Byte[] buffer = new byte[65536];

            try
            {
                bytesRead = sestarStream_.Read(buffer, 0, buffer.Length);
            }
            catch (SocketException)
            {
                break;
            }
            if (bytesRead != 0)
            {
                int numMessage = bytesRead / 116;

                // Read the body of the message
                lock (tcpLock_)
                {
                    for (int i = 0; i < numMessage; i++)
                    {
                        byte[] dataBytes = new byte[116];
                        Buffer.BlockCopy(buffer, i * 116, dataBytes, 0, 116);
                        GCHandle hdl = GCHandle.Alloc(dataBytes, GCHandleType.Pinned);
                        MessageSEStar message;
                        IntPtr bufferLocal = hdl.AddrOfPinnedObject();
                        message = (MessageSEStar)Marshal.PtrToStructure(bufferLocal, typeof(MessageSEStar));
                        msgsIn_.Add(message);
                    }
                }
            }
        }

        Debug.Log("tcpThread : End");
    }

    /// <summary>
    /// Thread for the UDP connection with SEStar to get SyntheticEntities.
    /// </summary>
    private void udpThreadSyntheticEntities()
    {
        Debug.Log("udpThreadSyntheticEntities : Begin");

        // Create an UDP connection for getting SyntheticEntities
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, basePortSyntheticEntities_);
        udpClientSyntheticEntities_ = new UdpClient(remoteIpEndPoint);
        udpClientSyntheticEntities_.Client.ReceiveBufferSize = 851968;
        Byte[] receiveBytes = null;

        // Loop to get the messages sent by SEStar via this UDP connection
        while (connected_ == true)
        {
            try
            {
                receiveBytes = udpClientSyntheticEntities_.Receive(ref remoteIpEndPoint);

                lock (udpLockSyntheticEntities_)
                {
                    numSyntheticEntities_[0] = 0;
                    Buffer.BlockCopy(receiveBytes, 0, numSyntheticEntities_, 0, sizeof(int));
                    byte[] dataBytes = new byte[36];

                    if (numSyntheticEntities_[0] > 0)
                    {
                        //Debug.Log("UDP Thread: " + numSyntheticEntities_[0] + " SyntheticEntities received");

                        for (int i = 0; i < numSyntheticEntities_[0]; i++)
                        {
                            // Get the message and parse it to Message3DVIA
                            Buffer.BlockCopy(receiveBytes, sizeof(int) + (i * 36), dataBytes, 0, 36);
                            GCHandle hdl = GCHandle.Alloc(dataBytes, GCHandleType.Pinned);
                            IntPtr bufferLocal = hdl.AddrOfPinnedObject();
                            syntheticEntities_[i] = (Message3DVIA)Marshal.PtrToStructure(bufferLocal, typeof(Message3DVIA));
                        }
                        newSyntheticEntitiesData_ = true;
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.LogError("udpThreadSyntheticEntities exception : " + ex.ToString());
            }
        }

        Debug.Log("udpThreadSyntheticEntities : End");
    }

    /// <summary>
    /// Thread for the UDP connection with SEStar to get Vehicles.
    /// </summary>
    private void udpThreadVehicles()
    {
        Debug.Log("udpThreadVehicles : Begin");

        // Creates an UDP connection for getting Vehicles
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, basePortVehicles_);
        udpClientVehicles_ = new UdpClient(remoteIpEndPoint);
        udpClientVehicles_.Client.ReceiveBufferSize = 851968;
        Byte[] receiveBytes = null;

        // Loop to get the messages sent by SEStar via this UDP connection
        while (connected_ == true)
        {
            try
            {
                receiveBytes = udpClientVehicles_.Receive(ref remoteIpEndPoint);
                lock (udpLockVehicles_)
                {
                    numVehicles_[0] = 0;
                    Buffer.BlockCopy(receiveBytes, 0, numVehicles_, 0, sizeof(int));
                    byte[] dataBytes = new byte[36];
                    if (numVehicles_[0] > 0)
                    {
                        Debug.Log("UDP Thread: " + numVehicles_[0] + " Vehicles received");

                        for (int i = 0; i < numVehicles_[0]; i++)
                        {
                            // Get the message and parse it to Message3DVIA
                            Buffer.BlockCopy(receiveBytes, sizeof(int) + (i * 36), dataBytes, 0, 36);
                            GCHandle hdl = GCHandle.Alloc(dataBytes, GCHandleType.Pinned);
                            IntPtr bufferLocal = hdl.AddrOfPinnedObject();
                            vehicles_[i] = (Message3DVIA)Marshal.PtrToStructure(bufferLocal, typeof(Message3DVIA));
                        }
                        newVehicleData_ = true;
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("udpThreadVehicles exception : " + ex.ToString());
            }
        }
        Debug.Log("udpThreadVehicles : End");
    }

    /// <summary>
    /// Sends a message to SEStar.
    /// </summary>
    /// <param name="msg">Message to send to SEStar.</param>
    private void sendMsg(MessageSEStar msg)
    {
        // Prepare the message and marshal it
        int size = Marshal.SizeOf(msg);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(msg, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        //Try to send the message to SEStar
        try
        {
            sestarStream_.Write(arr, 0, arr.Length);
        }
        catch (Exception ex)
        {
            if (ex is ObjectDisposedException || ex is System.IO.IOException)
            {
                // Do nothing - we've closed the stream
                Debug.Log("Crash in sendMsg");
                return;
            }
            else
            {
                Debug.Log("Unhandled crash in sendMsg: " + ex.Message);
            }
        }

        //Debug.Log("sendMsg : End");
    }

    /// <summary>
    /// Actions to perform when quitting (called at the end of the simulation).
    /// </summary>
    void OnDestroy()
    {
        connected_ = false;

        // Close the UDP connection for SyntheticEntities
        if (udpClientSyntheticEntities_ != null)
            udpClientSyntheticEntities_.Close();

        // Close the UDP connection for Vehicles
        if (udpClientVehicles_ != null)
            udpClientVehicles_.Close();

        // Close the TCP connection
        if (sestarStream_ != null)
        {
            sestarStream_.Close();
            sestarClient_.Close();
        }

        Debug.Log("SEStar Stop");
    }
}
