using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private List<Vector2> squareOffsets = new List<Vector2>();
    private bool isDragging = false;
    private Vector2 offset;
    private GridInventorySystem gridInventory;

    void Start()
    {
        gridInventory = FindObjectOfType<GridInventorySystem>();
        CalculateSquareOffsets();
    }

    void CalculateSquareOffsets()
    {
        ItemSquare[] squares = GetComponentsInChildren<ItemSquare>();

        if (squares.Length == 0)
        {
            Debug.LogError("No ItemSquare components found in children!");
            return;
        }

        squareOffsets.Clear();
        Vector2 centerPoint = Vector2.zero;

        foreach (ItemSquare square in squares)
        {
            squareOffsets.Add((Vector2)square.transform.localPosition);
            centerPoint += (Vector2)square.transform.localPosition;
        }

        centerPoint /= squares.Length;

        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] -= centerPoint;
        }
    }

    void Update()
    {
        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateItem();
            }
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gridInventory.StartMovingItem(this);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition + offset;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            SnapToGrid();
        }
    }

    void SnapToGrid()
    {
        Vector2 snappedPosition = gridInventory.SnapToGrid(transform.position);
        transform.position = snappedPosition;
        List<Vector2> squareWorldPositions = GetSquareWorldPositions();

        if (gridInventory.CanPlaceItem(squareWorldPositions))
        {
            gridInventory.PlaceItem(this, squareWorldPositions);
        }
        else
        {
            transform.position = gridInventory.SnapToGrid(transform.position);
        }
    }

    void RotateItem()
    {
        transform.Rotate(0, 0, 90);
        List<Vector2> originalOffsets = new List<Vector2>(squareOffsets);
        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] = new Vector2(-originalOffsets[i].y, originalOffsets[i].x);
        }
    }

    

    void RecalculateSquareOffsetsAfterTransform()
    {
        ItemSquare[] squares = GetComponentsInChildren<ItemSquare>();
        squareOffsets.Clear();
        foreach (ItemSquare square in squares)
        {
            squareOffsets.Add((Vector2)square.transform.localPosition);
        }
        Vector2 centerPoint = Vector2.zero;
        foreach (Vector2 offset in squareOffsets)
        {
            centerPoint += offset;
        }
        centerPoint /= squareOffsets.Count;
        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] -= centerPoint;
        }
    }

    public List<Vector2> GetSquareWorldPositions()
    {
        List<Vector2> worldPositions = new List<Vector2>();
        foreach (Vector2 offset in squareOffsets)
        {
            worldPositions.Add((Vector2)transform.position + offset);
        }
        return worldPositions;
    }
}