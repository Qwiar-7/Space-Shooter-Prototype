using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float continuousDamage = 0;
    protected BoundsCheck bndCheck;
    protected Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    protected WeaponType type;

    public WeaponType Type //возможно поменять, setType?
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
        WeaponParameters def = Hero.GetWeaponParameters(type);
        rend.material.color = def.projectileColor;
    }
}
