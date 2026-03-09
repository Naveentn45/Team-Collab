using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target; // Player to follow

    public List<Transform> cameraPoints = new List<Transform>();

    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    private int currentPoint = 0;

    void Update()
    {
        if (target == null) return;
        if (cameraPoints.Count == 0) return;

        Transform targetPoint = cameraPoints[currentPoint];

        // Move camera point relative to player
        Vector3 desiredPosition = targetPoint.position + target.position;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            moveSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetPoint.rotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
