using UnityEngine;
public class Enemy_boss : Enemy_4
{
    [Header("Set in Inspector: Boss")]
    public float waveFrequency = 2; //число секунд полного цикла синусойды

    [Header("Set Dynamically: Boss")]
    public float waveWidth;
    private float birthTime;
    private float x0; // начальное значение координаты х
    private void Start()
    {
        x0 = Position.x;
        birthTime = Time.time;
        waveWidth = (boundsCheck.cameraWidth - boundsCheck.radius) / 2;
        if (waveWidth < 0) waveWidth = 0;

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
    public override void Move()
    {
        Vector3 tempPosition = Position;
        // значение theta изменяется с течением времени
        float age = Time.time - birthTime; //для рассинхронизации движений
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPosition.x = x0 + waveWidth * sin;
        tempPosition.y -= speed * Time.deltaTime;
        Position = tempPosition;
    }
    private void OnDestroy()
    {
        if (boundsCheck.offDown)
            Hero.heroObj.ShieldLevel = -1;
    }
}
