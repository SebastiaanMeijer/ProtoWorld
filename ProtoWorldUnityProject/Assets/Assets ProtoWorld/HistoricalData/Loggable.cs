using UnityEngine;
using System.Collections.Generic;

public enum LogPriorities { Default, High, Critical};

public interface Loggable{
	LogDataTree getLogData();
	void rebuildFromLog(LogDataTree logData);
	LogPriorities getPriorityLevel();
}