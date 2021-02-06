using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {

    public EventSystem eventSystem;
    public GameObject SelectedObject;

    private bool buttonSelected;


	void Start ()
    {   
        // ?
        if (buttonSelected == true) ;  
        eventSystem.SetSelectedGameObject(SelectedObject);

    }
	

	void Update ()
    {
		if (Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(SelectedObject);
            buttonSelected = true;

        }

	}

    private void OnDisable()
    {
        buttonSelected = false;

    }
}
