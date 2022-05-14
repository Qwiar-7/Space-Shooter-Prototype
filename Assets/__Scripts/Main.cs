using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main mainObj;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;    //количество врагов в секунду
    public float enemyDefaultPadding = 1.5f;    //отступ от верхнего края
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster,
        WeaponType.blaster, WeaponType.spread, WeaponType.shield };

    private BoundsCheck boundsCheck;

    private void Awake()
    {
        mainObj = this;
        boundsCheck = GetComponent<BoundsCheck>();
        Invoke (nameof(SpawnEnemy), 1f/enemySpawnPerSecond);  //время до появления первого врага
    }

    public void SpawnEnemy()
    {
        int index = Random.Range(0, prefabEnemies.Length);
        GameObject gameObj = Instantiate<GameObject> (prefabEnemies[index]);

        float enemyPadding = enemyDefaultPadding;
        if (gameObj.GetComponent<BoundsCheck>() != null)
            enemyPadding = Mathf.Abs(gameObj.GetComponent<BoundsCheck>().radius);//?????????????абсолютное радиус с -

        Vector3 position = Vector3.zero;
        float xMin = -boundsCheck.cameraWidth + enemyPadding;
        float xMax = boundsCheck.cameraWidth - enemyPadding;
        position.x = Random.Range(xMin, xMax);
        position.y = boundsCheck.cameraHeight + enemyPadding;
        gameObj.transform.position = position;

        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart ( float delay )
    {
        Invoke(nameof(Restart), delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ShipDestroyed(Enemy enemy)
    {
        if (Random.value <= enemy.powerUpDropChance)
        {
            // Выбрать тип бонуса
            // Выбрать один из элементов в powerUpFrequency
            int index = Random.Range(0,powerUpFrequency.Length);
            WeaponType powerUpType = powerUpFrequency[index];

            // Создать экземпляр PowerUp
            GameObject gameObj = Instantiate (prefabPowerUp);
            PowerUp powerUp = gameObj.GetComponent<PowerUp>();
            // Установить соответствующий тип WeaponType
            powerUp.SetType(powerUpType);
            // Поместить в место, где находился разрушенный корабль
            powerUp.transform.position = enemy.transform.position;
        }
    }
}
