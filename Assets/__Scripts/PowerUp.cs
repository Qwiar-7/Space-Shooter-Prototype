using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // ���������, �� ������� ���������� Vector2. � ������ �����������
    // ��������, � � - ������������ �������� ��� ������ Random.Range(),
    // ������� ����� ���������� �����
    public Vector2 rotationMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;  // ����� � �������� ������������� PowerUp
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotationPerSecond; // �������� ��������
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck boundsCheck;
    private Renderer cubeRenderer;

    private void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        boundsCheck = GetComponent<BoundsCheck>();
        cubeRenderer = cube.GetComponent<Renderer>();

        // ������� ��������� ��������
        Vector3 vel = Random.onUnitSphere; // Random.onUnitSphere ���������� ������, ����������� �� ���������
        // �����, ����������� �� ����������� ����� � �������� 1 � � � �������
        // � ������ ���������
        vel.z = 0; // ���������� vel �� ��������� XY
        vel.Normalize(); // ������������ ������������� ����� Vector3 ������ 1 �
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // ���������� ���� �������� ����� �������� ������� ������ R:[ 0, 0, 0 ]
        transform.rotation = Quaternion.identity; // Quaternion.identity ���������� ���������� ��������.

        // ������� ��������� �������� �������� ��� ���������� ���� �
        // �������������� rotMinMax.x � rotMinMax.y
        rotationPerSecond = new Vector3(Random.Range(rotationMinMax.x, rotationMinMax.y),
            Random.Range(rotationMinMax.x, rotationMinMax.y), Random.Range(rotationMinMax.x, rotationMinMax.y));

        birthTime = Time.time;
    }

    private void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotationPerSecond * Time.time);

        // ������ ����������� ���� PowerUp � �������� �������
        // �� ���������� �� ��������� ����� ���������� 10 ������
        // � ����� ������������ � ������� 4 ������,
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // � ������� lifeTime ������ �������� u ����� <= 0. ����� ��� ������
        // ������������� � ����� fadeTime ������ ������ ������ 1.

        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        if (u > 0)
        {
            Color color = cubeRenderer.material.color;
            color.a = 1f - u;
            cubeRenderer.material.color = color;
            // ����� ���� ������ ������������, �� ���������
            color = letter.color;
            color.a = 1f - (u * 0.5f);
            letter.color = color;
        }

        if (!boundsCheck.isOnScreen)
            Destroy(gameObject);
    }

    public void SetType (WeaponType type)
    {
        // �������� WeaponDefinition �� Main
        WeaponParameters weaponDef = Hero.GetWeaponParameters(type);
        // ���������� ���� ��������� ����
        cubeRenderer.material.color = weaponDef.color;
        //letter.color = def.color;
        letter.text = weaponDef.letter;
        this.type = type; // � ���������� ���������� ����������� ���
    }

    public void AbsorbedBy(GameObject target)
    {
        // ��� ������� ���������� ������� ����, ����� ����� ��������� �����
        // ����� ���� �� ����������� ������ ���������� ������, �������� ���
        // ������� � ������� ���������� ������, �� ���� ������ ���������
        Destroy(this.gameObject);
    }
}
