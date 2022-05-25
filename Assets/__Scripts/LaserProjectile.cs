using UnityEngine;

public class LaserProjectile : Projectile
{
    public Weapon currentWeapon;
    private void Update()
    {
        if (Input.GetAxis("Jump") != 1)
            Destroy(gameObject);
        Move();
    }

    private void Move()
    {
        transform.position = currentWeapon.collar.transform.position + new Vector3(0, 50, 0);
    }
}
