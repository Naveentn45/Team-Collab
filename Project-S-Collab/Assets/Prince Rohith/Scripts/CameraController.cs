using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Tooltip("Camera stays in NEGATIVE Z")]
    public float cameraZ = -10f;

    public float height = 3f;
    public float smoothSpeed = 6f;

    [Header("Bounds")]
    public bool useBounds;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private void Awake()
    {
        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                target = player.transform;
            }
            else
            {
                PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();
                if (playerController)
                {
                    target = playerController.transform;
                }
            }
        }

        if (target)
        {
            transform.position = new Vector3(
                target.position.x,
                target.position.y + height,
                cameraZ
            );
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x,
            target.position.y + height,
            cameraZ
        );

        Vector3 smoothPos = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        if (useBounds)
        {
            smoothPos.x = Mathf.Clamp(smoothPos.x, minBounds.x, maxBounds.x);
            smoothPos.y = Mathf.Clamp(smoothPos.y, minBounds.y, maxBounds.y);
        }

        transform.position = smoothPos;
    }
}
