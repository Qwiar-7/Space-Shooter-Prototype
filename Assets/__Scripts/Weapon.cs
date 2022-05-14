using UnityEngine;


/// <summary>
/// ��� ������������ ���� ��������� ����� ������.
/// ����� �������� ��� "shield", ����� ���� ����������� ���������������� ������.
/// </summary>
public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield //����������� shieldLevel
}


/// <summary>
/// ����� WeaponDefinition ��������� ����������� ��������
/// ����������� ���� ������ � ����������. ��� ����� ����� Main
/// ����� ������� ������ ��������� ���� WeaponDefinition.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //����� �� ������, ������������ �����
    public Color color = Color.white; //���� ������ ������ � ������ ������
    public GameObject projectilePrefab; //������ ��������
    public Color projectileColor = Color.white; //���� ��������
    public float damageOnHit = 0; // �������������� ��������
    public float continuousDamage = 0; //������� ���������� � ������� (��� Laser)
    public float delayBetweenShots = 0; //�������� ����� ����������
    public float velocity = 20; //�������� ������ ��������
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    public WeaponType type = WeaponType.none;
    public WeaponDefinition weaponDef;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;

    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // ������� SetType(), ����� �������� ��� ������ �� ���������
        // WeaponType.none
        SetType(type);
        // ����������� ������� ����� �������� ��� ���� ��������
        if (PROJECTILE_ANCHOR ==  null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // ����� fireDelegate � �������� ������� �������
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)//�������� ������ �� ������
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public void SetType(WeaponType weaponType)
    {
        type = weaponType;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        weaponDef = Hero.GetWeaponDefinition(type);
        collarRend.material.color = weaponDef.color;
        lastShotTime = 0; // ����� ����� ��������� _type ����� ����������
    }

    public void Fire()
    {
        // ���� this.gameObject ���������, �����
        if (!gameObject.activeInHierarchy) return;
        // ���� ����� ���������� ������ ������������ ����� �������, �����
        if (Time.time - lastShotTime < weaponDef.delayBetweenShots) return;
        Projectile projectile;
        Vector3 velocity = Vector3.up * weaponDef.velocity;
        if (transform.up.y < 0)
        {
            velocity.y = -velocity.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                projectile = MakeProjectile();
                projectile.rigid.velocity = velocity;
                break;

            case WeaponType.spread:
                projectile = MakeProjectile();
                projectile.rigid.velocity = velocity;
                projectile = MakeProjectile();
                projectile.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);//������� ����� ��� Z
                projectile.rigid.velocity = projectile.transform.rotation * velocity;
                projectile = MakeProjectile();
                projectile.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                projectile.rigid.velocity = projectile.transform.rotation * velocity;
                break;
            case WeaponType.phaser:
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject gameObj = Instantiate<GameObject>(weaponDef.projectilePrefab);
        if (transform.parent.gameObject.CompareTag("Hero"))
        {
            gameObj.tag = "ProjectileHero";
            //go.layer = LayerMask.NameToLayer("ProjectileHero");
            gameObj.layer = 10;
        }
        else
        {
            gameObj.tag = "ProjectileEnemy";
            //go.layer = LayerMask.NameToLayer("ProjectileEnemy");
            gameObj.layer = 11;
        }
        gameObj.transform.position = collar.transform.position;
        gameObj.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile projectile = gameObj.GetComponent<Projectile>();
        projectile.Type = type;
        lastShotTime = Time.time;
        return (projectile);
    }
}
