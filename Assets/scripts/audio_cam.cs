using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_cam : MonoBehaviour
{
    public AudioSource AudioSource;
    static public float volume=1f;

    private Vector3 start_pos; 
    static public int vib_count = 0;
    private int vib_count_max = 10;
    static public bool vib_bool=false;
    
    // Start is called before the first frame update
    void Start()
    {
        start_pos = transform.localPosition;
        AudioSource.volume = volume;
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource.volume = volume;

        if (vib_bool && vib_count<=vib_count_max)
        {
            transform.localPosition = transform.localPosition + new Vector3(UnityEngine.Random.Range(-1f,1f),
                UnityEngine.Random.Range(-1f,1f),
                UnityEngine.Random.Range(-1f,1f));

            vib_count++;
        }

        if (vib_count>vib_count_max)
        {
            vib_bool = false;
            vib_count = 0;
            transform.localPosition = start_pos;
        }
    }
    
    
}
