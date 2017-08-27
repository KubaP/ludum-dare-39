using UnityEngine;

public class Tile : MonoBehaviour {

    public GameObject building;
    public int xPos;
    public int yPos;

    public enum types { Normal, Grass, Green_Band};
    public types slotType;

	// Use this for initialization
	void Start () {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        int type = Random.Range(1, 11);

        if (type <= 7) {
            slotType = types.Normal;
            renderer.material.color = new Color(0.2f, 0.2f, 0.2f);
        } if (type > 7 && type < 10) {
            slotType = types.Grass;
            renderer.material.color = new Color(0.41f, 0.62f, 0.24f);
        } else if (type == 10) {
            slotType = types.Green_Band;
            int bandModel = (int)Random.Range(1, 4);
            building = GameObject.Instantiate(Resources.Load("Prefabs/Building_Green_0" + bandModel) as GameObject, this.transform.position, Quaternion.Euler(-90f, Random.Range(0,4) * 90f, 0f), gameObject.transform);
            building.transform.localScale = Vector3.one;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
	}
	
    public Vector2 SetBuilding (string buildingName, BuildingManager bMan)
    {
        if (building == null) {
            bool placed = false;
            building = GameObject.Instantiate(Resources.Load("Prefabs/" + buildingName) as GameObject, this.transform.position, Quaternion.Euler(0f, (int) Random.Range(-2, 2) * 90f, 0f), gameObject.transform);
            building.transform.localScale = Vector3.one * 0.5f;
            placed = building.GetComponent<Building>().Create(this.GetComponent<MeshRenderer> ());
            if (placed) {
                bMan.AddToBuildingArray(xPos, yPos, building.GetComponent<Building>());
            }
        }

        return new Vector2(xPos, yPos);

    }

    public Vector2 SetSpecial (string buildingName, BuildingManager bMan)
    {
        bool placed = false;

        building = GameObject.Instantiate(Resources.Load("Prefabs/" + buildingName) as GameObject, this.transform.position, Quaternion.Euler(0f, (int)Random.Range(-2, 2) * 90f, 0f), gameObject.transform);
        building.transform.localScale = Vector3.one * 0.5f;
        placed = building.GetComponent<BuildingSpecial>().Create(this.GetComponent<MeshRenderer>());

        if (placed) {
            bMan.AddToBuildingArray(xPos, yPos, building.GetComponent<BuildingSpecial>());
        }
       
        return new Vector2(xPos, yPos);
    }

    public bool SetBuilding()
    {
        if (slotType != types.Green_Band) {
            building = GameObject.Instantiate(Resources.Load("Prefabs/Building_Office_Headquarters") as GameObject, this.transform.position, Quaternion.Euler(0f, (int)Random.Range(-2, 2) * 90f, 0f), gameObject.transform);
            building.transform.localScale = Vector3.one * 0.5f;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            return true;
        } else {
            return false;
        }
    }

    public types GetSlotType ()
    {
        return slotType;
    }

    void ShowBuildingUI()
    {

    }

}
