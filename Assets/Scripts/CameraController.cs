using UnityEngine;

public class CameraController : MonoBehaviour {

    Vector3 targetPosition;
    Vector3 targetObject;
    Vector3 gridTransform;

	// Use this for initialization
	void Start () {
        gridTransform = GameObject.FindGameObjectWithTag("Grid").transform.position;
        gridTransform.y = 0;
        targetObject = gridTransform;
        targetPosition = new Vector3(0, 15, -23);

    }
	
	// Update is called once per frame
	void LateUpdate () { 
        //this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, 0.1f);
        //this.transform.LookAt(targetObject);
    }

    public void ZoomOut()
    {
        //targetObject = gridTransform;
        //targetPosition = new Vector3(0, 15, -23);
    }

    public void ZoomInOn(Transform target)
    {
        //targetObject = target.position;
        //targetPosition = this.transform.position += this.transform.forward * 10f;
    }

}
