using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class valkriye_ : MonoBehaviour
{
    static public bool can_turn = true;
    Rigidbody rb;
    private bool zero_velocity = false;

    [Header("raycast and touch handler")] 
    Touch touch;
    bool ray_forward_c;
    bool ray_back_c;
    bool ray_left_c;
    bool ray_right_c;
    Ray _ray;
    private bool turn_right, turn_left, turn_fltf, turn_frtf = false;
    int turn_while_timer = 0;
    RaycastHit hit;

    [Header("speed and others according to char")]
    public float speed;

    [Header("decied driving direction")] private float look_d;
    private float x_speed;
    private float f_speed;
    private float xn_speed;
    private Vector3 speed_vector;

    [Header("colors")] public Material mat_;
    [Range(0, 100)] 
    static public float healt_ = 100;

    [Header("time travel")] 
    [SerializeField]
    private Vector3[] poses = new Vector3[300];
    private int ttp = 0;

    [Header("fight moves and anims")]
    static public bool afight_bool=false;
    //static public bool adefence_bool;
    public Animator anim;
    private float das=0;
    public static string anim_holder;
    [SerializeField]
    public String[] skills_a=new string[6];
    public Transform monster_parrent;
    public static GameObject c_enemy;
    public static bool can_go=true;
    float distance_betweeen_e;
    private bool _idle=false;
    private bool death_=false;
    public ParticleSystem flames;

    public Transform stuck_parrent;
    GameObject stuck_point;

    private bool u_can_escape=false;
    static public bool event_attack = false;
    static public float damage = 3;
    static public float get_damage=5;
    static public int jump_count=2;
    private float dynamic_distance;
    static public bool impact_boolean = false;
    
    [Header("audio")]
    public AudioClip roll;
    public AudioClip death_charr;
    public AudioClip hit_sound;

    public AudioSource sfx;

    private static float sfx_volume=1;

    private bool audio_protector = true;

    public GameObject spawner;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim=GetComponent<Animator>();
        sfx = GetComponent<AudioSource>();
        Canvas_Settings_Menu.enabled = false;
        Try_Again_Menu.enabled = false;
        sword_shild_menu.enabled = false;
        death_ = false;
        tmp_start_pos = tmp.transform.localPosition;
        healt_ = 100;
        start_point = transform.position.y;
    }

    
    void Update()
    {
        //idle and impact animation control
        anim.SetBool("idle",zero_velocity && !afight_bool && !impact_boolean && !death_ || Canvas_Menu.enabled || Canvas_Settings_Menu.enabled);
        anim.SetBool("impact", impact_boolean && !afight_bool && !death_ && !Canvas_Menu.enabled && !Canvas_Settings_Menu.enabled);

        //if c_enemy is null but there are monsters then get the first of the list
        if (c_enemy==null && monster_parrent.transform.childCount!=0)
        {
            if (monster_parrent.transform.childCount!=0)
            {
                c_enemy = monster_parrent.GetChild(0).gameObject;
            }
        } 
        
        //if c_enemy is null go forward
        while (monster_parrent.transform.childCount==0)
        {
            zero_velocity = false;
            transform.rotation=Quaternion.Euler(transform.rotation.x,0,transform.rotation.z);
            break;
        }

        touch_handler();
        time_travel();
        stuck_list_handler();
        score_canvas_handler();
        pause_game();
        
        //if c_enemy isn't equal to null then calculate distance between c_enemy and charr_
        while (c_enemy!=null)
        {
            dynamic_distance = Vector3.Distance(c_enemy.transform.position,transform.position);

            distance_betweeen_e = c_enemy.tag == "enemy" ? 8.5f : 9.5f;

            monster_list_handler();
            fight_things();
            
            if (c_enemy.CompareTag("died"))
            {
                c_enemy = null;
            }

            break;
        }

        when_die();
        
        //controlling sound level via menu
        sfx.volume = sfx_menu.value;
        audio_cam.volume = music_menu.value;
        //controlling sound level via menu

        if (Canvas_Menu.enabled || Canvas_Settings_Menu.enabled || Try_Again_Menu.enabled)
        {
            zero_velocity = true;
            score_menu.enabled = false;
        }
        
        if (!Canvas_Menu.enabled && !Canvas_Settings_Menu.enabled && !Try_Again_Menu.enabled)
        {
            score_menu.enabled = true;
        }
 
        
        mat_.SetColor("_EmissionColor", color_scheama(healt_));
    }


    void FixedUpdate()
    {
        //stop character if zero_velocity==true
        while (!zero_velocity)
        {
            rb.isKinematic = false;
            rb.velocity = (transform.forward * speed)+new Vector3(0,-15f,0);
            break;
        }
        
        while (zero_velocity)
        {
            //rb.isKinematic = true;
            rb.velocity = Vector3.zero+new Vector3(0,-15f,0);
            break;
        }
        
        while (roll_bool)
        {
            if (dynamic_distance < distance_betweeen_e)
            {
                roll_bool = false;
            }

            anim.SetTrigger("roll");

            rb.AddRelativeForce(new Vector3(0, 0, 1) * dynamic_distance * 100, ForceMode.Acceleration);
            break;
        }
        
        if (c_enemy!=null)
        {
            Looking();
        }
        
        combo_timer_func();
    }

    //have no idea what is that variable
    bool for_once = true;

    //animation and fight

    private int roll_or_jump;
    private int roll_or_jump_chance=0;

    public int combo_attack_number = 0;
    
    void fight_things()
    {
        //when finger moved, start to attacking with random attack animation (if you are not attacking)
        //if enemy is farther than 6f, then roll onto enemy
        
        if((touch.phase == TouchPhase.Moved || Input.GetKeyUp(KeyCode.Mouse0)) && !afight_bool && !death_)
        {
            _idle = false;
            anim.SetBool("idle",false);

            if (dynamic_distance>distance_betweeen_e)
            {
                roll_bool = true;
            }
            
            if (combo_timer>=combo_timer_public || combo_attack_number>=skills_a.Length)
            {
                combo_timer = 0;
                combo_attack_number = 0;
            }
            
            if(dynamic_distance < distance_betweeen_e && !roll_bool)
            {
                anim.SetTrigger(skills_a[combo_attack_number]);
                combo_attack_number++;
                afight_bool = true;
                //anim.SetTrigger(non_repeat_anim());
                //anim_holder = skills_a[Random.Range(0,skills_a.Length)];
            }
        }
        
        //dynamic_distance is current distanece, distance_betweeen_e is minimum distance to stop
        if (dynamic_distance < distance_betweeen_e)
        {
            zero_velocity = true;
            roll_bool = false;
        }
        
        if (dynamic_distance > distance_betweeen_e+1) 
        {
            zero_velocity = false;
            combo_attack_number = 0;
            combo_timer = 0;
        }
    }

    public int combo_timer = 0;
    public int combo_timer_public = 0;

    private void combo_timer_func()
    {
        if (!afight_bool)
        {
            combo_timer++;
        }
        
        if (afight_bool)
        {
            combo_timer=0;
        }
    }

    bool roll_bool=false;
    private float start_point;
    bool jump_attack_bool = false;
    private bool only_one_jump = false;
    private float jump_start_distance;
    
    void when_die()
    {
        //second_chance
        //while valkriye is dead, make every movement boolean false to stop
        if (healt_<=0)
        {
            anim.SetBool("death",true);
            death_ = true;
            
            if (!try_again && !Canvas_Menu.enabled && das<1)
            {
                Try_Again_Menu.enabled = true;
            }
            
            if (das>=1 && !try_again)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
            afight_bool = false;
            for_once = false;
            audio_handler(death_charr);
        }
        
        if (death_)
        {
            //when healt lower than or equal to 0, go back 300
            if (Vector3.Distance(transform.position,poses[0])>6.5f && try_again)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(poses[0].x,transform.position.y,poses[0].z),0.1f);
            }
            
            //second_chance

            //after lerp done, reset every thing
            while (Vector3.Distance(transform.position,poses[0])<=50f)
            {
                death_ = false;
                anim.SetBool("death",false);
                stop_event();
                try_again = false;
                break;
            }
            
        }
    }
    
    //protect to get hit only once a time
    
    private void OnTriggerStay(Collider other)
    {

        // if character does not attack and some monster attack, then make idle false, set impact on 
        if (other.CompareTag("enemy"))
        {
            // if (c_enemy.GetComponent<monster_script_little>().hit_event && !afight_bool)
            // {
            //     anim_holder = "impact";
            //     anim.SetBool(anim_holder,true);
            //     if (current_letter>=1)
            //     {
            //         current_letter--;
            //     }
            //     afight_bool = false;
            //     c_enemy.GetComponent<monster_script_little>().hit_event = false;
            // }
            
            if (c_enemy.GetComponent<monster_script_little>().hit_event && !afight_bool)
            {
                anim_holder = "impact";
                anim.SetBool(anim_holder,true);
                if (current_letter>=1)
                {
                    current_letter--;
                }
                afight_bool = false;
                c_enemy.GetComponent<monster_script_little>().hit_event = false;
            }
        }
        
        if ((other.CompareTag("enemy_2")))
        {
            if (c_enemy.GetComponent<monster_script>().hit_event && !afight_bool)
            {
                anim_holder = "impact";
                anim.SetBool(anim_holder,true);
                afight_bool = false;
                c_enemy.GetComponent<monster_script>().hit_event = false;
            }
        }
        
        if (other.CompareTag("fence") && !zero_velocity)
        {
            u_can_escape = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("home_fence") && !zero_velocity)
        {
            if (spawner.transform.position.x>transform.position.x)
            {
                rb.AddForce(Vector3.right*100,ForceMode.Acceleration);
            }
            
            if (spawner.transform.position.x<transform.position.x)
            {
                rb.AddForce(Vector3.right*-100,ForceMode.Acceleration);
            }
        }
    }

    void touch_handler()
    {
        if(Input.touchCount>0 && !death_)
        {
            touch = Input.GetTouch(0);
        }
    }

    float stop_event_timer_second;
    
    //will execute after any animation end (via animation event)
    public void stop_event()
    {
        afight_bool = false;
        event_attack = false;
        stop_event_timer_second = anim.GetCurrentAnimatorStateInfo(0).length -
                                  anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        StartCoroutine(stop_event_timer(stop_event_timer_second));
    }

    
    IEnumerator stop_event_timer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        anim.SetBool("impact",false);
        audio_protector = true;
        impact_boolean = false;
        jump_attack_bool = false;
    }
    
    public void stop_event_roll()
    {
        anim.SetBool("impact",false);
        afight_bool = false;
        event_attack = false;
        audio_protector = true;
        impact_boolean = false;
        jump_attack_bool = false;
    }
    
    private string prev_anim=" ";
    string non_repeat_anim()
    {
        prev_anim = anim_holder;
        anim_holder = skills_a[Random.Range(0, skills_a.Length)];
        return anim_holder != prev_anim ? anim_holder : non_repeat_anim();
    }

    //handeling color of object depend on healt
    Color color_scheama(float currentHealt)
    {
        if (currentHealt >= 70)
        {
            return new Color(255 - currentHealt * 2.55f, 0, 150);
        }

        if (currentHealt >= 41 && currentHealt <= 69)
        {
            return new Color(255 - currentHealt * 2.55f, 0, 50);
        }

        if (currentHealt >= 0 && currentHealt <= 40)
        {
            return new Color(255 - currentHealt * 2.55f, 0, 50);
        }

        return new Color(255, 0, 0);
    }



    //time travel
    void time_travel()
    {
        while (!zero_velocity && !death_)
        {
            //store 300 poss. , but 1 for a frame
            if (ttp <= 299)
            {
                poses[ttp] = transform.position;
                ttp++;
            }

            //if got all 300 frames, then slip 1 down all poss. and get current poss. to 299
            if (ttp>=300)
            {
                for (int i = 0; i <= 298; i++)
                {
                    poses[i] = poses[i+1];
                }
                poses[299] = transform.position;
            }
            
            break;
        }
    }
    
    
    //very basic algorithm to find closest enemy
    void monster_list_handler()
    {
        for (int i = 0; i < monster_parrent.childCount; i++)
        {
            float next_distance =Vector3.Distance(transform.position,
                                                    monster_parrent.GetChild(i).gameObject.transform.position);
            float current_distance = Vector3.Distance(transform.position,c_enemy.gameObject.transform.position);

            
            while (next_distance<=current_distance)
            {
                c_enemy = monster_parrent.GetChild(i).gameObject;
                break;
            }
        }
    }
    
    
    void stuck_list_handler()
    {
        if (stuck_parrent.childCount>0)
        {
            if (stuck_point==null)
            {
                stuck_point = stuck_parrent.GetChild(0).gameObject;
            }
        
            for (int i = 0; i < stuck_parrent.childCount; i++)
            {
                float next_distance =Vector3.Distance(transform.position,
                    stuck_parrent.GetChild(i).gameObject.transform.position);
                float current_distance = Vector3.Distance(transform.position,stuck_point.gameObject.transform.position);

            
                while (next_distance<=current_distance)
                {
                    stuck_point = stuck_parrent.GetChild(i).gameObject;
                    break;
                }
                
            }
        }
    }
    
    
    //rotate to c_enemy or stuck point
    private void Looking()
    {
        if (!u_can_escape)
        {
            Vector3 to_look=new Vector3(c_enemy.transform.position.x,transform.position.y,c_enemy.transform.position.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(to_look-transform.position), 0.1f);
        }

        if (u_can_escape)
        {
            transform.LookAt(new Vector3(stuck_point.transform.position.x,
                transform.position.y,
                stuck_point.transform.position.z));

            if (Vector3.Distance(transform.position, stuck_point.transform.position) < 2f)
            {
                u_can_escape = false;
            }
        }
    }

    //for protect the function to execute for once a time
    void event_hit()
    {
        event_attack = true;
    }

    //return audio on purpose
    void audio_handler(AudioClip clip)
    {
        if (audio_protector)
        {
            sfx.clip = clip;
            //sfx.volume = 1;
            sfx.Play();
            audio_protector = false;
        }
    }
    
    //audio when charr_ hit the monster
    void hit_event_fun()
    {
        event_attack = true;
       Debug.Log(dynamic_distance < distance_betweeen_e+1f); 
        if (dynamic_distance < distance_betweeen_e+1f)
        {
            sfx.clip = hit_sound;
            score_number_show++;
            score_number++;
            //flames.Play(); 
            audio_cam.vib_bool = true;
            sfx.Play();
        }
    }

    //pause game if pause_bool==true
    static public bool pause_bool=false;
    void pause_game()
    {
        if(pause_bool) Time.timeScale = 0.00001f;
        if(!pause_bool) Time.timeScale = 1;
    }
    
    //after this line, all about the menu and sound

    //D
    //B
    //C
    //A
    //S
    //SS
    //SSS
    //7

    //canvas_1
    [Header("Menu Settings")]
    //static public bool canvas_menu=true;
    //static public bool try_again_menu=true;
    public Canvas Canvas_Menu;
    public Canvas Canvas_Settings_Menu;
    public Canvas Try_Again_Menu;
    public Canvas score_menu;
    public Canvas sword_shild_menu;

    static public bool try_again = false;

    public Slider sfx_menu;
    public Slider music_menu;
    public Slider score_pointer;
    
    private int score;
    public TMP_Text tmp;
    public TMP_Text tmp_number;
    
    public String[] letters=new string[7];
    
    static public int current_letter = 0;
    
    private Vector3 tmp_start_pos;
    
    private int vib_count = 0;
    private int vib_count_max = 5;
    private bool vib_bool=false;
    private int vib_move;
    static public int score_number=0;
    private int score_number_show=0;

    private int armour_que=0;
    private int sword_que=0;

    [SerializeField]
    public GameObject[] sword_list = new GameObject[4];
    
    [SerializeField]
    public GameObject[] armour_list = new GameObject[2];

    public void menu_start_button()
    {
        if (das>=1)
        {
            anim.SetBool("impact",false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        healt_ = 100;
        Canvas_Menu.enabled = false;
    }

    public void menu_settings_button()
    {
        healt_ = 100;
        Canvas_Menu.enabled = false;
        Canvas_Settings_Menu.enabled = true;
        Try_Again_Menu.enabled = false;
    }

    public void settings_back_button()
    {
        healt_ = 100;
        Canvas_Menu.enabled = true;
        Canvas_Settings_Menu.enabled = false;
        Try_Again_Menu.enabled = false;
    }

    public void try_again_back_button()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    //second_chance

    public void try_again_secondchance_button()
    {
        Canvas_Menu.enabled = false;
        Canvas_Settings_Menu.enabled = false;
        Try_Again_Menu.enabled = false;
        healt_ = 100;
        das++;
        try_again = true;
    }

    void score_canvas_handler()
    {
        vib_bool = true;
        vib_move = current_letter;

        score_pointer.value = score_number;
        tmp.text = letters[current_letter];
        tmp_number.text = score_number_show.ToString();

        //if score reach to 10, level up letter
        if (score_number >= 10)
        {
            if(current_letter<6)
            {
                current_letter++;
                score_number += 5;
            }
            score_number = 0;
        }

        //power up after ever score_number_show
        if (/*score_number_show% 3==0 && score_number_show>0*/ false )
        {
            pause_bool = true;
            sword_shild_menu.enabled = true;
        }
        
        //vibrate combo level according to current_letter
        if (vib_bool && vib_count < vib_count_max)
        {
            tmp.transform.localPosition = tmp.transform.localPosition + new Vector3(UnityEngine.Random.Range(-vib_move,vib_move),
                                                                                    UnityEngine.Random.Range(-vib_move,vib_move),
                                                                                    UnityEngine.Random.Range(-vib_move,vib_move));
            
            vib_count++;
        }

        //reset vibration and start again
        if (vib_count>=vib_count_max)
        {
            vib_bool = false;
            vib_count = 0;
            tmp.transform.localPosition = tmp_start_pos;
        }
    }

    public void sword_button()
    {
        damage += damage * 0.5f;
        score_number_show += 1;

        if (sword_que < sword_list.Length)
        {
            sword_list[sword_que].SetActive(true);
            sword_que++;
        }
        
        pause_bool = false;
        sword_shild_menu.enabled = false;
    }
    
    public void shield_button()
    {
        get_damage -= get_damage * 0.5f;
        score_number_show += 1;

        // //don't ever never use empty_parrent for objects
        
        if (armour_que < armour_list.Length)
        {
            armour_list[armour_que].SetActive(true);
            armour_que++;
            armour_list[armour_que].SetActive(true);
            armour_que++;
        }

        pause_bool = false;
        sword_shild_menu.enabled = false;
    }

}

/*
    //return delta position, from began to end (return always positive)
    float delta_thing()
    {
        float touch_start = 0;
        float touch_ended = 0;

        if (touch.phase == TouchPhase.Began)
        {
            touch_start = touch.position.y;
        }

        else if (touch.phase == TouchPhase.Ended)
        {
            touch_ended = touch.position.y;
        }

        return Math.Abs(touch_ended - touch_start);
    }
 */