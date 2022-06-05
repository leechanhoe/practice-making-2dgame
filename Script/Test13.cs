using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[System.Serializable]

public class Test13 : MonoBehaviour
{
    [SerializeField]
    public Dialouge dialouge;

    private DialogueManager theDM;
    // Start is called before the first frame update
    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            theDM.ShowDialogue(dialouge);
        }
    }
}
