using UnityEngine;
using System.Collections;

public class CannonControl : MonoBehaviour {
	private HingeJoint2D myHJ;
	private JointMotor2D myJM;
	private Animator myAnim;
	public GameObject slime;
	public GameObject pointer;
	public GameObject blastFX;
    public GameObject touchpads;
	public float motorSpd;
	public bool Initiated = false;

    public bool Launch = false;

	public GameObject pauseObj;
	private float savedSpd = -20f;
	private bool pChk = false;

	private Slime slimeScript;

	void Start () {
		myHJ = GetComponent<HingeJoint2D> ();
		myAnim = GetComponent<Animator> ();
		slimeScript = slime.GetComponent<Slime> ();
	}

	void Update(){
		if (myHJ.limitState == JointLimitState2D.UpperLimit && pauseObj.tag == "UnPaused") {
			myJM = myHJ.motor;
			motorSpd = -20f;
			myJM.motorSpeed = motorSpd;
			myHJ.motor = myJM;
			pChk = false;
		}else if(myHJ.limitState == JointLimitState2D.LowerLimit && pauseObj.tag == "UnPaused"){
			myJM = myHJ.motor;
			motorSpd = 20f;
			myJM.motorSpeed = motorSpd;
			myHJ.motor = myJM;
			pChk = false;
		}
		if (Initiated || (pauseObj.tag == "Paused" && !pChk)) {
			pChk = true;
			myJM = myHJ.motor;
			savedSpd = myJM.motorSpeed;
			motorSpd = 0f;
			myJM.motorSpeed = motorSpd;
			myHJ.motor = myJM;
		}
		HandleInputs ();
        if (Launch)
        {
            LaunchSlime();
        }
	}

	void HandleInputs(){
		if (Input.GetKeyDown (KeyCode.Space)) {
            Launch = true;
		}
	}

	void LaunchSlime(){
		if (!Initiated) {
			Initiated = true;
			myJM.motorSpeed = 0f;
			myHJ.motor = myJM;
			gameObject.GetComponent<Animator> ().enabled = true;
			myAnim.SetTrigger ("Fire");
			Invoke ("blastNow", 0.3f);
			Invoke ("SendMsgToSlime", 0.4f);
		}
	}

	private void blastNow(){
		blastFX.SetActive (true);
		Destroy (blastFX, blastFX.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length + 0f);
	}

	private void SendMsgToSlime(){
        slime.transform.position = pointer.transform.position;
		slimeScript.angDeg = myHJ.jointAngle - 20f; // "-20f" is the upper bound of the joint
		slimeScript.MovInit = true;
		gameObject.GetComponent<Animator> ().enabled = false;
		Invoke ("DisableCannon", 0.5f);
	}
		
	private void DisableCannon(){
        //touchpads.SetActive(true);
        slimeScript.SendMessage("EnablePowerUps");
		Destroy (gameObject);
	}

	void UnPauseCannon(){
		pChk = false;
		myJM.motorSpeed = savedSpd;
		myHJ.motor = myJM;
	}
}
