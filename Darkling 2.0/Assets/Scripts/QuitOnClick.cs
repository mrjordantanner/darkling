using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitOnClick : MonoBehaviour
{
    Button button;

    public void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { Quit(); });
    }

    private void Quit()
    {
      //  yield return new WaitForSeconds(1f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit();

#endif  
    }
}
