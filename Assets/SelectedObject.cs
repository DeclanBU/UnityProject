using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObject : MonoBehaviour
{
    
    [SerializeField] 
    private Material Selected;
    [SerializeField] 
    private Material Unselected;

    private MeshRenderer myRend;

    [HideInInspector] 
    public bool currentlySelected = false;

    // Start is called before the first frame update
    void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        OnSelect(false);
    }

    public void OnSelect(bool toSelect)
    {
        currentlySelected = toSelect;
        if (currentlySelected == false)
        {
            myRend.material = Unselected;
        }
        else
        {
            myRend.material = Selected;
        }
    }
}
