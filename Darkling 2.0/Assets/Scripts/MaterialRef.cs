using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRef : MonoBehaviour {

    public static MaterialRef Instance;

    public Material normal, white, frozen, red;

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

    }
