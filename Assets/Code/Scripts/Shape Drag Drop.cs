using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private List<Vector2> squareOffsets = new List<Vector2>();
    private bool isDragging = false;
    private Vector2 offset;
    private GridInventorySystem gridInventory;
    private int rotationState = 0;

    private Vector2 targetPosition;
    private bool isSnapping = false;
    private float snapSpeed = 10f;

    public LayerMask shapeLayer;
    public LayerMask gridLayer;
    public LayerMask childSquaresLayer;

    void Start()
    {
        gridInventory = FindObjectOfType<GridInventorySystem>();
        CalculateSquareOffsets();
        gameObject.layer = LayerMask.NameToLayer("Shape");
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
        if (isSnapping)
        {
            SmoothSnapToGrid();
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, shapeLayer);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                StartDrag();
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Drag();
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }

        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R))
                RotateItem();

            if (Input.GetKeyDown(KeyCode.F))
                FlipItemHorizontally();
        }
    }

    void SmoothSnapToGrid()
    {
        transform.position = Vector2.Lerp(transform.position, targetPosition, snapSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isSnapping = false;
            gridInventory.PlaceItem(this, GetSquareWorldPositions());
        }
    }

    void StartDrag()
    {
        isDragging = true;
        isSnapping = false;
        offset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gridInventory.StartMovingItem(this);
    }

    void Drag()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition + offset;
    }

    void EndDrag()
    {
        isDragging = false;
        targetPosition = gridInventory.SnapToGrid(transform.position);
        isSnapping = true;
    }

    void RotateItem()
    {
        gridInventory.RemoveItem(this, GetSquareWorldPositions());
        transform.Rotate(0, 0, 90);
        rotationState = (rotationState + 1) % 4;

        List<Vector2> originalOffsets = new List<Vector2>(squareOffsets);
        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] = new Vector2(-originalOffsets[i].y, originalOffsets[i].x);
        }

        List<Vector2> newWorldPositions = GetSquareWorldPositions();
        gridInventory.PlaceItem(this, newWorldPositions);
    }

    void FlipItemHorizontally()
    {
        if (rotationState != 0 && rotationState != 2)
        {
            Debug.Log("Flipping is only allowed at 0° or 180° rotation.");
            return;
        }

        gridInventory.RemoveItem(this, GetSquareWorldPositions());
        List<Vector2> originalOffsets = new List<Vector2>(squareOffsets);

        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] = new Vector2(-originalOffsets[i].x, originalOffsets[i].y);
        }

        List<Vector2> newWorldPositions = GetSquareWorldPositions();

        if (gridInventory.CanPlaceItem(newWorldPositions))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
            gridInventory.PlaceItem(this, newWorldPositions);
        }
        else
        {
            squareOffsets = originalOffsets;
            Debug.Log("Cannot flip here. Invalid position.");
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