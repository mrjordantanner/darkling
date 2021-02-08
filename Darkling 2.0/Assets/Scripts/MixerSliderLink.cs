using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]
public class MixerSliderLink : MonoBehaviour
{
    public AudioMixer mixer;
    public string mixerParameter;

    public float maxAttenuation = 0.0f;
    public float minAttenuation = -80.0f;

    protected Slider slider;

    void Awake ()
    {
        slider = GetComponent<Slider>();
        mixer.GetFloat(mixerParameter, out float value);
        slider.value = (value - minAttenuation) / (maxAttenuation - minAttenuation);
        slider.onValueChanged.AddListener(SliderValueChange);
    }


    void SliderValueChange(float value)
    {
       // var atten = minAttenuation + value * (maxAttenuation - minAttenuation);
       // mixer.SetFloat(mixerParameter, atten);

        mixer.SetFloat(mixerParameter, Mathf.Log10(value) * 20);

    }
}
