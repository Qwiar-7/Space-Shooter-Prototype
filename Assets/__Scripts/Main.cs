using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Waves
{
    public float enemySpawnPerSecond = 0.5f;    //количество врагов в секунду
    public GameObject[] PrefabEnemies;
}

public class Main : MonoBehaviour
{
    static public Main mainObj;

    [Header("Set in Inspector")]
    public float waveChangeTime = 20f;
    public float gameRestartDelay = 2f;         //время до перезапуска
    public float enemyDefaultPadding = 2f;      //отступ от верхнего края
    public float timeBeforeBossEnter = 10f;
    public int bossSpawnPoints = 1000;
    public GameObject bossPrefab;
    public Waves[] enemyWaves;

    [Header("Set Dynamically")]
    public int waveNumber = -1;
    public static float startTime;
    public GameObject[] currentEnemyPrefab;
    private BoundsCheck boundsCheck;
    private void Awake()
    {
        mainObj = this;
        boundsCheck = GetComponent<BoundsCheck>();
        ChangeWave();
        startTime = Time.time;
        Cursor.lockState = CursorLockMode.Locked;
        Invoke(nameof(SpawnEnemy), 3f);  //время до появления первого врага
    }
    public void SpawnEnemy()
    {
        int index = Random.Range(0, currentEnemyPrefab.Length);
        GameObject gameObj = Instantiate(currentEnemyPrefab[index]);

        float enemyPadding = enemyDefaultPadding;
        if (gameObj.GetComponent<BoundsCheck>() != null)
            enemyPadding = gameObj.GetComponent<BoundsCheck>().radius;

        Vector3 position = Vector3.zero;
        float xMin = -boundsCheck.cameraWidth + enemyPadding;
        float xMax = boundsCheck.cameraWidth - enemyPadding;
        position.x = Random.Range(xMin, xMax);
        position.y = boundsCheck.cameraHeight + enemyPadding;
        gameObj.transform.position = position;
        if(ScoreManager.currentScore > bossSpawnPoints)
        {
            Invoke(nameof(SpawnBoss), timeBeforeBossEnter);
            return;
        }

        Invoke(nameof(SpawnEnemy), 1f / enemyWaves[waveNumber].enemySpawnPerSecond);
    }
    public void SpawnBoss()
    {
        GameObject boss = Instantiate(bossPrefab);
        float bossPadding = enemyDefaultPadding;
        if (boss.GetComponent<BoundsCheck>() != null)
            bossPadding = boss.GetComponent<BoundsCheck>().radius;
        Vector3 position = Vector3.zero;
        position.y = boundsCheck.cameraHeight + bossPadding;
        boss.transform.position = position;
    }
    public void ChangeWave()
    {
        if (waveNumber == enemyWaves.Length - 1) return;
        waveNumber++;
        currentEnemyPrefab = enemyWaves[waveNumber].PrefabEnemies;
        Invoke(nameof(ChangeWave), waveChangeTime);
    }
    public void DelayedRestart ()
    {
        ScoreManager.ShowBestScore();
        ScoreManager.ShowCurrentScore();
        CancelInvoke();
        Invoke(nameof(Restart), gameRestartDelay);
    }
    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(boundsCheck.cameraWidth * 2, boundsCheck.cameraHeight * 2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}