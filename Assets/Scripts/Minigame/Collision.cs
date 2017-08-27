using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {

	public List<GameObject> colliders = new List<GameObject>();

	Pipe parent;

	void Awake(){
		parent = transform.parent.gameObject.GetComponent<Pipe> ();
	}

	void OnTriggerEnter(Collider col){
		colliders.Add (col.gameObject);

		parent.AddConnectingNeighbour (col.gameObject.GetComponent<Collision>().parent.gameObject);
	}

	void OnTriggerExit(Collider col){
		colliders.Remove (col.gameObject);

		parent.RemoveConnectingNeighbour (col.gameObject.GetComponent<Collision>().parent.gameObject);
	}


}

