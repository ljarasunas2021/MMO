using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Weapon class itself handles the weapon mechanics
public class Weapon : MonoBehaviour
{
    public WeaponType type = WeaponType.Ranged;
    public RangedWeaponType rangedType = RangedWeaponType.Projectile;
    public RangedHoldType rangedHold = RangedHoldType.shotgun;

    public Vector3 startPos;
    public Vector3 startRot;

    public Auto auto = Auto.Full; // How does this weapon fire - semi-auto or full-auto

    public MeleeCombo[] meleeCombos;

    public GameObject weaponModel; // The actual mesh for this weapon
    public bool shootFromMiddleOfScreen = true;
    public Transform raycastStartSpot; // The spot that the raycasting weapon system should use as an origin for rays
    public float delayBeforeFire = 0.0f; // An optional delay that causes the weapon to fire a specified amount of time after it normally would (0 for no delay)

    public bool warmup = false; // Whether or not the shot will be allowed to "warmup" before firing - allows power to increase when the player holds down fire button longer, Only works on semi-automatic raycast and projectile weapons
    public float maxWarmup = 2.0f; // The maximum amount of time a warmup can have any effect on power, etc.
    public bool multiplyForce = true; // Whether or not the initial force of the projectile should be multiplied based on warmup heat value - only for projectiles
    public bool multiplyPower = false; // Whether or not the damage of the projectile should be multiplied based on warmup heat value - only for projectiles, Also only has an effect on projectiles using the direct damage system - an example would be an arrow that causes more damage the longer you pull back your bow
    public float powerMultiplier = 1.0f; // The multiplier by which the warmup can affect weapon power; power = power * (heat * powerMultiplier)
    public float initialForceMultiplier = 1.0f; // The multiplier by which the warmup can affect the initial force, assuming a projectile system
    public bool allowCancel = false; // If true, the player can cancel this warmed up shot by pressing the "Cancel" input button, otherwise a shot will be fired when the player releases the fire key
    private float heat = 0.0f; // The amount of time the weapon has been warming up, can be in the range of (0, maxWarmup)

    public GameObject projectile; // The projectile to be launched (if the type is projectile)
    public Transform projectileSpawnSpot; // The spot where the projectile should be instantiated

    public float power = 80.0f; // The amount of power this weapon has (how much damage it can cause) (if the type is raycast)
    public float forceMultiplier = 10.0f; // Multiplier used to change the amount of force applied to rigid bodies that are shot

    public float range = 9999.0f; // How far this weapon can shoot (for raycast)

    public float rateOfFire = 10; // The number of rounds this weapon fires per second
    private float actualROF; // The frequency between shots based on the rateOfFire
    private float fireTimer; // Timer used to fire at a set frequency

    public bool infiniteAmmo = false; // Whether or not this weapon should have unlimited ammo
    public int ammoCapacity = 12; // The number of rounds this weapon can fire before it has to reload
    public int shotPerRound = 1; // The number of "bullets" that will be fired on each round.  Usually this will be 1, but set to a higher number for things like shotguns with spread
    private int currentAmmo; // How much ammo the weapon currently has
    public float reloadTime = 2.0f; // How much time it takes to reload the weapon
    public bool showCurrentAmmo = true; // Whether or not the current ammo should be displayed in the GUI
    public bool reloadAutomatically = true; // Whether or not the weapon should reload automatically when out of ammo

    public float accuracy = 80.0f; // How accurate this weapon is on a scale of 0 to 100
    private float currentAccuracy; // Holds the current accuracy.  Used for varying accuracy based on speed, etc.
    public float accuracyDropPerShot = 1.0f; // How much the accuracy will decrease on each shot
    public float accuracyRecoverRate = 0.1f; // How quickly the accuracy recovers after each shot (value between 0 and 1)

    public int burstRate = 3; // The number of shots fired per each burst
    public float burstPause = 0.0f; // The pause time between bursts
    private int burstCounter = 0; // Counter to keep track of how many shots have been fired per burst
    private float burstTimer = 0.0f; // Timer to keep track of how long the weapon has paused between bursts

    public bool recoil = true; // Whether or not this weapon should have recoil
    public float recoilKickBackMin = 0.1f; // The minimum distance the weapon will kick backward when fired
    public float recoilKickBackMax = 0.3f; // The maximum distance the weapon will kick backward when fired
    public float recoilRotationMin = 0.1f; // The minimum rotation the weapon will kick when fired
    public float recoilRotationMax = 0.25f; // The maximum rotation the weapon will kick when fired
    public float recoilRecoveryRate = 0.01f; // The rate at which the weapon recovers from the recoil displacement

    public bool spitShells = false; // Whether or not this weapon should spit shells out of the side
    public GameObject shell; // A shell prop to spit out the side of the weapon
    public float shellSpitForce = 1.0f; // The force with which shells will be spit out of the weapon
    public float shellForceRandom = 0.5f; // The variant by which the spit force can change + or - for each shot
    public float shellSpitTorqueX = 0.0f; // The torque with which the shells will rotate on the x axis
    public float shellSpitTorqueY = 0.0f; // The torque with which the shells will rotate on the y axis
    public float shellTorqueRandom = 1.0f; // The variant by which the spit torque can change + or - for each shot
    public Transform shellSpitPosition; // The spot where the weapon should spit shells from
    public bool makeMuzzleEffects = true; // Whether or not the weapon should make muzzle effects
    public GameObject[] muzzleEffects = new GameObject[] { null }; // Effects to appear at the muzzle of the gun (muzzle flash, smoke, etc.)
    public Transform muzzleEffectsPosition; // The spot where the muzzle effects should appear from
    public bool makeHitEffects = true; // Whether or not the weapon should make hit effects
    public GameObject[] hitEffects = new GameObject[] { null }; // Effects to be displayed where the "bullet" hit

    public bool bulletHolesEnabled;

    public bool showCrosshair = true; // Whether or not the crosshair should be displayed
    public Texture2D crosshairTexture; // The texture used to draw the crosshair
    public int crosshairLength = 10; // The length of each crosshair line
    public int crosshairWidth = 4; // The width of each crosshair line
    public float startingCrosshairSize = 10.0f; // The gap of space (in pixels) between the crosshair lines (for weapon inaccuracy)
    private float currentCrosshairSize; // The gap of space between crosshair lines that is updated based on weapon accuracy in realtime

    public AudioClip fireSound; // Sound to play when the weapon is fired
    public AudioClip reloadSound; // Sound to play when the weapon is reloading
    public AudioClip dryFireSound; // Sound to play when the user tries to fire but is out of ammo

    private bool canFire = true; // Whether or not the weapon can currently fire (used for semi-auto weapons)
    private AudioSource audioSource;
    private BulletHoleController bulletHoleController;
    private AudioClip[] audioClipPrefabs;
    private GameObject[] effectPrefabs;
    private Material[] materialPrefabs;
    private GameObject user;
    private PlayerWeapon playerWeapon;
    private Animator userAnim;
    private Dictionary<UpperBodyStates, UpperBodyStates[]> meleeCombosDict = new Dictionary<UpperBodyStates, UpperBodyStates[]>();
    private int hotBarIndex;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        bulletHoleController = GameObject.FindObjectOfType<BulletHoleController>();
        audioClipPrefabs = GameObject.FindObjectOfType<AudioPrefabsController>().audioClipPrefabs;
        effectPrefabs = GameObject.FindObjectOfType<EffectsPrefabsController>().effectPrefabs;
        materialPrefabs = bulletHoleController.ReturnMaterialPrefabs();

        if (type == WeaponType.Ranged)
        {
            if (rateOfFire != 0) actualROF = 1.0f / rateOfFire;
            else actualROF = 0.01f;

            currentCrosshairSize = startingCrosshairSize;

            fireTimer = 0.0f;

            currentAmmo = ammoCapacity;

            if (shootFromMiddleOfScreen) raycastStartSpot = Camera.main.transform;
        }
        else
        {
            meleeCombos = new MeleeCombo[] { new MeleeCombo(UpperBodyStates.midInwardSlashRight, new UpperBodyStates[] { UpperBodyStates.midSlashLeft }), new MeleeCombo(UpperBodyStates.midSlashLeft, new UpperBodyStates[] { UpperBodyStates.midInwardSlashRight }), new MeleeCombo(UpperBodyStates.highToLowInwardSlashRight, new UpperBodyStates[] { UpperBodyStates.lowToHighSlashLeft }), new MeleeCombo(UpperBodyStates.lowToHighSlashLeft, new UpperBodyStates[] { UpperBodyStates.highToLowInwardSlashRight }), new MeleeCombo(UpperBodyStates.highToLowSlashLeft, new UpperBodyStates[] { UpperBodyStates.lowToHighInwardSlashRight }), new MeleeCombo(UpperBodyStates.lowToHighInwardSlashRight, new UpperBodyStates[] { UpperBodyStates.highToLowSlashLeft }) };
            foreach (MeleeCombo combo in meleeCombos) meleeCombosDict.Add(combo.attack, combo.combos);
        }
    }

    void Update()
    {
        if (type == WeaponType.Ranged)
        {
            currentAccuracy = Mathf.Lerp(currentAccuracy, accuracy, accuracyRecoverRate * Time.deltaTime);

            currentCrosshairSize = startingCrosshairSize + (accuracy - currentAccuracy) * 0.8f;

            fireTimer += Time.deltaTime;

            if (reloadAutomatically && currentAmmo <= 0) Reload();

            if (recoil)
            {
                weaponModel.transform.position = Vector3.Lerp(weaponModel.transform.position, transform.position, recoilRecoveryRate * Time.deltaTime);
                weaponModel.transform.rotation = Quaternion.Lerp(weaponModel.transform.rotation, transform.rotation, recoilRecoveryRate * Time.deltaTime);
            }
        }
    }

    // Checks for user input to use the weapons - only if this weapon is player-controlled
    public void CheckForUserInput()
    {
        if (type == WeaponType.Ranged)
        {
            if (rangedType == RangedWeaponType.Raycast)
            {
                if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
                {
                    if (Input.GetMouseButton(hotBarIndex))
                    {
                        if (!warmup) Fire();
                        else if (heat < maxWarmup) heat += Time.deltaTime;
                    }

                    if (warmup && Input.GetMouseButtonDown(hotBarIndex))
                    {
                        if (allowCancel && Input.GetButton("Cancel")) heat = 0.0f;
                        else Fire();
                    }
                }
            }

            if (rangedType == RangedWeaponType.Projectile)
            {
                if (fireTimer >= actualROF && burstCounter < burstRate && canFire)
                {
                    if (Input.GetMouseButton(hotBarIndex))
                    {
                        if (!warmup) Launch();
                        else if (heat < maxWarmup) heat += Time.deltaTime;
                    }
                    if (warmup && Input.GetMouseButtonUp(hotBarIndex))
                    {
                        if (allowCancel && Input.GetButton("Cancel")) heat = 0.0f;
                        else Launch();
                    }
                }

            }

            if (burstCounter >= burstRate)
            {
                burstTimer += Time.deltaTime;
                if (burstTimer >= burstPause)
                {
                    burstCounter = 0;
                    burstTimer = 0.0f;
                }
            }

            if (Input.GetButton("Reload")) Reload();

            if (Input.GetMouseButtonUp(hotBarIndex)) canFire = true;
        }
        else
        {
            if (Input.GetMouseButtonUp(hotBarIndex))
            {

                UpperBodyStates currentUpperBodyState = (UpperBodyStates)userAnim.GetInteger(Parameters.upperBodyState);
                int targetState = 0;

                if (currentUpperBodyState == UpperBodyStates.swordHold)
                {
                    List<UpperBodyStates> possibleStates = new List<UpperBodyStates>(meleeCombosDict.Keys);

                    targetState = (int)possibleStates[Random.Range(0, possibleStates.Count)];
                }
                else
                {
                    UpperBodyStates[] possibleStates = meleeCombosDict[currentUpperBodyState];

                    targetState = (int)possibleStates[Random.Range(0, possibleStates.Length)];
                }

                userAnim.SetInteger(Parameters.targetUpperBodyState, (int)targetState);
            }
        }
    }

    void OnGUI()
    {
        if (type == WeaponType.Ranged)
        {
            if (rangedType == RangedWeaponType.Projectile) currentAccuracy = accuracy;

            if (showCrosshair)
            {
                Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

                Rect leftRect = new Rect(center.x - crosshairLength - currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
                GUI.DrawTexture(leftRect, crosshairTexture, ScaleMode.StretchToFill);

                Rect rightRect = new Rect(center.x + currentCrosshairSize, center.y - (crosshairWidth / 2), crosshairLength, crosshairWidth);
                GUI.DrawTexture(rightRect, crosshairTexture, ScaleMode.StretchToFill);

                Rect topRect = new Rect(center.x - (crosshairWidth / 2), center.y - crosshairLength - currentCrosshairSize, crosshairWidth, crosshairLength);
                GUI.DrawTexture(topRect, crosshairTexture, ScaleMode.StretchToFill);

                Rect bottomRect = new Rect(center.x - (crosshairWidth / 2), center.y + currentCrosshairSize, crosshairWidth, crosshairLength);
                GUI.DrawTexture(bottomRect, crosshairTexture, ScaleMode.StretchToFill);
            }

            if (showCurrentAmmo) GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Ammo: " + currentAmmo);
        }
    }

    // Raycasting system
    void Fire()
    {
        fireTimer = 0.0f;

        burstCounter++;

        if (auto == Auto.Semi) canFire = false;

        if (currentAmmo <= 0)
        {
            DryFire();
            return;
        }

        if (!infiniteAmmo) currentAmmo--;

        for (int i = 0; i < shotPerRound; i++)
        {
            float accuracyVary = (100 - currentAccuracy) / 1000;
            Vector3 direction = raycastStartSpot.forward;
            direction.x += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
            direction.y += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
            direction.z += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
            currentAccuracy -= accuracyDropPerShot;
            if (currentAccuracy <= 0.0f) currentAccuracy = 0.0f;

            Ray ray = new Ray(raycastStartSpot.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range))
            {
                float damage = power;

                if (warmup)
                {
                    damage *= heat * powerMultiplier;
                    heat = 0.0f;
                }

                PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
                if (health != null) health.SubtractHealth(damage);

                if (bulletHolesEnabled)
                {
                    MeshRenderer colliderMR = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    if (colliderMR != null) bulletHoleController.RpcCreateBulletHole(FindIndexMaterial(colliderMR.sharedMaterial), hit.point, hit.normal);
                }

                if (makeHitEffects) CreateHitEffects(hit);

                if (hit.rigidbody) playerWeapon.CmdAddForce(hit.collider.gameObject, ray.direction * power * forceMultiplier);
            }
        }

        if (recoil) Recoil();

        if (makeMuzzleEffects) MakeMuzzleEffects();

        if (spitShells) SplitShells();

        PlaySound(fireSound);
    }

    // Projectile system
    public void Launch()
    {
        fireTimer = 0.0f;

        burstCounter++;

        if (auto == Auto.Semi) canFire = false;

        if (currentAmmo <= 0)
        {
            DryFire();
            return;
        }

        if (!infiniteAmmo) currentAmmo--;

        for (int i = 0; i < shotPerRound; i++)
        {
            GameObject proj = Instantiate(projectile, projectileSpawnSpot.position, projectileSpawnSpot.rotation);
            if (warmup) heat = 0.0f;
        }

        if (recoil) Recoil();

        if (makeMuzzleEffects) MakeMuzzleEffects();

        if (spitShells) SplitShells();

        PlaySound(fireSound);
    }

    private void CreateHitEffects(RaycastHit hit) { foreach (GameObject hitEffect in hitEffects) if (hitEffect != null) playerWeapon.RpcCreateHitEffect(FindIndexEffect(hitEffect), hit.point, hit.normal); }

    private void MakeMuzzleEffects()
    {
        if (playerWeapon == null || muzzleEffectsPosition == null) return;
        playerWeapon.RpcMakeMuzzleEffect(FindIndexEffect(muzzleEffects[Random.Range(0, muzzleEffects.Length)]), muzzleEffectsPosition.position, muzzleEffectsPosition.eulerAngles);
    }

    private void SplitShells()
    {
        if (playerWeapon == null || shellSpitPosition == null) return;
        playerWeapon.CmdSplitShells(shell, shellSpitPosition.position, shellSpitPosition.eulerAngles, shellSpitForce, shellForceRandom, shellSpitTorqueX, shellTorqueRandom, shellSpitTorqueY);
    }

    private void Reload()
    {
        currentAmmo = ammoCapacity;
        fireTimer = -reloadTime;
        PlaySound(reloadSound);
    }

    private void DryFire() { PlaySound(dryFireSound); }

    private void Recoil()
    {
        if (weaponModel == null) return;

        float kickBack = Random.Range(recoilKickBackMin, recoilKickBackMax);
        float kickRot = Random.Range(recoilRotationMin, recoilRotationMax);

        weaponModel.transform.Translate(new Vector3(0, 0, -kickBack), Space.Self);
        weaponModel.transform.Rotate(new Vector3(-kickRot, 0, 0), Space.Self);
    }

    private void PlaySound(AudioClip clip)
    {
        playerWeapon.RpcPlaySound(FindIndexAudioClip(clip));
    }

    public int FindIndexAudioClip(AudioClip clip)
    {
        int index = -1;
        for (int i = 0; i < audioClipPrefabs.Length; i++) if (clip.name.Contains(audioClipPrefabs[i].name)) index = i;
        return index;
    }

    public int FindIndexEffect(GameObject effect)
    {
        int index = -1;
        for (int i = 0; i < effectPrefabs.Length; i++) if (effect.name.Contains(effectPrefabs[i].name)) index = i;
        return index;
    }

    public int FindIndexMaterial(Material material)
    {
        int index = -1;
        for (int i = 0; i < materialPrefabs.Length; i++) if (material.name.Contains(materialPrefabs[i].name)) index = i;
        return index;
    }

    public void SetUser(GameObject user)
    {
        this.user = user;
        playerWeapon = user.GetComponent<PlayerWeapon>();
        userAnim = user.GetComponent<PlayerWeapon>().nonRagdoll.GetComponent<Animator>();
    }

    public void SetHotBarIndex(int hotBarIndex)
    {
        this.hotBarIndex = hotBarIndex;
    }
}

public enum RangedWeaponType
{
    Projectile,
    Raycast
}

public enum WeaponType
{
    Ranged,
    Melee
}

public enum Auto
{
    Full,
    Semi
}

public enum RangedHoldType
{
    shotgun,
    pistol
}

[System.Serializable]
public class MeleeCombo
{
    public UpperBodyStates attack;
    public UpperBodyStates[] combos;

    public MeleeCombo(UpperBodyStates attack, UpperBodyStates[] combos)
    {
        this.attack = attack;
        this.combos = combos;
    }
}
