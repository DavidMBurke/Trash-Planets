using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{
    [SerializeField]
    private Transform originPlanet;
    [SerializeField]
    private Transform targetPlanet;
    [SerializeField]
    private float gravityForce;
    [Tooltip("0.5 is unbiased. Values closer to 1 correct to target planet")]
    [SerializeField]
    private float targetBias = 0.5f;
    [SerializeField]
    public bool projectileActivated = true;

    private Rigidbody objectRigidBody;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            this.objectRigidBody = this.gameObject.GetComponent<Rigidbody>();
        }
        catch
        {
            throw new System.Exception("No rigidbody found on object using projectile motion!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (projectileActivated)
        {
            applyGravity((1 - this.targetBias), originPlanet);
            applyGravity(this.targetBias, targetPlanet);
        }
    }

    void applyGravity(float bias, Transform location)
    {
        Vector3 directionToPlanet = (location.position - this.gameObject.transform.position).normalized;

        float distanceToPlanet = Vector3.Distance(location.position, this.gameObject.transform.position);

        if (distanceToPlanet > 0f)
        {
            float gravitationalForce = gravityForce * bias / Mathf.Pow(distanceToPlanet, 2);
            this.objectRigidBody.AddForce(directionToPlanet * gravitationalForce);
        }
    }

    public void activateProjectile(Transform originPlanet, Transform targetPlanet)
    {
        this.originPlanet = originPlanet;
        this.targetPlanet = targetPlanet;
        this.projectileActivated = true;
    }
}
