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

namespace GaPSLabs.Geometry
{
    public class MapProperties
    {
        public string filename;
        public double minLat = 0;
        public double maxLat = 1;
        public double minLon = 0;
        public double maxLon = 1;
        public string Name;
        public float BuildingLineThickness = 1.2f;
        public float RoadLineThickness = 1.0f;
        // public bool CorrectAspectRatio = false;
        public float[] minMaxX = new float[2] { 0, 20000f };
        public float[] minMaxY = new float[2] { 0, 40000f };
        public Vector2 Scale = new Vector2() { x = 10, y = 10 };
        public Color BuildingColor;
        public Color LineColorStart;
        public Color LineColorEnd;
        public Vector3 MinPointOnMap;
        public Material BuildingMaterial;
        public Material RoadMaterial;
        public Material CycleWayMaterial;
        public Material FootWayMaterial;
        public Material RailWayMaterial;
        public Material StepsMaterial;
        public Material ResidentialMaterial;
        public Material ServiceMaterial;
        public Material AreaMaterial;
        public Material WaterMaterial;
        public float RoadWidth = 0.1f;
        public float CyclewayWidth = 0.05f;
        public float FootwayWidth = 0.025f;
        public float BuildingHeight = 7.5f;
        public DiscreetCurve BuildingHeightVariation;
        public Vector2 CombinationOptimizationSize = new Vector2(100, 100);
        public bool OverrideDatabaseConnection = false;
        public string OverridenConnectionString;
        public double[] GetBoundsMinMaxLatMinMaxLon()
        {
            return new double[] { this.minLat, this.maxLat, this.minLon, this.maxLon };
        }
   
    }

}
