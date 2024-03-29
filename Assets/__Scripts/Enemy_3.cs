using UnityEngine;

public class Enemy_3 : Enemy
{
    // ���������� �������� Enemy_3 ����������� ����� ��������
    // ������������ ������ ����� �� ����� ��� ���� ������.
    [Header("Set in Inspector: Enemy_3")]
    public float lifeTime = 5;

    private Vector3[] points;
    private float birthTime;
    private void Start()
    {
        points = new Vector3[3];
        // ��������� ������� ��� ���������� � Main.SpawnEnemy()
        points[0] = Position;

        // ���������� xMin � ���� ��� ��, ��� ��� ������ Main.SpawnEnemy()
        float xMin = -boundsCheck.cameraWidth + boundsCheck.radius;
        float xMax = boundsCheck.cameraWidth - boundsCheck.radius;

        Vector3 tempPos;
        // �������� ������� ������� ����� ���� ������ ������� ������
        tempPos = Vector3.zero;
        tempPos.x = Random.Range(xMin, xMax);
        tempPos.y = -boundsCheck.cameraHeight * Random.Range(2.75f, 2);
        points[1] = tempPos;

        // �������� ������� �������� ����� ���� ������� ������� ������
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
        Vector3 pos01, pos12;   // ��������������� ������ ����� �� ���� ������
        u -= 0.2f * Mathf.Sin(u * Mathf.PI * 2);
        pos01 = points[0] + (points[1] - points[0]) * u;
        pos12 = points[1] + (points[2] - points[1]) * u;
        Position = pos01 + (pos12 - pos01) * u;
    }
}