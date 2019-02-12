using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //getting position and get a ray - which object a ray is goin to hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log(hit.transform.name);
                //check the name of the 
                if (hit.transform.name == "Sphere")
                {
                    SceneManager.LoadScene("SceneOne");

                }
            }
        } 
        
    }
}
