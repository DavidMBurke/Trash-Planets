using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Catapult : WeaponScript
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    private float fireDelay;
    [SerializeField]
    float projectileSpeed;
    [SerializeField]
    float minRotationalSpeed;
    [SerializeField]
    float maxRotationalSpeed;

    public AudioSource fireSound;

    private float lastFire = 0;
    private Transform playerCameraTransform;


    // Start is called before the first frame update
    void Start()
    {

    }

    public override void startInteract(GameObject player)
    {
        Debug.Log("Started Interact");
        base.startInteract(player);
        lastFire = Time.time;
        playerCameraTransform = player.GetComponent<PlayerInteraction>().playerCamera.transform;
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
            checkExit();

            if (Time.time > lastFire + fireDelay)
            {
                bool shot = checkFire();
                if (shot)
                {
                    lastFire = Time.time;
                }
            }

        }
    }

    private bool checkFire()
    {
        //Implement Ammo Check Here
        if (this.Fire.IsPressed() && trashInAmmo <= currentPlayer.GetComponent<Player>().trashQty)
        {
            shoot();
            fireSound.Play();
            return true;
        }

        return false;
    }

    void shoot()
    {
        currentPlayer.GetComponent<Player>().trashQty = currentPlayer.GetComponent<Player>().trashQty - trashInAmmo;
        GameObject projectile = Instantiate(projectilePrefab, playerCameraTransform.position, Quaternion.identity);//Instantiate(this.projectilePrefab, this.cameraAnchor);
        //projectile.GetComponent<ProjectileMotion>().activateProjectile(this.originPlanet, this.targetPlanet);

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = playerCameraTransform.forward * projectileSpeed;
        float randomRotationalSpeedX = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedY = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedZ = Random.Range(minRotationalSpeed, maxRotationalSpeed);

        projectileRigidbody.angularVelocity = new Vector3(randomRotationalSpeedX, randomRotationalSpeedY, randomRotationalSpeedZ);
    }
}
