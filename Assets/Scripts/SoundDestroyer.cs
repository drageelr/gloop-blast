using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDestroyer : MonoBehaviour {

    private AudioSource audioS;
    public bool started;
	// Use this for initialization
	void Start () {
        audioS = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (started && !audioS.isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
