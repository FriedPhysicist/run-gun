using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class home_mat : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        Color color = new Color(UnityEngine.Random.Range(0, 4), UnityEngine.Random.Range(0, 4), UnityEngine.Random.Range(0, 4),1);
        _meshRenderer.material.SetColor("_BaseColor",color);
    }
}
