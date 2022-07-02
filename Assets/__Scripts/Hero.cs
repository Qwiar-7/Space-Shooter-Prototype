using System.Collections.Generic;
using UnityEngine;
public class Hero : MonoBehaviour
{
    static public Hero heroObj;
    static Dictionary<WeaponType, WeaponParameters> WeaponDictionary;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMultiplier = -45;
    public float pitchMultiplier = 30;
    public Turret turretPrefab;
    public GameObject explosionPrefab;
    public Weapon[] weapons;
    public Weapon[] missileLaunchers;
    public WeaponParameters[] weaponParams;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 4;
    public bool isDead = false;
    private GameObject lastTriggeredGameObj = null;
    public float ShieldLevel
    {
        get {  return _shieldLevel; }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                GameObject explosion = Instantiate(explosionPrefab);
                explosion.transform.position = transform.position;
                isDead = true;
                Destroy(gameObject);
                Main.mainObj.DelayedRestart();
            }
        }
    }
    private void Awake()
    {
        // Словарь с ключами типа WeaponType
        WeaponDictionary = new Dictionary<WeaponType, WeaponParameters>();
        //heroRigidbody = gameObject.GetComponent<Rigidbody>();
        foreach (WeaponParameters weapontDef in weaponParams)
        {
            WeaponDictionary[weapontDef.type] = weapontDef;
        }
        if (heroObj == null)
            heroObj = this;
    }
    private void Start()
    {
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
        turretPrefab.SetType(WeaponType.turret);
        turretPrefab.gameObject.SetActive(false);
        missileLaunchers[0].SetType(WeaponType.missile);
        missileLaunchers[1].SetType(WeaponType.missile);
    }
    private void Update()
    {
        if (Time.timeScale == 0) return;
        Vector3 position = transform.position;
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0f;
            if (Vector3.Magnitude(touchPosition - transform.position) > 2)
                position += Vector3.Normalize(touchPosition - transform.position) * speed * Time.deltaTime;

            if (Time.timeScale > 0)
                ShootFromAllWeapons();
            if (Input.touchCount == 2)
                ShootFromMissileLanucher();
        }
        else
        {
            position.x += xAxis * speed * Time.deltaTime;
            position.y += yAxis * speed * Time.deltaTime;
        }
        transform.SetPositionAndRotation(position, Quaternion.Euler(yAxis * pitchMultiplier, xAxis * rollMultiplier, 0));

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton2))
            ShootFromMissileLanucher();
        if (Input.GetAxis("Jump") == 1)
            ShootFromAllWeapons();
    }
    private void ShootFromMissileLanucher()
    {
        if (Time.time - missileLaunchers[0].lastShotTime < missileLaunchers[0].weaponParam.delayBetweenShots) return;
        foreach (var missileLauncher in missileLaunchers)
        {
            if (missileLauncher.isCharged)
            {
                missileLauncher.MakeAlternateShot();
                break;
            }
        }
    }
    private void ShootFromAllWeapons()
    {
        foreach (var weapon in weapons)
            weapon.MakeShot();
    }
    private void OnTriggerEnter(Collider otherCollider)
    {
        Transform otherRootTransform = otherCollider.gameObject.transform.root;
        GameObject otherGO = otherRootTransform.gameObject;
        bool isBoss = otherGO.GetComponent<Enemy_boss>() != null;

        if (otherGO == lastTriggeredGameObj) return;

        lastTriggeredGameObj = otherGO;

        if (otherGO.CompareTag("Enemy"))
        {
            if(isBoss)
            {
                ShieldLevel = -1;
            }
            else
            {
                ShieldLevel--;
                Destroy(otherGO);
            }
        }
        else if (otherGO.CompareTag("PowerUp"))
            AbsorbPowerUp(otherGO);
        else
            print("Triggered by non-Enemy: " + otherGO.name);
    }
    /// <summary>
    /// Статистическая функция возвращающая WeaponParameters из статистического
    /// защищенного поля WEAP_DICT класса Main.
    /// </summary>
    /// <param name="type">Тип оружия WeaponType, для которого требуется получить WeaponParameters</param>
    /// <returns>Экземпляр WeaponParameters или, если нет такого определения
    /// для указанного WeaponType, возвращает новый экземпляр WeaponParameters
    /// с типом none.</returns>
    static public WeaponParameters GetWeaponParameters(WeaponType type)
    {
        // Проверить наличие указанного ключа в словаре
        // Попытка извлечь значение по отсутствующему ключу вызовет ошибку,
        // поэтому следующая инструкция играет важную роль.
        if (WeaponDictionary.ContainsKey(type))
        {
            return WeaponDictionary[type];
        }

        // Следующая инструкция возвращает новый экземпляр WeaponDefinition
        // с типом оружия WeaponType.nоnе, что означает неудачную попытку
        // найти требуемое определение WeaponDefinition
        return new WeaponParameters();
    }
    public void AbsorbPowerUp (GameObject gameObj)
    {
        PowerUp powerUp = gameObj.GetComponent<PowerUp>();
        switch (powerUp.type)
        {
            case WeaponType.shield:
                ShieldLevel++;
                break;
            case WeaponType.turret:
                turretPrefab.gameObject.SetActive(true);
                turretPrefab.startTime = Time.time;
                break;
            case WeaponType.missile:
                Weapon missileLauncher = GetEmptyMissileLauncherSlot();
                if (missileLauncher != null)
                    missileLauncher.AddMissile();
                break;
            case WeaponType.bossCore:
                GameObject explosion = Instantiate(explosionPrefab);
                explosion.transform.position = transform.position + new Vector3(0, 5, 0);
                GameObject explosion1 = Instantiate(explosionPrefab);
                explosion1.transform.position = transform.position + new Vector3(5, 0, 0);
                GameObject explosion2 = Instantiate(explosionPrefab);
                explosion2.transform.position = transform.position + new Vector3(-5, 0, 0);
                WinScenario.main.EndLevel();
                break;
            default:
                if (powerUp.type == weapons[0].type)
                {
                    Weapon weapon = GetEmptyWeaponSlot();
                    if (weapon != null)
                    { weapon.SetType(powerUp.type); }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(powerUp.type);
                }
                break;
        }
        powerUp.AbsorbedBy(this.gameObject);
    }
    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        { 
            if ( weapons[i].type == WeaponType.none)
            { return weapons[i]; }
        }
        return null;
    }
    Weapon GetEmptyMissileLauncherSlot()
    {
        for (int i = 0; i < missileLaunchers.Length; i++)
        {
            if (missileLaunchers[i].isCharged == false)
            { return missileLaunchers[i]; }
        }
        return null;
    }
    void ClearWeapons()
    {
        foreach (Weapon weapon in weapons)
        { weapon.SetType(WeaponType.none);}
    }
}