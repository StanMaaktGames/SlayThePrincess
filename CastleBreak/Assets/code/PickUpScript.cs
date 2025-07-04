using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;

    public float throwForce = 500f;
    public float maxThrowForce = 1500f;
    public float throwChargeRate = 500f;
    public float pickUpRange = 5f;
    private float rotationSensitivity = 1f;

    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private float currentThrowForce;
    private bool isChargingThrow = false;

    // Cursor textures
    public Texture2D defaultCursor;
    public Texture2D holdingCursor;
    private bool isHolding = false;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        HandleCursor();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.CompareTag("canPickUp"))
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop)
                {
                    StopClipping();
                    DropObject();
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject();
            RotateObject();

            if (Input.GetKeyDown(KeyCode.T) && canDrop)
            {
                isChargingThrow = true;
                currentThrowForce = throwForce;
            }

            if (Input.GetKey(KeyCode.T) && isChargingThrow)
            {
                currentThrowForce += throwChargeRate * Time.deltaTime;
                currentThrowForce = Mathf.Clamp(currentThrowForce, throwForce, maxThrowForce);
            }

            if (Input.GetKeyUp(KeyCode.T) && isChargingThrow && canDrop)
            {
                isChargingThrow = false;
                StopClipping();
                ThrowObject(currentThrowForce);
            }
        }
    }

    void HandleCursor()
    {
        if (heldObj != null && !isHolding)
        {
            Cursor.SetCursor(holdingCursor, Vector2.zero, CursorMode.Auto);
            isHolding = true;
        }
        else if (heldObj == null && isHolding)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            isHolding = false;
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObj = null;
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.position;
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false;
            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            canDrop = true;
        }
    }

    void ThrowObject(float force)
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * force);
        heldObj = null;
    }

    void StopClipping()
    {
        float clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);

        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }
}
