using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpInitiator : MonoBehaviour {

    //Madness
    //Rainbow
    //Slicer

    public GameObject gameMech;
    public GameObject slime;
	public GameObject paddleH;
	public GameObject paddleV;
	public GameObject touchPads;
    public GameObject touchInput;
    public GameObject[] PUs = new GameObject[3];
    public Text[] numDisp = new Text[3];
    public int[] puCount = new int[3];
    private float[] puDuration = new float[3];
    private float[] puCooldownTime = new float[] 
    {
        10f, 12f, 12f
    };
    private string[] funcDisable = new string[]
    {
        "DisableMadnessMode", "DisableRainbowMode", "DisableSlicerMode" 
    };
    private string[] funcReactivate = new string[]
    {
        "ReactivatePowerUp1", "ReactivatePowerUp2", "ReactivatePowerUp3"
    };
    private float[,] lvlDurations = new float[,]
    {
        {3.5f, 4.25f, 5f, 6f, 7f, 8f, 9f, 10.5f, 12f, 13.5f, 15f },
        { 4f, 5f, 6f, 7f, 8f, 9f, 10.5f, 12.5f, 14f, 15.5f, 17f },
        {4f, 5f, 6f, 7f, 8f, 9f, 10.5f, 12.5f, 14f, 15.5f, 17f }
    };
    //Bools to activate PUs from editor
    //public bool madness, rainbow, slicer;

    public delegate void PowerUpInfo();
    public event PowerUpInfo PUMadnessStart;
    public event PowerUpInfo PUMadnessEnd;
    public event PowerUpInfo PURainbowStart;
    public event PowerUpInfo PURainbowEnd;
    public event PowerUpInfo PUSlicerStart;
    public event PowerUpInfo PUSlicerEnd;
    //public bool initiated;

    /*
    void FixedUpdate()
    {
        if (initiated)
        {
            initiated = false;
            InitiatePowerUp();
        }
    }
    */

    void Start()
    {
        touchInput.GetComponent<TouchInputAlt>().PUActivate += PowerUpInitiator_PUActivate;
        gameMech.GetComponent<GameMech>().gameEnded += PowerUpInitiator_gameEnded;
        gameMech.GetComponent<GameMech>().gameStarted += PowerUpInitiator_gameStarted;
    }

    private void PowerUpInitiator_gameStarted()
    {
        FetchGameData();
    }

    private void PowerUpInitiator_gameEnded()
    {
        DisableMadnessMode();
        DisableRainbowMode();
        DisableSlicerMode();
        for (int i = 0; i < 3; i++)
        {
            GameData.current.powerupCount[i] = puCount[i];
        }
        ReactivatePowerUp1();
        ReactivatePowerUp2();
        ReactivatePowerUp3();
    }

    //void FixedUpdate()
    //{
    //    if (madness)
    //    {
    //        madness = false;
    //        PowerUpInitiator_PUActivate(1);
    //    }
    //    if (rainbow)
    //    {
    //        rainbow = false;
    //        PowerUpInitiator_PUActivate(2);
    //    }
    //    if (slicer)
    //    {
    //        slicer = false;
    //        PowerUpInitiator_PUActivate(3);
    //    }
    //}

    private void FetchGameData()
    {
        for(int i = 0; i < puDuration.Length; i++)
        {
            int currentlvl = GameData.current.powerupLvls[i];
            puDuration[i] = lvlDurations[i, currentlvl - 1];
            puCount[i] = GameData.current.powerupCount[i];
            DisplayPUCount(i);
        }
    }

    private void DisplayPUCount(int index)
    {
        numDisp[index].text = puCount[index].ToString() + "x";
    }

    private void PowerUpInitiator_PUActivate(int PUnum)
    {
        int index = PUnum - 1;
        if (puCount[index] > 0)
        {
            PUs[index].GetComponent<CircleCollider2D>().enabled = false;
            PUs[index].transform.GetChild(0).gameObject.SetActive(true);
            puCount[index] -= 1;
            DisplayPUCount(index);
            if (PUnum == 1)
            {
                EnalbeMadnessMode();
            }
            else if (PUnum == 2)
            {
                EnalbeRainbowMode();
            }
            else
            {
                EnalbeSlicerMode();
            }
            Invoke(funcDisable[index], puDuration[index]);
            Invoke(funcReactivate[index], puCooldownTime[index]);
        }
    }

    private void ReactivatePowerUp(int puNum)
    {
        PUs[puNum].GetComponent<CircleCollider2D>().enabled = true;
        PUs[puNum].transform.GetChild(0).gameObject.SetActive(false);
    }

	void ReactivatePowerUp1()
	{
        ReactivatePowerUp(0);
    }

    void ReactivatePowerUp2()
    {
        ReactivatePowerUp(1);
    }

    void ReactivatePowerUp3()
    {
        ReactivatePowerUp(2);
    }

    void EnalbeMadnessMode()
    {
        if (PUMadnessStart != null)
        {
            PUMadnessStart();
        }
    }

    void EnalbeRainbowMode()
    {
        if (PURainbowStart != null)
        {
            PURainbowStart();
        }
    }

    void EnalbeSlicerMode()
    {
        if (PUSlicerStart != null)
        {
            PUSlicerStart();
        }
    }

    void DisableMadnessMode()
    {
        if (PUMadnessEnd != null)
        {

            PUMadnessEnd();
        }
    }

    void DisableRainbowMode()
    {
        if (PURainbowEnd != null)
        {
            PURainbowEnd();
        }
    }

    void DisableSlicerMode()
    {
        if (PUSlicerEnd != null)
        {
            PUSlicerEnd();
        }
    }

    private void DisplayMadnessCount()
    {
        DisplayPUCount(0);
    }
}
