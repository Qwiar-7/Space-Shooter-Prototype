using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector")]
    public float waveFrequency = 2; //число секунд полного цикла синусойды
    public float waveWidth = 4; //ширина синусойды в метрах
    public float waveRotY = 30; //градус поворота

    private float x0; // начальное значение координаты х
    private float birthTime;

    private void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        // значение theta изменяется с течением времени
        float age = Time.time - birthTime; //для рассинхронизации движений
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin*waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }
}
