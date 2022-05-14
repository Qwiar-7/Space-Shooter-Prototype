using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector")]
    public float waveFrequency = 2; //����� ������ ������� ����� ���������
    public float waveWidth = 4; //������ ��������� � ������
    public float waveRotY = 30; //������ ��������

    private float x0; // ��������� �������� ���������� �
    private float birthTime;

    private void Start()
    {
        x0 = Position.x;
        birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 tempPosition = Position;
        // �������� theta ���������� � �������� �������
        float age = Time.time - birthTime; //��� ���������������� ��������
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPosition.x = x0 + waveWidth * sin;
        Position = tempPosition;

        // ��������� ������� ������������ ��� Y
        Vector3 rotation = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rotation);

        base.Move();
    }
}
