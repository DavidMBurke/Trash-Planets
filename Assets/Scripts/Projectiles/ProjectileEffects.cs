using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffects : MonoBehaviour
{
    [SerializeField]
    private float damagePerSpeed;
    [SerializeField]
    private float timeAfterCollision;

    private float expireTime;
    private bool dying = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dying)
        {
            if (Time.time >= expireTime)
            {
                //Insert trash generation code
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon") || collision.gameObject.CompareTag("Building"))
        {
            Building buildingScript = collision.gameObject.GetComponent<Building>();
            buildingScript.applyDamage(this.gameObject.GetComponent<Rigidbody>().velocity.magnitude * damagePerSpeed);
        }

        if (collision.gameObject.CompareTag("Planet") && !dying)
        {
            dying = true;
            expireTime = Time.time + timeAfterCollision;
        }
    }
}