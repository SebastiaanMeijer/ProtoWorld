/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class that implements a pooling system. This can be used to cache any object such as game objects.
/// </summary>
/// <typeparam name="T">The type of the pool object.</typeparam>
public class ObjectPool<T> : System.IDisposable
{
    public List<T> pool;
    /// <summary>
    /// The default constructor for the GameObjectPool
    /// </summary>
    public ObjectPool()
    { 
        pool = new List<T>();
    }
    public T this[int index]
    {
        get { return pool[index]; }
        set { pool[index] = value; }
    }
    /// <summary>
    /// Adds an item of type T to the pool.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Add(T item)
    {
        pool.Add(item);
    }
    /// <summary>
    /// Removes an object from the pool.
    /// </summary>
    /// <param name="item">The item to be removed from the pool.</param>
    public void Remove(T item)
    {
        pool.Remove(item);
    }
    /// <summary>
    /// Removes and returns an item from the pool.
    /// </summary>
    /// <returns>Returns an item if there are any available, otherwise returns the default items. <see cref="default(T)"/></returns>
    public T Pop()
    {
        if (pool.Count == 0)
            return default(T);
        else
        {
            var temp = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return temp;
        }
    }
    /// <summary>
    /// Removes all items from the pool.
    /// </summary>
    public void Clear()
    {
        pool.Clear();
    }
    /// <summary>
    /// Disposes the GameObjectPool. This will clear the pool but will not call the Dispose on the pool items.
    /// </summary>
    public void Dispose()
    {
        pool.Clear();
    }
    
}
