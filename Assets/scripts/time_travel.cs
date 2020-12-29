using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class time_travel : MonoBehaviour
{

    int ttp=0;
    
    public void time_travell(GameObject go,Vector3 currentPos,Vector3[] poses)
    {
        
        if (ttp <= 299)
        {
            poses[ttp] = transform.position;
            ttp++;
        }

        if (ttp>=300)
        {
            for (int i = 0; i <= 298; i++)
            {
                poses[i] = poses[i+1];
            }
            //poses[299] = transform.position;
        }
    }

}
