using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void move()
    {
        transform.position+= Vector3.up;
    }
}
