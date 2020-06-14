using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hovering : MonoBehaviour
{

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 2f);
        Invoke("DestroyMe", 0.5f);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
