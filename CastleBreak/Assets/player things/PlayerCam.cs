using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform, playerModel;
    public float mouseSensitivity = 2f;
    public float moveSpeed = 6f;
    public float gravity = -9.81f;
    public float cameraDistance = 4f;
    public float cameraHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity, moveVelocity, lastPosition;
    private float moveVelocityFloat;
    Animator animator;

    private float yaw = 0f;
    private float pitch = 15f; // slight downward angle

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        HandleCameraRotation();
        HandleMovement();
        UpdateCameraPosition();

        animator.SetInteger("movingState", AnimateMovingState());
        lastPosition = transform.position;
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
        playerModel.rotation = Quaternion.Euler(0, yaw, 0);;
    }

    int AnimateMovingState()
    {
        moveVelocity = ((transform.position - lastPosition)) / Time.deltaTime;
        moveVelocityFloat = new Vector2(Mathf.Abs(moveVelocity.x), Mathf.Abs(moveVelocity.z)).magnitude;
        if (moveVelocityFloat > 0)
        {
            return 1;
        }
        return 0;
    }
}
