using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform BulletHitVFX;

    private Vector3 targetPosition;
    public void Setup(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
    }

    private void Update() {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(targetPosition, transform.position);

        float moveSpeed = 200f;
        Vector3 frameMovement = moveDir * moveSpeed * Time.deltaTime;
        if (frameMovement.magnitude < distanceBeforeMoving) {
            transform.position += frameMovement;
        } else {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            Instantiate(BulletHitVFX, targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
