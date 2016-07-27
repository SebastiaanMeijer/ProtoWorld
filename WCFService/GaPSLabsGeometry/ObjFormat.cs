/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaPSLabs.Geometry
{
    public class ObjFormat
    {

        public static string GameObjectToString(GameObject go)
        {
            Mesh mesh = go.mesh;
            Material[] mats = new Material[] { go.material };

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(go.Name).Append("\n");
            foreach (Vector3 v in mesh.vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in mesh.normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector2 v in mesh.uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < 1/*m.subMeshCount*/; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].Name).Append("\n");
                sb.Append("usemap ").Append(mats[material].Name).Append("\n");

                int[] triangles = mesh.triangles;
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }

        public static void MeshToFile(GameObject go, string filename, bool SkipIfExists = true)
        {
            if (SkipIfExists & System.IO.File.Exists(filename))
                return;
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(GameObjectToString(go));
            }
        }
    }

}
