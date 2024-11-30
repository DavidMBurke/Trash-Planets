using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Catapult : WeaponScript
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    float projectileSpeed;
    [SerializeField]
    float minRotationalSpeed;
    [SerializeField]
    float maxRotationalSpeed;
    [SerializeField]
    private GameObject playerCamera;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (this.playerActive)
        {
            weaponRotation();
            checkFire();
            checkExit();
        }
    }

    void checkFire()
    {
        //Implement Ammo Check Here
        if (this.Fire.IsPressed())
        {
            shoot();
        }
    }

    void shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, playerCamera.transform.position, Quaternion.identity);//Instantiate(this.projectilePrefab, this.cameraAnchor);
        projectile.GetComponent<ProjectileMotion>().activateProjectile(this.originPlanet, this.targetPlanet);

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = playerCamera.transform.forward * projectileSpeed;
        float randomRotationalSpeedX = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedY = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedZ = Random.Range(minRotationalSpeed, maxRotationalSpeed);

        projectileRigidbody.angularVelocity = new Vector3(randomRotationalSpeedX, randomRotationalSpeedY, randomRotationalSpeedZ);
    }
}
