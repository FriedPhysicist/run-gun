using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class when_stuck : MonoBehaviour
{
    public Transform parrent;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = parrent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
