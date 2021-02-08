using UnityEngine;

public class KeyCombo : MonoBehaviour
{
    [HideInInspector]
    public KeyCode[] buttons;
    private int currentIndex = 0;                       //moves along the array as buttons are pressed

    public float allowedTimeBetweenButtons = 0.3f; 
    private float timeLastButtonPressed;
    string previousButton;
    public bool buttonHeld;

    public KeyCombo(KeyCode[] b)
    {
        buttons = b;
    }


    public bool Check()
    {
        if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex = 0;
        {                               
            if (currentIndex < buttons.Length && buttons != null && (Time.time > timeLastButtonPressed))  
            {
                if (buttons[currentIndex] == KeyCode.L && Input.GetKeyDown(KeyCode.L) ||
                (buttons[currentIndex] == KeyCode.D && Input.GetKeyDown(KeyCode.D)) ||
                (buttons[currentIndex] == KeyCode.Alpha4 && Input.GetKeyDown(KeyCode.Alpha4)) ||
                (buttons[currentIndex] != KeyCode.L && buttons[currentIndex] != KeyCode.D && buttons[currentIndex] != KeyCode.Alpha4 && Input.GetKeyDown(buttons[currentIndex])))
                {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;

                    //previousButton = buttons[currentIndex];  //JT
                }

                if (currentIndex >= buttons.Length)
                {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }

        return false;
    }
}
