using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public float showDamageDuration = 0.1f; // Длительность эффекта попадания в секундах
    public float powerUpDropChance = 1f; //Вероятность сбросить бонус
    public int score = 100;
    public GameObject prefabPowerUp;
    public GameObject explosionPrefab;
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.shield };

    [Header("Set Dynamically: Enemy")]
    public float damageDoneTime; // Время прекращения отображения эффекта
    public bool showingDamage = false;
    public bool notifiedOfDestruction = false;
    public Color[] originalColors;
    public Material[] materials; // Все материалы игрового объекта и его потомков

    protected BoundsCheck boundsCheck;
    public Vector3 Position
    {
        get {  return this.transform.position; }
        set {  this.transform.position = value; }
    }
    private void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        // Получить материалы и цвет этого игрового объекта и его потомков
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }
    private void Update()
    {
        Move();

        if (showingDamage && Time.time > damageDoneTime)
            UnShowDamage();
        if (boundsCheck != null && boundsCheck.offDown)
            Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision otherCollision)
    {
        checkObjectInCollision(otherCollision);
    }
    private void OnCollisionStay(Collision otherCollision)
    {
        checkObjectInCollision(otherCollision);
    }
    private void checkObjectInCollision(Collision otherCollision)
    {
        GameObject otherGO = otherCollision.gameObject;
        Projectile projectileHero = otherGO.GetComponent<Projectile>();
        bool isLaser = otherGO.GetComponent<LaserProjectile>() != null;
        if (notifiedOfDestruction)
        {
            if (!isLaser)
                Destroy(otherGO);
            return;
        }
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                // Если вражеский корабль за границами экрана,
                // не наносить ему повреждений.
                if (!boundsCheck.isOnScreen)
                {
                    if (!isLaser)
                        Destroy(otherGO);
                    break;
                }
                ShowDamage();
                health -= projectileHero.damage;
                health -= projectileHero.continuousDamage * Time.deltaTime;
                if (health <= 0)
                {
                    if (!notifiedOfDestruction) // Сообщить об уничтожении
                    {
                        PowerUpDrop();
                        ScoreManager.UpdateCurrentScore(score);
                    }
                    notifiedOfDestruction = true;
                    Destroy(gameObject);
                }
                Missile missile = otherGO.GetComponent<Missile>();
                if (missile != null)
                {
                    GameObject explosion = Instantiate(explosionPrefab);
                    explosion.transform.position = missile.transform.position;
                }
                if (!isLaser)
                    Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }
    protected void PowerUpDrop()
    {
        if (Random.value <= powerUpDropChance)
        {
            // Выбрать тип бонуса
            // Выбрать один из элементов в powerUpFrequency
            int index = Random.Range(0, powerUpFrequency.Length);
            WeaponType powerUpType = powerUpFrequency[index];

            // Создать экземпляр PowerUp
            GameObject gameObj = Instantiate(prefabPowerUp);
            PowerUp powerUp = gameObj.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            powerUp.SetType(powerUpType);
            // Поместить в место, где находился разрушенный корабль
            powerUp.transform.position = transform.position;
        }
    }
    public virtual void Move()
    {
        Vector3 tempPosition = Position;
        tempPosition.y -= speed * Time.deltaTime;
        Position = tempPosition;
    }
    private void ShowDamage()
    {
        foreach (Material material in materials)
        {
            material.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    private void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}