using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;
    private GridPosition gridPosition;

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public void Damage() {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, +1f), 0, UnityEngine.Random.Range(-1f, +1f));
        ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position + randomDir, 10f);
        Destroy(gameObject);
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }
    private void ApplyExplosionToChildren(Transform rootBone, float explosionForce, Vector3 explosionPosition, float explosionRadius) {
        foreach (Transform child in rootBone) {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRadius);
        }
    }
}
