using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private int grenadeDamage;
    private Vector3 targetPosition;
    private Action OnGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;
    private float grenadeDamageRange;

    private void Update() {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        
        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - (distance / totalDistance);

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance) {
            float damageRadius = grenadeDamageRange;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach (Collider collider in colliderArray) {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit)) {
                    targetUnit.Damage(grenadeDamage);
                }
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate)) {
                    destructibleCrate.Damage();
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            
            trailRenderer.transform.parent = null;
            Instantiate(grenadeExplodeVFXPrefab, targetPosition + Vector3.up, Quaternion.identity);

            Destroy(gameObject);

            OnGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, int grenadeDamage, float grenadeDamageRange, Action OnGrenadeBehaviourComplete) { 
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.grenadeDamage = grenadeDamage;
        this.grenadeDamageRange = grenadeDamageRange;
        this.OnGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
