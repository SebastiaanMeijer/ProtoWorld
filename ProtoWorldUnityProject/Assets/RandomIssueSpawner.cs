using UnityEngine;
using System.Collections;

public class RandomIssueSpawner : MonoBehaviour {

	public GameObject IssueObject;
	public Transform parentTransform;
	// Use this for initialization
	void Start () {
		StartCoroutine (IssueSpawnerByChance ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator IssueSpawnerByChance(){
		yield return new WaitForSeconds (10f);
		//if(Random.Range(0,100) > 80)
		Instantiate (IssueObject, new Vector3(Random.Range(0,-10000),2,Random.Range(0,10000)), Quaternion.identity,parentTransform);
		StartCoroutine (IssueSpawnerByChance ());
		}


	}

