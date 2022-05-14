using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
    // Траектория движения Enemy_3 вычисляется путем линейной
    // интерполяции кривой Безье по более чем двум точкам.
    [Header("Set in Inspector: Enemy_3")]
    public float lifeTime = 5;

    [Header("Set Dynamically: Enemy_3")]
    public Vector3[] points;
    public float birthTime;

    private void Start()
    {
        points = new Vector3[3];

        // Начальная позиция уже определена в Main.SpawnEnemy()
        points[0] = Position;

        // Установить xMin и хМах так же, как это делает Main.SpawnEnemy()
        float xMin = -boundsCheck.cameraWidth + boundsCheck.radius;
        float xMax = boundsCheck.cameraWidth - boundsCheck.radius;

        Vector3 tempPos;
        // Случайно выбрать среднюю точку ниже нижней границы экрана
        tempPos = Vector3.zero;
        tempPos.x = Random.Range(xMin, xMax);
        tempPos.y = -boundsCheck.cameraHeight * Random.Range(2.75f, 2);
        points[1] = tempPos;

        // Случайно выбрать конечную точку выше верхней границы экрана
        tempPos = Vector3.zero;
        tempPos.y = Position.y;
        tempPos.x = Random.Range(xMin, xMax);
        points[2] = tempPos;

        birthTime = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;

        if (u>1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Интерполировать кривую Безье по трем точкам
        Vector3 pos01, pos12;
        u -= 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        pos01 = (1 - u) * points[0] + u * points[1];
        pos12 = (1 - u) * points[1] + u * points[2];
        Position = (1 - u) * pos01 + u * pos12;
    }
}
