using System.Collections;
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
    public WeaponParameters[] weaponParams;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 4;

    private GameObject lastTriggeredGameObj = null;

    public float ShieldLevel
    {
        get {  return _shieldLevel; }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
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
        weapons[0].SetType(WeaponType.phaser);
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
