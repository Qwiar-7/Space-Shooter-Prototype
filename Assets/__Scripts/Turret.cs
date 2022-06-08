using System.Collections.Generic;
using UnityEngine;

public class Turret : Weapon
{
    Dictionary<float, Vector3> distances;

    [Header("Set in Inspector: Turret")]
    public int turretTimeWork = 10;

    [Header("Set Dynamically: Turret")]
    public float startTime;
    bool isEnemyVisible = false;
    private void Update()
    {
            MakeShot();
    }
    public override void MakeShot()
    {
        if (!gameObject.activeInHierarchy) return;
        // Если между выстрелами прошло недостаточно много времени, выйти
        if (Time.time - lastShotTime < weaponParam.delayBetweenShots) return;

        FindClosestEnemy();

        if (isEnemyVisible && Time.time - startTime < 10f)
        {
            Vector3 velocity = Vector3.Normalize(transform.up) * weaponParam.velocity;
            Projectile projectile;
            projectile = MakeProjectileShot();
            projectile.transform.rotation = collar.transform.rotation;
            projectile.rigid.velocity = velocity;
        }
    }
    private void FindClosestEnemy()
    {
        Vector3 closestEnemy;
        float distMin = 1000f;
        distances = new Dictionary<float, Vector3>();
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            distances.Add(Mathf.Abs(Vector3.Distance(transform.position, enemy.transform.position)), enemy.transform.position);
        }
        foreach (var distance in distances)
        {
            distMin = Mathf.Min(distMin, distance.Key);
        }
        if (distances.Count == 0)
        {
            transform.up = Vector3.up;
            isEnemyVisible = false;
            return;
        }

        distances.TryGetValue(distMin, out closestEnemy);
        transform.up = closestEnemy - transform.position;
        isEnemyVisible = true;
    }
}