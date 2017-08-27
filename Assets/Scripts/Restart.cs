using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour {

	public void Reset(){
		SceneManager.LoadScene (1);

	}

	public void GoToMenu(){
		SceneManager.LoadScene (0);
	}



}
