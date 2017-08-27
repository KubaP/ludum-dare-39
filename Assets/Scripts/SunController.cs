using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {

    public Transform grid;
    public Material[] windowMaterial;

    [SerializeField]
    private float dayLength = 60;
    private float turnRate;

    private float timeToChange;

    private float emissionColour;
    private float targetEmission;

    private Color[] colour;

    enum time { Day, Night};
    time t = time.Day;

	// Use this for initialization
	void Start () {
        colour = new Color[windowMaterial.Length];
        for (int i = 0; i < windowMaterial.Length; i++) {
            colour[i] = windowMaterial[i].color;
            windowMaterial[i].SetColor("_EmissionColor", new Color(0f, 0f, 0f, 0f));
        }
        
        targetEmission = 0f;
        emissionColour = 0f;

        


        grid = GameObject.FindGameObjectWithTag("Grid").transform;
        turnRate = (360f / dayLength) / 2;
        Debug.Log("Turn rate " + turnRate);
        timeToChange = dayLength;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(grid.transform.position, turnRate * Time.deltaTime);
        timeToChange -= Time.deltaTime;

        if (timeToChange <= 0f) {
            if (t == time.Day) {
                t = time.Night;
                targetEmission = 1f;
            } else {
                t = time.Day;
                targetEmission = 0f;
            }
            timeToChange = dayLength;
        }

        if (emissionColour != targetEmission) {
            emissionColour = Mathf.Lerp(emissionColour, targetEmission, 0.5f);
            for (int i = 0; i < windowMaterial.Length; i++) {
                windowMaterial[i].SetColor("_EmissionColor", new Color(0.6f * emissionColour, 0.6f * emissionColour, 0.55f * emissionColour, emissionColour));
            }
        }

	}
}
