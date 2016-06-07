using UnityEngine;
using UnityEditor;
using System.Collections;


public class ComponentLister : EditorWindow
{
    private Hashtable sets = new Hashtable();
    private Vector2 scrollPosition;



    //[MenuItem("Component/Component lister")]
    public static void Launch()
    {
        EditorWindow window = GetWindow(typeof(ComponentLister));
        window.Show();
    }



    public void UpdateList()
    {
        Object[] objects;

        sets.Clear();

        objects = FindObjectsOfType(typeof(Component));
        foreach (Component component in objects)
        {
            if (!sets.ContainsKey(component.GetType()))
            {
                sets[component.GetType()] = new ArrayList();
            }

            ((ArrayList)sets[component.GetType()]).Add(component.gameObject);
        }
    }



    public void OnGUI()
    {
        GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
        GUILayout.Label("Components in scene:");
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh"))
        {
            UpdateList();
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (System.Type type in sets.Keys)
        {
            GUILayout.Label(type.Name + ":");
            foreach (GameObject gameObject in (ArrayList)sets[type])
            {
                if (GUILayout.Button(gameObject.name))
                {
                    Selection.activeObject = gameObject;
                }
            }
        }

        GUILayout.EndScrollView();
    }
}
