using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelSize : MonoBehaviour {

	public Text text1;
	public Text text2;

	public int x = 6;
	public int y = 6;


	void Start(){
		DontDestroyOnLoad (this.gameObject);
	}


	public void ChangeX(int amount){
		x = x + amount;

		x = Mathf.Clamp (x, 2, 9);

		text1.text = "Grid Size  " + x.ToString() + "  x  " + y.ToString();
		text2.text = "Grid Size  " + x.ToString() + "  x  " + y.ToString();
	}

	public void ChangeY(int amount){
		y = y + amount;

		y = Mathf.Clamp (y, 2, 9);

		text1.text = "Grid Size  " + x.ToString() + "  x  " + y.ToString();
		text2.text = "Grid Size  " + x.ToString() + "  x  " + y.ToString();
	}






}
