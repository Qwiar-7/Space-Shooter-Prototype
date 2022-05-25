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
public class WeaponParameters
{
    public WeaponType type = WeaponType.none;
    public string letter; //����� �� ������, ������������ �����
    public Color color = Color.white; //���� ������ ������ � ������ ������
    public GameObject projectilePrefab; //������ ��������
    public Color projectileColor = Color.white; //���� ��������
    //public float damageOnHit = 0; // �������������� ��������
    //public float continuousDamage = 0; //������� ���������� � ������� (��� Laser)
    public float delayBetweenShots = 0; //�������� ����� ����������
    public float velocity = 20; //�������� ������ ��������
}
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    public WeaponType type = WeaponType.none;
    public WeaponParameters weaponParam;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;

    LaserProjectile projectile;

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
            GameObject go = new GameObject("Projectile_Anchor");
            PROJECTILE_ANCHOR = go.transform;
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
        weaponParam = Hero.GetWeaponParameters(type);
        collarRend.material.color = weaponParam.color;
        lastShotTime = -weaponParam.delayBetweenShots; // ����� ����� ��������� _type ����� ����������
    }

    public void MakeShot()
    {
        // ���� this.gameObject ���������, �����
        if (!gameObject.activeInHierarchy) return;
        // ���� ����� ���������� ������ ������������ ����� �������, �����
        if (Time.time - lastShotTime < weaponParam.delayBetweenShots) return;
        Vector3 velocity = Vector3.up * weaponParam.velocity;
        if (transform.up.y < 0)
        {
            velocity.y = -velocity.y;
        }
        
        if(type == WeaponType.blaster)
        {
            Projectile projectile;
            projectile = MakeProjectileShot();
            projectile.rigid.velocity = velocity;
        }
        else if(type == WeaponType.spread)
        {
            Projectile projectile;
            projectile = MakeProjectileShot();
            projectile.rigid.velocity = velocity;
            projectile = MakeProjectileShot();
            projectile.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);//������� ����� ��� Z
            projectile.rigid.velocity = projectile.transform.rotation * velocity;
            projectile = MakeProjectileShot();
            projectile.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
            projectile.rigid.velocity = projectile.transform.rotation * velocity;
        }
        else if (type == WeaponType.phaser)
        {
            PhaserProjectile projectile;
            projectile = MakeProjectileShot() as PhaserProjectile;
            projectile.transform.position = collar.transform.position + Vector3.right;
            projectile.rigid.velocity = velocity;
            projectile = MakeProjectileShot() as PhaserProjectile;
            projectile.isReverse = true;
            projectile.transform.position = collar.transform.position + Vector3.left;
            projectile.rigid.velocity = velocity;
        }
        else if (type == WeaponType.laser)
        {
            if (projectile != null) return;
            projectile = MakeLaserShot();
            projectile.transform.position = collar.transform.position + new Vector3(0, 50, 0);
            projectile.currentWeapon = this;
        }
    }

    public Projectile MakeProjectileShot()
    {
        GameObject gameObj = Instantiate(weaponParam.projectilePrefab);
        if (transform.parent.gameObject.CompareTag("Hero"))
        {
            gameObj.tag = "ProjectileHero";
            gameObj.layer = 10;
        }
        else
        {
            gameObj.tag = "ProjectileEnemy";
            gameObj.layer = 11;
        }
        gameObj.transform.position = collar.transform.position;
        gameObj.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile projectile = gameObj.GetComponent<Projectile>();
        projectile.Type = type;
        lastShotTime = Time.time;
        return (projectile);
    }

    public LaserProjectile MakeLaserShot()
    {
        GameObject gameObj = Instantiate(weaponParam.projectilePrefab);
        if (transform.parent.gameObject.CompareTag("Hero"))
        {
            gameObj.tag = "ProjectileHero";
            gameObj.layer = 10;
        }
        else
        {
            gameObj.tag = "ProjectileEnemy";
            gameObj.layer = 11;
        }
        gameObj.transform.position = collar.transform.position;
        gameObj.transform.SetParent(PROJECTILE_ANCHOR, true);
        LaserProjectile projectile = gameObj.GetComponent<LaserProjectile>();
        projectile.Type = type;
        lastShotTime = Time.time;
        return (projectile);
    }
}
