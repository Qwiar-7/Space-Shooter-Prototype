using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Предотвращает выход игрового объекта за границы экрана.
/// Важно: работает ТОЛЬКО с ортографической камерой Main Camera в [ 0, 0, 0 ].
/// </summary>

public class BoundsCheck : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Set Dynamically")]
    public bool isOnScreen = true;
    public float cameraWidth;
    public float cameraHeight;

    [HideInInspector]
    public bool offRight, offLeft, offUp, offDown;

    private void Awake()
    {
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        isOnScreen = true;
        offRight = offLeft = offUp = offDown = false;

        if (position.x >  cameraWidth + radius)
        {
            position.x = cameraWidth + radius;
            offRight = true;
        }
        if (position.x < -cameraWidth -  radius)
        {
            position.x = -cameraWidth - radius;
            offLeft = true;
        }
        if (position.y > cameraHeight + radius)
        {
            position.y = cameraHeight + radius;
            offUp = true;
        }
        if (position.y < -cameraHeight - radius)
        {
            position.y = -cameraHeight - radius;
            offDown = true;
        }

        isOnScreen = !(offRight || offLeft || offUp || offDown);
        //если keepOnScreen = true оставить объект в пределах экрана
        if (keepOnScreen && !isOnScreen)
        {
            transform.position = position;
            isOnScreen = true;
            offRight = offLeft = offUp = offDown = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Vector3 boundSize = new Vector3(cameraWidth*2, cameraHeight*2, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, boundSize);
    }
}
