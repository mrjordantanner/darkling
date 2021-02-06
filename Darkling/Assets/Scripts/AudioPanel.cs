using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioPanel : MonoBehaviour
{
    public Slider masterSlider, ambientSlider, soundSlider, musicSlider;
    public float masterDefault, ambientDefault, soundDefault, musicDefault;


    void Start()
    {
        masterSlider.value = masterDefault;
        ambientSlider.value = ambientDefault;
        soundSlider.value = soundDefault;
        musicSlider.value = musicDefault;

    }

}

