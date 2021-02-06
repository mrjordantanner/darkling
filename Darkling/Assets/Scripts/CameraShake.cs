using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class CameraShake : MonoBehaviour {

    #region Singleton
    public static CameraShake Instance;

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

    #endregion

    float strength = 0.2f;
    float duration = 0.2f;
    float timer;
    Camera cam;
    public Camera gunCam, explosionCam;
    bool shouldShake = false;

    Vector2 startPosition, gunStartPosition, explosionStartPosition;


    void Start () {
        cam = GetComponent<Camera>();
        gunCam = GetComponent<Camera>();
        explosionCam = GetComponent<Camera>();
        //cam = GetComponent<CinemachineVirtualCamera>().transform;
        startPosition = cam.transform.localPosition;
        gunStartPosition = gunCam.transform.localPosition;
        explosionStartPosition = explosionCam.transform.localPosition;
    }



    public void Shake(float str, float dur)
    {
        duration = dur;
        strength = str;
        shouldShake = true;
    }

    void Update ()
    {

		if (shouldShake)
        {
            if (timer > 0 )
            {
                cam.transform.localPosition = startPosition + Random.insideUnitCircle * strength;
                gunCam.transform.localPosition = startPosition + Random.insideUnitCircle * strength;
                explosionCam.transform.localPosition = startPosition + Random.insideUnitCircle * strength;
                timer -= Time.unscaledDeltaTime;
            }
            else
            {
                shouldShake = false;
                timer = duration;
                cam.transform.localPosition = startPosition;
                gunCam.transform.localPosition = startPosition;
                explosionCam.transform.localPosition = startPosition;
            }
                  
        }
	}
}
