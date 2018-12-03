using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    private const float MIN_CAMERA_ROTATION = -80f;
    
    private const float MAX_CAMERA_ROTATION = 80f;

    [SerializeField, Range(1f, 5f)]
    private float movementSpeed = 1f;

    [SerializeField, Range(1f, 5f)]
    private float rotationSpeed = 1f;

    [SerializeField, Range(1f, 5f)]
    private float interactionRange = 1f;

    [SerializeField]
    private bool warnForUnhandledState = false;

    [SerializeField]
    private Sprite reticleDefault;

    [SerializeField]
    private Sprite reticleLocked;

    [SerializeField]
    private Sprite reticleOpen;

    private Camera lookCamera;

    private CharacterController controller;

    private RawImage reticleImage;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        lookCamera = GetComponentInChildren<Camera>();
        reticleImage = GetComponentInChildren<RawImage>();

        reticleImage.texture = reticleDefault.texture;
        reticleImage.SetNativeSize();
        UpdateReticle();
    }

    private void Update()
    {
        UpdatePosition();
        UpdateRotation();
        var hitInfo = UpdateReticle();
        UpdateInteraction(hitInfo);
    }

    private void UpdatePosition()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var speedVector = new Vector3(horizontal, 0, vertical);
        speedVector.ClampToOne();

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

        // TODO make simpler
        var currentRotation = lookCamera.transform.localRotation.eulerAngles;
        currentRotation.x = currentRotation.x >= 180f ? currentRotation.x - 360f : currentRotation.x;
        currentRotation.y = 0f;
        currentRotation.z = 0f;
        currentRotation.x = Mathf.Clamp(currentRotation.x, MIN_CAMERA_ROTATION, MAX_CAMERA_ROTATION);
        lookCamera.transform.localRotation = Quaternion.Euler(currentRotation);
    }

    private RaycastHit? UpdateReticle()
    {
        var viewDirection = lookCamera.transform.rotation * Vector3.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(lookCamera.transform.position, viewDirection, out hitInfo, interactionRange)
            && hitInfo.collider.gameObject.CompareTag(TimeLock.TAG))
        {
            var timeLock = hitInfo.collider.gameObject.GetComponent<TimeLock>();
            switch (timeLock.CurrentState)
            {
                case TimeLock.State.Locked:
                    reticleImage.texture = reticleLocked.texture;
                    break;
                case TimeLock.State.Unlocked:
                    reticleImage.texture = reticleOpen.texture;
                    break;
                default:
                    reticleImage.texture = reticleDefault.texture;
                    if(warnForUnhandledState)
                        Debug.LogWarning($"No reticle image defined for TimeLock.CurrentState({timeLock.CurrentState})");
                    break;
            }
            return hitInfo;
        }
        else
        {
            reticleImage.texture = reticleDefault.texture;
            return null;
        }
    }

    private void UpdateInteraction(RaycastHit? hitInfo)
    {
        if(Input.GetButtonDown("Interact") && hitInfo.HasValue)
        {
            hitInfo.Value.collider.GetComponent<TimeLock>().TryOpen();
        }
    }
}
