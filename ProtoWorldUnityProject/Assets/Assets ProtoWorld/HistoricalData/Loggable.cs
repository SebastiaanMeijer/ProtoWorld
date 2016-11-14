using UnityEngine;
using System.Collections.Generic;

public interface Loggable{
	NTree<KeyValuePair<string,string>> getLogData();
	void rebuildFromLog(NTree<KeyValuePair<string,string>> logData);
	int getPriorityLevel();
}