using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameOnClick : MonoBehaviour
{
    public float fadeDuration;
    Button button;

    public void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { StartTransition(fadeDuration); });
    }

    public void StartTransition(float duration)
    {
        AudioManager.Instance.Play("Start");
        StartCoroutine(GameManager.Instance.SceneTransition(duration));

        // Close User Info panel
        GameManager.Instance.CloseMenuPanel(GameManager.Instance.userInfoPanel, GameManager.Instance.userInfoPanelAnim);

      
    }
}
