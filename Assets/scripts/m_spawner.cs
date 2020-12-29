using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{


public class m_spawner : MonoBehaviour
{
    public GameObject big_golem;
    public GameObject little_golem;
    public float spawn_chance;
    public Transform monster_parrent;

    private int monster_chance;
    
    void Start()
    {

    }

    private void Update()
    {
        /*if (Vector3.Distance(GameObject.FindWithTag("Player").transform.position,transform.position)<200)
        {
            if(spawn_chance>=Random.Range(1,101))
            {
                monster_chance = Random.Range(0, 101);

                if (monster_chance>=90)
                {
                    Instantiate(big_golem, transform.position, Quaternion.identity,monster_parrent);
                }
                
                if (monster_chance<=89)
                {
                    Instantiate(little_golem, transform.position, Quaternion.identity,monster_parrent);
                }
            }
            
            Destroy(gameObject,0f);
        }*/
    }

}



}
