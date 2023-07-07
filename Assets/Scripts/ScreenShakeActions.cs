using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start() {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        KnifeAction.OnAnyKnifeHit += KnifeAction_OnAnyKnifeHit;
    }

    private void KnifeAction_OnAnyKnifeHit(object sender, System.EventArgs e) {
        ScreenShake.Instance.Shake(2);
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, System.EventArgs e) {
        ScreenShake.Instance.Shake(5);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e) {
        ScreenShake.Instance.Shake();
    }
}
