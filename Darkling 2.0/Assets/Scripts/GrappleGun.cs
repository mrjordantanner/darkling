using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrappleGun : MonoBehaviour
{
    // Different modes:  Retract or Swing

    public float range;
    public bool targetInRange;
    public float retractSpeed;
    public bool retracting;
    public GrapplePoint targetGrapplePoint;
    public GrapplePoint lockedGrapplePoint;
    PlayerCharacter player;
    //  Camera cam;
    public bool canShoot;
    public Ease retractEase;
    public float distanceToTarget;
    public float grappleDuration;
    public float minGrappleDuration, maxGrappleDuration;

    private void Start()
    {
        player = GetComponent<PlayerCharacter>();
        // cam = Camera.main;
        canShoot = true;
    }

    private void Update()
    {
        if (targetGrapplePoint != null)
        {
            distanceToTarget = CalculateDistance();
            grappleDuration = CalculateGrappleDuration(distanceToTarget);
            targetInRange = RangeCheck();
        }
        else
        {
            grappleDuration = 0;
            distanceToTarget = 0;
        }

        if (Input.GetKeyDown(InputManager.Instance.grapple) && targetGrapplePoint != null && canShoot && !retracting && targetInRange)
        {
                StartCoroutine(Fire());
        }

    }


    bool RangeCheck()
    {
        bool inRange = false;

        if (distanceToTarget <= range)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }

        targetGrapplePoint.inRange = inRange;
        return inRange;
    }


    IEnumerator Fire()
    {
        canShoot = false;
        lockedGrapplePoint = targetGrapplePoint;
        //turned off player movement in controller
        retracting = true;

        // yield return new WaitForSeconds(shootSpeed);
        var duration = CalculateGrappleDuration(distanceToTarget);
        player.gameObject.transform.DOMove(lockedGrapplePoint.transform.position, duration).SetEase(retractEase);

        yield return new WaitForSeconds(duration);

        // restored player movement in controller

        canShoot = true;
        retracting = false;
        lockedGrapplePoint = null;

    }



    float CalculateDistance()
    {
        Vector3 offset = targetGrapplePoint.transform.position - transform.position;
        float distance = offset.sqrMagnitude;
        return distance;
    }


    float CalculateGrappleDuration(float distance)
    {
        float duration = distance / retractSpeed;

        if (duration > maxGrappleDuration) duration = maxGrappleDuration;
        else if (duration < minGrappleDuration) duration = minGrappleDuration;

        return duration;
    }

}
