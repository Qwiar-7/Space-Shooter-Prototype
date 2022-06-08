using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float timeBeforeDestroy = 3f;
    void Start()
    {
        Invoke(nameof(DestroyExplosion), timeBeforeDestroy);
    }
    private void DestroyExplosion()
    {
        Destroy(gameObject);
    }
}