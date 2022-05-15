using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Необычное, но удобное применение Vector2. х хранит минимальное
    // значение, а у - максимальное значение для метода Random.Range(),
    // который будет вызываться позже
    public Vector2 rotationMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;  // Время в секундах существования PowerUp
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotationPerSecond; // Скорость вращения
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

        // Выбрать случайную скорость
        Vector3 vel = Random.onUnitSphere; // Random.onUnitSphere возвращает вектор, указывающий на случайную
        // точку, находящуюся на поверхности сферы с радиусом 1 м и с центром
        // в начале координат
        vel.z = 0; // Отобразить vel на плоскость XY
        vel.Normalize(); // Нормализация устанавливает длину Vector3 равной 1 м
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        // Установить угол поворота этого игрового объекта равным R:[ 0, 0, 0 ]
        transform.rotation = Quaternion.identity; // Quaternion.identity равноценно отсутствию поворота.

        // Выбрать случайную скорость вращения для вложенного куба с
        // использованием rotMinMax.x и rotMinMax.y
        rotationPerSecond = new Vector3(Random.Range(rotationMinMax.x, rotationMinMax.y),
            Random.Range(rotationMinMax.x, rotationMinMax.y), Random.Range(rotationMinMax.x, rotationMinMax.y));

        birthTime = Time.time;
    }

    private void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotationPerSecond * Time.time);

        // Эффект растворения куба PowerUp с течением времени
        // Со значениями по умолчанию бонус существует 10 секунд
        // а затем растворяется в течение 4 секунд,
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // В течение lifeTime секунд значение u будет <= 0. Затем оно станет
        // положительным и через fadeTime секунд станет больше 1.

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
            // Буква тоже должна растворяться, но медленнее
            color = letter.color;
            color.a = 1f - (u * 0.5f);
            letter.color = color;
        }

        if (!boundsCheck.isOnScreen)
            Destroy(gameObject);
    }

    public void SetType (WeaponType type)
    {
        // Получить WeaponDefinition из Main
        WeaponParameters weaponDef = Hero.GetWeaponParameters(type);
        // Установить цвет дочернего куба
        cubeRenderer.material.color = weaponDef.color;
        //letter.color = def.color;
        letter.text = weaponDef.letter;
        this.type = type; // В заключение установить фактический тип
    }

    public void AbsorbedBy(GameObject target)
    {
        // Эта функция вызывается классом Него, когда игрок подбирает бонус
        // Можно было бы реализовать эффект поглощения бонуса, уменьшая его
        // размеры в течение нескольких кадров, но пока просто уничтожим
        Destroy(this.gameObject);
    }
}
