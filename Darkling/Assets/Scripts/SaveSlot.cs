using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SaveSlot : MonoBehaviour
{
    // Sits on User buttons in UI

    public int index;
    public string slotID;    // used to determine which local file path to load from/save to, also could be used for sorting button order
    public User user;
    [HideInInspector]
    public Text buttonText;
    [HideInInspector]
    public Button button;
    public bool inUse;
    
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(delegate { UserController.Instance.SetActiveUser(user); });
        button.onClick.AddListener(delegate { GameManager.Instance.ShowUserInfo(); });

    }

    public void SetButton(User newUser)
    {
        inUse = true;
        button.interactable = true;
        user = newUser;
        buttonText.text = newUser.userName;
        newUser.saveSlot = this;
    }

    public void ClearButton()
    {
        inUse = false;
        button.interactable = false;
        buttonText.text = "";
        user = null;

    }

}
