using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour {

    public Vector3 ground;

	// Use this for initialization
	void Start () {
        ground = GameObject.FindGameObjectWithTag("Grid").GetComponent<Transform> ().position;
        ground.y += 36f;
	}

    // Update is called once per frame
    void Update() {
        this.transform.Rotate(Vector3.up, 5f * Time.deltaTime);
    }
}
