using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool inverse;
    private Transform cameraTransform;

    private void Awake() {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate() {
        if (inverse) {
            Vector3 lookDir = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position - lookDir);
        } else {
            transform.LookAt(cameraTransform.position);
        }
    }
}
