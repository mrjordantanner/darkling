using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrapplePoint : MonoBehaviour
{
    public GrappleGun grappleGun;
    public bool useGizmo;
    public float size = 2f;
    public bool isTargeted;
    public bool inRange;
    public bool playerPresent;
    public Material targetedMat;
    Material normalMat;
    MeshRenderer mesh;

    private void Start()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        normalMat = mesh.material;
        grappleGun = FindObjectOfType<GrappleGun>();

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (useGizmo) Gizmos.DrawWireSphere(transform.position, size);
    }


    private void OnMouseEnter()
    {
        grappleGun.targetGrapplePoint = this;
       // if (!inRange) return;

        mesh.material = targetedMat;
        isTargeted = true;
        AudioManager.Instance.Play("GrappleHover");
    }

    private void OnMouseExit()
    {
        grappleGun.targetGrapplePoint = null;
      //  if (!inRange) return;

        mesh.material = normalMat;
        isTargeted = false;
        AudioManager.Instance.Play("GrappleHoverExit");
    }

    private void OnMouseDown()
    {


    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerPresent = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerPresent = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerPresent = false;
        }

    }


}
