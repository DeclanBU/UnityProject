using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;

public class InputManager : MonoBehaviour
{
    //Static reference to the selected object to get it from other scripts easily
    public static SelectedObject TheSelectedObject;

    
    //Public variables:
    public float ScalingSpeed = 0.4f;
    public float MinDistanceToPinch = 20;

    
    //Private variables:
    private Camera mainCam;

    //Dragging variables
    private bool dragging = false;
    private float dragginDistance = 0;

    //Scaling variables
    private float scalingTouchDistance;

    //Rotation variables
    private Vector3 initialRotationVector;

    // Start is called before the first frame update
    void Start()
    {
        //Cache the main camera
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Select object on touch
        if (Input.touchCount == 1)
        {
            //Cast a ray on the touch position
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit rayHit;

                //Check if a selectable object was hit and select it
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
                        //Deselect object if a selectable object is not clicked
                        if (TheSelectedObject != null)
                        {
                            TheSelectedObject.OnSelect(false);
                            TheSelectedObject = null;
                        }
                    }
                }
                else
                {
                    //Deselect object if nothing is hit
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
            //Disable dragging if there is no touches
            dragging = false;
        }
        else
        {
            // Get the touch position
            var touch = Input.touches[0];
            Vector2 pos = touch.position;

            //Check if there is a selected object
            if (TheSelectedObject != null)
            {
                //Check if touch just started
                if (touch.phase == TouchPhase.Began)
                {
                    RaycastHit hit;
                    Ray ray = mainCam.ScreenPointToRay(pos);

                    //Check if there is an object at that point
                    if (Physics.Raycast(ray, out hit))
                    {
                        dragginDistance = Vector3.Distance(hit.transform.position, Camera.main.transform.position);
                        dragging = true;
                    }
                }

                //Check if the touch is moved
                if (dragging && touch.phase == TouchPhase.Moved)
                {
                    //When touch is moved - move the position of the object
                    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragginDistance);
                    position = mainCam.ScreenToWorldPoint(position);
                    TheSelectedObject.transform.position = position;
                }
            }

            //Check if the touch ended and disable dragging if it did
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                dragging = false;
            }
        }


        //Scaling and rotating object
        if (Input.touchCount == 2)
        {
            //Check if one of the touches just began
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
            {
                //Set the initial distance between touches and the vector between them, which is needed for rotation
                scalingTouchDistance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                initialRotationVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
            }

            //Check if at least one of the touches is moving
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                //Get the current distance between the touches and calculate the difference between that and the initial one
                float currentTouchDistance =
                    Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

                float deltaDistance = currentTouchDistance - scalingTouchDistance;
                
                //Get the current vector between the two touches
                Vector3 currentRotationVector = Input.GetTouch(1).position - Input.GetTouch(0).position;

                if (deltaDistance > MinDistanceToPinch)
                {
                    //Check if we have an object selected
                    if (TheSelectedObject)
                    {
                        //Check if we are pinching in or out
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
                            //Decrease the scale of the object by the scaling speed
                            Vector3 currentScale = TheSelectedObject.transform.localScale;
                            Vector3 newScale = new Vector3(currentScale.x - ScalingSpeed, currentScale.y - ScalingSpeed,
                                currentScale.z - ScalingSpeed);
                            TheSelectedObject.transform.localScale = newScale;
                        }
                    }
                }
                else
                {
                    //Handle rotation
                    //Get the angle between the starting vector and current vector between the two touches
                    float currentAngle = Vector3.Angle(initialRotationVector, currentRotationVector);
                    
                    //Get the difference between the x of the vectors so we can check what direction is the user rrotating
                    float deltaX = currentRotationVector.x - initialRotationVector.x;
                    if (TheSelectedObject)
                    {
                        //Rotate the object in the appropriate direction
                        if (Mathf.Abs(currentAngle) > 1f)
                        {
                            if(deltaX > 0)
                            TheSelectedObject.transform.Rotate(0, 0, currentAngle,
                                Space.World);
                            else if(deltaX < 0)
                                TheSelectedObject.transform.Rotate(0, 0, -currentAngle,
                                    Space.World);
                            initialRotationVector = currentRotationVector;
                        }
                    }
                    else
                    {
                        //Rotate the camera in the appropriate direction
                        if (Mathf.Abs(currentAngle) > 1f)
                        {
                            if(deltaX > 0)
                                mainCam.transform.Rotate(0, currentAngle, 0,
                                    Space.World);
                            else if(deltaX < 0)
                                mainCam.transform.Rotate(0, -currentAngle, 0,
                                    Space.World);
                            initialRotationVector = currentRotationVector;
                        }
                    }
                }

                //Reset the touch distance and the initial vector at end of touch
                if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
                {
                    scalingTouchDistance = 0;
                    initialRotationVector = currentRotationVector;
                }
            }
        }
    }
}