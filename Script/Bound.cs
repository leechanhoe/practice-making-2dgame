using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    private BoxCollider2D bound;
    public string boundName; // 나중에 불러올 때 바운드도 따라와야 카메라가 따라오기 때문
    private CameraManager theCamera;

    void Start()
    {
        bound = GetComponent<BoxCollider2D>();
        theCamera = FindObjectOfType<CameraManager>();
        theCamera.SetBound(bound);
    }

    public void SetBound()
    {
        if(theCamera != null)
        {
            theCamera.SetBound(bound);
        }
    }
}
