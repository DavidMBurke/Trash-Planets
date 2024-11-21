using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapscript : MonoBehaviour
{
    public Transform player;
    public Transform planetcenter;
    public float cameraOrbitRadius = 50f;
    public float smoothSpeed = 5f;
    private Vector3 targetPosition;
    void LateUpdate () {
        // 2D camera movement
        // Vector3 newPosition = player.position;
        // newPosition.y = transform.position.y;
        // transform.position = newPosition;
        Vector3 relativePlayerPosition = player.position - planetcenter.position;

        float playerLatitude = Mathf.Asin(relativePlayerPosition.normalized.y); // Latitude (vertical angle)
        float playerLongitude = Mathf.Atan2(relativePlayerPosition.z, relativePlayerPosition.x); // Longitude (horizontal angle)

        float x = cameraOrbitRadius * Mathf.Cos(playerLatitude) * Mathf.Cos(playerLongitude);
        float y = cameraOrbitRadius * Mathf.Sin(playerLatitude);
        float z = cameraOrbitRadius * Mathf.Cos(playerLatitude) * Mathf.Sin(playerLongitude);
        // Set the camera's position relative to the planet
        transform.position = planetcenter.position + new Vector3(x, y, z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        transform.LookAt(planetcenter);

    }
}
