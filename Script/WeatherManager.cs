using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    static public WeatherManager instance;

    void Awake()
    {
        // 중복으로 생기는거 방지
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }

    private AudioManager theAudio;

    public ParticleSystem rain;
    public string rain_sound;
       
    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
    }

    public void Rain()
    {
        theAudio.Play(rain_sound);
        rain.Play();
    }

    public void RainStop()
    {
        theAudio.Stop(rain_sound);
        rain.Stop();
    }

    public void RainDrop()
    {
        rain.Emit(10);//10방울만 내리기
    }
}
