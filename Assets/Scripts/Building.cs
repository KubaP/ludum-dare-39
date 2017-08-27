using UnityEngine;

public class Building : MonoBehaviour {

    public enum types { Factory, Office };
    public enum states { Working, Upgrading }
    public enum tiers { LowTech, MediumTech, HighTech };

    public string buildingName;

    public float powerUsage;
    public float moneyProduction;

    public float[] costs = new float[3];

    public float adjacencyBonus = 1f;
    public float adjacencyFactor;

    public float pollutionRate;

    [SerializeField]
    private types type;
    [SerializeField]
    private states state;
    [SerializeField]
    private tiers tier;

    [SerializeField]
    private float buildTime;
    private float buildTimeLeft;

    public void Create(float pUsage, float mProduction, types buildingType, tiers buildingTier, float bTime, float adjFactor) {
        powerUsage = pUsage;
        moneyProduction = mProduction;
        tier = buildingTier;
        buildTime = bTime;
        buildTimeLeft = bTime;
        state = states.Upgrading;
        type = buildingType;
        adjacencyFactor = adjFactor;
    }

    public bool Create (MeshRenderer renderer) {
        if ( Company.money >=  costs[0]) {
            if (type == types.Factory) {
                buildingName = BuildingNames.GenerateName(true);
            } else if (type == types.Office) {
                buildingName = BuildingNames.GenerateName(false);
            }
            state = states.Upgrading;
            buildTimeLeft = buildTime * ((float)tier + 1f);
            Company.money -=  costs[0];
            renderer.enabled = false;
            return true;
        } else {
            Destroy(gameObject);
            return false;
        }

    }

    new public string GetType ()
    {
        return type.ToString();
    }

    public tiers GetTier()
    {
        return tier;
    }

    public string GetUpgradeCost()
    {
        if (tier == tiers.LowTech) {
            return "§" + costs[1].ToString();
        } else if (tier == tiers.MediumTech) {
            return "§" + costs[2].ToString();
        } else {
            return "Fully Upgraded";
        }
    }

    public string GetCurrentStatus ()
    {
        if (state == states.Working) {
            return "Working";
        } else if (state == states.Upgrading && tier == tiers.LowTech) {
            return "Building - " + (int) buildTimeLeft + "s";
        } else {
            return "Upgrading - " + (int) buildTimeLeft + "s";
        }
    }

    public string GetStatusShort ()
    {
        return state.ToString();
    }

    public void CheckUsage()
    {
        if (state == states.Working) {
            Company.money += moneyProduction * Time.deltaTime * Company.marketingPower * (adjacencyBonus+1f) * (Company.reputation / 1000f);
            Company.powerLeft -= (powerUsage * Time.deltaTime) / Company.energySaved;
        } else {
            buildTimeLeft -= Time.deltaTime;
            if (buildTimeLeft <= 0f) {
                state = states.Working;
                buildTimeLeft = 0f;
            }
        }
    }

    public float GetMoney ()
    {
        if (state == states.Working) {
            return moneyProduction * Company.marketingPower * (adjacencyBonus + 1f) * (Company.reputation/1000f);
        }
        return 0;
    }

    public float GetPollution ()
    {
        if (state == states.Working) {
            return pollutionRate;
        }
        return 0;
    }

    public void Upgrade()
    {
        if (state != states.Upgrading) {
            Debug.Log(state.ToString() + tier.ToString());
            if (Company.money >=  costs[1] && tier == tiers.LowTech && buildTimeLeft == 0) {
                tier += 1;
                state = states.Upgrading;
                buildTimeLeft = buildTime * ((float)tier + 1f);
                pollutionRate = pollutionRate / 2f;
                powerUsage *= 3f;
                Company.money -=  costs[1];
                moneyProduction *= 2f;
                if (type == types.Factory) {
                    DestroyChildren(this.transform);
                    Debug.Log("L -> M");
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Factory_MedTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
                if (type == types.Office) {
                    DestroyChildren(this.transform);
                    Debug.Log("L -> M");
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Office_MedTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
            }
            if (Company.money >=  costs[2] && tier == tiers.MediumTech && buildTimeLeft == 0) {
                tier += 1;
                state = states.Upgrading;
                buildTimeLeft = buildTime * ((float)tier + 1f);
                pollutionRate = pollutionRate / 2f;
                powerUsage *= 3f;
                Company.money -=  costs[1];
                moneyProduction *= 3f;
                if (type == types.Factory) {
                    DestroyChildren(this.transform);
                    Debug.Log("M -> H");
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Factory_HighTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
                if (type == types.Office) {
                    DestroyChildren(this.transform);
                    Debug.Log("L -> M");
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Office_HighTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
            }

        }
    }

    public void UpgradeImmediate(tiers t)
    {
        switch (t) {
            case tiers.MediumTech:
                tier += 1;
                state = states.Upgrading;
                buildTimeLeft = buildTime * ((float)tier + 1f);
                pollutionRate = pollutionRate / 2f;
                powerUsage *= 3f;
                moneyProduction *= 2f;
                if (type == types.Factory) {
                    DestroyChildren(this.transform);
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Factory_MedTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
                if (type == types.Office) {
                    DestroyChildren(this.transform);
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Office_MedTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
                break;
            case tiers.HighTech:
                tier += 2;
                state = states.Upgrading;
                buildTimeLeft = buildTime * ((float)tier + 2f);
                pollutionRate = pollutionRate / 4f;
                powerUsage *= 15f;
                Company.money -= costs[1];
                moneyProduction *= 10f;
                if (type == types.Factory) {
                    DestroyChildren(this.transform);
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Factory_HighTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
                if (type == types.Office) {
                    DestroyChildren(this.transform);
                    GameObject.Instantiate(Resources.Load("Prefabs/Building_Office_HighTech") as GameObject, this.transform.position, this.transform.rotation, this.transform);
                }
                break;
        }
    }

    void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent) {
            GameObject.Destroy(child.gameObject);
        }
    }

}