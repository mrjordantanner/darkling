using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TimeAffectsAtmosphere : MonoBehaviour
{
    public PostProcessVolume volume;
  //  public PostProcessEffectSettings effectSettings;
   // public ColorGrading colorGradingLayer;
    //Light globalLight;

    public float changePerDuration = 0.001f;
    public float timer;
    public float duration = 1f;



    void Start()
    {
       // colorGradingLayer = GetComponent<ColorGrading>();
      //  PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
      //  volume.profile.TryGetSettings(out colorGradingLayer);

    }


    private void Update()
    {
        if (WaveController.Instance.waveInProgress)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                // globalLight.color = etc...
                volume.weight += changePerDuration;
               // colorGradingLayer.temperature.value += changePerDuration;
                timer = duration;
            }

        }

    }

    public void ResetToDefault()
    {
        volume.weight = 0;
    }


}
