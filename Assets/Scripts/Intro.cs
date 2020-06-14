using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {
	public GameObject StudioIntro;
	public GameObject TitleIntro;
	public GameObject SQIntro;
	public GameObject LogoSoundPrefab;
	public GameObject BGM;
	public bool startbool;

    public delegate void PlaySoundDelegate(GameObject soundObj);
    public event PlaySoundDelegate PlaySound;

	// Update is called once per frame
	void Start() {
		StudioIntro.SetActive (true);
        if(PlaySound!= null)
        {
            PlaySound(LogoSoundPrefab);
        }
		Invoke ("LogoIntro", StudioIntro.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length + 0f);
	}

	private void LogoIntro(){
		BGM.SetActive (true);
		TitleIntro.SetActive (true);
		SQIntro.SetActive (true);
		StudioIntro.SetActive (false);
	}
}
