using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private List<Vector2> squareOffsets = new List<Vector2>();
    private bool isDragging = false;
    private Vector2 offset;
    private GridInventorySystem gridInventory;
    private bool isFlippedHorizontally = false;
    private bool isFlippedVertically = false;

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

<<<<<<< Updated upstream
    void Update()
    {
        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateItem();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                FlipItemHorizontally();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                FlipItemVertically();
            }
        }
    }

=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    }

    void RotateItem()
    {
        transform.Rotate(0, 0, -90);
        List<Vector2> originalOffsets = new List<Vector2>(squareOffsets);
        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] = new Vector2(-originalOffsets[i].y, originalOffsets[i].x);
        }

        // Adjust flip flags after rotation
        if (transform.rotation.eulerAngles.z % 180 != 0)
        {
            bool temp = isFlippedHorizontally;
            isFlippedHorizontally = isFlippedVertically;
            isFlippedVertically = temp;
        }
        RecalculateSquareOffsetsAfterTransform();
    }

    void FlipItemHorizontally()
    {
        isFlippedHorizontally = !isFlippedHorizontally;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        RecalculateSquareOffsetsAfterTransform();
    }

    void FlipItemVertically()
    {
        isFlippedVertically = !isFlippedVertically;
        transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        RecalculateSquareOffsetsAfterTransform();
    }
=======
    } 
>>>>>>> Stashed changes

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