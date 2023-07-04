using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform rootBone;

    public void Setup(Transform originalRootBone) {
        MatchAllChildTransforms(originalRootBone, rootBone);
        ApplyExplosionToRagdoll(rootBone, 300, transform.position, 10);
    }

    private void MatchAllChildTransforms(Transform originalRootBone, Transform cloneRootBone) {
        foreach (Transform child in originalRootBone) {
            Transform cloneChild = cloneRootBone.Find(child.name);
            if (cloneChild != null) {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform rootBone, float explosionForce, Vector3 explosionPosition, float explosionRadius) {
        foreach (Transform child in rootBone) {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)) {
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRadius);
        }
    }
}
