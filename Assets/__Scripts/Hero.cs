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
    public Weapon[] weapons;
    public Turret turret;
    public Weapon[] missileLaunchers;
    public WeaponParameters[] weaponParams;
    public GameObject explosionPrefab;

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
                Destroy(this.gameObject);
                Main.mainObj.DelayedRestart();
            }
        }
    }

    private void Awake()
    {
        // ������� � ������� ���� WeaponType
        WeaponDictionary = new Dictionary<WeaponType, WeaponParameters>();
        foreach (WeaponParameters weapontDef in weaponParams)
        {
            WeaponDictionary[weapontDef.type] = weapontDef;
        }
        if (heroObj == null)
            heroObj = this;
        else
            Debug.LogError("Hero.Awake() - ������� ������� 2-�� ��������� Hero.S!");

    }
    private void Start()
    {
        ClearWeapons();
        weapons[0].SetType(WeaponType.laser);
        weapons[1].SetType(WeaponType.laser);
        weapons[2].SetType(WeaponType.laser);
        weapons[3].SetType(WeaponType.laser);
        weapons[4].SetType(WeaponType.laser);
        turret.SetType(WeaponType.turret);
        turret.gameObject.SetActive(false);
        missileLaunchers[0].SetType(WeaponType.missile);
        missileLaunchers[1].SetType(WeaponType.missile);
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 position = transform.position;
        position.x += xAxis * speed * Time.deltaTime;
        position.y += yAxis * speed * Time.deltaTime;
        
        transform.SetPositionAndRotation(position, Quaternion.Euler(yAxis * pitchMultiplier, xAxis * rollMultiplier, 0));

       if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.JoystickButton2))
            foreach (var missilelauncher in missileLaunchers)
            {
                if (missilelauncher.isCharged)
                {
                    missilelauncher.MakeAlternateShot();
                    break;
                }
            }

        // ������� ��������� ������� �������: Axis("Jump")
        if (Input.GetAxis("Jump") == 1)
            foreach (var weapon in weapons)
            {
                weapon.MakeShot();
            }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        Transform otherRootTransform = otherCollider.gameObject.transform.root;
        GameObject otherGO = otherRootTransform.gameObject;

        if (otherGO == lastTriggeredGameObj) return;

        lastTriggeredGameObj = otherGO;

        if (otherGO.CompareTag("Enemy"))
        {
            ShieldLevel--;
            Destroy(otherGO);
        }
        else if (otherGO.CompareTag("PowerUp"))
            AbsorbPowerUp(otherGO);
        else
            print("Triggered by non-Enemy: " + otherGO.name);
    }

    /// <summary>
    /// �������������� ������� ������������ WeaponParameters �� ���������������
    /// ����������� ���� WEAP_DICT ������ Main.
    /// </summary>
    /// <param name="type">��� ������ WeaponType, ��� �������� ��������� �������� WeaponParameters</param>
    /// <returns>��������� WeaponParameters ���, ���� ��� ������ �����������
    /// ��� ���������� WeaponType, ���������� ����� ��������� WeaponParameters
    /// � ����� none.</returns>
    static public WeaponParameters GetWeaponParameters(WeaponType type)
    {
        // ��������� ������� ���������� ����� � �������
        // ������� ������� �������� �� �������������� ����� ������� ������,
        // ������� ��������� ���������� ������ ������ ����.
        if (WeaponDictionary.ContainsKey(type))
        {
            return WeaponDictionary[type];
        }

        // ��������� ���������� ���������� ����� ��������� WeaponDefinition
        // � ����� ������ WeaponType.n�n�, ��� �������� ��������� �������
        // ����� ��������� ����������� WeaponDefinition
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
                turret.gameObject.SetActive(true);
                turret.isCombatModeOn = true;
                Invoke(nameof(DisableTurret), turret.turretTimeWork);
                break;
            case WeaponType.missile:
                Weapon missileLauncher = GetEmptyMissileLauncherSlot();
                if (missileLauncher != null)
                    missileLauncher.AddMissile();
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

    void DisableTurret()
    {
        turret.isCombatModeOn = false;
    }
}
