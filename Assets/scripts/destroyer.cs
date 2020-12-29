using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class destroyer : MonoBehaviour
{
    void Update()
    {
        if (GameObject.FindWithTag("Player").transform.position.z-transform.position.z>400)
        {
            Destroy(gameObject,0f);
        }
    }
}