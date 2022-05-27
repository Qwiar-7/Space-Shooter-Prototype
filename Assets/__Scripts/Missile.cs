using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Projectile
{
    public bool isLaunched = false;
    public Weapon currentMissileLauncher;
    public GameObject smoke;
    public GameObject explosionPrefab;
    Vector3 closestEnemy;
    public float accelerationTime = 0.5f;
    public float launchTime;

    Dictionary<float, Vector3> distances;
    public bool isEnemyVisible = false;
    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        smoke = transform.GetChild(2).gameObject;
        smoke.SetActive(false);
    }
    void Update()
    {
        if (!bndCheck.isOnScreen || currentMissileLauncher == null)
        {
            Destroy(gameObject);
            return;
        }
        if (!isLaunched)
        {
            transform.position = currentMissileLauncher.transform.position;
            return;
        }
        FindClosestEnemy();
        transform.up = Vector3.Lerp(transform.up, Vector3.Normalize(closestEnemy - transform.position), 0.05f);
        float u = (Time.time - launchTime) / accelerationTime;
        if (u < 1)
            rigid.velocity = Vector3.Lerp(Vector3.zero, transform.up * currentMissileLauncher.weaponParam.velocity, u);
        else
            rigid.velocity = transform.up * currentMissileLauncher.weaponParam.velocity;
    }

    private void FindClosestEnemy()
    {
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
        isEnemyVisible = true;
    }
}
