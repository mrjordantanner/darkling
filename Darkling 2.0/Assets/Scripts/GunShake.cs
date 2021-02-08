using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShake : MonoBehaviour
{
    #region Singleton
    public static GunShake Instance;

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

    public Vector3 idlePosition = new Vector3(0.81f, 0.12f, 1.13f);
    public bool shake = false;
    public float strength = 0.1f;
    public bool hitShake = false;
    public float hitShakeStrength = 0.2f;

    void Start()
    {
        // idlePosition = transform.localPosition;
    }


    void Update()
    {
        if (shake && !GameManager.Instance.gamePaused) transform.localPosition = idlePosition + Random.insideUnitSphere * strength;
        else if (hitShake && !GameManager.Instance.gamePaused) transform.localPosition = idlePosition + Random.insideUnitSphere * hitShakeStrength;
        else transform.localPosition = idlePosition;

    }

    public IEnumerator StartHitShake(float duration)
    {
        hitShake = true;
        yield return new WaitForSeconds(duration);
        hitShake = false;
    }

}
