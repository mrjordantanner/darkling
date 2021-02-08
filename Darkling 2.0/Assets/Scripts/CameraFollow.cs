using UnityEngine;
using System.Collections;
using Gamekit2D;

public class CameraFollow : MonoBehaviour {

	//public Controller2D target;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	//public Vector2 focusAreaSize;

	//FocusArea focusArea;

    /*
	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;

	bool lookAheadStopped;
    */

    public Transform target;
    Vector3 velocity = Vector3.zero;
    public float smoothTime = 0f;

    // enable and set the max Y value
    public bool YMaxEnabled = false;
    public float YMaxValue = 0;

    // enable and set min Y value
    public bool YMinEnabled = false;
    public float YMinValue = 0;

    // enable and set the max X value
    public bool XMaxEnabled = false;
    public float XMaxValue = 0;

    // enable and set min X value
    public bool XMinEnabled = false;
    public float XMinValue = 0;

    void Start() {
        //focusArea = new FocusArea (target.collider.bounds, focusAreaSize);
        target = FindObjectOfType<PlayerCharacter>().transform;
	}

    void FixedUpdate()
    {
       
    }

	void LateUpdate() {
        //focusArea.Update (target.collider.bounds);

        //Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        //currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        //focusPosition.y = Mathf.SmoothDamp (transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        //focusPosition += Vector2.right * currentLookAheadX;
        //transform.position = (Vector3)focusPosition + Vector3.forward * -10;

        //target pos
        Vector3 targetPos = target.position;

        //vertical
        if (YMinEnabled && YMaxEnabled)
            targetPos.y = Mathf.Clamp(target.position.y, YMinValue, YMaxValue);
        else if (YMinEnabled)
            targetPos.y = Mathf.Clamp(target.position.y, YMinValue, target.position.y);
        else if (YMaxEnabled)
            targetPos.y = Mathf.Clamp(target.position.y, target.position.y, YMaxValue);

        //horizontal
        if (XMinEnabled && XMaxEnabled)
            targetPos.x = Mathf.Clamp(target.position.x, XMinValue, XMaxValue);
        else if (XMinEnabled)
            targetPos.x = Mathf.Clamp(target.position.x, XMinValue, target.position.x);
        else if (XMaxEnabled)
            targetPos.x = Mathf.Clamp(target.position.x, target.position.x, XMaxValue);


        // align camera and the targets' Z pos
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0f);

    }



	//void OnDrawGizmos() {
	//	Gizmos.color = new Color (1, 0, 0, .1f);
	//	Gizmos.DrawCube (focusArea.centre, focusAreaSize);
	//}
	//struct FocusArea {
	//	public Vector2 centre;
	//	public Vector2 velocity;
	//	float left,right;
	//	float top,bottom;


	//	public FocusArea(Bounds targetBounds, Vector2 size) {
	//		left = targetBounds.center.x - size.x/2;
	//		right = targetBounds.center.x + size.x/2;
	//		bottom = targetBounds.min.y;
	//		top = targetBounds.min.y + size.y;
//
	//		velocity = Vector2.zero;
	//		centre = new Vector2((left+right)/2,(top +bottom)/2);
	//	}


/*
		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left+right)/2, (top +bottom)/2);
			velocity = new Vector2 (shiftX, shiftY);
		}
	}
    */

}
