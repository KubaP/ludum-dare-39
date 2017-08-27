using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{

    public bool inMinigame = false;
    public bool gameOver = false;

    [Range(0, 25)]
    public int gridXSize = 4;
    [Range(0, 25)]
    public int gridYSize = 4;

    public Vector2 HQPos = new Vector2();

    Vector2 gridSize = Vector2.zero;
    [SerializeField]
    Building[,] buildingArray;
    BuildingSpecial[,] buildingSpecialArray;

    public Slider powerMeter;
    public Text moneyText;
    public Text moneyPS;
    public Text polPS;

    public Text gameOverMoney;
    public Text gameOverMoneyShadow;

    private int[] factoryCosts = { 10, 60, 260 };
    private int[] officeCosts = { 5, 45, 165 };
    private int[] specialCosts = { 150, 250, 90 };

    #region Panels


    public GameObject gameOverPanel;
    [Space]

    [Header("Building Info Panel")]
    public GameObject BuildingPanel;
    public Text buildingNameText;
    public Text incomeText;
    public Text pollutionText;
    public Text powerText;
    public Text statusText;
    public Text adjacencyText;
    public Text typeText;
    public Text tierText;
    public Text upgradeCostText;
    public Button upgradeButton;

    [Space]

    [Header("Building Special Info Panel")]
    public GameObject BuildingSpecialPanel;
    public Text buildingSpecialNameText;
    public Text buildingSpecialPowerRedText;
    public Text pollutionSpecialText;
    public Text powerSpecialText;
    public Text statusSpecialText;


    [Space]

    [Header("Slot Info Panel")]
    public GameObject slotPanel;
    public Text slotType;
    public Button[] factoryButtons;
    public Button[] officeButtons;
    public Button[] specialButtons;



    [Space]

    #endregion

    private Building activeBuilding;
    private BuildingSpecial activeBuildingSpecial;
    private Tile activeSlot;

    public CameraController cameraController;



    private float moneyPSVal;

    float pollutionRate;

    private float timeToUpdate = 0.5f;

    // Use this for initialization
    void Start()
    {
        LevelSize lSize = GameObject.FindGameObjectWithTag("LevelSize").GetComponent<LevelSize>();
        gridXSize = lSize.x - 1;
        gridYSize = lSize.y - 1;

        Company.ResetCompany();

        gameOver = false;
        gameOverPanel.SetActive(false);

        BuildingPanel.SetActive(false);
        slotPanel.SetActive(false);

        powerMeter.maxValue = (float)Company.powerLeft;
        powerMeter.value = (float)Company.powerLeft;

        buildingArray = new Building[gridXSize + 1, gridYSize + 1];
        buildingSpecialArray = new BuildingSpecial[gridXSize + 1, gridYSize + 1];
        GetSlots();
        PlaceGrids();
        PlaceCompanyOffice();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver) {
            pollutionRate = 0f;
            Company.pollution = 0f;
            moneyPSVal = 0f;

            CheckInput();
            UpdateBuildings();
            UpdateUI();

            Mathf.Clamp(Company.money, 0, Mathf.Infinity);

            if (Company.powerLeft <= 0) {
                gameOver = true;
            }

            } else {
                gameOverPanel.SetActive(true);
                gameOverMoney.text = "You made §" + (int) Company.totalMoney;
                gameOverMoneyShadow.text = "You made §" + (int) Company.totalMoney;
        }
    }

    void UpdateUI()
    {

        polPS.text = pollutionRate.ToString() + " units";
        moneyPS.text = "§" + (int) moneyPSVal;

        if (timeToUpdate <= 0f) {

            moneyText.text = "§" + (int) Company.money;
            timeToUpdate = 0.5f;
            UpdateSlotPanel();
        }
        timeToUpdate -= Time.deltaTime;


        powerMeter.value = (float)Company.powerLeft;
        UpdateBuildingPanel();
        UpdateBuildingSpecialPanel();

    }

    void UpdateBuildingPanel()
    {
        if (activeBuilding != null) {
            buildingNameText.text = activeBuilding.buildingName;
            incomeText.text = "§" + activeBuilding.moneyProduction;
            pollutionText.text = activeBuilding.pollutionRate + " pollution";
            powerText.text = activeBuilding.powerUsage + " power";
            statusText.text = activeBuilding.GetCurrentStatus();
            adjacencyText.text = (activeBuilding.adjacencyBonus * 100f) + "%";
            typeText.text = activeBuilding.GetType().ToString();
            tierText.text = activeBuilding.GetTier().ToString();
            upgradeCostText.text = activeBuilding.GetUpgradeCost();

            if (activeBuilding.GetStatusShort() == "Upgrading") {
                upgradeButton.enabled = false;
                upgradeButton.GetComponentInChildren<Text>().text = "Upgrading";
            } else {
                upgradeButton.enabled = true;
                upgradeButton.GetComponentInChildren<Text>().text = "Upgrade";
            }
        }
    }

    private void UpdateBuildingSpecialPanel()
    {
        if (activeBuildingSpecial != null) {
            buildingSpecialNameText.text = activeBuildingSpecial.buildingName;
            pollutionSpecialText.text = activeBuildingSpecial.pollutionRate + " pollution";
            powerSpecialText.text = activeBuildingSpecial.powerUsage + " power per second";
            statusSpecialText.text = activeBuildingSpecial.GetCurrentStatus();

            if (activeBuildingSpecial.powerReduction > 0) {
                buildingSpecialPowerRedText.text = activeBuildingSpecial.powerReduction + "% less power used";
            } else {
                buildingSpecialPowerRedText.text = (activeBuildingSpecial.marketingPower * 1000) + "% more profit";
            }

        }
    }

    void UpdateSlotPanel()
    {
        string buildingType = "";

        if (activeSlot != null) {
            slotType.text = activeSlot.GetSlotType().ToString();
            for (int i = 0; i < 3; i++) {
                factoryButtons[i].interactable = true;
                officeButtons[i].interactable = true;
                specialButtons[i].interactable = true;
            }
            for (int i = 0; i < 3; i++) {
                if (Company.money < factoryCosts[i]) { factoryButtons[i].interactable = false; }
                if (Company.money < officeCosts[i]) { officeButtons[i].interactable = false; }
                if (Company.money < specialCosts[i]) { specialButtons[i].interactable = false; }
            }
            switch (activeSlot.GetSlotType()) {
                case Tile.types.Green_Band:
                    for (int i = 0; i < 3; i++) {
                        factoryButtons[i].interactable = false;
                        officeButtons[i].interactable = false;
                        specialButtons[i].interactable = false;
                    }
                    break;
                case Tile.types.Grass:
                    for (int i = 0; i < 3; i++) {
                        factoryButtons[i].interactable = false;
                        officeButtons[i].interactable = false;
                    }
                    specialButtons[1].interactable = false;
                    specialButtons[2].interactable = false;
                    break;
            }
        }
    }

    void DebugLog()
    {
        Debug.Log("Click");
    }

    public void CreateBuilding(string type)
    {
        if (activeSlot != null) {
            Vector2 bPos = activeSlot.SetBuilding(type, this);
            CheckAdjacency((int)bPos.x, (int)bPos.y);
            activeBuilding = activeSlot.GetComponentInChildren<Building>();
            activeSlot = null;
            slotPanel.SetActive(false);
            BuildingPanel.SetActive(true);
            BuildingSpecialPanel.SetActive(false);
        }
    }

    public void CreateUpgradedBuilding(string type)
    {
        if (activeSlot != null) {

            if (type.Contains("Factory")) {
                Vector2 bPos = activeSlot.SetBuilding("Building_Factory_LowTech", this);
                CheckAdjacency((int)bPos.x, (int)bPos.y);
            } else if (type.Contains("Office")) {
                Vector2 bPos = activeSlot.SetBuilding("Building_Office_LowTech", this);
                CheckAdjacency((int)bPos.x, (int)bPos.y);
            }
            activeBuilding = activeSlot.GetComponentInChildren<Building>();
            switch (type) {
                case "Building_Factory_MedTech":
                    activeBuilding.UpgradeImmediate(Building.tiers.MediumTech);
                    break;
                case "Building_Factory_HighTech":
                    activeBuilding.UpgradeImmediate(Building.tiers.HighTech);
                    break;
                case "Building_Office_MedTech":
                    activeBuilding.UpgradeImmediate(Building.tiers.MediumTech);
                    break;
                case "Building_Office_HighTech":
                    activeBuilding.UpgradeImmediate(Building.tiers.HighTech);
                    break;
            }

            activeSlot = null;
            slotPanel.SetActive(false);
            BuildingPanel.SetActive(true);
            BuildingSpecialPanel.SetActive(false);

        }
    }

    public void CreateSpecial(int index)
    {
        if (activeSlot != null) {
            string[] specials = { "Building_Special_Marketing", "Building_Special_Solar", "Building_Special_Nuclear" };
            Vector2 bPos = activeSlot.SetSpecial(specials[index], this);
            activeBuildingSpecial = activeSlot.GetComponentInChildren<BuildingSpecial>();
            activeSlot = null;
            slotPanel.SetActive(false);
            BuildingPanel.SetActive(false);
            BuildingSpecialPanel.SetActive(true);
        }
    }

    void UpdateBuildings()
    {
        for (int x = 0; x < buildingArray.GetLength(0); x++) {
            for (int y = 0; y < buildingArray.GetLength(1); y++) {
                if (buildingArray[x, y] != null) {
                    buildingArray[x, y].CheckUsage();
                    moneyPSVal += buildingArray[x, y].GetMoney();
                    pollutionRate += buildingArray[x, y].GetPollution();
                    Company.pollution += pollutionRate * Time.deltaTime;
                }

            }
        }

        Company.totalMoney += moneyPSVal * Time.deltaTime;

        for (int x = 0; x < buildingSpecialArray.GetLength(0); x++) {
            for (int y = 0; y < buildingSpecialArray.GetLength(1); y++) {
                if (buildingSpecialArray[x, y] != null) {
                    buildingSpecialArray[x, y].CheckUsage();
                    pollutionRate += buildingSpecialArray[x, y].GetPollution();
                    Company.pollution += pollutionRate * Time.deltaTime;
                }
            }
        }
    }

    void CheckAdjacency(int y, int x)
    {
        Building activeBuilding = buildingArray[x, y];
        if (x != buildingArray.GetLength(0) - 1) {
            if (buildingArray[x + 1, y] != null) {
                if (buildingArray[x + 1, y].GetType() == activeBuilding.GetType())
                    buildingArray[x + 1, y].adjacencyBonus += activeBuilding.adjacencyFactor;
                activeBuilding.adjacencyBonus += buildingArray[x + 1, y].adjacencyFactor;
            }
        }
        if (x != 0) {
            if (buildingArray[x - 1, y] != null) {
                if (buildingArray[x - 1, y].GetType() == activeBuilding.GetType())
                    buildingArray[x - 1, y].adjacencyBonus += activeBuilding.adjacencyFactor;
                activeBuilding.adjacencyBonus += buildingArray[x - 1, y].adjacencyFactor;
            }
        }
        if (y != buildingArray.GetLength(1) - 1) {
            if (buildingArray[x, y + 1] != null) {
                if (buildingArray[x, y + 1].GetType() == activeBuilding.GetType())
                    buildingArray[x, y + 1].adjacencyBonus += activeBuilding.adjacencyFactor;
                activeBuilding.adjacencyBonus += buildingArray[x, y + 1].adjacencyFactor;
            }
        }
        if (y != 0) {
            if (buildingArray[x, y - 1] != null) {
                if (buildingArray[x, y - 1].GetType() == activeBuilding.GetType())
                    buildingArray[x, y - 1].adjacencyBonus += activeBuilding.adjacencyFactor;
                activeBuilding.adjacencyBonus += buildingArray[x, y - 1].adjacencyFactor;
            }
        }
    }

    void GetSlots()
    {
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");
        grid.transform.localScale = new Vector3(buildingArray.GetLength(0), buildingArray.Length, buildingArray.GetLength(1)) * 4f;
        grid.transform.position = Vector3.down * (buildingArray.Length / 2f) * 4f;
        gridSize = new Vector2(grid.transform.localScale.x / 4f, grid.transform.localScale.z / 4f);
        Debug.Log(gridSize);
    }

    void PlaceGrids()
    {
        float xPos = -(3f * gridSize.x) + gridSize.x + 2f;
        float yPos = (3f * gridSize.y) - gridSize.y - 2;

        GameObject TileContainer = new GameObject();
        TileContainer.name = "Tile Container";

        for (int x = 0; x < buildingArray.GetLength(1); x++) {
            for (int y = 0; y < buildingArray.GetLength(0); y++) {
                GameObject tile = GameObject.Instantiate(Resources.Load("Prefabs/Building Slot") as GameObject, new Vector3(xPos, 0.2f, yPos), Quaternion.Euler(-90f, 0f, 0f), TileContainer.transform);
                tile.GetComponent<Tile>().xPos = x;
                tile.GetComponent<Tile>().yPos = y;
                tile.layer = LayerMask.NameToLayer("Tiles");
                xPos += 4f;
            }
            xPos = -(3f * gridSize.x) + gridSize.x + 2f;
            yPos -= 4f;
        }
    }

    void PlaceCompanyOffice()
    {
        bool valid = false;
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        int tileLength = tiles.Length;

        if (tileLength % 2 == 1) {
            tileLength += 1;
        }
        Debug.Log(tileLength);
        do {
            if (tiles[(tileLength / 2)].GetComponent<Tile>().SetBuilding()) {
                valid = true;
            } else {
                tileLength += 1;
            }
        } while (valid == false);
    }

    void CheckInput()
    {
        if (!inMinigame) {
            if (Input.GetButtonDown("Fire1")) {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) == false) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f)) {
                        GameObject buildingObj = hit.transform.gameObject;

                        if (buildingObj.GetComponent<Building>() != null) {
                            activeSlot = null;
                            activeBuildingSpecial = null;
                            activeBuilding = buildingObj.GetComponent<Building>();
                            cameraController.ZoomInOn(hit.transform);
                            BuildingSpecialPanel.SetActive(false);
                            BuildingPanel.SetActive(true);
                            slotPanel.SetActive(false);
                        } else {
                            BuildingPanel.SetActive(false);
                            activeBuilding = null;
                        }

                        if (buildingObj.GetComponent<BuildingSpecial>() != null) {
                            activeBuilding = null;
                            activeSlot = null;
                            activeBuildingSpecial = buildingObj.GetComponent<BuildingSpecial>();
                            cameraController.ZoomInOn(hit.transform);
                            BuildingPanel.SetActive(false);
                            BuildingSpecialPanel.SetActive(true);
                            slotPanel.SetActive(false);
                        } else {
                            BuildingSpecialPanel.SetActive(false);
                            activeBuildingSpecial = null;
                        }

                        if (buildingObj.GetComponent<Tile>() != null) {
                            Debug.Log("Clicked tile");
                            activeBuilding = null;
                            activeBuildingSpecial = null;
                            activeSlot = buildingObj.GetComponent<Tile>();
                            cameraController.ZoomInOn(hit.transform);
                            if (activeSlot.building == null || activeSlot.slotType == Tile.types.Green_Band) {
                                slotPanel.SetActive(true);
                                BuildingPanel.SetActive(false);
                                BuildingSpecialPanel.SetActive(false);
                                UpdateSlotPanel();
                            }

                        } else {
                            slotPanel.SetActive(false);
                            activeSlot = null;
                        }

                    } else {
                        slotPanel.SetActive(false);
                        BuildingPanel.SetActive(false);
                        BuildingSpecialPanel.SetActive(false);
                        activeSlot = null;
                        activeBuilding = null;
                        activeBuildingSpecial = null;

                    }

                }
            }
            if (Input.GetButtonDown("Fire2")) {
                slotPanel.SetActive(false);
                BuildingPanel.SetActive(false);
                BuildingSpecialPanel.SetActive(false);
                cameraController.ZoomOut();

                activeSlot = null;
                activeBuildingSpecial = null;
                activeBuilding = null;

            } else if (Input.GetKeyDown(KeyCode.Menu)) {
                Company.money += 1000;
            } else if (Input.GetKeyDown(KeyCode.Home)) {
                Company.powerLeft += 1000;
            } else if (Input.GetKeyDown(KeyCode.Escape)) {
                slotPanel.SetActive(false);
                BuildingPanel.SetActive(false);
                BuildingSpecialPanel.SetActive(false);
                activeSlot = null;
                activeBuilding = null;
                activeBuildingSpecial = null;
            }
        }
    }

    public void DrainPerSecond(float amount)
    {
        Company.powerLeft -= amount * Time.deltaTime;
    }

    public void UpgradeActiveBuilding()
    {
        activeBuilding.GetComponent<Building>().Upgrade();
    }

    public void AddToBuildingArray(int x, int y, Building b)
    {
        buildingArray[y, x] = b;
    }

    public void AddToBuildingArray(int x, int y, BuildingSpecial b)
    {
        buildingSpecialArray[y, x] = b;
    }

}
