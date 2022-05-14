using UnityEngine;


/// <summary>
/// Это перечисление всех возможных типов оружия.
/// Также включает тип "shield", чтобы дать возможность совершенствовать защиту.
/// </summary>
public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield //Увеличивает shieldLevel
}


/// <summary>
/// Класс WeaponDefinition позволяет настраивать свойства
/// конкретного вида оружия в инспекторе. Для этого класс Main
/// будет хранить массив элементов типа WeaponDefinition.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; //Буква на кубике, изображающем бонус
    public Color color = Color.white; //Цвет ствола оружия и кубика бонуса
    public GameObject projectilePrefab; //Шаблон снарядов
    public Color projectileColor = Color.white; //цвет снарядов
    public float damageOnHit = 0; // Разрушительная мощность
    public float continuousDamage = 0; //Степень разрушения в секунду (для Laser)
    public float delayBetweenShots = 0; //задержка между выстрелами
    public float velocity = 20; //Скорость полета снарядов
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

        // Вызвать SetType(), чтобы заменить тип оружия по умолчанию
        // WeaponType.none
        SetType(type);
        // Динамически создать точку привязки для всех снарядов
        if (PROJECTILE_ANCHOR ==  null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // Найти fireDelegate в корневом игровом объекте
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)//проверка оружия на игроке
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
        lastShotTime = 0; // Сразу после установки _type можно выстрелить
    }

    public void Fire()
    {
        // Если this.gameObject неактивен, выйти
        if (!gameObject.activeInHierarchy) return;
        // Если между выстрелами прошло недостаточно много времени, выйти
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
                projectile.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);//поворот вдоль оси Z
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
