using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffects : MonoBehaviour
{
    [SerializeField]
    private float damage = 1;
    [SerializeField]
    private float timeAfterCollision;
    [SerializeField]
    private LayerMask planetMeshLayer;

    private float expireTime;
    private bool dying = false;

    private float startup;
    private bool active = false;

    private HashSet<GameObject> collidedObjects;

    // Start is called before the first frame update
    void Start()
    {
        startup = Time.time + 0.5f;
        active = true;
        collidedObjects = new HashSet<GameObject>();
}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dying)
        {
            if (Time.time >= expireTime)
            {
                try
                {
                    //Trash generation code
                    Transform planet1 = GameObject.Find("Planet1").transform;
                    Transform planet2 = GameObject.Find("Planet2").transform;

                    Transform closestPlanet = planet1;

                    //Check which planet is actually the origin
                    if (Vector3.Distance(this.gameObject.transform.position, planet2.position) < Vector3.Distance(this.gameObject.transform.position, planet1.position))
                    {
                        closestPlanet = planet2;
                    }
                    Vector3 planetDirection = planet2.position - this.transform.position;
                    if (Physics.Raycast(this.transform.position, planetDirection, out RaycastHit hit, planetMeshLayer))
                    {
                        MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
                        if (meshFilter != null)
                        {
                            Mesh mesh = meshFilter.mesh;
                            int triangleIndex = hit.triangleIndex;
                            VertexManipulator.ExpandVerticesFromTriangle(meshFilter, closestPlanet.position, triangleIndex, 100, 2, 2);
                        }
                    }
                }
                catch
                {

                }

                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time > startup && active)
        {
            if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Drill") || collision.gameObject.CompareTag("Refinery"))
            {

                if (!collidedObjects.Contains(collision.gameObject))
                {
                    collidedObjects.Add(collision.gameObject);
                    Building buildingScript = collision.gameObject.GetComponent<Building>();
                    buildingScript.applyDamage(damage);
                }
            }

            if (!dying)
            {
                dying = true;
                expireTime = Time.time + timeAfterCollision;

                //Stop supermotion
                SuperProjectileMotion superProjectileScript = this.gameObject.GetComponent<SuperProjectileMotion>();
                if (superProjectileScript != null)
                {
                    superProjectileScript.projectileActivated = false;
                    superProjectileScript.projectileDying = true;
                }
            }
        }

    }
}
