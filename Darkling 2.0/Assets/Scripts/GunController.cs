using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GunController : MonoBehaviour
{
    #region Singleton
    public static GunController Instance;

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

    // TODO: Change name to gun controller

    // Parent of weapon objects
    // Stores/switches weapons, 
    // handles aiming/weapon state
    // handles Weapon physics
    // could also store different ammo types for the guns

    public List<Gun> loadout;
    public Gun currentGun;

    CharacterControls characterControls;
    Camera cam;
    PlayerCharacter player;
    float normalFOV = 77;
    Ease zoomInEase, zoomOutEase;
    bool zooming, zoomed;
    Tween zoomInTween, zoomOutTween;

    [HideInInspector]
    public bool isReloading, isAiming;

    [Header("Gun Bob")]
    public bool useGunBob;
    public float idle_x = 0.01f;
    public float idle_y = 0.01f, walking_x = 0.035f, walking_y = 0.035f, running_x = 0.15f, running_y = 0.055f;

    [Header("Elasticity")]
    public bool elastic = false;
    public float weaponElasticity = 4f;
    public float cameraElasticity = 2f;
    public Vector3 cameraIdlePosition = new Vector3(0, 0.624f, 0);

    [Header("Recoil")]
    public bool useRecoil;
    public float aimRecoilReduction = 0.25f;

    Quaternion origin_rotation;

    [HideInInspector]
    public int currentIndex;
    [HideInInspector]
    public GameObject CurrentWeapon;

    void Start()
    {
        cam = Camera.main;
        //  anim = GetComponentInChildren<Animator>();
        player = PlayerRef.Instance.player;
        characterControls = FindObjectOfType<CharacterControls>();

        // hitmarkerImage = GameObject.Find("HUD/Hitmarker/Image").GetComponent<Image>();
        // hitmarkerImage.color = CLEARWHITE;


        Init();
    }

    public void Init()
    {
        foreach (Gun a in loadout) a.Init();
        Equip(0);

        origin_rotation = CurrentWeapon.transform.localRotation;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CurrentWeapon.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && loadout[0])
        {
            if (!currentGun.shotOnCooldown)
                Equip(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && loadout[1])
        {
            if (!currentGun.shotOnCooldown)
                Equip(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && loadout[2])
        {
            if (!currentGun.shotOnCooldown)
                Equip(2);
        }

        //UpdateSway();

        // Are we zoomed in?
        zoomed = cam.fieldOfView == currentGun.zoomedFOV;

        // Get input
        if (!GameManager.Instance.gamePaused)
        {
            // Get aiming input
            if (InputManager.Instance.aimButtonPressed) StartAiming();
            if (InputManager.Instance.aimButtonReleased) EndAiming();

            // Get reload input
            if (Input.GetKeyDown(InputManager.Instance.reload) && CanReload())
            {
                StartReload();
            }
        }

        if (useGunBob)
             HandleGunBob();

        // Camera elasticity (head)
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraIdlePosition, Time.deltaTime * cameraElasticity);

        // Weapon elasticity (hands)
        if (elastic)
        {
            // Move towards normal position
            if (!isAiming)
            {
                CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, currentGun.pose.hipPosition, Time.deltaTime * weaponElasticity);
                CurrentWeapon.transform.localRotation = Quaternion.Lerp(CurrentWeapon.transform.localRotation, Quaternion.Euler(currentGun.pose.hipRotation), Time.deltaTime * weaponElasticity);
            }
            else
            {
                // Move towards aiming position
                CurrentWeapon.transform.localPosition = Vector3.Lerp(CurrentWeapon.transform.localPosition, currentGun.pose.aimPosition, Time.deltaTime * weaponElasticity);
                CurrentWeapon.transform.localRotation = Quaternion.Lerp(CurrentWeapon.transform.localRotation, Quaternion.Euler(currentGun.pose.aimRotation), Time.deltaTime * weaponElasticity);
            }
        }
    }

    bool CanReload()
    {
        var rel = !isReloading && currentGun.stats.ammo > 0;
        return rel;
    }

    public void StartReload()
    {
        StartCoroutine(Reload(currentGun.stats.reloadSpeed));
    }

    IEnumerator Reload(float reloadSpeed)
    {
        isReloading = true;
        if (isAiming) EndAiming();

        //if (CurrentWeapon.GetComponent<Animator>())
        //    CurrentWeapon.GetComponent<Animator>().Play("Reload", 0, 0);
        //else
        //    CurrentWeapon.SetActive(false);

        MoveGun(currentGun.pose.reloadPosition, currentGun.pose.reloadRotation, currentGun.stats.reloadSpeed * 0.25f);
        //CurrentWeapon.transform.DOLocalMove(loadout[currentIndex].reloadPosition, 1f).SetEase(Ease.OutElastic);
       // CurrentWeapon.transform.DOLocalRotate(loadout[currentIndex].reloadRotation, 1f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(reloadSpeed -= currentGun.stats.reloadSpeed * 0.5f);
        // CurrentWeapon.transform.DOLocalMove(loadout[currentIndex].hipPosition,1f).SetEase(Ease.OutElastic);
        // CurrentWeapon.transform.DOLocalRotate(Vector3.zero, 1f).SetEase(Ease.OutElastic);
        MoveGun(currentGun.pose.hipPosition, currentGun.pose.hipRotation, currentGun.stats.reloadSpeed * 0.25f);
        currentGun.Reload();
        //CurrentWeapon.SetActive(true);
        isReloading = false;
    }

    void Equip(int index)
    {
        if (CurrentWeapon != null)
        {
            if (isReloading) StopCoroutine("Reload");
           // Destroy(CurrentWeapon);
        }

        currentIndex = index;

        //GameObject NewWeapon = Instantiate(loadout[index].GunPrefab, transform.position, transform.rotation, transform) as GameObject;
        //NewWeapon.transform.localPosition = Vector3.zero;
        //NewWeapon.transform.localEulerAngles = Vector3.zero;

        //NewWeapon.GetComponent<Animator>().Play("Equip", 0, 0);

        // CurrentWeapon = NewWeapon;

        foreach (var gun in loadout)
            gun.gameObject.SetActive(false);

        currentGun = loadout[index];
        currentGun.OnEquip();
        CurrentWeapon = currentGun.gameObject;
        CurrentWeapon.SetActive(true);
    }


    void PickupWeapon(string name)
    {
       //// Gun newWeapon = GunLibrary.FindGun(name);
       // newWeapon.Init();

       // if (loadout.Count >= 2)
       // {
       //     loadout[currentIndex] = newWeapon;
       //     Equip(currentIndex);
       // }
       // else
       // {
       //     loadout.Add(newWeapon);
       //     Equip(loadout.Count - 1);
       // }
    }

    public float headWeight = 0.5f;

    // For takeoff and landing, add weight to the gun
    // Elasticity should automatically bring it back up
    public void TakeoffPhysics()
    {
        // Gun weight
        MoveGun(currentGun.pose.jumpPosition, currentGun.pose.jumpRotation, currentGun.stats.weight * 0.5f);

        //  head weight
        var headPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y - headWeight, cam.transform.localPosition.z);
        cam.transform.DOLocalMove(headPos, headWeight * 0.1f);
    }

    public void LandingPhysics()
    {
        // Gun weight
        MoveGun(currentGun.pose.landingPosition, 
            currentGun.pose.landingRotation, 
            currentGun.stats.weight * 0.5f);

        //  head weight
        var headPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y - headWeight, cam.transform.localPosition.z);
        cam.transform.DOLocalMove(headPos, headWeight * 0.1f);
    }

    public void Recoil()
    {
        var side = Random.Range(-currentGun.stats.drift, currentGun.stats.drift);
        var up = currentGun.stats.recoil;

        // Normal recoil
        if (!isAiming)
        {
            // Kickback
            CurrentWeapon.transform.DOLocalMove(currentGun.pose.recoilPosition, currentGun.stats.weight * 0.5f).SetEase(Ease.OutElastic);
        }
        else // aiming
        {
           // Kickback
            CurrentWeapon.transform.DOLocalMove(currentGun.pose.aimRecoilPosition, currentGun.stats.weight * 0.25f).SetEase(Ease.OutElastic);

            // camera upwards recoil
            side *= aimRecoilReduction;
            up *=  aimRecoilReduction;
        }

        characterControls.mouseLook.AddRecoil(side, up);

    }



    public void MoveToHip()
    {
        MoveGun(currentGun.pose.hipPosition, currentGun.pose.hipRotation, currentGun.stats.weight * 0.5f);
    }

    public void MoveGun(Vector3 position, Vector3 rotation, float duration)
    {
        CurrentWeapon.transform.DOLocalMove(position, duration);
        CurrentWeapon.transform.DOLocalRotate(rotation, duration);

    }


    public void StartRunCycle()
    {
        if (isAiming) EndAiming();
        elastic = false;
        MoveGun(currentGun.pose.runPosition, currentGun.pose.runRotation, currentGun.stats.weight * 0.5f);
    }

    public void StopRunCycle()
    {
        MoveToHip();
        StartCoroutine(RestoreElasticity(0.5f));
    }

    IEnumerator RestoreElasticity(float duration)
    {
        yield return new WaitForSeconds(duration);
        elastic = true;
    }


    public void StartAiming()
    {
        if (zooming || zoomed) return;
        if (characterControls.running) characterControls.StopRunning();
        // Zoom camera
        zoomInTween = DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, loadout[currentIndex].zoomedFOV, loadout[currentIndex].stats.weight * 0.5f).SetEase(zoomInEase);
        // Center gun
        // loadout[currentIndex].transform.DOLocalMove(loadout[currentIndex].aimPosition, loadout[currentIndex].weight * 0.5f).SetEase(zoomInEase); 
        MoveGun(currentGun.pose.aimPosition, currentGun.pose.aimRotation, currentGun.stats.weight * 0.5f);

        isAiming = true;
        StartCoroutine(ClearFlagOnComplete(zoomInTween));
    }

    public void EndAiming()
    {
        if (zooming) return;
        zoomOutTween = DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, normalFOV, currentGun.stats.weight);//.SetEase(zoomOutEase);
        MoveToHip();
        isAiming = false;
        StartCoroutine(ClearFlagOnComplete(zoomOutTween));
    }

    IEnumerator ClearFlagOnComplete(Tween tween)
    {
        zooming = true;
        yield return tween.onComplete;
        zooming = false;
    }



    // gun sway
    //public float swayIntensity;
    //public float swaySmoothing;
    //Quaternion origin_rotation;

    //private void UpdateSway()
    //{
    //    //controls
    //    float t_x_mouse = Input.GetAxis("Mouse X");
    //    float t_y_mouse = Input.GetAxis("Mouse Y");

    //    //if (!isMine)
    //    //{
    //    //    t_x_mouse = 0;
    //    //    t_y_mouse = 0;
    //    //}

    //    //calculate target rotation
    //    Quaternion t_x_adj = Quaternion.AngleAxis(-swayIntensity * t_x_mouse, Vector3.up);
    //    Quaternion t_y_adj = Quaternion.AngleAxis(swayIntensity * t_y_mouse, Vector3.right);
    //    Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;

    //    //rotate towards target rotation
    //    CurrentWeapon.transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * swaySmoothing);
    //}


    void HandleGunBob()
    {
        if (!characterControls.grounded)
        {
            //airborne
            HeadBob(idleCounter, idle_x, idle_y);
            idleCounter += 0;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f * 0.2f);
        }
        //else if (sliding)
        //{
        //    //sliding
        //    HeadBob(movementCounter, 0.15f, 0.075f);
        //    transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f * 0.2f);
        //}
        else if (characterControls.input == Vector3.zero)//(input.x == 0 && input.z == 0)
        {
            //idling
            HeadBob(idleCounter, idle_x, idle_y);
            idleCounter += Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f * 0.2f);
        }
        //else if (crouched)
        //{
        //    //crouching
        //    HeadBob(movementCounter, 0.02f, 0.02f);
        //    movementCounter += Time.deltaTime * 4f;
        //    transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f * 0.2f);
        //}
        else if (characterControls.running)
        {
            //sprinting
            HeadBob(movementCounter, running_x, running_y);
            movementCounter += Time.deltaTime * 13.5f;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f * 0.2f);
        }
        else
        {
            //walking
            HeadBob(movementCounter, walking_x, walking_y);
            movementCounter += Time.deltaTime * 6f;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f * 0.2f);
        }
    }

    Vector3 targetWeaponBobPosition;
    Vector3 weaponParentOrigin;
    Vector3 weaponParentCurrentPos;

    float movementCounter;
    float idleCounter;
    //[HideInInspector]
    //public Transform weaponParent;

    // GUN bob
    void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
    {
        float t_aim_adjust = 1f;
        if (isAiming) t_aim_adjust = 0.025f;
        targetWeaponBobPosition = weaponParentCurrentPos + new Vector3(Mathf.Cos(p_z) * p_x_intensity * t_aim_adjust, Mathf.Sin(p_z * 2) * p_y_intensity * t_aim_adjust, 0);
    }


    public void GainAmmo(int amount)
    {
        var stats = currentGun.stats;
        stats.ammo += amount;

        if (stats.ammo > stats.maxAmmo)
            stats.ammo = stats.maxAmmo;

        if (stats.clip == 0) StartReload();

       // HUD.Instance.UpdatePlayerUI();
    }

}
