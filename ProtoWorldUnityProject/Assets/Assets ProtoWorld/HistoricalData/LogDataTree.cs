using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogDataTree
{
	public string Key;
	public string Value;
	private LinkedList<LogDataTree> children;

	public LogDataTree(string key, string value)
	{
		this.Key = key;
		this.Value = value;
		children = new LinkedList<LogDataTree>();
	}

    public bool containsKey(string key)
    {
        if (this.Key.Equals(key))
        {
            return true;
        }
        else
        {
            foreach(LogDataTree child in children)
            {
                if (child.Key.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }
    }

	public LogDataTree AddChild(LogDataTree child)
	{
		children.AddLast (child);
		return children.Last.Value;
	}

	public LogDataTree GetChild(string childKey)
	{
		foreach (LogDataTree n in children)
			if (n.Key.Equals(childKey))
				return n;
		return null;
	}

	public LinkedList<LogDataTree> getChildren(){
		return children;
	}

	public List<LogDataTree> getChildren(string childrenKey){
		List<LogDataTree> items = new List<LogDataTree> ();
		foreach (LogDataTree n in children)
			if (n.Key.Equals (childrenKey))
				items.Add (n);
		return items;
	}

}
