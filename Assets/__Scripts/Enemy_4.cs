using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    // Занчения этих трех полей должны определяться в инспекторе
    public string name; // Имя этой части
    public float health; // Степень стойкости этой части
    public string[] protectedBy; // Другие части, защищающие эту

    [HideInInspector]
    public GameObject GameObj;
    [HideInInspector]
    public Material material;
}

/// <summary>
/// Enemy_4 создается за верхней границей, выбирает случайную точку на экране
/// и перемещается к ней. Добравшись до места, выбирает другую случайную точку
/// и продолжает двигаться, пока игрок не уничтожит его»
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts; // Массив частей, составляющих корабль
    public int numberOfMoves = 5;

    private Vector3 pos0, pos1; // Две точки для интерполяции
    private float timeStart; // Время создания этого корабля
    private readonly float duration = 4; // Продолжительность перемещения

    private void Start()
    {
        // Начальная позиция уже выбрана в Main.SpawnEnemy(),
        // поэтому запишем ее как начальные значения в р0 и p1
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
        // Выбрать новую точку на экране
        float widthMinRad = boundsCheck.cameraWidth - boundsCheck.radius;
        float heightMinRad = boundsCheck.cameraHeight - boundsCheck.radius;
        pos1.x = Random.Range( -widthMinRad, widthMinRad);
        pos1.y = Random.Range( -heightMinRad, heightMinRad);

        // Сбросить время
        timeStart = Time.time;
    }

    void LastMovement()
    {
        pos0 = pos1;
        float widthMinRad = boundsCheck.cameraWidth + boundsCheck.radius; // выбрать точку за пределами экрана
        if (Random.value > 0.5f)
            widthMinRad *= -1;
        float heightMinRad = boundsCheck.cameraHeight - boundsCheck.radius;
        pos1.x = widthMinRad;
        pos1.y = Random.Range(-heightMinRad, heightMinRad);

        // Сбросить время
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

        u = 1 - Mathf.Pow(1-u, 2); // Применить плавное замедление
        Position = (1 - u) * pos0 + u * pos1; // Простая линейная интерполяция
    }

    // Эти две функции выполняют поиск части в массиве parts п
    // по имени или по ссылке на игровой объект
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

    // Эти функции возвращают true, если данная часть уничтожена
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
        if (part == null) return true; // Если ссылка на часть не была передана - вернуть true (то есть: да, была уничтожена)

        // Вернуть результат сравнения: prt.health <= 0
        // Если prt.health <= 0, вернуть true (да, была уничтожена)
        return (part.health <= 0);
    }

    // Окрашивает в красный только одну часть, а не весь корабль.
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
                // Если корабль за границами экрана, не повреждать его.
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

                // Поразить вражеский корабль
                GameObject hitedGO = otherCollision.contacts[0].thisCollider.gameObject;
                Part hitedPart = FindPart(hitedGO);
                if (hitedPart == null)
                {
                    hitedGO = otherCollision.contacts[0].otherCollider.gameObject;
                    hitedPart = FindPart(hitedGO);
                }

                // Проверить, защищена ли еще эта часть корабля
                if (hitedPart.protectedBy != null)
                {
                    foreach( string str in hitedPart.protectedBy)
                    {
                        // Если хотя бы одна из защищающих частей еще
                        // не разрушена...
                        if (!Destroyed(str))
                        {
                            if (!isLaser)
                                Destroy(otherGO); // ...не наносить повреждений этой части Уничтожить снаряд ProjectileHero
                            return; // выйти, не повреждая Enemy_4
                        }
                    }
                }

                // Эта часть не защищена, нанести ей повреждение
                // Получить разрушающую силу из Projectile.type и Main.WEAP_DICT
                hitedPart.health -= projectile.damage;
                hitedPart.health -= projectile.continuousDamage * Time.fixedDeltaTime;
                // Показать эффект попадания в часть
                ShowLocalizedDamage(hitedPart.material);
                if (hitedPart.health <= 0)
                {
                    // Вместо разрушения всего корабля
                    // деактивировать уничтоженную часть
                    hitedPart.GameObj.SetActive(false);
                }
                // Проверить, был ли корабль полностью разрушен
                bool allDestroyed = true; // Предположить, что разрушен
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
                    Destroy(otherGO); // Уничтожить снаряд ProjectileHero
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
                // Если корабль за границами экрана, не повреждать его.
                if (!boundsCheck.isOnScreen)
                {
                    if (!isLaser)
                        Destroy(otherGO);
                    break;
                }
                // Поразить вражеский корабль
                GameObject hitedGO = otherCollision.contacts[0].thisCollider.gameObject;
                Part hitedPart = FindPart(hitedGO);
                if (hitedPart == null)
                {
                    hitedGO = otherCollision.contacts[0].otherCollider.gameObject;
                    hitedPart = FindPart(hitedGO);
                }

                // Проверить, защищена ли еще эта часть корабля
                if (hitedPart.protectedBy != null)
                {
                    foreach (string str in hitedPart.protectedBy)
                    {
                        // Если хотя бы одна из защищающих частей еще
                        // не разрушена...
                        if (!Destroyed(str))
                        {
                            if (!isLaser)
                                Destroy(otherGO); // ...не наносить повреждений этой части Уничтожить снаряд ProjectileHero
                            return; // выйти, не повреждая Enemy_4
                        }
                    }
                }

                // Эта часть не защищена, нанести ей повреждение
                // Получить разрушающую силу из Projectile.type и Main.WEAP_DICT
                hitedPart.health -= projectile.damage;
                hitedPart.health -= projectile.continuousDamage * Time.fixedDeltaTime;
                // Показать эффект попадания в часть
                ShowLocalizedDamage(hitedPart.material);
                if (hitedPart.health <= 0)
                {
                    // Вместо разрушения всего корабля
                    // деактивировать уничтоженную часть
                    hitedPart.GameObj.SetActive(false);
                }
                // Проверить, был ли корабль полностью разрушен
                bool allDestroyed = true; // Предположить, что разрушен
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
                    Destroy(otherGO); // Уничтожить снаряд ProjectileHero
                break;
        }
    }

    void EnemyDestroy()
    {
        Destroy(gameObject);
    }
}
