using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    // �������� ���� ���� ����� ������ ������������ � ����������
    public string name; // ��� ���� �����
    public float health; // ������� ��������� ���� �����
    public string[] protectedBy; // ������ �����, ���������� ���

    [HideInInspector]
    public GameObject GameObj;
    [HideInInspector]
    public Material material;
}

/// <summary>
/// Enemy_4 ��������� �� ������� ��������, �������� ��������� ����� �� ������
/// � ������������ � ���. ���������� �� �����, �������� ������ ��������� �����
/// � ���������� ���������, ���� ����� �� ��������� ���
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // ������ ������, ������������ �������
    public int numberOfMoves = 5;

    private Vector3 pos0, pos1; // ��� ����� ��� ������������
    private float timeStart; // ����� �������� ����� �������
    private readonly float duration = 4; // ����������������� �����������

    private void Start()
    {
        // ��������� ������� ��� ������� � Main.SpawnEnemy(),
        // ������� ������� �� ��� ��������� �������� � �0 � p1
        pos0 = pos1 = Position;
        InitMovement();

        Transform transf;
        foreach (Part part in parts)
        {
            transf = transform.Find(part.name);
            if (transf != null)
            {
                part.GameObj = transf.gameObject;
                part.material = part.GameObj.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        pos0 = pos1;
        // ������� ����� ����� �� ������
        float widthMinRad = boundsCheck.cameraWidth - boundsCheck.radius;
        float heightMinRad = boundsCheck.cameraHeight - boundsCheck.radius;
        pos1.x = Random.Range( -widthMinRad, widthMinRad);
        pos1.y = Random.Range( -heightMinRad, heightMinRad);

        // �������� �����
        timeStart = Time.time;
    }

    void LastMovement()
    {
        pos0 = pos1;
        float widthMinRad = boundsCheck.cameraWidth + boundsCheck.radius; // ������� ����� �� ��������� ������
        if (Random.value > 0.5f)
            widthMinRad *= -1;
        float heightMinRad = boundsCheck.cameraHeight - boundsCheck.radius;
        pos1.x = widthMinRad;
        pos1.y = Random.Range(-heightMinRad, heightMinRad);

        // �������� �����
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            if (numberOfMoves <= 1)
            {
                LastMovement();
                Invoke(nameof(EnemyDestroy), duration);
            }
            else
                InitMovement();
            u = 0;
            numberOfMoves--;
        }

        u = 1 - Mathf.Pow(1-u, 2); // ��������� ������� ����������
        Position = (1 - u) * pos0 + u * pos1; // ������� �������� ������������
    }

    // ��� ��� ������� ��������� ����� ����� � ������� parts �
    // �� ����� ��� �� ������ �� ������� ������
    Part FindPart (GameObject GameObj)
    {
        foreach (Part part in parts)
        {
            if (part.GameObj == GameObj)
                return part;
        }
        return null;
    }

    Part FindPart(string partName)
    {
        foreach (Part part in parts)
        {
            if (part.name == partName)
                return part;
        }
        return null;
    }

    // ��� ������� ���������� true, ���� ������ ����� ����������
    /*bool Destroyed(GameObject GameObj)
    {
        return (Destroyed(FindPart(GameObj)));
    }*/

    bool Destroyed(string name)
    {
        return (Destroyed(FindPart(name)));
    }

    bool Destroyed(Part part)
    {
        if (part == null) return true; // ���� ������ �� ����� �� ���� �������� - ������� true (�� ����: ��, ���� ����������)

        // ������� ��������� ���������: prt.health <= 0
        // ���� prt.health <= 0, ������� true (��, ���� ����������)
        return (part.health <= 0);
    }

    // ���������� � ������� ������ ���� �����, � �� ���� �������.
    void ShowLocalizedDamage(Material material)
    {
        material.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    private void OnCollisionEnter(Collision otherCollision)
    {
        GameObject otherGO = otherCollision.gameObject;
        switch(otherGO.tag)
        {
            case "ProjectileHero":
                Projectile projectile = otherGO.GetComponent<Projectile>();
                bool isLaser = otherGO.GetComponent<LaserProjectile>() != null;
                // ���� ������� �� ��������� ������, �� ���������� ���.
                if (!boundsCheck.isOnScreen)
                {
                    if (!isLaser)
                        Destroy(otherGO);
                    break;
                }

                Missile missile = otherGO.GetComponent<Missile>();
                if (missile != null)
                {
                    GameObject explosion = Instantiate(explosionPrefab);
                    explosion.transform.position = missile.transform.position;
                }

                // �������� ��������� �������
                GameObject hitedGO = otherCollision.contacts[0].thisCollider.gameObject;
                Part hitedPart = FindPart(hitedGO);
                if (hitedPart == null)
                {
                    hitedGO = otherCollision.contacts[0].otherCollider.gameObject;
                    hitedPart = FindPart(hitedGO);
                }

                // ���������, �������� �� ��� ��� ����� �������
                if (hitedPart.protectedBy != null)
                {
                    foreach( string str in hitedPart.protectedBy)
                    {
                        // ���� ���� �� ���� �� ���������� ������ ���
                        // �� ���������...
                        if (!Destroyed(str))
                        {
                            if (!isLaser)
                                Destroy(otherGO); // ...�� �������� ����������� ���� ����� ���������� ������ ProjectileHero
                            return; // �����, �� ��������� Enemy_4
                        }
                    }
                }

                // ��� ����� �� ��������, ������� �� �����������
                // �������� ����������� ���� �� Projectile.type � Main.WEAP_DICT
                hitedPart.health -= projectile.damage;
                hitedPart.health -= projectile.continuousDamage * Time.fixedDeltaTime;
                // �������� ������ ��������� � �����
                ShowLocalizedDamage(hitedPart.material);
                if (hitedPart.health <= 0)
                {
                    // ������ ���������� ����� �������
                    // �������������� ������������ �����
                    hitedPart.GameObj.SetActive(false);
                }
                // ���������, ��� �� ������� ��������� ��������
                bool allDestroyed = true; // ������������, ��� ��������
                foreach (Part part in parts)
                {
                    if(!Destroyed(part))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    if (!notifiedOfDestruction)
                    {
                        PowerUpDrop();
                        ScoreManager.UpdateCurrentScore(score);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                }
                if (!isLaser)
                    Destroy(otherGO); // ���������� ������ ProjectileHero
                break;
        }
    }

    private void OnCollisionStay(Collision otherCollision)
    {
        GameObject otherGO = otherCollision.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile projectile = otherGO.GetComponent<Projectile>();
                bool isLaser = otherGO.GetComponent<LaserProjectile>() != null;
                // ���� ������� �� ��������� ������, �� ���������� ���.
                if (!boundsCheck.isOnScreen)
                {
                    if (!isLaser)
                        Destroy(otherGO);
                    break;
                }
                // �������� ��������� �������
                GameObject hitedGO = otherCollision.contacts[0].thisCollider.gameObject;
                Part hitedPart = FindPart(hitedGO);
                if (hitedPart == null)
                {
                    hitedGO = otherCollision.contacts[0].otherCollider.gameObject;
                    hitedPart = FindPart(hitedGO);
                }

                // ���������, �������� �� ��� ��� ����� �������
                if (hitedPart.protectedBy != null)
                {
                    foreach (string str in hitedPart.protectedBy)
                    {
                        // ���� ���� �� ���� �� ���������� ������ ���
                        // �� ���������...
                        if (!Destroyed(str))
                        {
                            if (!isLaser)
                                Destroy(otherGO); // ...�� �������� ����������� ���� ����� ���������� ������ ProjectileHero
                            return; // �����, �� ��������� Enemy_4
                        }
                    }
                }

                // ��� ����� �� ��������, ������� �� �����������
                // �������� ����������� ���� �� Projectile.type � Main.WEAP_DICT
                hitedPart.health -= projectile.damage;
                hitedPart.health -= projectile.continuousDamage * Time.fixedDeltaTime;
                // �������� ������ ��������� � �����
                ShowLocalizedDamage(hitedPart.material);
                if (hitedPart.health <= 0)
                {
                    // ������ ���������� ����� �������
                    // �������������� ������������ �����
                    hitedPart.GameObj.SetActive(false);
                }
                // ���������, ��� �� ������� ��������� ��������
                bool allDestroyed = true; // ������������, ��� ��������
                foreach (Part part in parts)
                {
                    if (!Destroyed(part))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    if (!notifiedOfDestruction)
                    {
                        PowerUpDrop();
                        ScoreManager.UpdateCurrentScore(score);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                }
                if (!isLaser)
                    Destroy(otherGO); // ���������� ������ ProjectileHero
                break;
        }
    }

    void EnemyDestroy()
    {
        Destroy(gameObject);
    }
}
