using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class JoystickDragger : MonoBehaviour
{

    public GameObject touchInput;
    public GameObject paddleH;
    public GameObject paddleV;
    private PaddleHor paddleHScript;
    private PaddleVer paddleVScript;

    private Transform tipT;
    private const float maxX = 2f;
    private const float maxY = 2f;

    void Awake()
    {
        tipT = transform.GetChild(0);
        touchInput.GetComponent<TouchInputAlt>().joystickTD += JoystickDragger_joystickTD;
        touchInput.GetComponent<TouchInputAlt>().joystickTU += JoystickDragger_joystickTU;
        touchInput.GetComponent<TouchInputAlt>().joystickTM += JoystickDragger_joystickTM;
        paddleHScript = paddleH.GetComponent<PaddleHor>();
        paddleVScript = paddleV.GetComponent<PaddleVer>();
    }

    private void JoystickDragger_joystickTM(Vector2 pos)
    {
        //Debug.Log("TM recived");
        float magX = pos.x - transform.position.x;
        float magY = pos.y - transform.position.y;
        if (magX > maxX)
        {
            pos.x = transform.position.x + maxX;
        }
        else if(magX < -maxX)
        {
            pos.x = transform.position.x - maxX;
        }
        if (magY > maxY)
        {
            pos.y = transform.position.y + maxY;
        }
        else if (magY < -maxY)
        {
            pos.y = transform.position.y - maxY;
        }

        tipT.position = new Vector3(pos.x, pos.y, 0f);
        paddleHScript.horizontal = ReturnCalcValues(magX / 2f);
        paddleVScript.vertical = ReturnCalcValues(magY / 2f);
        //pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x) * 2;
        //pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y) * 2;
        //pos = (pos.magnitude > 1.0f) ? pos.normalized : pos;

        //joystickImg.rectTransform.anchoredPosition = new Vector3(pos.x * bgImg.rectTransform.sizeDelta.x / 3, pos.y * bgImg.rectTransform.sizeDelta.y / 3);
    }

    private void JoystickDragger_joystickTU(Vector2 screenpoints)
    {
        //Debug.Log("TU recived");
        tipT.localPosition = Vector3.zero;
        paddleHScript.horizontal = 0f;
        paddleVScript.vertical = 0f;
        gameObject.SetActive(false);
    }

    private void JoystickDragger_joystickTD(Vector2 pos)
    {
        //Debug.Log("TD recived");
        //Debug.Log("Spawn pos = " + pos);
        transform.position = new Vector3(pos.x, pos.y, 0f);
        
    }

    private float ReturnCalcValues(float ratio)
    {
        bool neg = false;
        if(ratio < 0)
        {
            neg = true;
            ratio = -ratio;
        }
        float returnValue = Mathf.Pow(5, ratio) / 5;
        if (neg)
        {
            return -returnValue;
        }
        else if(ratio == 0f)
        {
            return 0f;
        }
        else
        {
            return returnValue;
        }
    }
}
