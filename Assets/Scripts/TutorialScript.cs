using UnityEngine;

public class TutorialScript : MonoBehaviour {

	public int page = 0;
	public GameObject movingCannon;
	public GameObject firingCannonPrefab;
	public GameObject slimePrefab;
	public GameObject colSlime1;
	public GameObject colSlime2;
	public GameObject TileNPg3;
    public GameObject colSlime3;
    public GameObject colSlime4;
    public GameObject BrickBatPg4;
    public GameObject impFX;

    private HingeJoint2D mcHJ;
	private JointMotor2D mcdummyJM;
	private GameObject firingCannonClone;
	private GameObject firingCannonSlime;
	private float angDegPg23;
    private float angDegPg4;

    private bool firinganimrunning = false;
	private bool slimeColpg2running = false;
    private bool slimeColpg3running = false;
    private bool slimeColpg4running = false;

    public GameObject TutorialPage;
    public GameObject Page1;
	public GameObject Page2;
    public GameObject Page3;
    public GameObject Page4;
    public GameObject Page5;

    // Use this for initialization
    void Start () {
		mcHJ = movingCannon.GetComponent<HingeJoint2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (page == 1) {
			if (mcHJ.limitState == JointLimitState2D.UpperLimit) {
				mcdummyJM = mcHJ.motor;
				mcdummyJM.motorSpeed = -20f;
				mcHJ.motor = mcdummyJM;
			} else if (mcHJ.limitState == JointLimitState2D.LowerLimit) {
				mcdummyJM = mcHJ.motor;
				mcdummyJM.motorSpeed = 20f;
				mcHJ.motor = mcdummyJM;
			}
			if (!firinganimrunning) {
				firinganimrunning = true;
				RunFiringAnim ();
			}
		} else if (page == 2) {
			if (!slimeColpg2running) {
				slimeColpg2running = true;
				angDegPg23 = 328f;
				MoveSlime ();
			}
		}
		else if (page == 3) 
		{
			if (!slimeColpg3running) {
                Debug.Log("Page 3 Bool not true");
                slimeColpg3running = true;
                Debug.Log("Page 3 Bool turned true");
                angDegPg23 = 328f;
				StopTile ();
				MoveSlime ();
				MoveTile ();
			}
		}
        else if(page == 4)
        {
            if (!slimeColpg4running)
            {
                Debug.Log("Page 4 Bool not true");
                slimeColpg4running = true;
                Debug.Log("Page 4 Bool turned true");
                angDegPg4 = 310f;
                MoveSlimePg4();
            }
        }
	}

	// PAGE 1 STUFF--[[
	private void RunFiringAnim()
	{
		firingCannonClone = Instantiate (firingCannonPrefab);
		firingCannonClone.transform.parent = Page1.transform;
		firingCannonClone.transform.localScale = new Vector3 (5.320563f, 5.320563f, 0f);
		firingCannonClone.transform.localPosition = new Vector3 (-110f, -57f, 0);
		Animator fcAnim = firingCannonClone.GetComponent<Animator> ();
		firingCannonClone.GetComponent<Animator> ().enabled = true;
		fcAnim.SetTrigger ("Fire");
		Invoke ("blastNow", 1f);
		Invoke ("SendMsgToSlime", 1.1f);
	}

	private void blastNow(){
		GameObject blastFX = firingCannonClone.GetComponent<Transform> ().Find ("blast").gameObject;
        blastFX.GetComponent<SpriteRenderer>().sortingOrder = 23;
		blastFX.SetActive (true);
		Destroy (blastFX, blastFX.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length + 0f);
	}

	private void SendMsgToSlime(){
		firingCannonSlime = Instantiate (slimePrefab);
		firingCannonSlime.transform.parent = Page1.transform;
		firingCannonSlime.transform.localScale = new Vector3 (3.591381f,3.591381f, 0f);
		Transform pointerT = firingCannonClone.transform.Find ("Pointer");
		float angDeg;
		HingeJoint2D myHJ = firingCannonClone.GetComponent<HingeJoint2D> ();
		firingCannonSlime.transform.position = pointerT.position;
		angDeg = myHJ.jointAngle - 20f; // "-20f" is the upper bound of the joint
		firingCannonSlime.GetComponent<Animator> ().enabled = false;
		firingCannonSlime.GetComponent<SpriteRenderer> ().color = Color.white;
		angDeg = 360f - angDeg;
		float angRad = (angDeg * Mathf.PI) / 180f;
		firingCannonSlime.GetComponent<Rigidbody2D>().velocity = new Vector2 ( 5 * Mathf.Cos (angRad), 5 * Mathf.Sin (angRad));
		firingCannonSlime.transform.rotation = Quaternion.Euler (0, 0, angDeg - 90.0f);
		Invoke ("DisableFiringCannonAndSlime", 1.25f);
	}

	void DisableFiringCannonAndSlime()
	{
		Destroy (firingCannonClone);
		Destroy (firingCannonSlime);
		firinganimrunning = false;
	}
	// PAGE 1 STUFF]]--

	// PAGE 2 AND 3 STUFF--[[
	void MoveSlime()
	{
        Debug.Log("MoveSlime!!!");
        GameObject colSlime;
		if (page == 2) 
		{
            Debug.Log("Pg 2 Slime set");
            colSlime = colSlime1;
		}
		else
		{
            Debug.Log("Page3 Slime set");
            colSlime = colSlime2;
		}
		colSlime.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		colSlime.transform.localPosition = new Vector3 (-63.6f, -39.2f, 0f);
		colSlime.transform.rotation = Quaternion.Euler (0f, 0f, angDegPg23);
		colSlime.GetComponent<SpriteRenderer> ().color = Color.white;
		float angRad = ((angDegPg23 + 90f) / 180f) * Mathf.PI; 
		colSlime.GetComponent<Rigidbody2D> ().velocity = new Vector2 (3 * Mathf.Cos (angRad), 3 * Mathf.Sin (angRad));
        Debug.Log("Invoking DeflectSlie in 1.95 secs");
        Invoke ("DeflectSlime", 1.95f);
	}

	void DeflectSlime()
	{
        Debug.Log("DeflectSlime!!!");
        GameObject colSlime;
		float angFactor;
		if (page == 2) 
		{
            Debug.Log("Pg 2 slime plus factor set");
            colSlime = colSlime1;
			angFactor = 90f;
		}
		else
		{
            Debug.Log("page 3 slime plus factor set");
            colSlime = colSlime2;
			angFactor = 70f;
		}
		float angRad = ((angDegPg23 + 90f - angFactor) / 180f) * Mathf.PI;
		colSlime.GetComponent<Rigidbody2D> ().velocity = new Vector2 (3 * Mathf.Cos (angRad), 3 * Mathf.Sin (angRad));
		colSlime.GetComponent<SpriteRenderer> ().color = Color.red;
		colSlime.transform.rotation = Quaternion.Euler (0f, 0f, angDegPg23 - angFactor);
        Debug.Log("Invoking StopSlime in 2.5 secs");
        Invoke ("StopSlime", 2.5f);
	}

	void StopSlime()
	{
        Debug.Log("StopSlime!!!");
        GameObject colSlime;
		if (page == 2) 
		{
            Debug.Log("Pg 2 slime set");
            colSlime = colSlime1;
		}
		else
		{
            Debug.Log("Pg 3 slime set");
            colSlime = colSlime2;
		}
		colSlime.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		colSlime.transform.localPosition = new Vector3 (-63.6f, -39.2f, 0f);
		colSlime.transform.rotation = Quaternion.Euler (0f, 0f, angDegPg23);
		colSlime.GetComponent<SpriteRenderer> ().color = Color.white;
		if (page == 2) 
		{
            Debug.Log("Pg 2 slime bool turned false");
            slimeColpg2running = false;
		}
		else
		{
            Debug.Log("Pg 3 slime bool turned false");
            slimeColpg3running = false;
		}
	}

	void MoveTile()
	{
        Debug.Log("MoveTile!!!");
		TileNPg3.GetComponent<Rigidbody2D> ().velocity = new Vector2 (1f, 0f);
	}

	void StopTile()
	{
        Debug.Log("StopTile!!!");
        TileNPg3.transform.localPosition = new Vector3 (-37.3f, 41.8f, 0f);
		TileNPg3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}
    // PAGE 2 AND 3 STUFF]]--

    // PAGE 4 STUFF--[[
    void MoveSlimePg4()
    {
        colSlime3.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        colSlime4.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        colSlime3.transform.localPosition = new Vector3(-133.9f, -39.2f, 0f);
        colSlime4.transform.localPosition = new Vector3(24.5f, -39.2f, 0f);
        colSlime3.transform.rotation = Quaternion.Euler(0f, 0f, angDegPg4);
        colSlime4.transform.rotation = Quaternion.Euler(0f, 0f, angDegPg4);
        colSlime3.GetComponent<SpriteRenderer>().color = Color.white;
        colSlime4.GetComponent<SpriteRenderer>().color = Color.red;
        float angRad = ((angDegPg4 + 90f) / 180f) * Mathf.PI;
        colSlime3.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * Mathf.Cos(angRad), 3 * Mathf.Sin(angRad));
        colSlime4.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * Mathf.Cos(angRad), 3 * Mathf.Sin(angRad));
        Invoke("DeflectSlimePg4", 1.5f);
    }

    void DeflectSlimePg4()
    {
        BrickBatPg4.SetActive(false);
        GameObject implode = Instantiate(impFX);
        implode.transform.position = BrickBatPg4.gameObject.transform.position;
        SpriteRenderer SR = implode.gameObject.GetComponent<SpriteRenderer>();
        SR.sortingLayerName = "GUI";
        SR.sortingOrder = 12;
        SR.color = Color.red;
        implode.SetActive(true);
        Destroy(implode, implode.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + 0f);
        float angFactor = 90f;
        float angRad = ((angDegPg4 + 90f - angFactor) / 180f) * Mathf.PI;
        colSlime3.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * Mathf.Cos(angRad), 3 * Mathf.Sin(angRad));
        colSlime4.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * Mathf.Cos(angRad), 3 * Mathf.Sin(angRad));
        colSlime3.transform.rotation = Quaternion.Euler(0f, 0f, angDegPg4 - angFactor);
        colSlime4.transform.rotation = Quaternion.Euler(0f, 0f, angDegPg4 - angFactor);
        Invoke("StopSlimePg4", 1.2f);
    }

    void StopSlimePg4()
    {
        colSlime3.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        colSlime4.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        colSlime3.transform.localPosition = new Vector3(-133.9f, -39.2f, 0f);
        colSlime4.transform.localPosition = new Vector3(24.5f, -39.2f, 0f);
        colSlime3.transform.rotation = Quaternion.Euler(0f, 0f, angDegPg4);
        colSlime4.transform.rotation = Quaternion.Euler(0f, 0f, angDegPg4);
        BrickBatPg4.SetActive(true);
        slimeColpg4running = false;
    }


    // PAGE 4 STUFF]]--

    void TurnPgsOff()
    {
        Page1.SetActive(false);
        Page2.SetActive(false);
        Page3.SetActive(false);
        Page4.SetActive(false);
        Page5.SetActive(false);
    }

    void ResetBools()
    {
        firinganimrunning = false;
        slimeColpg2running = false;
        slimeColpg3running = false;
        slimeColpg4running = false;
    }

    void NextPage()
    {
        TurnPgsOff();
        CancelInvoke();
        if (page == 1)
        {
            Page2.SetActive(true);
        }
        else if (page == 2)
        {
            Page3.SetActive(true);
        }
        else if(page == 3)
        {
            Page4.SetActive(true);
        }
        else
        {
            Page5.SetActive(true);
        }
        page = page + 1;
        ResetBools();
    }

    void PreviousPage()
    {
        TurnPgsOff();
        CancelInvoke();
        if (page == 5)
        {
            Page4.SetActive(true);
        }
        else if (page == 4)
        {
            Page3.SetActive(true);
        }
        else if (page == 3)
        {
            Page2.SetActive(true);
        }
        else
        {
            Page1.SetActive(true);
        }
        page = page - 1;
        ResetBools();
    }

    void Close()
    {
        page = 0;
        TurnPgsOff();
        TutorialPage.SetActive(false);
        ResetBools();
    }

    void Open()
    {
        TutorialPage.SetActive(true);
        Page1.SetActive(true);
        page = 1;
        ResetBools();
    }
}
