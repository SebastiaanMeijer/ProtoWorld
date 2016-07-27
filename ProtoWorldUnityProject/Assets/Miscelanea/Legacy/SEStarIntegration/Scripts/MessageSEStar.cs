/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Runtime.InteropServices;
using Connector.Generic;

namespace Connector.Messages.SEStar
{
    /// <summary>
    /// Enumerate that defines external application types. 
    /// </summary>
    public enum ExternalApplicationTypes
    {
        TEA_UNREGISTERED = 0,
        TEA_SESTAR,
        TEA_BUSINESSSTUDIO,
        TEA_MINDSTORMS,
        TEA_AXIS
    }

    /// <summary>
    /// Enumerate that defines external application modes. 
    /// </summary>
    public enum ExternalApplicationModes
    {
        MEA_BOTH = 0,
        MEA_HMI,
        MEA_SIMULATOR
    }

    /// <summary>
    /// Enumerate that defines message types.
    /// </summary>
    public enum MessageTypes
    {
        TMSG_FIRST_TYPEMESSAGE = 0,
        TMSG_UNKNOWN = TMSG_FIRST_TYPEMESSAGE,

        TMSG_SET_POSITION,                  // param1 : x, param2 : y, param3 : z
        TMSG_SET_ORIENTATION,               // param1 : yaw, param2 : pitch, param3 : roll
        TMSG_SET_COLOR,                     // param1 : r, param2 : g, param3 : b, param4 : alpha, range for parameters : [0.0,..,1.0]
        TMSG_UPDATE_AREAS_GRP,
        TMSG_REMOVE_AREAS_GRP,
        TMSG_AREAS_NAVMESH,

        TMSG_STOP_SIMULATION = TMSG_FIRST_TYPEMESSAGE + 41, // NDNicolas : wut ?
        TMSG_PAUSE_SIMULATION,
        TMSG_PLAY_SIMULATION,
        TMSG_START_DEBUGVIEW,
        TMSG_STOP_DEBUGVIEW,
        TMSG_FORWARD_MESSAGE,
        TMSG_REGISTER_ENTITY,
        TMSG_UNREGISTER_ENTITY,
        TMSG_GET_ENTITYSTATUS,

        TMSG_REGISTER_APPLICATION,
        TMSG_UNREGISTER_APPLICATION,
        TMSG_APPLICATION_COMMAND,
        TMSG_COMMAND,
        TMSG_APPLICATION_ALERT,
        TMSG_COMMUNICATION_TWEET,

        TMSG_SET_TIMER_PROPPERTIES,

        TMSG_NOT_A_MESSAGE_TYPE
    }

    public enum CommandeTypes
    {
        CT_REGISTER = 0,
        CT_REGISTER_END,
        CT_CHANGE_STATE,
        CT_SPAWN_ENTITY,
        CT_SPAWN_EXTERNAL_ENTITY,
        CT_REMOVE_EXTERNAL_ENTITY,
        CT_CAMERA_SELECT,
        CT_CAMERA_UNSELECT,
        CT_CAMERA_START_RECORD,
        CT_CAMERA_STOP_RECORD,
        CT_CAMERA_PAN_TILT,
        CT_CAMERA_ZOOM,
        CT_CAMERA_PAN_TILT_ZOOM,
        CT_CAMERA_ABSOLUTE_POSITION,
        CT_CAMERA_LOOK_AT,
        CT_REGISTER_ZONE,
        CT_GET_ENTITIES_ROLE_ZONE,
        CT_CHANGE_ENTITY_ROLE,
        CT_CHANGE_VARIABLE_FLOAT,
        CT_SMARTOBJECT_UPDATE_POS_ORI,
        CT_CREATE_SYNTHETICENTITY,
        CT_DELETE_SYNTHETICENTITY,
        CT_DELETE_VEHICLE,
        CT_REGISTER_SMARTOBJECT_POSITION,
        CT_SPAWN_SMARTOBJECT,
        CT_REMOVE_SMARTOBJECT,
        CT_REGISTER_SMARTOBJECT,
        CT_REPLAN_SYNTHETICENTITY,
        CT_DELETE_ALL_SYNTHETICENTITY,
        CT_CAMERA_SET_NETWORK_CAMERA,
        CT_CAMERA_INCREMENT_POSITION,
        CT_REGISTER_SMARTOBJECT_LINKS,
        CT_MONITOR_VARIABLE,
        CT_GET_QUEUE_INFO,
        CT_QUEUE_INFO_RESULT,
        CT_CAMERA_VISIBLE_ENTITIES,
        CT_ENTITY_GO_TO,
        CT_DATA_COLLECTOR_UPDATE_INFO,
        CT_SIMULATION_STATE,
        CT_NOT_A_CMD = 100000            //should not change : used to generate id for command
    }

    /// <summary>
    /// Enumerate that defines object types. 
    /// </summary>
    public enum ObjectTypes
    {
        OT_OTHER = 0,

        OT_BARRIER,       // required for unity
        OT_CAMERA,
        OT_ESCALATOR,     // required for unity
        OT_EXITBARRIER,   // required for unity
        OT_FIREPANEL,
        OT_GATE,
        OT_LUGGAGE,       // required for unity
        OT_SMOKEDETECTOR,
        OT_SPEAKER,
        OT_TICKETBARRIER, // required for unity
        OT_TRASH,         // required for unity
        OT_DOOR,
        OT_ZONE,
        OT_XRAY_BELT_INPUT,
        OT_XRAY_BELT_OUTPUT,
        OT_XRAY_CORE,
        OT_AGGREGATED_QUEUE,
        OT_METAL_DETECTOR,
        OT_PIF,
        OT_ELEVATOR,
        OT_HVAC
    }

    /// <summary>
    /// Enumerate that defines camera position flags. 
    /// </summary>
    [Flags]
    public enum CameraPositionFlag
    {
        PTZ_Pan  = 1 << 0,
        PTZ_Tilt = 1 << 1,
        PTZ_Zoom = 1 << 2
    };

    /// <summary>
    /// Struct that contains the parameters of a SeStar message.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MessageParameter
    {
        [FieldOffset(0)]
        public float float_;

        [FieldOffset(0)]
        public uint unsigned_;

        [FieldOffset(0)]
        public byte char_;

        [FieldOffset(0)]
        public int int_;

        [FieldOffset(0)]
        public bool bool_;

        [FieldOffset(0)]
        public byte char1_;

        [FieldOffset(1)]
        public byte char2_;

        [FieldOffset(2)]
        public byte char3_;

        [FieldOffset(3)]
        public byte char4_;
    }

    /// <summary>
    /// Struct that contains the parameters of a message 3DVIA (?) from SeStar.
    /// </summary>
    public struct Message3DVIAFromSEStar
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

    /// <summary>
    /// Class that implements a SeStar message.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MessageSEStar : IMessage
    {
        /// <summary>
        /// Size of the message.
        /// </summary>
        public uint size;

        /// <summary>
        /// Type of the message.
        /// </summary>
        public MessageTypes type;

        /// <summary>
        /// Subtype of the message.
        /// </summary>
        public MessageTypes subtype; 

        /// <summary>
        ///  SE-Star identifier of the sender of the message.
        /// </summary>
        public uint idSender;

        /// <summary>
        /// SE-Star identifier of the receiver of the message.
        /// </summary>
        public uint idReceiver; 

        /// <summary>
        /// Parameters of the message (16 + 8 to be able to support longer message).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public MessageParameter[] parameters;

        /// <summary>
        /// Constructor of the class. 
        /// First overload.
        /// </summary>
        /// <param name="_type">Type of the message.</param>
        /// <param name="_subType">Subtype of the message.</param>
        /// <param name="_sender">Int that identifies the sender.</param>
        /// <param name="_receiver">Int that identifies the receiver.</param>
        /// <param name="_parameters">Parameters of the message.</param>
        public MessageSEStar(MessageTypes _type, MessageTypes _subType, uint _sender, uint _receiver, MessageParameter[] _parameters)
        {
            type = _type;
            _subType = subtype;
            idSender = _sender;
            idReceiver = _receiver;
            parameters = _parameters;
            size = 112;
        }

        /// <summary>
        /// Constructor of the class. 
        /// Second overload.
        /// </summary>
        public MessageSEStar()
        {
            size = 112;
        }
    }
}
