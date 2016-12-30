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
		cleanup ();
		List<Loggable> objects = new List<Loggable> (subscribedObjects);
		return objects;
	}

	private static void cleanup(){
		lock (subscribedObjects) {
			subscribedObjects.RemoveAll (item => item == null);
		}
	}

}