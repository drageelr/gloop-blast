using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Slime : MonoBehaviour {

	private Rigidbody2D myRB;
	SpriteRenderer SR;
	public int scoreThisLvl;
    public int goldThisLvl;

    public delegate void PlaySoundDelegate(GameObject soundObj);
    public event PlaySoundDelegate PlaySound;

    private bool destroyer = false;
	private float vx;
	private float vy;
	private float Q;
    private float Qrad;
	private float delx;
	private float dely;
	private bool vdirchng = false;
    private bool yneg = false;
    private bool xneg = false;
	private float TileVelocityFactor = 1f;
    private float _vLvl = 5f; // Changes according to the lvl
    public bool velocityFreeze = false;
    private bool rainbowMode = false; //make it private later on
    public bool slicerMode = false; //make it private latre on
    private float vxBeforeCol;
    private float vyBeforeCol;
    private float oldvLvl;

    public bool fetchPUs = false;
	public bool MovInit = false;
	public float angDeg;
	private GameObject Cannon;
	private GameObject implode;
	private GameObject splash;
	public GameObject madnessFX;
    public GameObject pupad;
    public GameObject scoreSpawn;
    public GameObject coinSpawn;
    public GameObject canvas1;

	public bool halfscore;
	private Vector2 collisionPoint;

	public GameObject splFX;
	public GameObject impFX;
	public GameObject gameMech;
	public GameObject pauseObj;
	public GameObject cutterNormal;
	public GameObject cutterRainbow;

	private Vector2 Spd;
	private bool pauseCheck = false;    

	public GameObject splashsound;
	public GameObject cannonfiresound;
    public GameObject poofSound;
    //public GameObject hitSound;

	public GameObject PaddleH;
	public GameObject PaddleV;

    private GameMech gamemechScript;

    private Animator myAnim;

    public float vLvl
    {
        get
        {
            return _vLvl;
        }
        set
        {
            if(!velocityFreeze)
            {
                _vLvl = value;
            }
        }
    }

	void Start () {
        myAnim = GetComponent<Animator>();
		myRB = GetComponent<Rigidbody2D> ();
		SR = GetComponent<SpriteRenderer>();
        gamemechScript = gameMech.GetComponent<GameMech>();
        gamemechScript.gameEnded += GamemechScript_gameEnded;
        gamemechScript.gameStarted += GamemechScript_gameStarted;
        PowerUpInitiator puIniti = pupad.GetComponent<PowerUpInitiator>();
        puIniti.PURainbowStart += PuIniti_PURainbowStart;
        puIniti.PURainbowEnd += PuIniti_PURainbowEnd;
        puIniti.PUSlicerStart += PuIniti_PUSlicerStart;
        puIniti.PUSlicerEnd += PuIniti_PUSlicerEnd;
        puIniti.PUMadnessStart += PuIniti_PUMadnessStart;
        puIniti.PUMadnessEnd += PuIniti_PUMadnessEnd;
    }

	private void PuIniti_PUMadnessStart()
	{
		PaddleH.transform.localScale = new Vector3(2.24f, 1f, 1f);
		PaddleH.transform.position = new Vector3(0f, 0f, 0f);
		PaddleV.transform.localScale = new Vector3(1f, 2f, 1f);
		PaddleV.transform.position = new Vector3(0f, -0.15f, 0f);
		oldvLvl = vLvl;
		vLvl = 30f;
		AdjustVelocityComponents();
		madnessFX.SetActive(true);
	}

    private void PuIniti_PUMadnessEnd()
    {
        PaddleH.transform.localScale = new Vector3(1f, 1f, 1f);
        PaddleV.transform.localScale = new Vector3(1f, 1f, 1f);
        PaddleH.transform.position = new Vector3(0f, 0f, 0f);
        PaddleV.transform.position = new Vector3(0f, 0f, 0f);
        vLvl = oldvLvl;
        AdjustVelocityComponents();
		madnessFX.SetActive(false);
    }
  

	private void PuIniti_PUSlicerStart()
	{
		slicerMode = true;
		if (rainbowMode) {
			cutterRainbow.SetActive (true);
		} else {
			cutterNormal.SetActive (true);
			cutterNormal.GetComponent<SpriteRenderer> ().color = SR.color;
		}
	}


    private void PuIniti_PUSlicerEnd()
    {
        slicerMode = false;
		cutterRainbow.SetActive (false);
		cutterNormal.SetActive (false);
    }

	private void PuIniti_PURainbowStart()
	{
		rainbowMode = true;
		//Add colour change code here
		myAnim.SetTrigger("RainbowMode");
		SR.color = Color.white;
		if (slicerMode) {
			cutterNormal.SetActive (false);
			cutterRainbow.SetActive (true);
		}
	}

    private void PuIniti_PURainbowEnd()
    {
        rainbowMode = false;
        //Add colour change code here
        myAnim.SetTrigger("NormalMode");
		if (slicerMode) {
			cutterRainbow.SetActive (false);
			cutterNormal.SetActive (true);
		}
    }
		

    private void GamemechScript_gameStarted()
    {
        velocityFreeze = false;
    }

    private void GamemechScript_gameEnded()
    {
        velocityFreeze = true;
    }

    void Update(){
		if (pauseObj.tag == "UnPaused") {
			pauseCheck = false;
			if (MovInit) {
				MovInit = false;
				CannonLaunch ();
			}
            
		} else if(pauseObj.tag == "Paused" && !pauseCheck) {
			Spd = myRB.velocity;
			myRB.velocity = Vector2.zero;
			pauseCheck = true;
		}
	}


    void OnCollisionEnter2D(Collision2D col){
		string Tag = col.gameObject.tag;
		if (Tag == "Destroyer") {
			cutterNormal.SetActive (false);
			cutterRainbow.SetActive (false);
            transform.position = new Vector3(-50, -50, 0);
            myRB.velocity = new Vector2 (0, 0);
            pupad.SetActive(false);
			GameMech gamemechScript = gameMech.GetComponent<GameMech> ();
			gamemechScript.lives = gamemechScript.lives - 1;
			gamemechScript.SendMessage ("DeployCannon");
		}
		else if ((Tag == "TileN") || (Tag == "TileW")|| (Tag == "TileE")|| (Tag == "TileS")) { 
            if (!rainbowMode)
            {
                SendPlaySound(splashsound);
                collisionPoint = col.contacts[0].point;
                Color paintC = col.gameObject.transform.Find("paintL").gameObject.GetComponent<SpriteRenderer>().color; //Gets color of SpriteRenderer component of child 'paintL' of collider gameObject TileW etc.
                SR.color = paintC; //Paint layer color assigned to slime SpriteRenderer color.
				if (slicerMode){
					cutterNormal.GetComponent<SpriteRenderer> ().color = paintC;
				}
            }
		}
		else if(Tag == "Enemy"){
			Color Ecolor = col.gameObject.GetComponent<SpriteRenderer> ().color;
			if (Ecolor == SR.color || rainbowMode) {
                Vector3 objPos = col.gameObject.transform.position;
                SendPlaySound(poofSound);
                implode = Instantiate(impFX);
				implode.transform.position = objPos;
				implode.gameObject.GetComponent<SpriteRenderer> ().color = Ecolor;
				implode.SetActive (true);
                Transform scoreT = col.gameObject.transform.Find("Score");
				Destroy (col.gameObject);
				Destroy (implode, implode.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length + 0f);
                float score = scoreT.localPosition.x;
				if (halfscore)
				{
                    score = score / 2;
                }
                int gold = (int)(score / 5f);
                scoreThisLvl = scoreThisLvl + ((int)score);
                GameObject cloneScore = Instantiate(scoreSpawn);
                cloneScore.transform.GetChild(0).GetComponent<Text>().text = "+" + score.ToString();
                cloneScore.transform.parent = canvas1.transform;
                cloneScore.transform.position = objPos;
                cloneScore.transform.localScale = new Vector3(17.29167f, 17.29167f, 1f);
                GameObject cloneCoin = Instantiate(coinSpawn);
                cloneCoin.transform.GetChild(0).GetComponent<Text>().text = "+" + gold.ToString();
                cloneCoin.transform.parent = canvas1.transform;
                cloneCoin.transform.localScale = new Vector3(12f, 12f, 1f);
                cloneCoin.transform.position = objPos;
                goldThisLvl += gold;
                //OnCollisionExit2D (col);
            }
		}
	}

	void OnCollisionExit2D(Collision2D col){
        if(col.gameObject.tag != "Destoyer")
        {
            bool slicerChk = false;
            if (col.gameObject.tag == "Enemy")
            {
                if (rainbowMode || col.gameObject.GetComponent<SpriteRenderer>().color == SR.color)
                {
                    slicerChk = true;
                }
            }

            // Fetch latest velocity after deflection
            vx = myRB.velocity.x;
            vy = myRB.velocity.y;

            if (col.gameObject.tag == "TileN" || col.gameObject.tag == "TileS")
            {
                if (!rainbowMode)
                {
                    slicerChk = false;
                    splash = Instantiate(splFX);
                    if (col.gameObject.tag == "TileN")
                    {
                        splash.transform.position = new Vector3(collisionPoint.x, collisionPoint.y - 1f, 0);
                        splash.transform.rotation = Quaternion.Euler(0, 0, 180);
                        yneg = true;
                    }
                    else
                    {
                        splash.transform.position = new Vector3(collisionPoint.x, collisionPoint.y + 1f, 0);
                        splash.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    splash.gameObject.GetComponent<SpriteRenderer>().color = SR.color;
                    splash.SetActive(true);
                    Destroy(splash, splash.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 0f);
                }
                delx = PaddleH.GetComponent<Rigidbody2D>().velocity.x * TileVelocityFactor;
                //UnityEngine.Debug.Log ("delx = " + delx);
                //UnityEngine.Debug.Log ("paddleh veloctiy x = " + PaddleH.GetComponent<Rigidbody2D> ().velocity.x);
                if (delx != 0f)
                {
                    vdirchng = true;
                }
            }
            else if (col.gameObject.tag == "TileW" || col.gameObject.tag == "TileE")
            {
                if (!rainbowMode)
                {
                    splash = Instantiate(splFX);
                    if (col.gameObject.tag == "TileW")
                    {
                        splash.transform.position = new Vector3(collisionPoint.x + 1f, collisionPoint.y, 0);
                        splash.transform.rotation = Quaternion.Euler(0, 0, 270);
                    }
                    else
                    {
                        splash.transform.position = new Vector3(collisionPoint.x - 1f, collisionPoint.y, 0);
                        splash.transform.rotation = Quaternion.Euler(0, 0, 90);
                        xneg = true;
                    }
                    splash.gameObject.GetComponent<SpriteRenderer>().color = SR.color;
                    splash.SetActive(true);
                    Destroy(splash, splash.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 0f);
                }
                dely = PaddleV.GetComponent<Rigidbody2D>().velocity.y * TileVelocityFactor;
                //UnityEngine.Debug.Log ("dely = " + dely);
                if (dely != 0f)
                {
                    vdirchng = true;
                }
            }


            //UnityEngine.Debug.Log("vx = " + vx + " vy = " + vy);


            if (vdirchng)
            { // Mobile GameObject is hit
              // Previous code commented below the functions!
              // Method:
              // Add the specific x or y component
              // Alter the remaining component such that the mag = vLvl
              // Use Quad calc
              //UnityEngine.Debug.Log("MOVING TILE!!!!");
                if (delx != 0f)
                {
                    vx = vx + delx;
                }
                else
                {
                    vy = vy + dely;
                }
                AdjustVelocityComponents();
                vdirchng = false;
            }
            else if (slicerChk && slicerMode)
            {
                myRB.velocity = new Vector2(vxBeforeCol, vyBeforeCol);
            }
            else // Any non-mobile GameObject is hit
            {
                // Method:
                // Calc Angle and Quad First
                // Then Alter the x and y Components
                if (col.gameObject.tag != "Destroyer")
                {
                    AdjustVelocityComponents();
                }

            }
            delx = 0f;
            dely = 0f;
            xneg = false;
            yneg = false;
            vxBeforeCol = myRB.velocity.x;
            vyBeforeCol = myRB.velocity.y;
            //Debug.Log("Velocity Of Slime = " + myRB.velocity.magnitude);
        }
        else
        {
            myRB.velocity = Vector2.zero;
        }
    }

	float MathAbsF(float x){
		float val;
		if (x < 0) {
			val = -1.0f * x;
		} 
		else {
			val = x;
		}
		return val;
	}

	void CannonLaunch(){
		Cannon = gamemechScript.currentCannon;
		SR.color = Color.white;
		angDeg = 360f - angDeg;
		float angRad = (angDeg * Mathf.PI) / 180f;
		vx = vLvl * Mathf.Cos (angRad);
		vy = vLvl * Mathf.Sin (angRad);
		myRB.velocity = new Vector2 (vx, vy);
		Q = angDeg;
		transform.localRotation = Quaternion.Euler (0, 0, Q - 90.0f);
		SendPlaySound(cannonfiresound);
        pupad.SetActive(true);
        if (fetchPUs)
        {
            fetchPUs = false;
            pupad.GetComponent<PowerUpInitiator>().SendMessage("PowerUpInitiator_gameStarted");
        }
	}

	void UnPauseSlime(){
		if (pauseObj.tag == "UnPaused") {
			myRB.velocity = Spd;
		}
	}

    void QuadCalcAndRot()
    {
        Qrad = Mathf.Atan2(MathAbsF(vy), MathAbsF(vx));
        Q = (Qrad * 180) / Mathf.PI;
		//UnityEngine.Debug.Log ("Q = " + Q);
		if (vx < 0 && vy > 0) { //2nd Quad
			Q = 180f - Q;
			xneg = true;
		} else if (vx < 0 && vy < 0) { //3rd Quad
			Q = 180f + Q;
			yneg = true;
			xneg = true;
		} else if (vx > 0 && vy < 0) { //4th Quad
			Q = 360f - Q;
			yneg = true;
		} else if (vx == 0f) {
			if (vy > 0) {
				Q = 90f;
			} else if (vy < 0) {
				Q = 270f;
				yneg = true;
			}
		} else if (vy == 0f) {
			if (vx > 0) {
				Q = 0f;
			} else if (vx < 0) {
				Q = 180f;
				xneg = true;
			}
		}
        transform.localRotation = Quaternion.Euler(0, 0, Q - 90f);
    }

    void AdjustVelocityComponents()
    {
        xneg = false;
        yneg = false;
        QuadCalcAndRot();
        vy = vLvl * Mathf.Sin(Qrad);
        vx = vLvl * Mathf.Cos(Qrad);
        if (xneg)
        {
            vx = -vx;
        }
        if (yneg)
        {
            vy = -vy;
        }
        if (!destroyer)
        {
            myRB.velocity = new Vector2(vx, vy);
        }
        else
        {
            destroyer = false;
        }
    }

    private void EnablePowerUps()
    {
        pupad.SetActive(true);
    }

    private void SendPlaySound(GameObject soundObj)
    {
        if(PlaySound != null)
        {
            PlaySound(soundObj);
        }
    }
}
