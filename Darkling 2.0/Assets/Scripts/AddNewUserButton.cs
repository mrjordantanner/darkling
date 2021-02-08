using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddNewUserButton : MonoBehaviour
{
    Button button;
    public SwapMenuPanels swapPanel;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { StartUserNameCheck(); });
    }


    private void Update()
    {
        // Causing problems
        //if (GameManager.Instance.usernameInputField.isFocused && GameManager.Instance.usernameInputField.text != "" && Input.GetKey(KeyCode.Return))
        //{
        //    StartCoroutine(StartUserNameCheck());

        //}

        if (GameManager.Instance.usernameInputField.text.Length == 0)
        {

            button.interactable = false;
        }
        else
            button.interactable = true;

    }

    void StartUserNameCheck()
    {
        //if (GameManager.Instance.usernameInputField.text.Length == 0)
        //{
        //    print("Prevented blank username entry.");
        //    return;
        //}

        StartCoroutine(UserNameCheck());
    }


    IEnumerator UserNameCheck()
    {

        print("Checking user name...");
        Dreamlo.Instance.DownloadAll();
        yield return new WaitForSeconds(0.5f);
        CheckUserName(GameManager.Instance.usernameInputField.text);

    }


    void CheckUserName(string newUserName)
    {

        if (Dreamlo.Instance.UsernameUnique(newUserName))
        {
            UserController.Instance.CreateUser(newUserName);
            GameManager.Instance.usernameInputField.text = "";
            swapPanel.SwapPanels();
        }
        else
        {
            GameManager.Instance.ShowUserNameTakenPanel();
        }
    }


    public void GiveInputFieldFocus()
    {
        GameManager.Instance.usernameInputField.ActivateInputField();
    }


    ////Called when Input changes
    //private void InputSubmitCallBack()
    //{
    //    StartCoroutine(StartUserNameCheck());

    //    print("InputField Submitted");
    //    GameManager.Instance.usernameInputField.text = "";                               //Clear Inputfield text
    //    GameManager.Instance.usernameInputField.ActivateInputField();                   //Re-focus on the input field
    //    GameManager.Instance.usernameInputField.Select();                              //Re-focus on the input field
    //}

    //////Called when Input is submitted
    ////private void inputChangedCallBack()
    ////{
    ////    Debug.Log("Input Changed");
    ////}

    //void OnEnable()
    //{
    //    //Register InputField Events
    //    GameManager.Instance.usernameInputField.onEndEdit.AddListener(delegate { InputSubmitCallBack(); });
    //    //GameManager.Instance.usernameInputField.onValueChanged.AddListener(delegate { inputChangedCallBack(); });
    //}

    //void OnDisable()
    //{
    //    //Un-Register InputField Events
    //    GameManager.Instance.usernameInputField.onEndEdit.RemoveAllListeners();
    //    GameManager.Instance.usernameInputField.onValueChanged.RemoveAllListeners();
    //}

}
