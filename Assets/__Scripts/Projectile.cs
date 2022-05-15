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

    public WeaponType Type //�������� ��������, setType?
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
        //������� ������ ����� ������ �� ������� ������
        if (bndCheck.offUp)
            Destroy(gameObject);
    }

    /// <summary>
    /// �������� ������� ���� _type � ������������� ���� ����� �������,
    /// ��� ���������� � WeaponDefinition.
    /// </summary>
    /// <param name="currenType">��� WeaponType ������������� ������.</param>
    public void SetType(WeaponType currenType)
    {
        type = currenType;
        WeaponParameters def = Hero.GetWeaponParameters(type);
        rend.material.color = def.projectileColor;
    }
}
