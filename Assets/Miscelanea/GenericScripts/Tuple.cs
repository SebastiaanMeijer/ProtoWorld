using UnityEngine;
using System.Collections;

/// <summary>
/// Class that defines a Tuple.
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public class Tuple<T1, T2>
{
    private T1 item1 { get; set; }
    private T2 item2 { get; set; }

    /// <summary>
    /// Constructor of the class.
    /// </summary>
    /// <param name="item1">Value of field item1.</param>
    /// <param name="item2">Value of field item2.</param>
    public Tuple(T1 item1, T2 item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }

    /// <summary>
    /// Get field item1.
    /// </summary>
    public T1 Item1
    {
        get { return item1; }
    }

    /// <summary>
    /// Get field item2.
    /// </summary>
    public T2 Item2
    {
        get { return item2; }
    }
}
