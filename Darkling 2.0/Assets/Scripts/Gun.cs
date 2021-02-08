using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum FirePattern { Single, Burst, Auto }

public class Gun : Damager
{
    // TODO:  Break this out into smaller component scripts
    // Make Scriptable object?

    [Serializable]
    public class Pose
    {
        public Vector3 hipPosition, hipRotation, aimPosition, aimRotation, runPosition, runRotation, jumpPosition;
        public Vector3 jumpRotation, reloadPosition, reloadRotation, recoilPosition, recoilRotation, aimRecoilPosition, aimRecoilRotation, landingPosition, landingRotation;
    }

    [Serializable]
    public class GunStats
    {
        public string gunName;
        public int clip;               // how many rounds in current clip
        public int maxClip;            // how many rounds magazine can hold
        public int ammo;               // how many rounds we have outside of the current clip
        public int maxAmmo;            // how many rounds we can hold total
       // public float damage;
      //  public int roundsPerMinute;
        //[HideInInspector]
      //  public float shotDelay;            // 1 / roundsPerMinute = shotDelay
        public float shotRate;
        public float shotCooldown;        // between bursts or single shots, how long until we can fire again
        public float muzzleFlashDuration;
        public float recoil = 0.5f;       // applies upwards force on gun when firing
        public float drift = 0.5f;        // horizontal drift amount when firing
        public float range = 100f;
        public float rangeFalloffThreshold = 0.5f;     //  0-1, at what % of range does damage start to fall off?
        public float rangeFalloffAmount = 0.05f;        // how much damage is rolled off (per meter?) if the player is further away than rangeFalloffThreshold
        public int burstShots;
        public float projectileSpeed;
        public float hitForce = 50f;           // apply physics to rigidbody hit
        public float reloadSpeed = 3f;
        public float weight = 0.3f;      //scale 0-1, for jumping/landing animations, how quickly/slowly the weapon moves with you, also weapon switching, aiming

        public bool infiniteAmmo;
    }

    // Projectile weapon
    // Similar to the Weapon.cs, each gun will have an instance of this script on it that:
    // dictates the gun's properties (bas, the properties of its projectile,

    // Should these actually be CharacterStats placed on the player, and the gun just has base stats?  YES
    // gun bsae stats * player stats and modifiers = final stats


    public bool hitScan;                     // deal Hitscan damage?
    public bool shootProjectile = true;      // shoot projectile prefab?
    public FirePattern firePattern;

    [Header("Prefabs")]
    public GameObject GunPrefab;
    public GameObject Projectile;
    public GameObject BulletHole;         
    public GameObject MuzzleFlash;

    public SimplePool.Pool pool_bullet;

    public Transform firingPoint;
    public string shotSound;
    bool muzzleFlashing;

    [Header("Scope")]
    public float zoomedFOV = 40;

    [Header("Firing Conditionals")]
    public bool fireButtonReleased;     // for single and burst firing, must release fire button to begin firing again
    public bool shotOnCooldown;         // flag for cooldown between single shots or bursts, regardless of fireButtonReleased
    public bool rapidFire;
    public bool burstFire;
    public int shotsFired;              // keep track of how many burst shots fired, so know when to stop, even if firebutton still pressed

    Camera cam;
    PlayerCharacter player;
    Rigidbody rigidbody;
    CameraZoom cameraZoom;
    CharacterControls characterControls;
    Vector3 rayOrigin;
    public GunStats stats;
    public Pose pose;
    private CustomFixedUpdate FU_instance;

    void Awake()
    {
        InitCustomUpdate();
    }

    public override void Start()
    {
        base.Start();
        cam = Camera.main;
        //  anim = GetComponentInChildren<Animator>();
        player = PlayerRef.Instance.player;
        cameraZoom = GetComponent<CameraZoom>();
        characterControls = FindObjectOfType<CharacterControls>();

        pose.hipPosition = transform.localPosition;

        Init();
    }

    public override void Init()
    {
        base.Init();
        // fill up ammo, set idle position, etc
        stats.clip = stats.maxClip;
        stats.ammo = stats.maxAmmo;

        pool_bullet = new SimplePool.Pool(Projectile, 300);

        // CalculateShotDelay();

    }

    void InitCustomUpdate()
    {
        FU_instance = new CustomFixedUpdate(stats.shotRate, RapidUpdate);
    }



    void Update()
    {
        HandleInput();

        FU_instance.Update();

        if (Input.GetKeyDown(KeyCode.H))
        {
            print("Input received");
        }
    }


    void RapidUpdate(float dt)
    {
        if (rapidFire || burstFire)
        {
            if (AmmoCheck())
                Shoot();
            else
            {
                rapidFire = false;
                StopBurstFire();
            }
        }

        if (burstFire && (shotsFired > stats.burstShots - 1))
            StopBurstFire();
    }


    #region Handle Input

    void HandleInput()
    {
        // Stop running on first click - user must click again to fire
        if (Input.GetKeyDown(InputManager.Instance.fire) && characterControls.running)
        {
            characterControls.StopRunning();
            return;
        }

        if (GunController.Instance.isReloading) return;

        switch (firePattern)
        {
            case FirePattern.Single:
            if (Input.GetKeyDown(InputManager.Instance.fire) && !shotOnCooldown)
            {
                SingleFire();
            }
            break;

            case FirePattern.Burst:
            if (Input.GetKeyDown(InputManager.Instance.fire) && !shotOnCooldown && !burstFire)
            {
                burstFire = true;
            }
            break;

            case FirePattern.Auto:
            if (Input.GetKey(InputManager.Instance.fire))
            {
                fireButtonReleased = false;
                rapidFire = true;
            }
            break;

        }

        // Stop shooting and reset
        if (Input.GetKeyUp(InputManager.Instance.fire))
        {
            if (burstFire && (shotsFired > stats.burstShots - 1))
            {
                burstFire = false;
                shotsFired = 0;
            }

            fireButtonReleased = true;
            rapidFire = false;
        }

    }
    #endregion

    void SingleFire()
    {
        Shoot();
        StartCoroutine(ShotCooldown());
    }

    void StopBurstFire()
    {
        burstFire = false;
        StartCoroutine(ShotCooldown());
    }

    IEnumerator ShotCooldown()
    {
        shotOnCooldown = true;
        yield return new WaitForSeconds(stats.shotCooldown);
        ResetCooldown();
    }

    void ResetCooldown()
    {
        rapidFire = false;
        burstFire = false;
        shotOnCooldown = false;
        shotsFired = 0;
    }

    #region Shoot

   // EnemyHealth enemyHealthTarget;
   // CritZone critZoneTarget;

    void Shoot()
    {
        RaycastHit hit;
        Vector3 shotDirection;
        float damage = Damage;

        rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        bool raycastHit = Physics.Raycast(rayOrigin, cam.transform.forward, out hit, stats.range);

        if (raycastHit && hit.collider.gameObject.layer != 5)  // NOT player weapon layer, cant hit own bullets
            shotDirection = (hit.point - firingPoint.transform.position).normalized;
        else
            shotDirection = cam.transform.forward;

        //// Hitscan damage
        //if (hitScan && hit.collider != null)
        //{
        //    var crit = false;
        //    EnemyHealth enemyHealthTarget = null;

        //    if (hit.collider.GetComponent<CritZone>() != null)
        //    {
        //        crit = true;
        //       // print("Crit!");
        //        enemyHealthTarget = hit.collider.GetComponentInParent<EnemyHealth>();
        //    }
        //    else
        //    {
        //        crit = false;
        //        enemyHealthTarget = hit.collider.GetComponent<EnemyHealth>();
        //    }

        //    if (enemyHealthTarget != null)
        //        enemyHealthTarget.TakeDamage(Combat.Instance.EnemyHitscan(this, hit.point, crit), crit);



        //    // Hit Obstacle
        //    if (hit.collider.gameObject.layer == 29)
        //    {
        //        SpawnBulletHole(hit);

        //        if (hit.collider.gameObject.CompareTag("Destructible"))
        //             hit.collider.gameObject.GetComponent<DestructibleTimed>().Destroy();

        //        if (hit.rigidbody != null)
        //        {
        //            hit.rigidbody.AddForce(-hit.normal * stats.hitForce);
        //        }
        //    }

        //   // HUD.Instance.hitInfoText.text = hit.collider.name;
        //}


        // Fire a prefab
        if (shootProjectile)
        {
            //var ProjectileGameObject = SimplePool.Spawn(Projectile, firingPoint.position, Quaternion.identity, FX.Instance.Container);

            //// Set projectile stats if it's not a dummy projectile
            //PlayerProjectile projectile = ProjectileGameObject.GetComponent<PlayerProjectile>();
            //if (projectile != null)
            //{
            //    projectile.Init();
            //    projectile.BaseDamage = Damage;
            //    projectile.Damage = Damage;
            //    projectile.playerPositionWhenFired = firingPoint.transform.position;
            //    //projectile.lifeSpan = lifeSpan;
            //    projectile.falloffThreshold = stats.rangeFalloffThreshold;
            //    projectile.falloffAmount = stats.rangeFalloffAmount;
            //    projectile.range = stats.range;
            //}

            //TrailRenderer trail = ProjectileGameObject.GetComponent<TrailRenderer>();
            //if (trail != null) trail.Clear();

            //ProjectileGameObject.GetComponent<Rigidbody>().velocity = stats.projectileSpeed * shotDirection;
        }

        HandleRecoil();

        if (!stats.infiniteAmmo) stats.clip--;
        if (stats.clip == 0 && stats.ammo > 0) GunController.Instance.StartReload();

        shotsFired++;

        AudioManager.Instance.Play(shotSound);
        SpawnMuzzleFlash();

    }
    #endregion

    void HandleRecoil()
    {
        // Weapon position and Recoil
        GunController.Instance.elastic = false;
        if (GunController.Instance.isAiming)
        {
            gameObject.transform.DOLocalMove(pose.aimPosition, stats.weight * 0.5f);
            gameObject.transform.DOLocalRotate(pose.aimRotation, stats.weight * 0.5f);
        }
        else
        {
            gameObject.transform.DOLocalMove(pose.hipPosition, stats.weight * 0.5f);
            gameObject.transform.DOLocalRotate(pose.hipRotation, stats.weight * 0.5f);
        }

        if (GunController.Instance.useRecoil)
            GunController.Instance.Recoil();

        GunController.Instance.elastic = true;
    }

    // TODO: Change this to just enable/disable a muzzle flash gameobject rapidly rather than instantiating/destroying
    void SpawnMuzzleFlash()
    {
        if (MuzzleFlash != null)
        {
            var flash = Instantiate(MuzzleFlash, firingPoint.transform.position, Quaternion.identity, firingPoint.transform);
            flash.transform.Rotate(0, -270f, 0);
            Destroy(flash, stats.muzzleFlashDuration);
        }
    }


    void SpawnBulletHole(RaycastHit hit)
    {
        if (BulletHole != null)
        {
            // var t_newHole = SimplePool.Spawn(BulletHole, hit.point + hit.normal * 0.001f, Quaternion.identity, hit.collider.gameObject);
            //  t_newHole.transform.LookAt(hit.point + hit.normal);
            var t_newHole = SimplePool.Spawn(BulletHole, hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal), hit.collider.gameObject);
        }

    }

    public void Reload()
    {
        var freshAmmo = stats.maxClip - stats.clip;
        stats.clip += freshAmmo;
        stats.ammo -= freshAmmo;
    }

    public void OnEquip()
    {

    }


    //public void CalculateShotDelay()
    //{
    //    rpm = stats.roundsPerMinute;
    //    delay = 1 / rpm;
    //    stats.shotDelay = (float)delay;
    //}

    bool AmmoCheck()
    {
        if (stats.infiniteAmmo) return true;

        if (stats.clip >= 1 && !GunController.Instance.isReloading)
            return true;
        else if (stats.clip <= 0 && stats.ammo <= 0)
        {
            stats.clip = 0;
            // out of ammo
            // play out of ammo "click" sound
            // display message
            return false;
        }
        else return false;
    }

    //public float intensity = 1f;
    //public float smooth = 1f;

    //private Quaternion origin_rotation;
    //void UpdateSway()
    //{
    //    // TODO:  This does not work at all

    //    ////controls
    //    //float t_x_mouse = Input.GetAxis("Mouse X");
    //    //float t_y_mouse = Input.GetAxis("Mouse Y");

    //    ////if (!isMine)
    //    ////{
    //    ////    t_x_mouse = 0;
    //    ////    t_y_mouse = 0;
    //    ////}

    //    ////calculate target rotation
    //    //Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * t_x_mouse, Vector3.up);
    //    //    Quaternion t_y_adj = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right);
    //    //    Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;

    //    //    //rotate towards target rotation
    //    //    transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);

    //}


        // TODO: Move this and Hitscan code to Combat.cs
    //public float CalculateDamageFalloff(Gun gun, Vector3 hitPoint)
    //{
    //    float dmgReduction = 0;

    //    // Damage falloff based on weapon range
    //    Vector3 attackVector = hitPoint - firingPoint.transform.position;
    //    float distance = attackVector.magnitude;
    //    float threshold = stats.range * stats.rangeFalloffThreshold;

    //    // print("Hit distance: " + distance.ToString());

    //    if (distance > threshold)
    //    {
    //        // how far are we over the damage falloff threshold?
    //        double overRange = distance - threshold;

    //        // convert to % and calculate damage reduction
    //        double range = stats.range;
    //        double overRangePercent = overRange / range;
    //        float amount = (float)overRangePercent * stats.rangeFalloffAmount;
    //        print("% over threshold: " + overRangePercent.ToString());
    //        dmgReduction = Damage * amount;
    //        //damage -= reduction;

    //        //  print("Over threshold by " + overRangePercent.ToString() + "%");
    //        print("Damage reduction: " + dmgReduction.ToString());
    //        return dmgReduction;
    //    }

    //    print("Damage reduction: " + dmgReduction.ToString());
    //    return dmgReduction = 0;

    //}



}

