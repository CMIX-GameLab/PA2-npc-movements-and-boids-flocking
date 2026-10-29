using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamMovement : MonoBehaviour
{

    public Transform target;
    public float lerpFactor = .2f;
    public float sensitivity = 7;

    public Vector2 mouseInput;
    public Vector2 camRot;

    InputAction lookAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookAction = InputSystem.actions.FindAction("look");
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseInput = lookAction.ReadValue<Vector2>();
        camRot.x += mouseInput.x / sensitivity;
        camRot.y -= mouseInput.y / sensitivity;
        camRot.y = Math.Clamp(camRot.y, -20, 70);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(camRot.y, camRot.x, 0);
        transform.position = Vector3.Lerp(transform.position, target.position, lerpFactor);
    }
}
