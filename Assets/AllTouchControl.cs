using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllTouchControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Ray laser = Camera.main.ScreenPointToRay(Input.touches[0].position);
            Debug.DrawRay(laser.origin,100*laser.direction);
            RaycastHit info;
            if (Physics.Raycast(laser, out info))
            {
                Manipulate theObject = info.collider.GetComponent<Manipulate>();
                if (theObject) theObject.move();
                print("Yes");
            }
        }
        
    }
}
