using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public static class LoggableManager {

	private static List<Loggable> subscribedObjects  = new List<Loggable> ();

	static LoggableManager(){}

	public static void subscribe(Loggable loggable){
		lock (subscribedObjects) {
			LoggableManager.subscribedObjects.Add (loggable);
		}
	}

	public static void unsubscribe(Loggable loggable){
		lock (subscribedObjects) {
			LoggableManager.subscribedObjects.Remove (loggable);
		}
	}

	public static List<Loggable> getCurrentSubscribedLoggables(){
        cleanup();
        List<Loggable> objects = new List<Loggable>(subscribedObjects);
        return objects;
    }

	private static void cleanup(){
		lock (subscribedObjects) {
			subscribedObjects.RemoveAll (item => item == null);
		}
	}

}