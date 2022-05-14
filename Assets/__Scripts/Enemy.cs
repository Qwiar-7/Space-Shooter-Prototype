using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    public int score = 100;
    public float showDamageDuration = 0.1f; // ������������ ������� ��������� � ��������
    public float powerUpDropChance = 1f; //����������� �������� �����

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; // ��� ��������� �������� ������� � ��� ��������
    public bool showingDamage = false;
    public float damageDoneTime; // ����� ����������� ����������� �������
    public bool notifiedOfDestruction = false;

    protected BoundsCheck boundsCheck;

    public Vector3 Position
    {
        get {  return this.transform.position; }
        set {  this.transform.position = value; }
    }

    private void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        // �������� ��������� � ���� ����� �������� ������� � ��� ��������
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
        {
            UnShowDamage();
        }

        if (boundsCheck != null && boundsCheck.offDown)
        {
                Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile projectile = otherGO.GetComponent<Projectile>();
                // ���� ��������� ������� �� ��������� ������,
                // �� �������� ��� �����������.
                if (!boundsCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                // �������� ��������� �������
                ShowDamage();
                // �������� ����������� ���� �� WEAP_DICT � ������ Main,
                health -= Hero.GetWeaponDefinition(projectile.Type).damageOnHit;
                if (health <= 0)
                {
                    // �������� �������-�������� Main �� �����������
                    if (!notifiedOfDestruction)
                        Main.mainObj.ShipDestroyed(this);
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }

    public virtual void Move()
    {
        Vector3 tempPosition = Position;
        tempPosition.y -= speed * Time.deltaTime;
        Position = tempPosition;
    }

    void ShowDamage()
    {
        foreach (Material material in materials)
        {
            material.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}
