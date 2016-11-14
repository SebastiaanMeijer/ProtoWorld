using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NTree<T>
{
	public T data;
	private LinkedList<NTree<T>> children;

	public NTree(T data)
	{
		this.data = data;
		children = new LinkedList<NTree<T>>();
	}

	public NTree<T> AddChild(T data)
	{
		children.AddLast (new NTree<T> (data));
		return children.Last.Value;
	}

	public NTree<T> AddChild(NTree<T> data)
	{
		children.AddLast (data);
		return children.Last.Value;
	}

	public NTree<T> GetChild(int i)
	{
		foreach (NTree<T> n in children)
			if (--i == 0)
				return n;
		return null;
	}

	public LinkedList<NTree<T>> getChildren(){
		return children;
	}

}
