using System.Collections.Generic;

[System.Serializable]

public class Event {

    [System.Serializable]
    public struct Prerequisite
    {
        public string factor;
        public float value;
    }

    [System.Serializable]
    public enum types { EnergyChange, MoneyChange, ReputationChange };

    public string eventName;
    public string description;

    public types type;
    public float eventMagnitude;

    public List<Prerequisite> prerequisites = new List<Prerequisite>();

    public void DoOperation () {
        switch (type) {
            case types.EnergyChange:
                Company.powerLeft += eventMagnitude;
                break;
            case types.MoneyChange:
                Company.money += eventMagnitude;
                break;
            case types.ReputationChange:
                Company.reputation += eventMagnitude;
                break;
        }

    }

    public Event ReturnSelf ()
    {
        return this;
    }
}
