using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class Parallaxing : MonoBehaviour {

    //TODO:  REFINE / REDESIGN THIS!

    public Transform[] backgrounds;
    private float[] parallaxScales;
    public float smoothing = 1f;

    private Transform cam;
    private Vector3 previousCamPos;

    void Awake()
    {
        cam = Camera.main.transform;
        //cam = FindObjectOfType<CinemachineVirtualCamera>().transform;
    }

    void Start ()
    {
        previousCamPos = cam.position;

        parallaxScales = new float[backgrounds.Length];

        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].position = cam.position; // JT
            parallaxScales[i] = backgrounds[i].position.z * -1;
        }
	}
	
	void Update ()
    {
        //for each background
        if (backgrounds != null)
        {
            for (int i = 0; i < backgrounds.Length; i++)
            {
                //the parallax is the opposite of the camera movement b/c the previous frame is multiplied by the scale
                float parallaxX = (previousCamPos.x - cam.position.x) * parallaxScales[i];
                float parallaxY = (previousCamPos.y - cam.position.y) * parallaxScales[i];

                //set a target x position which is the current position plus the parallax
                float backgroundTargetPosX = backgrounds[i].position.x + parallaxX;
                float backgroundTargetPosY = backgrounds[i].position.y + parallaxY;

                //create a target position which is the background's current position with it's target x position
                //  Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgrounds[i].position.y, backgrounds[i].position.z);
                Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, backgroundTargetPosY, backgrounds[i].position.z);

                //fade between current position and the target position using lerp
                backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPos, smoothing * Time.deltaTime);
            }

            //set the previous cam position to the camera's position at the end of the frame
            previousCamPos = cam.position;
        }

	}
}
