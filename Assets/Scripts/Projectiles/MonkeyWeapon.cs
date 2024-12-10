using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyWeapon : WeaponScript
{
    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    private float fireDelay;
    [SerializeField]
    private float burstDelay;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    private float minRotationalSpeed;
    [SerializeField]
    private float maxRotationalSpeed;
    [SerializeField]
    private GameObject targetPrefab;
    [SerializeField]
    private float maxTargetDistance;
    [SerializeField]
    private LayerMask planetLayer;


    private float lastFire = 0;
    private Transform playerCameraTransform;

    //Storing Mechanics
    public float currentTrash = 0;
    private GameObject autoTarget;
    private float addDelay;


    // Start is called before the first frame update
    void Start()
    {

    }

    public override void startInteract(GameObject player)
    {
        base.startInteract(player);
        lastFire = Time.time - fireDelay + 2;
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
            checkSetTarget();
            checkTrashAdded();
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
        else
        {
            //Autoshoot
            if (autoTarget != null)
            {
                autoShoot();
            }
        }
    }

    private bool checkFire()
    {
        //Implement Ammo Check Here
        if (this.Fire.IsPressed() && currentPlayer.GetComponent<Player>().trashQty >= (trashInAmmo * burstSize))
        {
            shoot();
            return true;
        }

        return false;
    }

    void shoot()
    {
        for (int i = 0; i < burstSize; i++)
        {
            Invoke("launchProjectile", i * burstDelay);
        }
    }

    void launchProjectile()
    {
        currentPlayer.GetComponent<Player>().trashQty = currentPlayer.GetComponent<Player>().trashQty - trashInAmmo;
        GameObject projectile = Instantiate(projectilePrefab, playerCameraTransform.position, Quaternion.identity);//Instantiate(this.projectilePrefab, this.cameraAnchor);
        //projectile.GetComponent<ProjectileMotion>().activateProjectile(this.originPlanet, this.targetPlanet);
        ProjectileMotion motionScript = projectile.GetComponent<ProjectileMotion>();
        if (motionScript is SuperProjectileMotion superMotionScript)
        {
            if (autoTarget == null)
            {
                superMotionScript.activateProjectile(null);
            }
            else
            {
                superMotionScript.activateProjectile(autoTarget.transform);
            }

        }

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.velocity = playerCameraTransform.forward * projectileSpeed;
        float randomRotationalSpeedX = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedY = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedZ = Random.Range(minRotationalSpeed, maxRotationalSpeed);

        projectileRigidbody.angularVelocity = new Vector3(randomRotationalSpeedX, randomRotationalSpeedY, randomRotationalSpeedZ);
    }

    void checkSetTarget()
    {
        if (this.Target.IsPressed())
        {
            setTarget();
        }
    }

    void checkTrashAdded()
    {
        if (this.ScrollUp.IsPressed() && Time.time >= addDelay)
        {
            if (currentPlayer.GetComponent<Player>().trashQty >= (trashInAmmo * burstSize))
            {
                addDelay = Time.time + 0.2f;
                currentPlayer.GetComponent<Player>().trashQty = currentPlayer.GetComponent<Player>().trashQty - (int)(trashInAmmo * burstSize);
                this.currentTrash += (trashInAmmo * burstSize);
            }
        }
    }

    void setTarget()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out hit, maxTargetDistance, planetLayer))
        {
            if (autoTarget != null)
            {
                Destroy(autoTarget);
            }
            GameObject hitObject = hit.collider.gameObject;
            //Try to find the planet
            for (int i = 0; i < 5; i++)
            {
                if (hitObject.name.Contains("Planet"))
                {
                    break;
                }
                hitObject = hitObject.transform.parent.gameObject;
            }
            if (hitObject.name.Contains("Planet"))
            {
                Vector3 hitPoint = hit.point;
                GameObject newTarget = new GameObject("MonkeyTarget");
                newTarget.transform.position = hitPoint;
                newTarget.transform.SetParent(hitObject.transform);
                this.autoTarget = newTarget;
            }
        }
    }

    void autoShoot()
    {
        RaycastHit hit;
        //Checks if autotarget is visible from camera anchor
        if (Physics.Raycast(cameraAnchor.position, (autoTarget.transform.position - cameraAnchor.position).normalized, out hit, Vector3.Distance(cameraAnchor.position, autoTarget.transform.position)))
        {
            //Resources Check
            if (currentTrash >= (trashInAmmo * burstSize))
            {
                for (int i = 0; i < burstSize; i++)
                {
                    Invoke("autoLaunchProjectile", i * burstDelay);
                }
            }

        }
    }

    void autoLaunchProjectile()
    {
        currentTrash -= trashInAmmo;
        GameObject projectile = Instantiate(projectilePrefab, cameraAnchor.position, Quaternion.identity);//Instantiate(this.projectilePrefab, this.cameraAnchor);
        //projectile.GetComponent<ProjectileMotion>().activateProjectile(this.originPlanet, this.targetPlanet);
        ProjectileMotion motionScript = projectile.GetComponent<ProjectileMotion>();
        if (motionScript is SuperProjectileMotion superMotionScript)
        {
            if (autoTarget == null)
            {
                superMotionScript.activateProjectile(null);
            }
            else
            {
                superMotionScript.activateProjectile(autoTarget.transform);
            }

        }

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        Vector3 direction = autoTarget.transform.position - projectile.transform.position;
        projectileRigidbody.velocity = direction.normalized * projectileSpeed;
        float randomRotationalSpeedX = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedY = Random.Range(minRotationalSpeed, maxRotationalSpeed);
        float randomRotationalSpeedZ = Random.Range(minRotationalSpeed, maxRotationalSpeed);

        projectileRigidbody.angularVelocity = new Vector3(randomRotationalSpeedX, randomRotationalSpeedY, randomRotationalSpeedZ);
    }

}
