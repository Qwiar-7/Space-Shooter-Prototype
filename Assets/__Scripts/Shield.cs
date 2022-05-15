using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float rotationPerSecond = 0.1f;

    [Header("Set Dynamically")]
    public int visibleShieldLevel = 0;

    Material material;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        int currentLevel = Mathf.FloorToInt(Hero.heroObj.ShieldLevel);

        if (visibleShieldLevel != currentLevel)
        {
            visibleShieldLevel = currentLevel;
            material.mainTextureOffset = new Vector2(0.2f * visibleShieldLevel, 0);
        }
        float rotationZ = -(rotationPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}
