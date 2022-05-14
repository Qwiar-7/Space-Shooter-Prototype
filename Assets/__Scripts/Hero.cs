using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero heroObj;
    static Dictionary<WeaponType, WeaponDefinition> WeaponDictionary;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMultiplier = -45;
    public float pitchMultiplier = 30;
    public float gameRestartDelay = 2f; //����� �� �����������
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public WeaponDefinition[] weaponDefinitions;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 4;

    private GameObject lastTriggeredGameObj = null;

    // ���������� ������ �������� ���� WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // ������� ���� ���� WeaponFireDelegate � ������ fireDelegate.
    public WeaponFireDelegate fireDelegate;

    public float ShieldLevel
    {
        get {  return _shieldLevel; }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.mainObj.DelayedRestart(gameRestartDelay);
            }
        }
    }

    private void Awake()
    {
        // ������� � ������� ���� WeaponType
        WeaponDictionary = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition weapontDef in weaponDefinitions)
        {
            WeaponDictionary[weapontDef.type] = weapontDef;
        }
    }
    private void Start()
    {
        if (heroObj == null)
            heroObj = this;
        else
            Debug.LogError("Hero.Awake() - ������� ������� 2-�� ��������� Hero.S!");
        //fireDelegate += TempFire;
        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 position = transform.position;
        position.x += xAxis * speed * Time.deltaTime;
        position.y += yAxis * speed * Time.deltaTime;
        
        transform.SetPositionAndRotation(position, Quaternion.Euler(yAxis * pitchMultiplier, xAxis * rollMultiplier, 0));

        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))

        // ���������� ������� �� ���� ����� ������ ������� fireDelegate
        // ������� ��������� ������� �������: Axis("Jump")
        // ����� ���������, ��� �������� fireDelegate �� ����� null,
        // ����� �������� ������
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
            fireDelegate();
    }

    private void OnTriggerEnter(Collider otherGO)
    {
        Transform rootTransform = otherGO.gameObject.transform.root;
        GameObject gameObj = rootTransform.gameObject;

        if (gameObj == lastTriggeredGameObj) return;

        lastTriggeredGameObj = gameObj;

        if (gameObj.CompareTag("Enemy"))
        {
            ShieldLevel--;
            Destroy(gameObj);
        }
        else if (gameObj.CompareTag("PowerUp"))
            AbsorbPowerUp(gameObj);
        else
            print("Triggered by non-Enemy: " + gameObj.name);
    }

    /// <summary>
    /// �������������� �������� ������������ WeaponDefinition �� ���������������
    /// ����������� ���� WEAP_DICT ������ Main.
    /// </summary>
    /// <param name="type">��� ������ WeaponType, ��� �������� ��������� �������� WeaponDefinition</param>
    /// <returns>��������� WeaponDefinition ���, ���� ��� ������ �����������
    /// ��� ���������� WeaponType, ���������� ����� ��������� WeaponDefinition
    /// � ����� none.</returns>
    static public WeaponDefinition GetWeaponDefinition(WeaponType type)
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
        return new WeaponDefinition();
    }

    public void AbsorbPowerUp (GameObject gameObj)
    {
        PowerUp powerUp = gameObj.GetComponent<PowerUp>();
        switch (powerUp.type)
        {
            case WeaponType.shield:
                ShieldLevel++;
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

    void ClearWeapons()
    {
        foreach (Weapon weapon in weapons)
        { weapon.SetType(WeaponType.none);}
    }
}
