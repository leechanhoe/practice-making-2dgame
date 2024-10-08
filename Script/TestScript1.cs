﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript1 : MonoBehaviour
{
    private BGMManager BGM;

    public int playMusicTrack;

    // Start is called before the first frame update
    void Start()
    {
        BGM = FindObjectOfType<BGMManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BGM.Play(playMusicTrack);
        this.gameObject.SetActive(false);
    }
}
