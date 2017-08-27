using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour {


	public GameObject cloud1;
	public GameObject cloud2;
	public GameObject cloud3;
	public GameObject cloud4;


	public BuildingManager buildingManager;

	public bool run = true;

	void Start(){

		if(run == false){
			return;
		}

		int x = buildingManager.gridXSize + 1;
		int y = buildingManager.gridYSize + 1;

		x *= 2;
		y *= 2;

		x += 29;
		y += 29;

		cloud1.transform.position = new Vector3 (cloud1.transform.position.x, cloud1.transform.position.y, y);
		cloud2.transform.position = new Vector3 (x, cloud2.transform.position.y, cloud2.transform.position.z);
		cloud3.transform.position = new Vector3 (cloud3.transform.position.x, cloud3.transform.position.y, -y);
		cloud4.transform.position = new Vector3 (-x, cloud4.transform.position.y, cloud4.transform.position.z);

	}



}
