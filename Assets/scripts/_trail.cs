using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class _trail : MonoBehaviour
{
    private Vector3 next_pos;
    public float locate_timer;
    public float pub_locate_timer;
    public float move_speed;
    private Vector3 start_pos;
    private GameObject c_enemy;
    private int hit_count=1;
    public ParticleSystem spark;
    public AudioClip spark_clip;
    public AudioSource AudioSource;
    public Slider value;
    
    private void Start()
    {
        next_pos=transform.localPosition;
        start_pos = transform.localPosition;
        AudioSource.volume = value.value;
    }

    void Update()
    {
        c_enemy = valkriye_.c_enemy;
        
        if (c_enemy!=null)
        {
            if (Vector3.Distance(c_enemy.transform.position,transform.position)>11)
            {
                hit_count = 1;
                if (UnityEngine.Random.Range(0,101)>=99)
                {
                    one_attack = true;
                }
            }
        }
        
        //transform.localPosition=new Vector3(UnityEngine.Random.Range(0,1),UnityEngine.Random.Range(0,1),UnityEngine.Random.Range(0,1));
        //transform.localPosition = start_pos;
    }

    private bool one_attack;
    void FixedUpdate()
    {
        while (!one_attack)
        {
            path();
            break;
        }


        while (c_enemy!=null)
        {
           if(!one_attack && Vector3.Distance(c_enemy.transform.position,transform.position)>=10) path();
           
           if (Vector3.Distance(c_enemy.transform.position,transform.position)<=10 && hit_count>=1 && one_attack) 
           {
               transform.position=Vector3.Lerp(transform.localPosition, c_enemy.transform.position, 1f);
           }
           
           if (Vector3.Distance(c_enemy.transform.position,transform.position)<=0.1f)
           {

               if (c_enemy.CompareTag("enemy"))
               {
                   c_enemy.GetComponent<monster_script_little>().health-=2;
               }
               
               if (c_enemy.CompareTag("enemy_2"))
               {
                   c_enemy.GetComponent<monster_script>().health-=2;
               }

               spark.Play();
               AudioSource.Play();
               one_attack = false;
               hit_count--;
           }
           
           break;
        }
    }

    private void path()
    {
        //move to location when time's up
        if (locate_timer >= pub_locate_timer)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, next_pos, move_speed);
        }

        //when object in the loc, start timer
        if (locate_timer <= pub_locate_timer)
        {
            locate_timer++;
        }

        //when object reach the loc, generate new pos and set timer to 0 again
        if (Vector3.Distance(transform.localPosition, next_pos) < 0.1f)
        {
            next_pos = new Vector3(UnityEngine.Random.Range(0, 20), UnityEngine.Random.Range(0, 3),
                                                                     UnityEngine.Random.Range(0, 20));
            locate_timer = 0;
        }
    }
}

