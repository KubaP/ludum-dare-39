using UnityEngine;

public static class Company {

    public static float powerLeft = 10000;
    public static float money = 20;
    public static float totalMoney;

    public static float pollution;
    public static float reputation = 1000f;
    public static float energySaved = 1f;
    public static float marketingPower = 1f;

    public static int[] factoryCount = new int[3];
    public static int[] officeCount = new int[3];
    public static int[] specialCount = new int[3];

    public static AnimationCurve costCurve = new AnimationCurve();

    public static void ResetCompany()
    {
        powerLeft = 10000;
        money = 20;
        totalMoney = 0;
        pollution = 0;
        reputation = 1000f;
        energySaved = 1f;
        marketingPower = 1;

        for (int i = 0; i < 3; i++) {
            factoryCount[i] = 0;
            officeCount[i] = 0;
            specialCount[i] = 0;
        }

    }
	
}
