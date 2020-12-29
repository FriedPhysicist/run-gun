using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;



public class monster_script : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer _renderer;
    
    [Header("Patrols")] 
    [SerializeField] 
    private Vector3[] patrol_poses;
    private Vector3 to_go;
    private bool can_go_else_where = false;
    GameObject target;
    bool _getback=false;
    private SkinnedMeshRenderer material_;
    
    [Header("Fight")]
    public float health;
    private Animator _animator;
    public bool anim_bool=false;
    [SerializeField]
    private string[] _animations;
    bool death_bool=false;
    private String current_anim;
    public BoxCollider _Collider;
    
    public float speed;
    bool can_run=true;
    private float distance_betweeen_e = 7.5f;

    public bool hit_event=false;
    private float distance_b_player;

    [Header("audio")]
    public AudioClip death_big;
    public AudioClip hit_sound;
    public AudioSource sfx;
    private bool audio_protector = true;
    
    public ParticleSystem ParticleSystem;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        material_ = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

        sfx = GetComponent<AudioSource>();
        
        switch (Random.Range(0,3))
        {
            case 0:
                material_.material.SetColor("_EmissionColor", Color.green*60);
                break;
            case 1:
                material_.material.SetColor("_EmissionColor", Color.red*110);
                break;
            case 2:
                material_.material.SetColor("_EmissionColor", Color.blue*100);
                break;
        }

        _animator = GetComponent<Animator>();
        patrol();
    }

    void Update()
    {
        target = GameObject.FindWithTag("valk");
        
        distance_b_player = Vector3.Distance(target.transform.position, transform.position);
        
        if (transform.position.y>=30 ||transform.position.y<=-2)
        {
            Destroy(gameObject,0f);
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("impact") && !valkriye_.event_attack)
        {
            _animator.SetBool("impact",false);
            make_it_false();
        }
        
        _animator.SetBool("idle", (_animator.GetCurrentAnimatorStateInfo(0).IsName("impact") && !can_run && !anim_bool));
        
        //get variable from valkriye_
        pause_game(valkriye_.pause_bool);
        fight_and_death();
    }

    private bool go_back = false;
    
    void FixedUpdate()
    {
        run();
        
        if(!death_bool) patroling();

        while (go_back)
        {
            rb.isKinematic = false;
            rb.AddRelativeForce(new Vector3(0,2f,-5f)*1000f,ForceMode.Acceleration);
            rb.velocity =new Vector3(0,-1,0)* speed;
            break;
        }
    }

    private void patroling()
    {
        //move and look at to the target if distance between <50 and >5
        if (distance_b_player < 10)
        {
            //transform.position = Vector3.Lerp(transform.position,target.transform.position,0.01f);
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        }

        //if target further than 50 then move around          change this
        else if (distance_b_player >= 10)
        {
            //when reach the destination, create new one and make true to go
            if (!can_go_else_where)
            {
                to_go = patrol_poses[UnityEngine.Random.Range(0, 4)];
                can_go_else_where = true;
            }

            while (can_go_else_where)
            {
                //transform.position = Vector3.Lerp(transform.position,to_go,0.07f);
                transform.LookAt(new Vector3(to_go.x, transform.position.y, to_go.z));
                
                //transform.LookAt(new Vector3(to_go.x, transform.position.y, to_go.z));
                if (Vector3.Distance(transform.position, to_go) <= 2f)
                {
                    can_go_else_where = false;
                }

                break;
            }
        }
    }

    bool for_once=true;
    
    void fight_and_death()
    {
        if (distance_b_player < distance_betweeen_e)
        {
            _animator.SetBool("impact",valkriye_.afight_bool && valkriye_.event_attack && !death_bool);
            
            can_run = false;
            
            if (health>0 && !valkriye_.afight_bool && !anim_bool && timer_boolean)
            {
                //fight starts here
                current_anim = _animations[Random.Range(0, 3)];
                anim_bool = true;
                _animator.SetBool(current_anim, true);
                timer_boolean = false;
                timer = 0;
            }

            //execut function when golem die
            if (health<=0)
            {
                audio_handler(death_big);
                make_it_false();
                rb.velocity=Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
                _Collider.isTrigger = true;
                transform.tag = "died";
                transform.parent = null;
                //go_back = true;
                death_bool = true;
                _animator.SetBool("death", true);
            }
            
        }
        
        if (distance_b_player >= distance_betweeen_e )
        {
            can_run = true;
            anim_bool = false;
        }
    }
    
    //execute to make false every animation
    void make_it_false()
    {
        //for_once_collider = true;
        for_once = true;
        anim_bool = false;
        _animator.SetBool(current_anim,false);
        
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("impact"))
        {
            _animator.SetBool("impact",false);
        }
        
        hit_event = false;

        if (health>0)
        {
            audio_protector = true;
        }
    }

    
    
    //get damage from valkriye_
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("valk") && valkriye_.event_attack && for_once && valkriye_.afight_bool)
        {
            for_once = false;
            hit_value();
        }
    }


    void run()
    {
        if (can_run)
        {
            rb.isKinematic = false;
            if (!go_back)
            {
                //rb.velocity =new Vector3(0,-1,1)* speed;
                rb.velocity =(transform.forward+transform.up*-1/5)* speed;
            }
        }
        
        if (!can_run)
        {
            if (!death_bool)
            {
                rb.isKinematic = true;
            }
            
            rb.velocity = Vector3.zero;
        }
        
    }

    private float timer;
    public float timer_public;
    private bool timer_boolean = false;
    
    private void timer_func()
    {
        timer += 0.1f;

        if (timer >= timer_public)
        {
            timer_boolean = true;
            timer = 0;
        }
    }
    
    void patrol()
    {
        for (int i = 0; i < 4; i++)
        {
            patrol_poses[i] = new Vector3(  transform.position.x - UnityEngine.Random.Range(-20, 20),
                                            transform.position.y + 2.5f, 
                                            transform.position.z - UnityEngine.Random.Range(-20, 20));
        }
    }
    
    void pause_game(bool pause_bool)
    {
        if(pause_bool) Time.timeScale = 0;
        if(!pause_bool) Time.timeScale = 1;
    }

    public ParticleSystem critical_particle;
    private float critical_chance = 10;
    private float random_critical_chance;    
    
    private void hit_value()
    {
        random_critical_chance = Random.Range(0, 100);
        
        if(random_critical_chance>critical_chance)
        {
            health -= valkriye_.damage;
        }

        if (random_critical_chance<=critical_chance)
        {
            health -= health + 5;
            critical_particle.GetComponent<ParticleSystemRenderer>().material = material_.material;
            critical_particle.Play();
        }
        
      
        ParticleSystem.Play();
        timer_boolean = false;
        timer = 0;
    }
    
    void audio_handler(AudioClip clip)
    {
        if (audio_protector)
        {
            sfx.clip = clip;
            sfx.Play();
            audio_protector = false;
        }
    }
    
    void audio_event()
    {
        //audio that charr get damage
        //when monster hit to the valkriye_
        if (audio_protector && !valkriye_.afight_bool && distance_b_player<=12f)
        {
            sfx.clip =hit_sound;
            valkriye_.impact_boolean = true;
            valkriye_.healt_ -= valkriye_.get_damage*2;
            audio_cam.vib_bool = true;
            valkriye_.score_number=0;
            
            if (valkriye_.current_letter>=1)
            {
                valkriye_.current_letter--;
            }
            
            sfx.Play();
            timer_boolean = false;
            timer = 0;
            audio_protector = false;
        }
        
        audio_protector = true;
    }
    
    void hit_event_fun()
    {
        hit_event = true;
    }
}

