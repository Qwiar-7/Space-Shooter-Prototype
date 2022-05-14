using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // ќпредел€ют, насколько €рко будет выражен синусоидальный характер движени€
    public float sinEccentricity = 0.6f;
    public float lifeTime = 6;

    [Header("Set Dynamically: Enemy_2")]
    // Enemy_2 использует линейную интерпол€цию между двум€ точками,
    // измен€€ результат по синусоиде
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    private void Start()
    {
        // ¬ыбрать случайную точку на левой границе экрана
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // ¬ыбрать случайную точку на правой границе экрана
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // —лучайно помен€ть начальную и конечную точку местами
        if (Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;
    }

    public override void Move()
    {
        //  ривые Ѕезье вычисл€ютс€ на основе значени€ и между 0 и 1
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 100)
        {
            Destroy(this.gameObject);
            return;
        }

        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2)); //дл€ ослаблени€ эффекта замедлени€

        pos = (1 - u) * p0 + u * p1;
    }
}
