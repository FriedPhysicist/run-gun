using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_cam_holder : MonoBehaviour
{

    public GameObject charr_;
    private Vector3 target;
    
    void FixedUpdate()
    {
        target = charr_.transform.position + new Vector3(1, 1, -2f);
        
        //transform.position =Vector3.Lerp(transform.position,target,0.8f);
        transform.position = target;
    }
    
}
