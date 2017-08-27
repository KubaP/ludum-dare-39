using UnityEngine;

public class BuildingSpecial : MonoBehaviour
{

    public enum types { Office, Power};
    public enum states { Building, Working};

    public string buildingName;

    public states state;
    public types type;

    public float powerUsage;

    public float powerReduction;

    public float marketingPower;

    public float cost;

    public float pollutionRate;

    [SerializeField]
    private float buildTime;
    private float buildTimeLeft;

    public void Create(float pUsage, float mProduction, float bTime, float adjFactor, types t)
    {
        powerUsage = pUsage;
        buildTime = bTime;
        buildTimeLeft = bTime;
        state = states.Building;
        type = t;
    }

    public bool Create(MeshRenderer renderer)
    {
        if (Company.money >= cost) {
            state = states.Building;
            buildTimeLeft = buildTime;
            Company.money -= cost;
            Company.marketingPower += marketingPower;
            Company.energySaved += powerReduction;
            renderer.enabled = false;
            return true;
        } else {
            Destroy(gameObject);
            return false;
        }

    }

    new public string GetType()
    {
        return type.ToString();
    }

    public string GetUpgradeCost()
    {
        return "Fully Upgraded";
    }

    public string GetCurrentStatus()
    {
        if (state == states.Working) {
            return "Working";
        } else {
            return "Building - " + (int)buildTimeLeft + "s";
        }
    }

    public string GetStatusShort()
    {
        return state.ToString();
    }

    public float GetPollution()
    {
        if (state == states.Working) {
            return pollutionRate;
        }
        return 0;
    }

    public void CheckUsage()
    {
        if (state == states.Working) {
            Company.powerLeft -= (powerUsage * Time.deltaTime) / Company.energySaved;
        } else {
            buildTimeLeft -= Time.deltaTime;
            if (buildTimeLeft <= 0f) {
                state = states.Working;
                buildTimeLeft = 0f;
            }
        }
    }
}