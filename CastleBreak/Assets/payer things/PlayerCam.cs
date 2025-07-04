using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
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
    private float pitch = 15f;

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
        pitch = Mathf.Clamp(pitch, -30f, 60f); // vertical limits
    }

    void HandleMovement()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = camRight * moveX + camForward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 desiredOffset = rotation * new Vector3(0, 0, -cameraDistance);
        Vector3 targetPosition = transform.position + Vector3.up * cameraHeight;
        Vector3 desiredCameraPos = targetPosition + desiredOffset;

        RaycastHit hit;
        float adjustedDistance = cameraDistance;

        if (Physics.SphereCast(targetPosition, 0.3f, desiredOffset.normalized, out hit, cameraDistance))
        {
            adjustedDistance = hit.distance - 0.1f;
            adjustedDistance = Mathf.Clamp(adjustedDistance, 0.5f, cameraDistance);
        }

        Vector3 finalOffset = desiredOffset.normalized * adjustedDistance;
        cameraTransform.position = targetPosition + finalOffset;
        cameraTransform.rotation = rotation;
    }
}
