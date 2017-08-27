using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingNames {

    private static string[] nameFirst = { "Seaway", "Flux", "Robinbridge", "Marsh", "Phoenix", "Sol", "Micro", "Apexi", "Mazewater", "Jupiter",
        "Herbstar", "Nymph", "Pixie", "Aura", "Plutronics", "Saturnworks", "Apollo", "Green", "Red", "Mars", "Munar", "Firehut",
        "Dragonking", "KubaP", "Whizz", "Titanium", "Sunwood", "Wondersys", "Monotronic", "Frostfire", "Star", "Imaga", "Clee", "Wavefruit",
        "Siren", "Shade", "Explorata", "Voidus", "Smilestar", "Rootair", "Kast", "Kuru", "Goza", "Tuturu", "Grizzly" };

    private static string[] officeLast = { " Design", " Arts", " Interactive", " Comms", " Security", " Corporation", " Limited", " Systems" };

    private static string[] factoryLast = { " Industries", " Fabrication", " Microsystems", " Comms", " Automobiles", " Heavy Industry", " Systems",
        " Foundry", " Engineering", " Manufacturing" };

    public static string GenerateName (bool factory)
    {
        if (factory == true) {
            return nameFirst[Random.Range(0, nameFirst.Length - 1)] + factoryLast[Random.Range(0, factoryLast.Length - 2)];
        } else {
            return nameFirst[Random.Range(0, nameFirst.Length - 1)] + officeLast[Random.Range(0, factoryLast.Length - 2)];
        }
    }

}
