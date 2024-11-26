using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject projectile;
    public Transform launchOrigin;
    public Transform cannonCenter;

    public float launchVelocity = 100f;
    Vector3 LaunchVector;
    void Start()
    {
        LaunchVector = cannonCenter.position - launchOrigin.position;
    }
    // Update is called once per frame
    void Update()
    {
        LaunchVector = cannonCenter.position - launchOrigin.position;
        if(Input.GetButtonDown("Fire1")){

            GameObject ball = Instantiate(projectile, cannonCenter.position,
                                                     cannonCenter.rotation);
            ball.GetComponent<Rigidbody>().AddRelativeForce(LaunchVector * launchVelocity);

        }

    }
    void OnMouseOver(){
        // if(Input.GetButtonDown("Fire1")){

        //     GameObject ball = Instantiate(projectile, launchOrigin.position,
        //                                              launchOrigin.rotation);
        //     ball.GetComponent<Rigidbody>().AddRelativeForce(LaunchVector * launchVelocity);
        //     Debug.Log("Registered");
        // }

    }
}
