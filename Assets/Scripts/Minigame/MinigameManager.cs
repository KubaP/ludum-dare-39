using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour {

	public List<Camera> miniCameras = new List<Camera>();
	public Camera miniCamera;
	public Camera mainCamera;
	float mainCameraZoom;
	public Image fadeImage;
	public Slider timeLeft;
	public Canvas mainCanvas;
	public Canvas miniGameCanvas;
	public RectTransform popup;
	public GameObject highlightBox;
	public BuildingManager buildingManager;

	public List<GameObject> pieces = new List<GameObject>();

	public Pipe[,] map = new Pipe[12, 10];

	public Material poweredMaterial;
	public Material unpoweredMaterial;
	public GameObject endCube;

	bool foundPath = false;

	bool isRunning = false;
	bool isPlaying = false;
	int numberCompleted = 0;

	UnityEvent resetParents = new UnityEvent();

	private static MinigameManager instance = null;

	public static MinigameManager Instance{
		get{ return instance;}
	}

	void Awake(){
		if(instance != null && instance != this){
			Destroy (this.gameObject);

		}

		instance = this;
	}


	IEnumerator TransitionToMinigame(){

		int rnd = UnityEngine.Random.Range (0, 3);
		miniCamera = miniCameras [rnd];

		yield return new WaitForSeconds (0.5f);

		Color currentColor = fadeImage.color;
		Color targetColor = new Color(0,0,0,1);

		while(fadeImage.color.a < 0.95){
			fadeImage.color = Color.Lerp(fadeImage.color, targetColor, Time.deltaTime * 2f);
			yield return null;
		}

		float time = 0;
		LoadLevelFromFile (out time);

		time = time * (1 - (numberCompleted * 0.05f));
		time = Mathf.Clamp (time, 10f, 100f);

		mainCamera.gameObject.SetActive (false);
		mainCanvas.gameObject.SetActive (false);
		miniGameCanvas.gameObject.SetActive (true);
		miniCamera.gameObject.SetActive (true);
		highlightBox.SetActive (false);
		popup.gameObject.SetActive (false);
		timeLeft.value = 1;
		buildingManager.inMinigame = true;

		currentColor = fadeImage.color;
		targetColor = new Color(0, 0, 0, 0);

		while(fadeImage.color.a > 0.1){
			fadeImage.color = Color.Lerp(fadeImage.color, targetColor, Time.deltaTime * 1.5f);
			yield return null;
		}

		fadeImage.color = new Color (0, 0, 0, 0);

		foundPath = false;
		isRunning = true;

		yield return new WaitForSeconds (1f);

		StartCoroutine (TimeCountdown (time));

	}

	IEnumerator TransitionToGame(){

		yield return new WaitForSeconds (2f);

		Color currentColor = fadeImage.color;
		Color targetColor = new Color(0,0,0,1);

		while(fadeImage.color.a < 0.95){
			fadeImage.color = Color.Lerp(fadeImage.color, targetColor, Time.deltaTime * 2f);
			yield return null;
		}

		mainCamera.gameObject.SetActive (true);
		mainCanvas.gameObject.SetActive (true);
		miniGameCanvas.gameObject.SetActive (false);
		miniCamera.gameObject.SetActive (false);
		buildingManager.inMinigame = false;
		popup.gameObject.transform.GetChild (0).gameObject.GetComponent<Button> ().interactable = true;
		isRunning = false;
		foundPath = false;
		isPlaying = false;

		Material[] mats = endCube.GetComponent<Renderer> ().materials;
		mats [0] = unpoweredMaterial;
		endCube.GetComponent<Renderer> ().materials = mats;

		currentColor = fadeImage.color;
		targetColor = new Color(0, 0, 0, 0);

		while(fadeImage.color.a > 0.1){
			fadeImage.color = Color.Lerp(fadeImage.color, targetColor, Time.deltaTime * 1.5f);
			yield return null;
		}

		fadeImage.color = new Color (0, 0, 0, 0);

		foreach(Transform child in this.transform){
			if(child.tag == "Pipe"){
				Destroy (child.gameObject);
			}
		}

	}

	IEnumerator TimeCountdown(float totalTime){

		while(timeLeft.value > 0 && foundPath != true){
			timeLeft.value -= (1 / totalTime) * Time.deltaTime;
			yield return null;
		}

		if(foundPath != true){
			Debug.Log ("youve failed");
			isRunning = false;

			//drain battery by x percent
			StartCoroutine (TransitionToGame ());
			StartCoroutine (GenerateNewMission (20f));

			yield return new WaitForSeconds (5f);
			StartCoroutine (DrainBattery (500, 2));
		}
	}

	IEnumerator DrainBattery(float amount, float time){

		for (float i = 0; i < time; i+=Time.deltaTime) {
			buildingManager.DrainPerSecond (amount);

			yield return null;
		}

	}

	IEnumerator GenerateNewMission(float minWaitTime){

		yield return new WaitForSeconds (minWaitTime);

		float waitTime = UnityEngine.Random.Range (1f, 5f);

		yield return new WaitForSeconds (waitTime);


		GameObject[] buildings = GameObject.FindGameObjectsWithTag ("Building");
		int rnd = (int)UnityEngine.Random.Range (0, buildings.Length - 1);
		Vector3 pos = buildings [rnd].transform.position;

		highlightBox.SetActive (true);
		highlightBox.transform.position = new Vector3 (pos.x, 2f, pos.z);
		popup.gameObject.SetActive (true);

		yield return new WaitForSeconds (10f);

		if(isRunning == false && foundPath != true && isPlaying != true){
			Debug.Log ("not accepted mission, failed");

			StartCoroutine (DrainBattery (500,4));

			highlightBox.SetActive (false);
			popup.gameObject.SetActive (false);

			StartCoroutine (GenerateNewMission (10f));
		}


	}

	


	public void AcceptMission(){
		isPlaying = true;
		numberCompleted++;
		popup.gameObject.transform.GetChild (0).gameObject.GetComponent<Button> ().interactable = false;
		StartCoroutine (TransitionToMinigame ());
	}


	void Start(){

		StartCoroutine (GenerateNewMission (7f));

	}

	void LoadLevelFromFile(out float levelTime){
		levelTime = 10;

		int z = UnityEngine.Random.Range (0, 3);
		string levelName = "level" + z.ToString();	

		Debug.Log ("minigame level: " + z.ToString ());
		switch (z){
		case 0:
			levelTime = 30f;
			break;
		case 1:
			levelTime = 30f;
			break;
		case 2:
			levelTime = 30f;
			break;
		case 3:
			levelTime = 30f;
			break;
		}
		

		TextAsset text = (TextAsset)Resources.Load (levelName, typeof(TextAsset));

		//String input = File.ReadAllText( @"c:\myfile.txt" );
		string[] data = text.text.Split ("\n" [0]);
		int[,] dataMap = new int[12, 10];
	
		for(int i = 0; i < data.Length-1;i++){
			int j = 0;
			foreach(char c in data[i]){
				if(c == 's'){
					dataMap[i,j] = 2;
				}else if(c == 'l'){
					dataMap[i,j]=0;
				}else if(c == 't'){
					dataMap[i,j] = 1;
				}else if(c == 'c'){
					dataMap[i,j] = 3;
				}else if(c == ' '){
					dataMap[i,j] = 100;
				}

				j++;
			}
		}

		GenerateMap (dataMap);
	}

	public void GenerateMap(int[,] dataMap){
		for (int x = 0; x < 12; x++) {
			for (int y = 0; y < 10; y++) {
				/*if (x == 0) {
					if (y != 4) {
						continue;
					}
					GameObject obj = GameObject.Instantiate (pieces [2], new Vector3 (x * 2, -24, y * 2), Quaternion.Euler (new Vector3 (0, -90, 90)), this.transform);
					Pipe p = obj.GetComponent<Pipe> ();
					p.postion = new Vector2 (x, y);
					map [x, y] = p;
					p.node.Initialise (p.transform.position, p);
				}else if(x == 11){
					if (y !=4){
						continue;
					}
					GameObject obj = GameObject.Instantiate (pieces [2], new Vector3 (x * 2, -24, y * 2), Quaternion.Euler (new Vector3 (0, -90, 90)), this.transform);
					Pipe p = obj.GetComponent<Pipe> ();
					p.postion = new Vector2 (x, y);
					map [x, y] = p;
					p.node.Initialise (p.transform.position, p);
				}else{
					float rnd = UnityEngine.Random.Range (0.0f, 1.0f);
					if((x==10 && y==4) || (x == 1 && y==4)){
						rnd = UnityEngine.Random.Range (0.0f, 0.90f);
					}
					int type;
					if(0f <= rnd && rnd < 0.35f){
						type = 0;
					}else if(0.35f <= rnd && rnd < 0.60f){
						type = 1;
					}else if(0.60f <= rnd && rnd < 0.80f){
						type = 2;
					}else if(0.80f <= rnd && rnd <= 0.90f){
						type = 3;
					}else{
						type = 100;
					}

					if (type != 100) {
						GameObject obj = GameObject.Instantiate (pieces [type], new Vector3 (x * 2, -24, y * 2), Quaternion.Euler (new Vector3 (0, -90, 90)), this.transform);
						Pipe p = obj.GetComponent<Pipe> ();
						p.postion = new Vector2 (x, y);
						map [x, y] = p;
						p.node.Initialise (p.transform.position, p);
						resetParents.AddListener(p.node.ResetParent);
					}
				}*/

				if(dataMap[x,y] == 100){
					continue;
				}

				GameObject obj = GameObject.Instantiate (pieces [dataMap[x,y]], new Vector3 (x * 2, 2, y * 2) + transform.position, Quaternion.Euler (new Vector3 (0, -90, 90)), this.transform);
				Pipe p = obj.GetComponent<Pipe> ();
				p.postion = new Vector2 (x, y);
				map [x, y] = p;
				p.node.Initialise (p.transform.position, p);
				resetParents.AddListener (p.node.ResetParent);

			}
		}

	}

	void Update(){

		Vector2 viewportPos = mainCamera.WorldToViewportPoint (highlightBox.transform.position);
		Vector2 popupPos = new Vector2 ((viewportPos.x * mainCanvas.GetComponent<RectTransform>().sizeDelta.x) - (mainCanvas.GetComponent<RectTransform>().sizeDelta.x*0.5f),
			(viewportPos.y * mainCanvas.GetComponent<RectTransform>().sizeDelta.y) - (mainCanvas.GetComponent<RectTransform>().sizeDelta.y*0.5f));
		popup.anchoredPosition = popupPos;


		if(isRunning == false){			
			return;
		}
		
		if(foundPath == true){
			StartCoroutine (TransitionToGame ());
			isRunning = false;
			StartCoroutine (GenerateNewMission (10f));
			foundPath = false;
		}

		FindPath(map[0,4].node, map[11,4].node);

		if(Input.GetMouseButtonDown(0)){
			Ray ray = miniCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, 1000f)){
				GameObject obj = hit.collider.gameObject;

				if(obj.tag == "Pipe"){
					obj.transform.RotateAround (obj.transform.position, Vector3.up, 90);
				}
			}
		}





	}

	#region "Pathfinding"

	//the actual a* pathfinding algorithm
	//the startNode and endNode are two nodes of one wall which arent connected together forcing the pathfinding to make a 'u'-shaped path
	//for documentation on this algorithm, find a tutorial on youtube
	void FindPath(Node startNode, Node endNode) {
		resetParents.Invoke ();
		startNode.pipe.hasPower = true;

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();


		openSet.Add(startNode);

		Node currentNode = null;
		while(openSet.Count > 0) {
			currentNode = openSet[0];

			for(int i = 1; i < openSet.Count; i++) {
				if(openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)) {
					currentNode = openSet[i];
				}
			}

			openSet.Remove(currentNode);
			closedSet.Add(currentNode);

			if(currentNode.worldPosition == endNode.worldPosition) {
				RetracePath(startNode, currentNode, endNode);
				return;
			}

			List<Node> neighbours = new List<Node>(GetNeighbours(currentNode, closedSet));
			foreach(Node neighbour in neighbours) {
				if(closedSet.Contains(neighbour)) {
					continue;
				}

				float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

				if(newMovementCostToNeighbour < neighbour.gCost || openSet.Contains(neighbour) == false) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, endNode);
					neighbour.parent = currentNode;

					if(openSet.Contains(neighbour) == false) {
						openSet.Add(neighbour);
					}
				}

			}


		}

		if(currentNode != null){
			RetracePath(startNode, currentNode, endNode);
		}

	}

	//retraces the path back and creates a room
	void RetracePath(Node startNode, Node currentNode, Node endNode) {
		List<Node> path = new List<Node>();

		if(currentNode == endNode){
			Debug.Log ("path found");
			Material[] mats = endCube.GetComponent<Renderer> ().materials;
			mats [0] = poweredMaterial;
			endCube.GetComponent<Renderer> ().materials = mats;
			foundPath = true;
		}

		//populate the path with the nodes
		while(currentNode != startNode) {
			path.Add(currentNode);
			currentNode.pipe.hasPower = true;
			currentNode = currentNode.parent;
		}

		path.Add (startNode);
		startNode.pipe.hasPower = true;
		//reverse path, although not strictly needed
		path.Reverse();

	}

	//returns the distance between two nodes
	float GetDistance(Node nodeA, Node nodeB) {
		return Vector3.Distance(nodeA.worldPosition, nodeB.worldPosition);
	}

	//returns a list of neighbours for a specific node
	List<Node> GetNeighbours(Node node, HashSet<Node> prevNodes) {
		Pipe currentPipe = node.pipe;

		Node prevNode;
		Pipe prevPipe;

		//gets the previous node/wall before the current node
		if(prevNodes.Count > 1) {
			prevNode = prevNodes.ElementAt(prevNodes.Count - 2);
			prevPipe = prevNode.pipe;
		} else {
			prevNode = null;
			prevPipe = null;
		}

		List<Node> neighbours = new List<Node>();

		foreach(Pipe pipe in currentPipe.neighbours){
			if(pipe == prevPipe){
				continue;
			}else{
				neighbours.Add (pipe.node);
			}
		}

		return neighbours;
	}




	#endregion

		

}
