using UnityEngine;

public class PhaserProjectile : Projectile
{
    [Header("PhaserProjectile Params")]
    public float waveFrequency = 1; //����� ������ ������� ����� ���������
    public float waveWidth = 1; //������ ��������� � ������
    public bool isReverse = false;

    private float x0; // ��������� �������� ���������� �
    private float birthTime;

    private void Start()
    {
        x0 = transform.position.x;
        birthTime = Time.time;
    }
    private void Update()
    {
        if (bndCheck.offUp)
            Destroy(gameObject);

        Vector3 tempPosition = transform.position;
        // �������� theta ���������� � �������� �������
        float age = Time.time - birthTime; //��� ���������������� ��������
        float theta = Mathf.PI * 2 * age / waveFrequency;
        if (isReverse) theta += Mathf.PI;
        float sin = Mathf.Sin(theta);
        tempPosition.x = x0 + waveWidth * sin;
        transform.position = tempPosition;
    }
}
