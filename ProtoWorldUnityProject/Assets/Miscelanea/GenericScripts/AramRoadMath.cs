/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;

/// <summary>
/// A class for handling road geometry.
/// </summary>
public class AramRoadMath
{
	/// <summary>
	/// The center line of the road
	/// </summary>
    public LineMath Line;
	/// <summary>
	/// The two starting points
	/// </summary>
    public Vector3[] StartingPoints;
	/// <summary>
	/// The two ending points
	/// </summary>
    public Vector3[] EndingPoints;
	/// <summary>
	/// Starting point on the right. This is populated automaticly upon using UpdateLogic().
	/// </summary>
    public Vector3 stR;
	/// <summary>
    /// Starting point on the left. This is populated automaticly upon using UpdateLogic().
	/// </summary>
    public Vector3 stL;
	/// <summary>
    /// Ending point on the right. This is populated automaticly upon using UpdateLogic().
	/// </summary>
    public Vector3 enR;
	/// <summary>
    /// Ending point on the left. This is populated automaticly upon using UpdateLogic().
	/// </summary>
    public Vector3 enL;
    /// <summary>
    /// Creates the necessary matrices for road direction discovery and updates stR, stL, enR and enL fields.
    /// </summary>
    public void UpdateLogic()
    {
        matP1 = new float[]
            {
                1,Line.p1.x,Line.p1.z,
                1,StartingPoints[0].x,StartingPoints[0].z,     
                1,Line.p2.x,Line.p2.z
            };
        matP2 = new float[]
            {
                1,Line.p1.x,Line.p1.z,
                1,EndingPoints[0].x,EndingPoints[0].z,
                1,Line.p2.x,Line.p2.z
            };
        stR = matP1.Det() > 0 ? StartingPoints[0] : StartingPoints[1];
        stL = matP1.Det() > 0 ? StartingPoints[1] : StartingPoints[0];
        enR = matP2.Det() > 0 ? EndingPoints[0] : EndingPoints[1];
        enL = matP2.Det() > 0 ? EndingPoints[1] : EndingPoints[0];
    }
    /// <summary>
    /// Direction matrices
    /// </summary>
    private float[] matP1;
    /// <summary>
    /// Direction matrices
    /// </summary>
    private float[] matP2;

    //private LineMath _rightLine;
    //private LineMath _leftLine;
    /// <summary>
    /// The line to the right of the center line.
    /// </summary>
    public LineMath RightLine
    {
        get
        {
            return new LineMath(stR, enR);
        }
    }
    /// <summary>
    /// The line to the left of the center line.
    /// </summary>
    public LineMath LeftLine
    {
        get
        {
            return new LineMath(stL, enL);
        }
    }
}
