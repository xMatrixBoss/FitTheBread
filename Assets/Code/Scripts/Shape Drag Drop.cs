using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private List<Vector2> squareOffsets = new List<Vector2>();
    public bool isDragging = false;
    private Vector2 offset;
    private GridInventorySystem gridInventory;
    private Vector2 targetPosition;
    private bool isSnapping = false;
    private int rotationState = 0;
    private const float snapSpeed = 10f;

    public LayerMask shapeLayer;

    public bool IsDragging => isDragging;

    void Start()
    {
        gridInventory = FindObjectOfType<GridInventorySystem>();
        CalculateSquareOffsets();
        gameObject.layer = LayerMask.NameToLayer("Shape");
    }

    void Update()
    {
        if (isSnapping)
            SmoothSnapToGrid();

        if (Input.GetMouseButtonDown(0))
            TryStartDrag();
        else if (isDragging && Input.GetMouseButton(0))
            Drag();
        else if (isDragging && Input.GetMouseButtonUp(0))
            EndDrag();

        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R))
                RotateItem();
            if (Input.GetKeyDown(KeyCode.F))
                FlipItemHorizontally();
        }
    }

    void TryStartDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, shapeLayer);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
            StartDrag();
    }

    void StartDrag()
    {
        isDragging = true;
        isSnapping = false;
        offset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gridInventory.StartMovingItem(this);
        AudioManager.Instance.PlayShapePickUp();
    }


    void Drag()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    }

    void EndDrag()
    {
        isDragging = false;
        Vector2 snappedPosition = gridInventory.SnapToGrid(transform.position);
        if (Vector2.Distance(transform.position, snappedPosition) <= 1f)
        {
            targetPosition = snappedPosition;
            isSnapping = true;
            AudioManager.Instance.PlayShapePlace();
        }
        else
        {
            gridInventory.RemoveItem(this, GetSquareWorldPositions());
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

    public void RotateItem()
    {
        gridInventory.RemoveItem(this, GetSquareWorldPositions());
        transform.Rotate(0, 0, 90);
        rotationState = (rotationState + 1) % 4;

        List<Vector2> originalOffsets = new List<Vector2>(squareOffsets);
        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] = new Vector2(-originalOffsets[i].y, originalOffsets[i].x);
        }

        gridInventory.PlaceItem(this, GetSquareWorldPositions());
        AudioManager.Instance.PlayShapeRotate();
    }

    public void FlipItemHorizontally()
    {
        if (rotationState % 2 != 0)
        {
            Debug.Log("Flipping is only allowed at 0 or 180 degrees.");
            return;
        }

        gridInventory.RemoveItem(this, GetSquareWorldPositions());
        for (int i = 0; i < squareOffsets.Count; i++)
        {
            squareOffsets[i] = new Vector2(-squareOffsets[i].x, squareOffsets[i].y);
        }
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        gridInventory.PlaceItem(this, GetSquareWorldPositions());

        AudioManager.Instance.PlayShapeFlip();
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
            centerPoint += (Vector2)square.transform.localPosition;

        centerPoint /= squares.Length;

        foreach (ItemSquare square in squares)
            squareOffsets.Add((Vector2)square.transform.localPosition - centerPoint);
    }

    public List<Vector2> GetSquareWorldPositions()
    {
        List<Vector2> worldPositions = new List<Vector2>();
        foreach (Vector2 offset in squareOffsets)
            worldPositions.Add((Vector2)transform.position + offset);
        return worldPositions;
    }
}