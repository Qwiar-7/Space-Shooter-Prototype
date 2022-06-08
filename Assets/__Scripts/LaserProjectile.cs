using UnityEngine;

public class LaserProjectile : Projectile
{
    [Header("Set in Inspector: Laser")]
    public float timeToChangeColor = 2f; //время за которое изменится цвет
    public Material originalMaterial;
    public Material changedMaterial;

    [Header("Set Dynamically: Laser")]
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
        if (Input.GetAxis("Jump") == 0 && Input.touchCount == 0 || currentWeapon.type != WeaponType.laser || Hero.heroObj.isDead)
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
          float u = (Time.time - timeStart) / timeToChangeColor;
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