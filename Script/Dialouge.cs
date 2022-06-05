using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialouge 
{
    [SerializeField]
    [TextArea(1,2)] // 공간 2줄
    public string[] sentences;
    public Sprite[] sprites;
    public Sprite[] dialogueWindows;
}
