using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    //public static CameraZoom Instance;

    [Header("Zoom Properties")]
    public bool allowZooming;
    public float zoomedInSize = 4;
    public float zoomedOutSize = 7;
    public float TransitionTime = 0.35f;


    // TODO:  
    //[Header("Background Scaling")]
    //public bool scaleStaticBackground;
    //public float backgroundScaleZoomedIn;
    //public float backgroundScaleZoomedOut;
    //private float currentBackgroundScale;

    // [HideInInspector]
    // public CinemachineVirtualCamera cam;
    Camera cam;
    [HideInInspector]
    public bool zooming;
   
    public float currentZoom;
    private float lerpTime;

    //private void Awake()
    //{
    //    if (Instance == null)
    //        Instance = this;
    //    else
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    DontDestroyOnLoad(gameObject);
    //}

    public float initialZoom;

    private void Start()
    {
        //  cam = GetComponent<CinemachineVirtualCamera>();
        cam = GetComponent<Camera>();
        allowZooming = true;

        initialZoom = cam.fieldOfView;
    }


    void Update()
    {
        if (!GameManager.Instance.gamePaused && allowZooming && GameManager.Instance.playerHasControl && (Input.GetKeyDown(InputManager.Instance.zoom)))
        {
            zooming = true;
        }

        if (zooming)
        {
            ChangeFOV();
        }
    }

    void ChangeFOV()
    {
        // Haven't reached our target zoom level yet
        if (Mathf.Abs(currentZoom - zoomedOutSize) > float.Epsilon)
        {
            lerpTime += Time.deltaTime;
            var t = lerpTime / TransitionTime;

            //Different ways of interpolation if you comment them all it is just a linear lerp
            t = Mathf.SmoothStep(0, 1, t); //Mathf.SmoothStep() can be used just like Lerp, here it is used to calc t so it works with the other examples.
            t = SmootherStep(t);
            //t = t * t;
            //t = t * t * t;

            currentZoom = Mathf.Lerp(zoomedInSize, zoomedOutSize, t);

        }
        // We've reached our target zoom level
        else if (Mathf.Abs(currentZoom - zoomedOutSize) < float.Epsilon)
        {
            lerpTime = 0;
            var tmp = zoomedInSize;
            zoomedInSize = zoomedOutSize;
            zoomedOutSize = tmp;

            zooming = false;
        }

       // cam.m_Lens.OrthographicSize = currentZoom;  // for cinemachine
       // cam.orthographicSize = currentZoom;  // plain ortho
        cam.fieldOfView = currentZoom;
    }

    private float SmootherStep(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }


}
