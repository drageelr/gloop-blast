using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public GameMech gameMechscript;
    public Slime slimeScript;
    public Intro introScript;
    private bool _muteMusic = false;
    private bool _muteEffects = false;

    public bool muteMusic
    {
        get
        {
            return _muteMusic;
        }
        set
        {
            _muteMusic = value;
        }
    }
    public bool muteEffects
    {
        get
        {
            return _muteEffects;
        }
        set
        {
            _muteEffects = value;
        }
    }

    // Use this for initialization
    void Awake () {
        gameMechscript.OnSoundDataChange += GameMechscript_OnSoundDataChange;
        slimeScript.PlaySound += SlimeScript_PlaySound;
        introScript.PlaySound += IntroScript_PlaySound;
	}

    private void IntroScript_PlaySound(GameObject soundObj)
    {
        PlaySound(soundObj, _muteMusic);
    }

    private void SlimeScript_PlaySound(GameObject soundObj)
    {
        PlaySound(soundObj, _muteEffects);
    }

    private void GameMechscript_OnSoundDataChange(int soundType, bool mutebool)
    {
        if (soundType == 0)
        {
            muteMusic = mutebool;
        }
        else if(soundType == 1)
        {
            muteEffects = mutebool;
        }
        AudioSource[] sounds = transform.GetChild(soundType).GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].mute = mutebool;
        }
    }

    private void PlaySound(GameObject soundObj, bool mutebool)
    {
        GameObject clone = Instantiate(soundObj);
        clone.GetComponent<AudioSource>().mute = mutebool;
        clone.GetComponent<AudioSource>().Play();
        clone.GetComponent<SoundDestroyer>().started = true;
    }

    private void MuteEffects()
    {
        GameMechscript_OnSoundDataChange(1, true);
    }

    private void MuteMusic()
    {
        GameMechscript_OnSoundDataChange(0, true);
    }

    private void UnMuteEffects()
    {
        GameMechscript_OnSoundDataChange(1, false);
    }

    private void UnMuteMusic()
    {
        GameMechscript_OnSoundDataChange(0, false);
    }
}
