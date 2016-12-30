/* 

This file is part of ProtoWorld. 
	
ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer. 

*/

/*
 * Historical Data Module
 * 
 * Marten van Antwerpen
 */
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

}
