using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main mainObj;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies;
    public float gameRestartDelay = 2f;         //время до перезапуска
    public float enemySpawnPerSecond = 0.5f;    //количество врагов в секунду
    public float enemyDefaultPadding = 2f;      //отступ от верхнего края
    public GameObject[] panels;
    public float scrollSpeed = -30f;

    [Header("Set Dynamically")]

    private BoundsCheck boundsCheck;

    private void Awake()
    {
        mainObj = this;
        boundsCheck = GetComponent<BoundsCheck>();

        Invoke(nameof(SpawnEnemy), 3f);  //время до появления первого врага
    }

    public void SpawnEnemy()
    {
        int index = Random.Range(0, prefabEnemies.Length);
        GameObject gameObj = Instantiate(prefabEnemies[index]);

        float enemyPadding = enemyDefaultPadding;
        if (gameObj.GetComponent<BoundsCheck>() != null)
            enemyPadding = gameObj.GetComponent<BoundsCheck>().radius;

        Vector3 position = Vector3.zero;
        float xMin = -boundsCheck.cameraWidth + enemyPadding;
        float xMax = boundsCheck.cameraWidth - enemyPadding;
        position.x = Random.Range(xMin, xMax);
        position.y = boundsCheck.cameraHeight + enemyPadding;
        gameObj.transform.position = position;

        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart ()
    {
        ScoreManager.ShowBestScore();
        CancelInvoke();
        Invoke(nameof(Restart), gameRestartDelay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
