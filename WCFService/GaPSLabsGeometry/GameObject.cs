/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace GaPSLabs.Geometry
{
    [ProtoContract]
    public class GameObject
    {
        public enum OSMType { Node, Line, Polygon, Relation };
        [ProtoMember(1)]
        public OSMType type;
        [ProtoMember(2)]
        public string Name;
        [ProtoMember(3)]
        public string tag;
        [ProtoMember(4)]
        public Vector3 position;
        [ProtoMember(5)]
        public Material material;
        [ProtoMember(6)]
        public Mesh mesh;
        public GameObject() { }
        public GameObject(string Name) 
        {
            this.Name = Name;
        }
        public GameObject(GameObject gameobject)
        {
            this.Name = gameobject.Name;
            this.type = gameobject.type;
            this.tag = gameobject.tag;
            this.position = gameobject.position;
            this.material = gameobject.material;
            this.mesh = gameobject.mesh;
        }
    }
}
