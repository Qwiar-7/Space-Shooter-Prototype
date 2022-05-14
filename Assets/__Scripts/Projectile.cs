using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType type;

    public WeaponType Type
    {
        get { return type; }
        set { SetType(value); }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //удалить снаряд после выхода за пределы экрана
        if (bndCheck.offUp)
            Destroy(gameObject);
    }

    /// <summary>
    /// Изменяет скрытое поле _type и устанавливает цвет этого снаряда,
    /// как определено в WeaponDefinition.
    /// </summary>
    /// <param name="currenType">Тип WeaponType используемого оружия.</param>
    public void SetType(WeaponType currenType)
    {
        type = currenType;
        WeaponDefinition def = Hero.GetWeaponDefinition(type);
        rend.material.color = def.projectileColor;
    }
}
