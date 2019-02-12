using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;

public class InputManager : MonoBehaviour
{
    public float ScalingSpeed = 0.4f;
    public float RotationSpeed = 1f;
    public float MinDistanceToPinch = 20;

    private Camera mainCam;

    public static SelectedObject TheSelectedObject;

    //Dragging variables
    private bool dragging = false;
    private float dragginDistance = 0;

    //Scaling variables
    private float scalingTouchDistance;

    //Rotation variables
    private Vector3 rotationStartingVector;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Select object on touch
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit rayHit;

                if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity))
                {
                    SelectedObject oldSelectedObject = TheSelectedObject;
                    TheSelectedObject = rayHit.collider.GetComponent<SelectedObject>();

                    //Select object
                    if (TheSelectedObject != null)
                    {
                        if (oldSelectedObject != null && TheSelectedObject != oldSelectedObject)
                        {
                            oldSelectedObject.OnSelect(false);
                        }

                        TheSelectedObject.OnSelect(true);
                    }
                    else
                    {
                        //Deselect object
                        if (TheSelectedObject != null)
                        {
                            TheSelectedObject.OnSelect(false);
                            TheSelectedObject = null;
                        }
                    }
                }
                else
                {
                    //Deselect object
                    if (TheSelectedObject != null)
                    {
                        TheSelectedObject.OnSelect(false);
                        TheSelectedObject = null;
                    }
                }
            }
        }


        //Drag object
        if (Input.touchCount < 1)
        {
            dragging = false;
        }
        else
        {
            // Get the touch position
            var touch = Input.touches[0];
            Vector2 pos = touch.position;

            //Check if we have a selected object
            if (TheSelectedObject != null)
            {
                //Check if touch just started
                if (touch.phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = mainCam.ScreenPointToRay(pos);

                    //Check if we have an object in that point
                    if (Physics.Raycast(ray, out hit))
                    {
                        dragginDistance = Vector3.Distance(hit.transform.position, Camera.main.transform.position);
                        dragging = true;
                    }
                }

                //Check if we moved the touch
                if (dragging && touch.phase == TouchPhase.Moved)
                {
                    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragginDistance);
                    position = mainCam.ScreenToWorldPoint(position);
                    TheSelectedObject.transform.position = position;
                }
            }

            //Check if the touch ended
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                dragging = false;
            }
        }


        //Scaling and rotating object
        //Check if we have a object selected
        if (TheSelectedObject)
        {
            if (Input.touchCount == 2)
            {
                //Check if one of the touches just began
                if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    scalingTouchDistance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    rotationStartingVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
                }

                //Check if at least one of the touches is moving
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    float currentTouchDistance =
                        Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

                    float deltaDistance = currentTouchDistance - scalingTouchDistance;

                    if (deltaDistance > MinDistanceToPinch)
                    {
                        if (deltaDistance > 0)
                        {
                            //We are pinching out
                            //Increase the scale of the object by the scaling speed
                            Vector3 currentScale = TheSelectedObject.transform.localScale;
                            Vector3 newScale = new Vector3(currentScale.x + ScalingSpeed, currentScale.y + ScalingSpeed,
                                currentScale.z + ScalingSpeed);
                            TheSelectedObject.transform.localScale = newScale;
                        }
                        else if (deltaDistance < 0)
                        {
                            //Pinching in
                            Vector3 currentScale = TheSelectedObject.transform.localScale;
                            Vector3 newScale = new Vector3(currentScale.x - ScalingSpeed, currentScale.y - ScalingSpeed,
                                currentScale.z - ScalingSpeed);
                            TheSelectedObject.transform.localScale = newScale;
                        }
                    }
                    else
                    {
                        Vector3 currentRotationVector = Input.GetTouch(1).position - Input.GetTouch(0).position;

                        float angle = Vector3.Angle(rotationStartingVector, currentRotationVector);
                        Vector3 cross = Vector3.Cross(rotationStartingVector, currentRotationVector);

                        if(cross.z != 0)
                            rotationStartingVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
                        
                        //Check rotation side
                        if (cross.z > 0)
                        {
                            TheSelectedObject.transform.Rotate(0, 0, angle * RotationSpeed);
                        }
                        else if (cross.z < 0)
                        {
                            TheSelectedObject.transform.Rotate(0, 0, angle * -RotationSpeed);
                        }
                    }
                }

                if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
                {
                    scalingTouchDistance = 0;
                }
            }
        }
    }
}