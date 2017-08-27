using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public Vector3 worldPosition;
	public Pipe pipe;

	public float gCost;
	public float hCost;

	public Node parent;

	public void ResetParent(){
		parent = null;
	}

	public float fCost {
		get {
			return gCost + hCost;
		}
	}


	public void Initialise(Vector3 _position, Pipe _pipe) {
		pipe = _pipe;
		worldPosition = _position;
	}



}

