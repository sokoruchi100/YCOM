using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_OFFSET_Y = 1;
    private const float MAX_FOLLOW_OFFSET_Y = 12;

    private CinemachineTransposer cinemachineTransposer;
    private Vector3 targetFollowOffset;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Start() {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update() {
        HandleMovement();

        HandleRotation();

        HandleZoom();
    }

    private void HandleZoom() {
        float zoomIncreaseAmount = 1f;

        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_OFFSET_Y, MAX_FOLLOW_OFFSET_Y);
        float zoomSpeed = 5f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
    }

    private void HandleRotation() {
        Vector3 inputRotateDir = new Vector3();
        inputRotateDir.y = InputManager.Instance.GetCameraRotateAmount();

        float rotateSpeed = 100;
        transform.eulerAngles += inputRotateDir * rotateSpeed * Time.deltaTime;
    }

    private void HandleMovement() {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10;
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }
}
