using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour {

	public GameObject slime;
	public GameObject gameMech;

	private GameObject[] enemies;

	void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.tag == "Player") {
			enemies = GameObject.FindGameObjectsWithTag ("Enemy");
			if (enemies.Length == 0) {
                slime.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				slime.transform.position = new Vector3 (-100, -100, 0);
				GameMech gmScript = gameMech.GetComponent<GameMech> ();
				gmScript.SendMessage ("LevelClear");
				Destroy (gameObject);
			}
		}
	}

}