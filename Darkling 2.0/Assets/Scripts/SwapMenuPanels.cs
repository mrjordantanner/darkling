using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMenuPanels : MonoBehaviour
{
    public CanvasGroup oldPanel;
    public Animator oldAnim;
    public CanvasGroup newPanel;
    public Animator newAnim;

    private void Start()
    {
       // oldAnim = oldPanel.GetComponent<Animator>();
       // newAnim = newAnim.GetComponent<Animator>();
    }

    public void SwapPanels()
    {
        GameManager.Instance.CloseMenuPanel(oldPanel, oldAnim);
        GameManager.Instance.OpenMenuPanel(newPanel, newAnim);
    }

}
