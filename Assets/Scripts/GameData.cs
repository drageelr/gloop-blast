using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameData {

    public static GameData current = new GameData();
    public bool firstTime = true;
    public bool[] lvlUnlocked = new bool[10];
    public int[] lvlStars = new int[10];
    public int[] lvlBestScore = new int[10];
    public int[] powerupLvls = new int[] 
    {
        1, 1, 1
    };
    public int[] powerupCount = new int[3];
    public int coins = 0;
}
