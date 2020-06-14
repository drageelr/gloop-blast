using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchInputAlt : MonoBehaviour {

    public GameObject vj;
    public GameObject touchpad;
    public GameObject pauseObj;
    public GameObject swapperPad;

    public SpriteRenderer tileNPaintSR;
    public SpriteRenderer tileSPaintSR;
    public SpriteRenderer tileEPaintSR;
    public SpriteRenderer tileWPaintSR;

    private const int jstconst = -100;
    private bool joystickSpawned = false;
    private int joystickTouchIndex = jstconst;

    public delegate void TouchPhases(Vector2 pos);
    public event TouchPhases joystickTD;
    public event TouchPhases joystickTU;
    public event TouchPhases joystickTM;

    public delegate void InitiatePowerUp(int PUnum);
    public event InitiatePowerUp PUActivate;

    private const float upperLimit = 5.4f;
    private const float lowerLimit = -4.85f;

    void Update()
    {
        if (pauseObj.tag == "UnPaused")
        {
            bool objTouched = ObjectTouch();
            JoystickSpawnerAndController(objTouched);
        }
    }


    private void JoystickSpawnerAndController(bool objT)
    {
        if (Input.touchCount > 0) //If screen is being touched then proceed
        {
            if (!joystickSpawned && touchpad.activeSelf) //If joystick is not already spawned then proceed
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if(Input.GetTouch(i).phase == TouchPhase.Began && !objT) 
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                        if(ray.origin.y > lowerLimit && ray.origin.y < upperLimit)
                        {
                            //Store index pos of joystick touch to later detect it for removal
                            joystickTouchIndex = i;
                            //Debug.Log("Initial Joystick Index = " + joystickTouchIndex);

                            //Spawn Joystick
                            vj.SetActive(true);

                            if (joystickTD != null)
                                joystickTD(new Vector2(ray.origin.x, ray.origin.y));

                            //Prevent multiple spawns of joystick
                            joystickSpawned = true;
                        } 
                    }
                }
            }
            else if (joystickSpawned)
            {
                int shiftindex = 0;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if(Input.GetTouch(i).phase == TouchPhase.Ended) //Either joystick touch is removed or another touch
                    {
                        if (i == joystickTouchIndex) //Joystick finger is removed
                        {
                            joystickTouchIndex = jstconst;
                            //j_BG.SetActive(false);
                            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                            if (joystickTU!=null)
                            joystickTU(new Vector2(ray.origin.x, ray.origin.y));
                            joystickSpawned = false;
                        }
                        else if(joystickTouchIndex != jstconst) //This is not the joystick finger
                        {
                            if(i < joystickTouchIndex)
                            {
                                shiftindex = shiftindex + 1;
                               //Debug.Log("Shift Index = " + shiftindex);
                            }
                        }
                    }
                }
                if (shiftindex != 0)
                {
                    joystickTouchIndex = joystickTouchIndex - shiftindex;
                    //Debug.Log("Adjusted Joystick Index = " + joystickTouchIndex);
                }
                if (joystickTouchIndex != jstconst)
                {
                    if (Input.GetTouch(joystickTouchIndex).phase == TouchPhase.Moved && joystickTouchIndex != jstconst)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(joystickTouchIndex).position);
                        if (joystickTM != null)
                        {
                            joystickTM(new Vector2(ray.origin.x, ray.origin.y));
                        }
                    }
                }
            }
        }
    }

    private bool ObjectTouch()
    {
        bool returnval = false;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (i != joystickTouchIndex && Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                    RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), new Vector2(ray.direction.x, ray.direction.y), Mathf.Infinity);
                    if (hit2D.transform != null)
                    {
                        UnityEngine.Debug.Log(hit2D.transform.tag);
                        if (hit2D.transform.tag == "Cannon")
                        {
                            GameObject.Find("Cannon(Clone)").transform.gameObject.GetComponent<CannonControl>().Launch = true;
                            returnval = true;
                        }
                        else if (hit2D.transform.tag == "PU1")
                        {
                            if(PUActivate != null)
                            {
                                PUActivate(1);
                            }
                            returnval = true;
                        }
                        else if (hit2D.transform.tag == "PU2")
                        {
                            if (PUActivate != null)
                            {
                                PUActivate(2);
                            }
                            returnval = true;
                        }
                        else if (hit2D.transform.tag == "PU3")
                        {
                            if (PUActivate != null)
                            {
                                PUActivate(3);
                            }
                            returnval = true;
                        }
                        else if(hit2D.transform.tag == "Swapper")
                        {
                            hit2D.collider.enabled = false;
                            swapperPad.GetComponent<Animator>().SetTrigger("Rotate");
                            Invoke("EnableSwapperCollider", 1f);
                            SwapColour();
                            returnval = true;
                        }
                    }
                }
            }
        }
        return returnval;
    }

    void SwapColour()
    {
        Color32 n, s, e, w;
        n = tileNPaintSR.color;
        s = tileSPaintSR.color;
        e = tileEPaintSR.color;
        w = tileWPaintSR.color;
        tileNPaintSR.color = e;
        tileEPaintSR.color = s;
        tileSPaintSR.color = w;
        tileWPaintSR.color = n;
    }

    void EnableSwapperCollider()
    {
        swapperPad.GetComponent<CircleCollider2D>().enabled = true;
    }

}
