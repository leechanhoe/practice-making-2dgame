using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    private FadeManager theFade;
    private AudioManager theAudio;
    private PlayerManager thePlayer;
    private GameManager theGM;

    public GameObject hpbar;
    public GameObject mpbar;

    public string click_sound;

    // Start is called before the first frame update
    void Start()
    {
        theFade = FindObjectOfType<FadeManager>();
        theAudio = FindObjectOfType<AudioManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theGM = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        theFade.FadeOut();
        theAudio.Play(click_sound);
        yield return new WaitForSeconds(2f);
        Color color = thePlayer.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        thePlayer.GetComponent<SpriteRenderer>().color = color;
        thePlayer.currentMapName = "Start";
        thePlayer.currentSceneName = "Start";
        hpbar.SetActive(true);
        mpbar.SetActive(true);

        theGM.LoadStart();
        SceneManager.LoadScene("Start");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
