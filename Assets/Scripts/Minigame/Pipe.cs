using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

	public Vector2 postion;
	public List<Pipe> neighbours = new List<Pipe>();
	public Node node = new Node();

	public bool hasPower = false;

	void Update(){
		if (node.parent != null) {
			hasPower = true;
		}else{
			hasPower = false;
		}


		Material[] mats = GetComponent<Renderer> ().materials;


		if(hasPower == true){
			mats [0] = MinigameManager.Instance.poweredMaterial;
		}else{
			mats [0] = MinigameManager.Instance.unpoweredMaterial;
		}


		GetComponent<Renderer> ().materials = mats;

	}

	public void AddConnectingNeighbour(GameObject pipe){
		neighbours.Add (pipe.GetComponent<Pipe> ());
	}

	public void RemoveConnectingNeighbour(GameObject pipe){
		neighbours.Remove (pipe.GetComponent<Pipe> ());
	}



}
