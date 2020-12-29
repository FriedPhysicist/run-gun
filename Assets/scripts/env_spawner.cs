using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class env_spawner : MonoBehaviour
{
    public GameObject _park;
    GameObject copy_park;
    
    public GameObject _basketball_court;
    GameObject copy_basketball_court;
    
    public GameObject _clock_tower;
    GameObject copy_clock_tower;
    
    public GameObject _fountain;
    GameObject copy_fountain;

    private float face_it = 0;
    
    void Start()
    {
        if (transform.position.x>0)
        {
            face_it = 180;
        }
        
        switch (UnityEngine.Random.Range(0,5))
        {
            case 0:
                copy_park = Instantiate(_park, transform.position-new Vector3(0,0.6f,0), Quaternion.Euler(-90,face_it,0),transform);
                break;
        
            case 1:
                copy_basketball_court = Instantiate(_basketball_court, transform.position-new Vector3(0,0.3f,0), Quaternion.Euler(-90,90,0),transform);
                break;
        
            case 2:
                copy_clock_tower = Instantiate(_clock_tower, transform.position+new Vector3(0,14.3f,0), Quaternion.Euler(-90,face_it+180,0),transform);
                break;
        
            case 3:
                copy_fountain = Instantiate(_fountain, transform.position+new Vector3(0,11f,0), Quaternion.Euler(-90,face_it,0),transform);
                break;
            
            case 4:
                break;
        }
    }
}
