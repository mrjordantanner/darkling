using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    public bool blink = false;
    public Color color1, color2;
    public float blinkRate;
    float timer;
    Text text;

    void Start()
    {
        text = GetComponent<Text>();
        text.color = color1;
        timer = blinkRate;
    }

    void Update()
    {
        if (blink && text.text != "")
        {
            timer -= Time.unscaledDeltaTime;

            if (timer <= 0)
            {
                if (text.color == color1)
                {
                    text.color = color2;
                }
                else
                    text.color = color1;

                timer = blinkRate;
            }

        }

    }
}
