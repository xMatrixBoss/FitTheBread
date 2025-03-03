using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndToggleShapes : MonoBehaviour
{
    public GameObject shapeR;
    public GameObject shapeL;

    private bool isShapeLActive = false;

    void Start()
    {
        if (shapeR == null || shapeL == null)
        {
            Debug.LogError("ShapeR or ShapeL GameObject is not assigned!");
        }
        else
        {
            shapeL.SetActive(false); // Ensure ShapeL is initially off
        }
    }

    void Update()
    {
        // Rotate the object 90 degrees clockwise with "R"
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 0, 90);
        }

        // Toggle shapeR and shapeL with "F"
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleShapes();
        }
    }

    void ToggleShapes()
    {
        isShapeLActive = !isShapeLActive; // Toggle the state

        if (shapeR != null && shapeL != null)
        {
            shapeR.SetActive(!isShapeLActive);
            shapeL.SetActive(isShapeLActive);
        }
    }
}