using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test16 : MonoBehaviour
{

    private WeatherManager theWeather;
    public bool rain;

    // Start is called before the first frame update
    void Start()
    {
        theWeather = FindObjectOfType<WeatherManager>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rain)
            theWeather.Rain();
        else
            theWeather.RainStop();
    }
}
