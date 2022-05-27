using UnityEngine;

public class LaserProjectile : Projectile
{
    public float timeDuration = 2f; //время за которое изменится цвет
    public Material originalMaterial;
    public Material changedMaterial;

    [Header("Set Dynamically")]
    public Weapon currentWeapon;
    public float timeStart;
    public bool isNormalColor;

    private void Start()
    {
        timeStart = Time.time;
        isNormalColor = true;
    }
    private void Update()
    {
        if (Input.GetAxis("Jump") != 1 || currentWeapon.type != WeaponType.laser)
            Destroy(gameObject);
        Move();
        ChangeColor();
    }

    private void Move()
    {
        transform.position = currentWeapon.collar.transform.position + new Vector3(0, 50, 0);
    }

    private void ChangeColor()
    {
          float u = (Time.time - timeStart) / timeDuration;
          if (!isNormalColor)
            u = 1 - u;
        if (u > 1)
        {
            u = 1;
            timeStart = Time.time;
            isNormalColor = false;
        }
        else if (u < 0)
        {
            u = 0;
            timeStart = Time.time;
            isNormalColor = true;
        }

        rend.material.color = (1 - u) * originalMaterial.color + u * changedMaterial.color;
    }
}
