using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Advertisements;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class GameMech : MonoBehaviour {

    public delegate void GameStartEnd();
    public event GameStartEnd gameEnded;
    public event GameStartEnd gameStarted;

    //soundTypes:
    //0--> Music
    //1--> Effects
    public delegate void AudioControl(int soundType, bool mutebool);
    public event AudioControl OnSoundDataChange;
    public AudioSource LevelSelectPositive;
    public AudioSource LevelSelectNegative;
    public GameObject jungleBGM;

    public int Level;
    public int lives;

	public GameObject[] enemyList;

	public GameObject[] padList = new GameObject[2];
	public GameObject[] tileColour = new GameObject[4];
	private Color[] tileSetColour = new Color[4];

	public GameObject gateSource;
	public GameObject currentGate;
	public GameObject cannonSource;
	public GameObject currentCannon;
	public GameObject slime;
	public GameObject pauseObj;
	public GameObject lvlClear;
    public GameObject lvlSelect;
    public GameObject livesObj;
    public GameObject mainscreen;
    public GameObject gameover;
    public GameObject pauseBtn;
    public GameObject settingsBtn;
    public GameObject touchpads;
    public GameObject tittleLogo;
    public GameObject mainBtns;
    public GameObject puPad;

	public bool startintro;
    public Slime slimescript;
    public TimerScript timerscript;

	private List<string> enemyType = new List<string>(); //Basically the name of the enemy
	private List<Color> enemyColour = new List<Color>();
	private List<int> enemyCount = new List<int>();

    private int totaltime;
    private int halfscoretime;
    private int[] minstarscore = new int[2];
    private int maxLevel = 9;
    private int stars;
    private Color gateColor;

    //public ADManager adManagerScript;
    //public bool triggerExtraLife;
    //public bool triggerOnNegativeShowAd;

    void Start()
    {
        LoadData();
        //GameData.current = new GameData(); //Delete this line later
        if (GameData.current.firstTime)
        {
            GameData.current.firstTime = false;
            GameData.current.lvlUnlocked[0] = true;
            GameData.current.powerupCount[0] = 3;
            GameData.current.powerupCount[1] = 3;
            GameData.current.powerupCount[2] = 3;
            SaveData();
        }
    }

    //void Update()
    //{
    //    if (triggerExtraLife)
    //    {
    //        triggerExtraLife = false;
    //        ExtraLife();
    //    }
    //    if (triggerOnNegativeShowAd)
    //    {
    //        triggerOnNegativeShowAd = false;
    //        OnNegativeShowAd();
    //    }
    //}


    void GameStart(){
        if (gameStarted!= null)
        {
            gameStarted();
        }
        jungleBGM.SetActive(true);
		LvlData ();
        lives = 3;
		EnemyPlacing ();
		TileColouring ();
		SpawnGate ();
		pauseObj.tag = "UnPaused";
        slimescript.scoreThisLvl = 0;
        slimescript.goldThisLvl = 0;
        slimescript.halfscore = false;
        timerscript.timeinseconds = totaltime;
        timerscript.yellowtime = halfscoretime;
		timerscript.gameObject.transform.parent.gameObject.SetActive (true);
        timerscript.run = true;
        livesObj.SetActive(true);
		ShowLives ();
        pauseBtn.SetActive(true);
        settingsBtn.SetActive(true);
		DeployCannon ();
        touchpads.SetActive(true);
    }

	void RestartGame(){
        RemoveGameAssestsForRestarting();
        GameStart ();
	}

    void UpdateLevelSelect()
    {
        for(int i = 0; i < GameData.current.lvlUnlocked.Length; i++)
        {
            string lvlnum = (i + 1).ToString();
            Transform lvlT = lvlSelect.transform.Find("Lv" + lvlnum);
            GameObject lvlLock = lvlT.Find("Lock").gameObject;
            if(GameData.current.lvlUnlocked[i] == true) //Level Unlocked:
            {
                lvlLock.SetActive(false);
                GameObject star1 = lvlT.Find("star1").gameObject;
                GameObject star2 = lvlT.Find("star2").gameObject;
                GameObject star3 = lvlT.Find("star3").gameObject;
                int stars = GameData.current.lvlStars[i];
                if (stars == 1)
                {
                    star1.SetActive(true);
                }
                else if(stars == 2)
                {
                    star1.SetActive(true);
                    star2.SetActive(true);
                }
                else if(stars == 3)
                {
                    star1.SetActive(true);
                    star2.SetActive(true);
                    star3.SetActive(true);
                }
                else
                {
                    star1.SetActive(false);
                    star2.SetActive(false);
                    star3.SetActive(false);
                }
            }
            else //Level Locked:
            {
                lvlLock.SetActive(true);
            }
        }
    }

	void LevelClear (){
        if(gameEnded!= null)
        {
            gameEnded();
        }
        jungleBGM.SetActive(false);
        timerscript.running = false;
        pauseObj.tag = "Paused";
        GameObject menuBtn = lvlClear.GetComponent<RectTransform>().Find("Menu_btn").gameObject;
        GameObject nextlvlBtn = lvlClear.GetComponent<RectTransform>().Find("NextLvl_btn").gameObject;
        GameObject replayBtn = lvlClear.GetComponent<RectTransform>().Find("Replay_btn").gameObject;
        menuBtn.SetActive(false);
        nextlvlBtn.SetActive(false);
        replayBtn.SetActive(false);
        pauseBtn.SetActive(false);
        settingsBtn.SetActive(false);
        touchpads.SetActive(false);

        ShowScore();
        ShowGold(ShowStars());
        UpdateGameData();
        SaveData();
        RemoveGameAssestsForRestarting();

        menuBtn.SetActive(true);
        nextlvlBtn.SetActive(true);
        replayBtn.SetActive(true);
    }

	void EnemyPlacing(){ //DO NOT DO ANY "PANGA" WITH THIS CODE
		for (int i = 0; i < enemyType.Count; i++) {
			List<Vector3> enemyPos = EnemyPos (enemyType[i], enemyColour[i]);
			int enemyposinList = -1;
			for (int e = 0; e < enemyList.Length; e++) {
				if (enemyposinList <= 0) {
					if (enemyList [e].name == enemyType [i]) {
						enemyposinList = e;
					}
				}
			}
			GameObject enemy = enemyList [enemyposinList];
			for (int q = 0; q < enemyCount [i]; q++) {
				GameObject enem = Instantiate (enemy);
				enem.transform.position = enemyPos [q];
				SpriteRenderer enemSR = enem.GetComponent<SpriteRenderer> ();
				enemSR.color = enemyColour [i];
			}
		}
	}

	void LvlData(){
        enemyType = new List<string>();
        enemyColour = new List<Color>();
        enemyCount = new List<int>();
        if (Level == 1)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(Color.red);
            enemyCount.Add(8);

            tileSetColour[0] = Color.red; // North
            tileSetColour[1] = Color.red; // South
            tileSetColour[2] = Color.red; // East
            tileSetColour[3] = Color.red; // West

            gateColor = Color.red;

            totaltime = 100;
            halfscoretime = 40;

            minstarscore[0] = 110;
            minstarscore[1] = 140;
        }
        else if (Level == 2)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(Color.yellow);
            enemyCount.Add(12);

            tileSetColour[0] = Color.yellow; // North
            tileSetColour[1] = Color.yellow; // South
            tileSetColour[2] = Color.yellow; // East
            tileSetColour[3] = Color.yellow; // West

            gateColor = Color.yellow;

            totaltime = 180;
            halfscoretime = 80;

            minstarscore[0] = 120;
            minstarscore[1] = 200;
        }
        else if (Level == 3)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(188, 0, 255, 255));
            enemyCount.Add(10);

            tileSetColour[0] = new Color32(188, 0, 255, 255); // North
            tileSetColour[1] = new Color32(188, 0, 255, 255); // South
            tileSetColour[2] = new Color32(188, 0, 255, 255); // East
            tileSetColour[3] = new Color32(188, 0, 255, 255); // West

            gateColor = new Color32(188, 0, 255, 255);

            totaltime = 150;
            halfscoretime = 60;

            minstarscore[0] = 100;
            minstarscore[1] = 180;
        }
        else if (Level == 4)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(0, 255, 255, 255));
            enemyCount.Add(10);

            tileSetColour[0] = new Color32(0, 255, 255, 255); // North
            tileSetColour[1] = new Color32(0, 255, 255, 255); // South
            tileSetColour[2] = new Color32(0, 255, 255, 255); // East
            tileSetColour[3] = new Color32(0, 255, 255, 255); // West

            gateColor = new Color32(0, 255, 255, 255);

            totaltime = 150;
            halfscoretime = 65;

            minstarscore[0] = 100;
            minstarscore[1] = 180;
        }
        else if (Level == 5)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(0, 255, 0, 255));
            enemyCount.Add(10);

            tileSetColour[0] = new Color32(0, 255, 0, 255); // North
            tileSetColour[1] = new Color32(0, 255, 0, 255); // South
            tileSetColour[2] = new Color32(0, 255, 0, 255); // East
            tileSetColour[3] = new Color32(0, 255, 0, 255); // West

            gateColor = new Color32(0, 255, 0, 255);

            totaltime = 150;
            halfscoretime = 65;

            minstarscore[0] = 100;
            minstarscore[1] = 180;
        }
        else if (Level == 6)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(255, 200, 0, 255));
            enemyCount.Add(5);

            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(255, 150, 0, 255));
            enemyCount.Add(5);

            tileSetColour[0] = new Color32(255, 200, 0, 255); // North
            tileSetColour[1] = new Color32(255, 200, 0, 255); // South
            tileSetColour[2] = new Color32(255, 150, 0, 255); // East
            tileSetColour[3] = new Color32(255, 150, 0, 255); // West

            gateColor = new Color32(255, 150, 0, 255);

            totaltime = 180;
            halfscoretime = 80;

            minstarscore[0] = 100;
            minstarscore[1] = 180;
        }
        else if (Level == 7)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(0, 179, 107, 255));
            enemyCount.Add(10);

            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(119, 179, 0, 255));
            enemyCount.Add(10);

            tileSetColour[0] = new Color32(0, 179, 107, 255); // North
            tileSetColour[1] = new Color32(0, 179, 107, 255); // South
            tileSetColour[2] = new Color32(119, 179, 0, 255); // East
            tileSetColour[3] = new Color32(119, 179, 0, 255); // West

            gateColor = new Color32(119, 179, 0, 255);

            totaltime = 260;
            halfscoretime = 130;

            minstarscore[0] = 200;
            minstarscore[1] = 350;
        }
        else if (Level == 8)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(255, 0, 0, 255));
            enemyCount.Add(8);

            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(255, 100, 0, 255));
            enemyCount.Add(8);

            tileSetColour[0] = new Color32(255, 0, 0, 255); // North
            tileSetColour[1] = new Color32(255, 0, 0, 255); // South
            tileSetColour[2] = new Color32(255, 100, 0, 255); // East
            tileSetColour[3] = new Color32(255, 100, 0, 255); // West

            gateColor = new Color32(255, 70, 0, 255);

            totaltime = 220;
            halfscoretime = 110;

            minstarscore[0] = 170;
            minstarscore[1] = 290;
        }
        else if (Level == 9)
        {
            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(179, 0, 89, 255));
            enemyCount.Add(9);

            enemyType.Add("BrickBat");
            enemyColour.Add(new Color32(153, 0, 0, 255));
            enemyCount.Add(9);

            tileSetColour[0] = new Color32(179, 0, 89, 255); // North
            tileSetColour[1] = new Color32(179, 0, 89, 255); // South
            tileSetColour[2] = new Color32(153, 0, 0, 255); // East
            tileSetColour[3] = new Color32(153, 0, 0, 255); // West

            gateColor = new Color32(153, 0, 0, 255);

            totaltime = 240;
            halfscoretime = 120;

            minstarscore[0] = 180;
            minstarscore[1] = 320;
        }
    }

	List<Vector3> EnemyPos(string Etype, Color Ecolor){ //Define for every level's type and colour enemy's position here
		List<Vector3> Epos = new List<Vector3>();
        //Mashallah! Nazar na lage. 
		if(Level == 1 && Etype == "BrickBat" && Ecolor == Color.red){
            Epos.AddRange(new Vector3[] { new Vector3(1.5f, -0.75f, 0f), new Vector3(0f, -0.75f, 0f), new Vector3(-1.5f, -0.75f, 0f), new Vector3(-1.5f, 0.75f, 0f), new Vector3(0f, 0.75f, 0f), new Vector3(1.5f, 0.75f, 0f), new Vector3(1.5f, 0f, 0f), new Vector3(-1.5f, 0f, 0f) });
		}
        else if(Level == 2 && Etype == "BrickBat" && Ecolor == Color.yellow)
        {
            Epos.AddRange(new Vector3[] { new Vector3(-1.5f, 0f, 0f), new Vector3(1.5f, 0f, 0f), new Vector3(1.5f, 0.75f, 0f), new Vector3(0f, 0.75f, 0f), new Vector3(-1.5f, 0.75f, 0f), new Vector3(-1.5f, -0.75f, 0f), new Vector3(0f, -0.75f, 0f), new Vector3(1.5f, -0.75f, 0f), new Vector3(0f, 1.5f, 0f), new Vector3(0f, -1.5f, 0f), new Vector3(-3f, 0f, 0f), new Vector3(3f, 0f, 0f) });
        }
        else if (Level == 3 && Etype == "BrickBat" && Ecolor == new Color32(188, 0, 255, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(-0.95f, 0.95f, 0f), new Vector3(0.95f, 0.95f, 0f), new Vector3(0.95f, -0.95f, 0f), new Vector3(-0.95f, -0.95f, 0f), new Vector3(0.95f, 0f, 0f), new Vector3(-0.95f, 0f, 0f), new Vector3(2.85f, 0f, 0f), new Vector3(-2.85f, 0f, 0f), new Vector3(0f, -1.9f, 0f), new Vector3(0f, 1.9f, 0f) });
        }
        else if (Level == 4 && Etype == "BrickBat" && Ecolor == new Color32(0, 255, 255, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(0f, 0.9f, 0f), new Vector3(0f, -0.9f, 0f), new Vector3(1.2f, 0f, 0f), new Vector3(-1.2f, 0f, 0f), new Vector3(-1.7f, 1.2f, 0f), new Vector3(-1.7f, -1.2f, 0f), new Vector3(1.7f, -1.2f, 0f), new Vector3(1.7f, 1.2f, 0f), new Vector3(0f, 1.9f, 0f), new Vector3(0f, -1.9f, 0f) });
        }
        else if (Level == 5 && Etype == "BrickBat" && Ecolor == new Color32(0, 255, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f), new Vector3(-1.5f, 0f, 0f), new Vector3(1.5f, 0f, 0f), new Vector3(1.9f, 1.2f, 0f), new Vector3(-1.9f, 1.2f, 0f), new Vector3(-1.9f, -1.2f, 0f), new Vector3(1.9f, -1.2f, 0f), new Vector3(3.4f, 0f, 0f), new Vector3(-3.4f, -0f, 0f), new Vector3(0f, 2f, 0f), new Vector3(0f, -2f, 0f) });
        }
        else if (Level == 6 && Etype == "BrickBat" && Ecolor == new Color32(255, 200, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(1f, -0.75f, 0f), new Vector3(-1f, 0.75f, 0f), new Vector3(1f, -1.5f, 0f), new Vector3(-1f, 1.5f, 0f), new Vector3(2f, 0f, 0f) });
        }
        else if (Level == 6 && Etype == "BrickBat" && Ecolor == new Color32(255, 150, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(-1f, -0.75f, 0f), new Vector3(1f, 0.75f, 0f), new Vector3(1f, 1.5f, 0f), new Vector3(-1f, -1.5f, 0f), new Vector3(-2f, 0f, 0f) });
        }
        else if (Level == 7 && Etype == "BrickBat" && Ecolor == new Color32(0, 179, 107, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(3.4f, 2.5f, 0f), new Vector3(-3.4f, 2.5f, 0f), new Vector3(1.7f, 2.5f, 0f), new Vector3(-1.7f, 2.5f, 0f), new Vector3(0f, 2.5f, 0f), new Vector3(1.7f, 1.5f, 0f), new Vector3(-1.7f, 1.5f, 0f), new Vector3(0f, 1.5f, 0f), new Vector3(0.75f, 0.75f, 0f), new Vector3(-0.75f, 0.75f, 0f) });
        }
        else if (Level == 7 && Etype == "BrickBat" && Ecolor == new Color32(119, 179, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(3.4f, -2.5f, 0f), new Vector3(-3.4f, -2.5f, 0f), new Vector3(1.7f, -2.5f, 0f), new Vector3(-1.7f, 2.5f, 0f), new Vector3(0f, -2.5f, 0f), new Vector3(1.7f, -1.5f, 0f), new Vector3(-1.7f, -1.5f, 0f), new Vector3(0f, -1.5f, 0f), new Vector3(0.75f, -0.75f, 0f), new Vector3(-0.75f, -0.75f, 0f) });
        }
        else if (Level == 8 && Etype == "BrickBat" && Ecolor == new Color32(255, 0, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(3f, 0.5f, 0f), new Vector3(-3f, 0.5f, 0f), new Vector3(1.5f, 0.5f, 0f), new Vector3(-1.5f, 0.5f, 0f), new Vector3(1.5f, 1.5f, 0f), new Vector3(-1.5f, 1.5f, 0f), new Vector3(0f, 1.5f, 0f), new Vector3(0f, 2.5f, 0f) });
        }
        else if (Level == 8 && Etype == "BrickBat" && Ecolor == new Color32(255, 100, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(3f, -2.5f, 0f), new Vector3(-3f, -2.5f, 0f), new Vector3(1.5f, -2.5f, 0f), new Vector3(-1.5f, -2.5f, 0f), new Vector3(1.5f, -1.5f, 0f), new Vector3(-1.5f, -1.5f, 0f), new Vector3(0f, -1.5f, 0f), new Vector3(0f, -0.5f, 0f) });
        }
        else if (Level == 9 && Etype == "BrickBat" && Ecolor == new Color32(153, 0, 0, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(-1f, 0f, 0f), new Vector3(-2f, 0f, 0f), new Vector3(-3f, 0f, 0f), new Vector3(-2f, 1f, 0f), new Vector3(-2f, -1f, 0f), new Vector3(-1f, 2f, 0f), new Vector3(-1f, -2f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, -2f, 0f) });
        }
        else if (Level == 9 && Etype == "BrickBat" && Ecolor == new Color32(179, 0, 89, 255))
        {
            Epos.AddRange(new Vector3[] { new Vector3(1f, 0f, 0f), new Vector3(2f, 0f, 0f), new Vector3(3f, 0f, 0f), new Vector3(2f, 1f, 0f), new Vector3(2f, -1f, 0f), new Vector3(1f, 2f, 0f), new Vector3(1f, -2f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 2f, 0f) });
        }
        return Epos;
	}

	void TileColouring(){
		for (int i = 0; i < tileColour.Length; i++) {
			tileColour [i].GetComponent<SpriteRenderer> ().color = tileSetColour [i];
		}
	}

	void DeployCannon(){
        if (lives > 0)
        {
			ShowLives ();
            currentCannon = Instantiate(cannonSource);
            CannonControl cannonScript = currentCannon.GetComponent<CannonControl>();
            cannonScript.slime = slime;
            cannonScript.pauseObj = pauseObj;
            cannonScript.touchpads = touchpads;
            puPad.SetActive(false);
            if(lives == 3)
            {
                slimescript.fetchPUs = true;
            }
            //if(lives == 1)
            //{
            //    LoadVideoAd();
            //    adManagerScript.SendMessage("LoadVideoAd");
            //}
            //touchpads.SetActive(false);
        }
        else
        {
            pauseObj.tag = "Paused";
            ShowLives();
            timerscript.running = false;
            touchpads.SetActive(false);
            gameover.SetActive(true);
            pauseBtn.SetActive(false);
            ShowEnemGoldOnly();
            slimescript.goldThisLvl = 0;
        }
	}

	void UnPauseGame(){
		if (currentCannon) {
			CannonControl cannonScript = currentCannon.GetComponent<CannonControl> ();
			cannonScript.SendMessage ("UnPauseCannon");
		}
		Slime slimeScript = slime.GetComponent<Slime> ();
		slimeScript.SendMessage ("UnPauseSlime");
        timerscript.run = true;
	}

	void RemoveEnemies(){
		GameObject[] enemAlive = GameObject.FindGameObjectsWithTag ("Enemy");
		for (int i = 0; i < enemAlive.Length; i++) {
			Destroy (enemAlive [i]);
		}
	}

	void ResetTilePos(){
		for (int i = 0; i < padList.Length; i++) {
			padList [i].transform.position = Vector3.zero;
		}
	}

	void SpawnGate(){
		currentGate = Instantiate (gateSource);
		GateScript gScript = currentGate.GetComponent<GateScript> ();
        currentGate.GetComponent<SpriteRenderer>().color = gateColor;
		gScript.slime = slime;
		gScript.gameMech = gameObject;
	}

    void ShowScore()
    {
        GameObject scoreLbl = lvlClear.GetComponent<RectTransform>().Find("Score").gameObject;
        GameObject bscoreLbl = lvlClear.GetComponent<RectTransform>().Find("BScore").gameObject;
        Text scoreTxt = scoreLbl.GetComponent<Text>();
        Text bscoreTxt = bscoreLbl.GetComponent<Text>();
        int score = slimescript.scoreThisLvl;
        scoreTxt.text = score.ToString();
        lvlClear.SetActive(true);
        score = score + timerscript.timeinseconds;
        slimescript.scoreThisLvl = score;
        scoreTxt.text = score.ToString();
        bscoreTxt.text = GameData.current.lvlBestScore[Level - 1].ToString();
        timerscript.gameObject.transform.parent.gameObject.SetActive(false);
    }

    void ShowGold(int stars)
    {
        Text Egold = lvlClear.transform.Find("GoldEnemies").GetComponent<Text>();
        Text Sgold = lvlClear.transform.Find("GoldStar").GetComponent<Text>();
        Text Tgold = lvlClear.transform.Find("GoldTotal").GetComponent<Text>();
        int egold = slimescript.goldThisLvl;
        Egold.text = egold.ToString();
        int newstars = stars - GameData.current.lvlStars[Level];
        int sgold = newstars * 50;
        Sgold.text = sgold.ToString();
        int tgold = egold + sgold;
        GameData.current.coins += tgold;
        Tgold.text = tgold.ToString();
    }

    void ShowEnemGoldOnly()
    {
        Text Egold = gameover.transform.Find("GoldEnemies").GetComponent<Text>();
        int egold = slimescript.goldThisLvl;
        Egold.text = egold.ToString();
        GameData.current.coins += egold;
    }

    int ShowStars()
    {
        int score = slimescript.scoreThisLvl;
        stars = 0;
        if(score >= minstarscore[1])
        {
            stars = 3;
        }
        else if(score >= minstarscore[0])
        {
            stars = 2;
        }
        else
        {
            stars = 1;
        }
        Transform sb = lvlClear.GetComponent<Transform>().Find("Star Base");
        GameObject star1 = sb.Find("star1").gameObject;
        GameObject star2 = sb.Find("star2").gameObject;
        GameObject star3 = sb.Find("star3").gameObject;
        star2.SetActive(false);
        star3.SetActive(false);
        if (stars == 1)
        {
            star1.SetActive(true);
        }
        if(stars == 2)
        {
            star1.SetActive(true);
            star2.SetActive(true);
        }
        if (stars == 3)
        {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        return stars;
    }

	void ShowLives(){
		Transform lb = livesObj.transform;
		GameObject heart1 = lb.Find("H1").gameObject;
		GameObject heart2 = lb.Find("H2").gameObject;
		GameObject heart3 = lb.Find("H3").gameObject;
		heart1.SetActive (false);
		heart2.SetActive (false);
		heart3.SetActive (false);

		if (lives == 1) {
			heart1.SetActive (true);
		} else if (lives == 2) {
			heart1.SetActive (true);
			heart2.SetActive (true);
		} else if (lives == 3) {
			heart1.SetActive (true);
			heart2.SetActive (true);
			heart3.SetActive (true);
		} else {
			heart1.SetActive (false);
			heart2.SetActive (false);
			heart3.SetActive (false);
		}
	}

    void RemoveGameAssestsForRestarting()
    {
        slime.transform.position = new Vector3(-100f, -100f, 0f);
        RemoveEnemies();
        ResetTilePos();
        Destroy(currentCannon);
        Destroy(currentGate);
		timerscript.gameObject.transform.parent.gameObject.SetActive(false);
        livesObj.SetActive(false);
        touchpads.SetActive(false);
        jungleBGM.SetActive(false);
    }

    void SaveData()
    {
        SaveLoad.Save();
    }

    void LoadData()
    {
        SaveLoad.Load();
    }

    void ResetGameData()
    {
        GameData.current.firstTime = false;
        GameData.current.coins = 0;
        for(int i = 0; i <= 9; i++)
        {
            GameData.current.lvlBestScore[i] = 0;
            GameData.current.lvlStars[i] = 0;
            GameData.current.lvlUnlocked[i] = false;
            if(i < 3)
            {
                GameData.current.powerupLvls[i] = 1;
                GameData.current.powerupCount[i] = 5;
            }
        }
        GameData.current.lvlUnlocked[0] = true;
        SaveData();
    }

    void ArrangeLevelSelect()
    {
        for(int i = 0; i <= 9; i++)
        {
            if(GameData.current.lvlUnlocked[i] == true)
            {
                string lvlnum = i.ToString();
                Transform lvl = lvlSelect.transform.Find("Lv" + lvlnum);
                lvl.Find("Lock").gameObject.SetActive(false);
                GameObject star1 = lvl.Find("star1").gameObject;
                GameObject star2 = lvl.Find("star2").gameObject;
                GameObject star3 = lvl.Find("star3").gameObject;
                if(GameData.current.lvlStars[i] == 1)
                {
                    star1.SetActive(true);
                }
                else if(GameData.current.lvlStars[i] == 2)
                {
                    star1.SetActive(true);
                    star2.SetActive(true);
                }
                else if (GameData.current.lvlStars[i] == 3)
                {
                    star1.SetActive(true);
                    star2.SetActive(true);
                    star3.SetActive(true);
                }
            }
        }
    }

    void GameOver()
    {
        RemoveEnemies();
        ResetTilePos();
        if(gameEnded != null)
        {
            gameEnded();
        }
        jungleBGM.SetActive(false);
        SaveData();
        touchpads.SetActive(false);
        timerscript.gameObject.transform.parent.gameObject.SetActive(false);
        livesObj.SetActive(false);
        mainscreen.SetActive(true);
    }

    void ExtraLife()
    {
        lives = 1;
        livesObj.GetComponent<Text>().text = "Lives: " + lives;
        currentCannon = Instantiate(cannonSource);
        CannonControl cannonScript = currentCannon.GetComponent<CannonControl>();
        cannonScript.slime = slime;
        cannonScript.pauseObj = pauseObj;
        pauseObj.tag = "UnPaused";
        timerscript.run = true;
        gameover.SetActive(false);
        pauseBtn.SetActive(true);
    }

    void OnNegativeShowAd()
    {
        RemoveGameAssestsForRestarting();
        gameover.SetActive(false);
        SaveData();
        tittleLogo.SetActive(true);
        mainBtns.SetActive(true);
    }

    /*
    public void ShowAD()
    {
        if (Advertisement.IsReady("rewardedVideoZone"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideoZone", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                UnityEngine.Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                ExtraLife();
                break;
            case ShowResult.Skipped:
                UnityEngine.Debug.Log("The ad was skipped before reaching the end.");
                GameOver();
                break;
            case ShowResult.Failed:
                UnityEngine.Debug.LogError("The ad failed to be shown.");
                GameOver();
                break;
        }
    }
*/
    void QuitGame()
    {
        SaveData();
        Application.Quit();
    }

    void NextLevel()
    {
        if (Level < maxLevel)
        {
            lvlClear.SetActive(false);
            Level = Level + 1;
            RestartGame();
        }
    }

    void UpdateGameData()
    {
        if(GameData.current.lvlBestScore[Level - 1] < slimescript.scoreThisLvl)
        {
            GameData.current.lvlBestScore[Level - 1] = slimescript.scoreThisLvl;
            if(GameData.current.lvlStars[Level - 1] < stars)
            {
                GameData.current.lvlStars[Level - 1] = stars;
            }
            if (Level < maxLevel)
            {
                GameData.current.lvlUnlocked[Level] = true;
            } 
        }
        
    }

    void Level1()
    {
        if (GameData.current.lvlUnlocked[0] == true)
        {
            LevelSelectPositive.Play();
            Level = 1;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level2()
    {
        if (GameData.current.lvlUnlocked[1] == true)
        {
            LevelSelectPositive.Play();
            Level = 2;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level3()
    {
        if (GameData.current.lvlUnlocked[2] == true)
        {
            LevelSelectPositive.Play();
            Level = 3;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level4()
    {
        if (GameData.current.lvlUnlocked[3] == true)
        {
            LevelSelectPositive.Play();
            Level = 4;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level5()
    {
        if (GameData.current.lvlUnlocked[4] == true)
        {
            LevelSelectPositive.Play();
            Level = 5;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level6()
    {
        if (GameData.current.lvlUnlocked[5] == true)
        {
            LevelSelectPositive.Play();
            Level = 6;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level7()
    {
        if (GameData.current.lvlUnlocked[6] == true)
        {
            LevelSelectPositive.Play();
            Level = 7;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level8()
    {
        if (GameData.current.lvlUnlocked[7] == true)
        {
            LevelSelectPositive.Play();
            Level = 8;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level9()
    {
        if (GameData.current.lvlUnlocked[8] == true)
        {
            LevelSelectPositive.Play();
            Level = 9;
            lvlSelect.SetActive(false);
            GameStart();
        }
        else
        {
            LevelSelectNegative.Play();
        }
    }

    void Level10()
    {
        LevelSelectNegative.Play();
    }
}
