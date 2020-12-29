using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monster_bloom : MonoBehaviour
{
    SkinnedMeshRenderer _renderer;
    Shader shader;
    Texture texture;
    Color color;

    [Range(0, 255)] public int color_;
    
    // Start is called before the first frame update
    void Start()
    {
        //_renderer = GetComponent<SkinnedMeshRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        // _renderer.material.SetColor("_EmmisionColor",new Color(color_,0,0));
        // _renderer.materials[0].SetColor("_EmmisionColor",new Color(color_,0,0));
    }
}
