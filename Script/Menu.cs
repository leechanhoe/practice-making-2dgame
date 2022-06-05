using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu instance;


    public GameObject go;
    public AudioManager theAudio;
    public OrderManager theOrder;

    public GameObject[] gos;

    public string call_sound;
    public string cancel_sound;

    private bool activated;

    private void Awake()
    {
        if (instance == null) // 파괴방지
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    public void Exit()
    {
        Application.Quit(); // 게임 종료시킴
    }

    public void Continue()
    {
        activated = false;
        go.SetActive(false);
        theAudio.Play(cancel_sound);
        theOrder.Move();
    }

    public void GoToTitle()
    {
        for (int i = 0; i < gos.Length; i++)
            Destroy(gos[i]);
        go.SetActive(false);
        activated = false;
        SceneManager.LoadScene("Title");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            activated = !activated;

            if(activated)
            {
                theOrder.NotMove();
                go.SetActive(true);
                theAudio.Play(call_sound);
            }
            else
            {
                go.SetActive(false);
                theAudio.Play(cancel_sound);
                theOrder.Move();
            }
        }
    }
}
