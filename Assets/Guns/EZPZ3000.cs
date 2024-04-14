using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EZPZ3000 : Gun
{
    [SerializeField] GameObject prefabEZPZBlast;
    [SerializeField] AudioSource audioSource;

    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<HomingProjectile>().Initialize(100, 10, 10, 5, null); // version without special effect

        Instantiate(prefabEZPZBlast, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        audioSource.Play();

        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }
}
