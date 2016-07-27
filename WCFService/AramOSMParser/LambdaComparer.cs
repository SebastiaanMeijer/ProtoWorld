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

namespace Aram.OSMParser
{
    /// <summary>
    /// A generic implementation of <see cref="IEqualityComparer&lt;T>"/>.
    /// </summary>
    /// <typeparam name="T">The target type for comparison.</typeparam>
	public class LambdaComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T, T, bool> _lambdaComparer;
		private readonly Func<T, int> _lambdaHash;
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="lambdaComparer">The comparer method to use</param>
		public LambdaComparer(Func<T, T, bool> lambdaComparer) :
			this(lambdaComparer, o => 0)
		{
		}
        /// <summary>
        /// The constructor of LambdaComparer.
        /// </summary>
        /// <param name="lambdaComparer">The function that performs the comparison.</param>
        /// <param name="lambdaHash">The hash function that returns a unique value for different elements.</param>
		public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
		{
			if (lambdaComparer == null)
				throw new ArgumentNullException("lambdaComparer");
			if (lambdaHash == null)
				throw new ArgumentNullException("lambdaHash");

			_lambdaComparer = lambdaComparer;
			_lambdaHash = lambdaHash;
		}
        /// <summary>
        /// Checks whether two objects of type T are equal, given the lambdaComparer method.
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns>Returns true if the conditions are met, false otherwise.</returns>
		public bool Equals(T x, T y)
		{
			return _lambdaComparer(x, y);
		}
        /// <summary>
        /// Gets the hash code of an object, given the lambdaHash function.
        /// <remarks>The LambdaHash function by default returns 0 for all values. It is up to you to implement a hash method.</remarks>
        /// </summary>
        /// <param name="obj">The object to get the hash for.</param>
        /// <returns>Returns an integer representing the hash.</returns>
		public int GetHashCode(T obj)
		{
			return _lambdaHash(obj);
		}
	}
}
