using UnityEngine;
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
