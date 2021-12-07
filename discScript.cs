using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class discScript : MonoBehaviour
{
    public float rotationSpeed=10;
    private Rigidbody discRB;

    public float jumpVal;
    // Start is called before the first frame update
    void Start()
    {
        /*discRB = GetComponent<Rigidbody>();
        discRB.AddForce(Vector3.up*jumpVal);*/
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,rotationSpeed*Time.deltaTime,0 );
        
    }
    
    
}
