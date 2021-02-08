using UnityEngine;
using UnityEngine.UI;

public class SwapMenuButton : MonoBehaviour
{
    // Switches resume/start button in Main Menu
    public Button startButton, resumeButton;

    public void SwitchButtons()
    {
        startButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);
    }



}
