using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Set in Inspector")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f; //����� �� �����������
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 4;

    private GameObject lastTriggerGo = null;

    // ���������� ������ �������� ���� WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    // ������� ���� ���� WeaponFireDelegate � ������ fireDelegate.
    public WeaponFireDelegate fireDelegate;

    public float shieldLevel
    {
        get {  return _shieldLevel; }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    private void Start()
    {
        if (S == null)
            S = this;
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

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))

        // ���������� ������� �� ���� ����� ������ ������� fireDelegate
        // ������� ��������� ������� �������: Axis("Jump")
        // ����� ���������, ��� �������� fireDelegate �� ����� null,
        // ����� �������� ������
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
            fireDelegate();
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;

        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
            AbsorbPowerUp(go);
        else
            print("Triggered by non-Enemy: " + go.name);
    }

    public void AbsorbPowerUp (GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    { w.SetType(pu.type); }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                
                break;
        }
        pu.AbsorbedBy(this.gameObject);
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
        foreach (Weapon w in weapons)
        { w.SetType(WeaponType.none);}
    }
}