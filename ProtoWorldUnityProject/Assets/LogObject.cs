using UnityEngine;
using System.Collections.Generic;

interface LogObject{
	Dictionary<string, Dictionary<string, string>> getLogData();
	void rebuildFromLog(Dictionary<string, Dictionary<string, string>> logData);
}
