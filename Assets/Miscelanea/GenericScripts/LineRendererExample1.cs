using UnityEngine;
using System.Collections;

public class LineRendererExample1 : MonoBehaviour {
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 6;
    void Start() {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(0.2F, 0.2F);
        
		Vector3[] nodes= new Vector3[]{
			new Vector3(0,0,0),
			new Vector3(1,0,0),
			new Vector3(2,0,1),
			new Vector3(2.5f,0,2),
			new Vector3(3,0,2),
			new Vector3(3,0,3),
			new Vector3(4,0,4),
			new Vector3(5,0,5),
			new Vector3(-2,0,4),
			new Vector3(-3,0,3),
		};
		lengthOfLineRenderer= nodes.Length;
		lineRenderer.SetVertexCount(nodes.Length);
        int i = 0;
        while (i < lengthOfLineRenderer) {
            
            lineRenderer.SetPosition(i, nodes[i%nodes.Length]);
            i++;
        }
    }
    void Update() {
	
    }
}