using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicCameraController : MonoBehaviour {



	public float maxZoom = 4f;
	public float minZoom = 8f;
	public float zoomSpeed = 2f;
	public float targetZoom;

	public float moveSpeed = 2f;
	public Vector2 targetMove;
	Vector2 prevMousePos;

	public Camera camera;


	void Start(){
		camera = GetComponent<Camera> ();
		targetZoom = camera.orthographicSize;

		prevMousePos = Input.mousePosition;
	}


	void Update(){

		if(camera.orthographicSize != targetZoom){
			camera.orthographicSize = Mathf.Lerp (camera.orthographicSize, targetZoom, 5f * Time.deltaTime);
		}


		if(Input.GetAxis("Mouse ScrollWheel") != 0){
			targetZoom += -Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed;

			targetZoom = Mathf.Clamp (targetZoom, maxZoom, minZoom);
		}

		if(Input.GetMouseButtonDown(2)){
			prevMousePos = Input.mousePosition;
		}

		if(Input.GetMouseButton(2)){	
			prevMousePos.x += (Input.mousePosition.x - prevMousePos.x)/7;
			prevMousePos.y += (Input.mousePosition.y - prevMousePos.y)/12;

			targetMove.x = Input.mousePosition.x - prevMousePos.x;
			targetMove.y = Input.mousePosition.y - prevMousePos.y;

		}

		if(Input.GetMouseButtonUp(2)){
			targetMove.x = 0;
			targetMove.y = 0;
		}


		if(targetMove.x != 0){
			transform.Translate (-targetMove.x * moveSpeed * Time.deltaTime, 0f,0f,Space.Self);

		}

		if(targetMove.y != 0){
			transform.Translate (-targetMove.y * transform.GetChild (0).transform.forward * moveSpeed * Time.deltaTime, Space.World);

		}

	}


}
