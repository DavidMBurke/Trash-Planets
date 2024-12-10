using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperProjectileMotion : ProjectileMotion
{
    [SerializeField]
    private float projectileTrackingForce = 10f;
    [SerializeField]
    private float gravityMultiplier = 0.2f;

    public bool projectileDying = false;

    private Transform targetPoint;

    protected override void FixedUpdate()
    {
        if (projectileActivated)
        {
            if (targetPoint != null)
            {
                // Move the object towards the target object's position
                Vector3 direction = (targetPoint.transform.position - this.transform.position).normalized;

                // Apply the force in the direction of the target object
                this.objectRigidBody.AddForce(direction * projectileTrackingForce);
                applyGravity((1 - this.targetBias) * gravityMultiplier, originPlanet);
                applyGravity(this.targetBias * gravityMultiplier, targetPlanet);
            }
            else
            {
                applyGravity((1 - this.targetBias), originPlanet);
                applyGravity(this.targetBias, targetPlanet);
            }
        }

        if (projectileDying && !projectileActivated)
        {
            applyGravity((1 - this.targetBias), originPlanet);
            applyGravity(this.targetBias, targetPlanet);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateProjectile(Transform target)
    {
        this.targetPoint = target;
        this.projectileActivated = true;
    }
}
