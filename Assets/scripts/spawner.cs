using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class spawner : MonoBehaviour
{
    [Header("spawn objects and copies")]
    public GameObject spawn_platform;
    GameObject spawn_platform_copy;
    public GameObject spawn_platform_left;
    GameObject spawn_platform_copy_left;
    public GameObject spawn_platform_right;
    GameObject spawn_platform_copy_right;
    public GameObject spawn_platform_left_p;
    GameObject spawn_platform_copy_left_p;
    public GameObject spawn_platform_right_p;
    GameObject spawn_platform_copy_right_p;
    /*public GameObject spawn_spot;
    GameObject spawn_spot_copy;
    public GameObject spawn_enemy;
    GameObject spawn_enemy_copy;
    public GameObject spawn_wall;
    GameObject spawn_wall_copy;
    public GameObject p_15_35;
    GameObject p_15_35_c;
    public GameObject p_35_50;
    GameObject p_35_50_c;
    public GameObject p_50_15;
    GameObject p_50_15_c;
    public int how_much_copy;
    public float distance_35_35;
    public GameObject p_5_5;
    GameObject p_5_5_c;*/
    public Transform monster_parrent;
    public GameObject big_golem;
    public GameObject little_golem;
    GameObject copy_big_golem;
    GameObject copy_little_golem;

    public GameObject when_stuck_object;
    GameObject copy_when_stuck_object;
    public Transform when_stuck_parrent;
    
    [Header("Move Directions")] 
    public float turn_chance;
    private float last;
    public float left_right_x=22.5f;
    public float left_right_z=23f;
    public GameObject character_;
    
    void Start()
    {
        for (int j = 0; j < 2; j++)
        {
            spawn_platform_copy = Instantiate(spawn_platform, transform.position, Quaternion.Euler(0,0,0));
            
            transform.position+=new Vector3(0,0,50);
        }   
    }

    void Update()
    {
        //character_ = GameObject.FindWithTag("Player");
            
        while ((transform.position.z-character_.transform.position.z)<160)
        {
            spawn_platform_copy = Instantiate(spawn_platform, transform.position, Quaternion.Euler(0,0,0));
            monster_spawner();
            
            transform.position+=new Vector3(0,0,50);
            last = 0;
            
            if (Random.Range(1,101)>=turn_chance)
            {
                switch (Random.Range(1,3))
                {   
                    //left spawns
                    case 1:
                        transform.position+=new Vector3(left_right_x,0,-left_right_z);
                        spawn_platform_copy_right = Instantiate(spawn_platform_right, transform.position, Quaternion.Euler(0,90,0));
                        monster_spawner();
                        
                        transform.position+=new Vector3(left_right_x,0,left_right_z);
                        
                        spawn_platform_copy_right_p = Instantiate(spawn_platform_right_p, transform.position+new Vector3(0,0,0), Quaternion.identity);
                        when_stuck_spawner();
                        monster_spawner();
                        last = 1;
                        break;
                    
                    //right spawns
                    case 2:
                        transform.position+=new Vector3(-left_right_x,0,-left_right_z);
                        spawn_platform_copy_left = Instantiate(spawn_platform_left, transform.position, Quaternion.Euler(0,-90,0));
                        monster_spawner();
                        
                        transform.position+=new Vector3(-left_right_x,0,left_right_z);
                        
                        spawn_platform_copy_left_p= Instantiate(spawn_platform_left_p, transform.position+new Vector3(0,0,0), Quaternion.identity);
                        when_stuck_spawner();
                        monster_spawner();
                        last = 2;
                        break;
                }
                
                transform.position+=new Vector3(0,0,50);
            }

            break;
        }
        
    }

    void trying_right()
    {

        Vector3 tepm_pos = transform.position - new Vector3(50, 0, 0);

        for (int i = 1; i <= 18; i++)
        {
            for (int j = 1; j <= 48; j++)
            {
                //p_5_5_c = Instantiate(p_5_5,tepm_pos-new Vector3(5f*j,-10f,5f*i),Quaternion.identity);
            }
        }

    }
    
    void trying_left()
    {

        Vector3 tepm_pos = transform.position + new Vector3(300, 0, 0);

        for (int i = 1; i <= 18; i++)
        {
            for (int j = 1; j <= 48; j++)
            {
                //p_5_5_c = Instantiate(p_5_5,tepm_pos-new Vector3(5f*j,-10f,5f*i),Quaternion.identity);
            }
        }

    }
    
    //determin spawn chance
    int little_spawn_chance = 90;
    
    //spawn monsters between x:-90,90 and charr.pos.x-spawner.z
    void monster_spawner()
    {
        if(monster_parrent.childCount<15)
        {
            int monster_chance = Random.Range(0, 101);

            if (monster_chance>=little_spawn_chance)
            {
                copy_big_golem=Instantiate(big_golem, new Vector3(transform.position.x+Random.Range(-35,35),transform.position.y,UnityEngine.Random.Range(character_.transform.position.z+50,transform.position.z)), Quaternion.identity,monster_parrent);
            }
                
            if (monster_chance<little_spawn_chance)
            {
                copy_little_golem=Instantiate(little_golem,new Vector3(transform.position.x+Random.Range(-35,35),transform.position.y,UnityEngine.Random.Range(character_.transform.position.z+15,transform.position.z)), Quaternion.identity,monster_parrent);
            }
        }
    }

    void when_stuck_spawner()
    {
        copy_when_stuck_object = Instantiate(when_stuck_object, transform.position + new Vector3(90, 2, 20),
            Quaternion.identity, when_stuck_parrent);
        
        copy_when_stuck_object = Instantiate(when_stuck_object, transform.position - new Vector3(90, -2, -20),
            Quaternion.identity, when_stuck_parrent);
    }
    

}
