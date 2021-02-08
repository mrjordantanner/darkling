using UnityEngine;

public class PlayerRef : MonoBehaviour {


    public static PlayerRef Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }


    public PlayerCharacter player;

    void Start()
    {
      //  player = FindObjectOfType<PlayerCharacter>();
    }

    public void UpdatePlayer(GameObject NewPlayer)
    {
      //  player = NewPlayer.GetComponent<PlayerCharacter>();
    }





}
