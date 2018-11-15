using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [SerializeField, Range(1f, 5f)]
    private float movementSpeed = 1f;

    [SerializeField, Range(1f, 5f)]
    private float rotationSpeed = 1f;

    [SerializeField]
    private Camera lookCamera;

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var speedVector = new Vector3(horizontal, 0, vertical);
        speedVector.ClampToOne();

        // TODO speedVector needs to be rotated so it is relative to the gameObject
        controller.SimpleMove(transform.rotation * speedVector * movementSpeed);
    }

    private void UpdateRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up, mouseX * rotationSpeed, Space.Self);
        lookCamera.transform.Rotate(Vector3.left, mouseY * rotationSpeed);
    }
}
