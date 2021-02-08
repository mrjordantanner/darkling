using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class GhostTrails : MonoBehaviour {

    public bool on;
    public bool flipSpriteX;
    float spawnTimer = 0f;
    public int sortingOrder = 12;

	List<GameObject> trailParts = new List<GameObject>();

    public float duration = 0.3f;

    public float repeatRate = 0.01f;

	[Range(0.0f, 1.0f)]
	public float trailOpacity = 0.8f;

    [Range(0.0f, 1.0f)]
    public float trailColorRed;

    [Range(0.0f, 1.0f)]
    public float trailColorGreen;

    [Range(0.0f, 1.0f)]
    public float trailColorBlue;


    private void Update()
    {

        if (on) // && onDuration > 0)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= repeatRate)
            {
                SpawnTrailPart();
                spawnTimer = 0f;
            }

        }




    }

    void SpawnTrailPart()
	{

        GameObject trailPart = new GameObject();

        // Assign to container
        trailPart.transform.SetParent(Combat.Instance.VFXContainer.transform);        
              
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPart.AddComponent<Billboard>();  
        trailPart.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        trailPartRenderer.sortingOrder = sortingOrder;
        trailPartRenderer.sortingLayerName = "Level";
        trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        trailPart.transform.position = transform.position;
        trailPart.transform.localScale = transform.localScale;
        if (flipSpriteX) trailPartRenderer.flipX = true;
        trailParts.Add(trailPart);

        StartCoroutine(FadeTrailPart(trailPartRenderer));

        Destroy(trailPart, duration);

	}


	IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
	{
		Color trailColor = new Color(trailColorRed, trailColorGreen, trailColorBlue, trailOpacity);
		trailPartRenderer.color = trailColor;
		yield return new WaitForEndOfFrame();
	}


}
