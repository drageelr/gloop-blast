using UnityEngine;
using System.Collections;

public class PaddleVer : MonoBehaviour {
	public Transform paddleV;
	private Rigidbody2D myRB;
	public GameObject pauseObj;
    private Vector3 currentPos;
	public float vertical;

    void Start () {
		myRB = GetComponent<Rigidbody2D> ();
        currentPos = paddleV.position;
    }

	void Update () {
		currentPos = paddleV.position;
		currentPos.y = Mathf.Clamp(currentPos.y, -2.14f, 2.05f);
		paddleV.position = currentPos;
    }

	void FixedUpdate () {
		//vertical = Input.GetAxis ("Vertical"); //Comment this line in android mode.
        //Debug.Log("ver = " + vertical);
        if (pauseObj.tag == "UnPaused")
        {
            myRB.velocity = new Vector2(0, vertical * 15);
        }
        else
        {
            myRB.velocity = Vector2.zero;
        }
		if (vertical != 0f) 
		{
			vertical = 0f;
		}
	}
}
