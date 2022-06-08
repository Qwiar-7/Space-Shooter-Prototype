using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // ����������, ��������� ���� ����� ������� �������������� �������� ��������
    public float sinEccentricity = 0.6f;
    public float lifeTime = 6;

    // Enemy_2 ���������� �������� ������������ ����� ����� �������,
    // ������� ��������� �� ���������
    private Vector3 pos0;
    private Vector3 pos1;
    private float birthTime;
    private void Start()
    {
        // ������� ��������� ����� �� ����� ������� ������
        pos0 = Vector3.zero;
        pos0.x = -boundsCheck.cameraWidth - boundsCheck.radius;
        pos0.y = Random.Range(-boundsCheck.cameraHeight, boundsCheck.cameraHeight);

        // ������� ��������� ����� �� ������ ������� ������
        pos1 = Vector3.zero;
        pos1.x = boundsCheck.cameraWidth + boundsCheck.radius;
        pos1.y = Random.Range(-boundsCheck.cameraHeight, boundsCheck.cameraHeight);

        // �������� �������� ��������� � �������� ����� �������
        if (Random.value > 0.5f)
        {
            pos0.x *= -1;
            pos1.x *= -1;
        }
        birthTime = Time.time;
    }
    public override void Move()
    {
        // ������ ����� ����������� �� ������ �������� � ����� 0 � 1
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            Destroy(gameObject);
            return;
        }
        u += sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2)); //��� ���������� ������� ����������
        Position = pos0 + (pos1 - pos0) * u;
    }
}