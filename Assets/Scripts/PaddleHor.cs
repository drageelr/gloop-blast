using UnityEngine;
using System.Collections;

public class PaddleHor : MonoBehaviour {
	public Transform paddleH;
	private Rigidbody2D myRB;
	public GameObject pauseObj;
    private Vector3 currentPos;
	public float horizontal;

	void Start () {
		myRB = GetComponent<Rigidbody2D> ();
        currentPos = paddleH.position;
	}

	void Update () {
		currentPos = paddleH.position;
		currentPos.x = Mathf.Clamp(currentPos.x, -5.84f, 5.84f);
		paddleH.position = currentPos;
    }

	void FixedUpdate () {
        //horizontal = Input.GetAxis ("Horizontal"); //Comment this line in android mode.
        //Debug.Log("hor = " + horizontal);
        if (pauseObj.tag == "UnPaused")
		{
			myRB.velocity = new Vector2 (horizontal * 15, 0);
		}
		else
		{
			myRB.velocity = Vector2.zero;
		}
		if (horizontal != 0f) {
			horizontal = 0f;
		}
	}
}