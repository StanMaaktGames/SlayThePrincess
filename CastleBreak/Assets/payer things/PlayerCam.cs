using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float moveSpeed = 6f;
    public float gravity = -9.81f;
    public float cameraDistance = 4f;
    public float cameraHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;

    private float yaw = 0f;
    private float pitch = 15f; // slight downward angle

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleMovement();
        UpdateCameraPosition();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // limit vertical angle
    }

    void HandleMovement()
    {
        // Move relative to camera direction
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Debug.Log(Input.GetAxis("Horizontal"));

        Vector3 move = camRight * moveX + camForward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateCameraPosition()
    {
        // Orbit around player
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -cameraDistance);
        cameraTransform.position = transform.position + offset + Vector3.up * cameraHeight;
        cameraTransform.rotation = rotation;
    }
}
