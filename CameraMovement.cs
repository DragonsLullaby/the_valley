using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject gameManager;

    public Transform playerObj;

    public float mouseSensitivity;
    float xRotation = 0f;

    void Update()
    {
        if (!gameManager.GetComponent<GameManager>().gamePaused && !gameManager.GetComponent<GameManager>().inventoryOpen)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            playerObj.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localEulerAngles = new Vector3(xRotation, 0f, 0f);
        }
    }
}
