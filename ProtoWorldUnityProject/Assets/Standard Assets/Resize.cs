// System.Array.Resize doesn't work in JS in Unity 2.6 ("ref" has to be explicitly stated),
// so this C# class only exists to be called by JS scripts when Vector2 array resizing is needed.

public class Resize {
	public static void Array (ref UnityEngine.Vector2[] array, int newSize) {
		System.Array.Resize(ref array, newSize);
	}
}