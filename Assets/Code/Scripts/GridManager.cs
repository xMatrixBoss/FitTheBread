using UnityEngine;

using System.Collections.Generic;
using System.Collections; // Ensure System.Collections is included for Coroutines


public class GridInventorySystem : MonoBehaviour
{
    public int gridWidth = 5; // Number of grid squares horizontally
    public int gridHeight = 5; // Number of grid squares vertically
    public float cellSize = 1f; // Size of each grid cell
    public Vector2 gridCenter = Vector2.zero; // Center of the grid
    public GameObject gridCellPrefab; // Prefab to represent a grid cell

    public Color freeColor = Color.white; // Color for free grid cells
    public Color occupiedColor = Color.red; // Color for occupied grid cells

    private bool[,] gridSpaces; // Tracks occupied grid spaces
    private GameObject[,] gridCellObjects; // Stores instantiated grid cell objects

    private List<InventoryItem> itemsInGrid = new List<InventoryItem>(); // Track all items in the grid
    private Dictionary<InventoryItem, List<Vector2>> itemOccupiedCells = new Dictionary<InventoryItem, List<Vector2>>(); // Track occupied cells per item

    // New Variables for tracking cell counts
    public int totalCells { get; private set; } // Total number of cells in the grid, read-only property
    public int freeCellsCount { get; private set; } // Number of free cells
    public int occupiedCellsCount { get; private set; } // Number of occupied cells


    void Start()
    {
        InitializeGrid();
        CreateGridVisuals();
        StartCoroutine(ValidateGridState()); // Start the grid validation coroutine

        // Calculate and assign totalCells after grid initialization
        totalCells = gridWidth * gridHeight;
        Debug.Log("Grid Initialized: Total Cells = " + totalCells + ", Free Cells = " + freeCellsCount + ", Occupied Cells = " + occupiedCellsCount);
    }

    void InitializeGrid()
    {
        gridSpaces = new bool[gridWidth, gridHeight];
        gridCellObjects = new GameObject[gridWidth, gridHeight];

        freeCellsCount = 0; // Initialize free cells count
        occupiedCellsCount = 0; // Initialize occupied cells count

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridSpaces[x, y] = false; // Mark all spaces as free initially
                freeCellsCount++; // Increment free cells count as each cell is initialized as free
            }
        }
    }

    void CreateGridVisuals()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("Grid Cell Prefab is not assigned!");
            return;
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Calculate the position of the grid cell
                Vector2 cellPosition = new Vector2(
                    gridCenter.x + x * cellSize,
                    gridCenter.y + y * cellSize
                );

                // Instantiate the grid cell prefab
                GameObject cell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f); // Scale to match cell size
                gridCellObjects[x, y] = cell; // Store the reference

                // Set the initial color of the grid cell
                UpdateGridCellColor(x, y);
            }
        }
    }

    public Vector2 SnapToGrid(Vector2 position)
    {
        // Calculate the snapped position based on the grid center and cell size
        float snappedX = Mathf.Round((position.x - gridCenter.x) / cellSize) * cellSize + gridCenter.x;
        float snappedY = Mathf.Round((position.y - gridCenter.y) / cellSize) * cellSize + gridCenter.y;
        return new Vector2(snappedX, snappedY);
    }

    public bool CanPlaceItem(List<Vector2> squareWorldPositions)
    {
        // Check if all squares fit within the grid bounds and are not occupied
        foreach (Vector2 squarePosition in squareWorldPositions)
        {
            int gridX = Mathf.RoundToInt((squarePosition.x - gridCenter.x) / cellSize);
            int gridY = Mathf.RoundToInt((squarePosition.y - gridCenter.y) / cellSize);

            // Check if the square is outside the grid bounds
            if (gridX < 0 || gridY < 0 || gridX >= gridWidth || gridY >= gridHeight)
            {
                return false;
            }

            // Check if the square is already occupied
            if (gridSpaces[gridX, gridY])
            {
                return false;
            }
        }

        return true;
    }

    public void PlaceItem(InventoryItem item, List<Vector2> squareWorldPositions)
    {
        // Clear previously occupied cells for this item, if any (important for item movement)
        if (itemOccupiedCells.ContainsKey(item))
        {
            ClearItemCells(item); // Clear cells based on previously stored positions
            itemOccupiedCells[item].Clear(); // Clear the stored positions themselves
        }
        else
        {
            itemOccupiedCells.Add(item, new List<Vector2>()); // Initialize if item is placed for the first time
        }

        // Mark the grid spaces as occupied and store occupied cells
        foreach (Vector2 squarePosition in squareWorldPositions)
        {
            int gridX = Mathf.RoundToInt((squarePosition.x - gridCenter.x) / cellSize);
            int gridY = Mathf.RoundToInt((squarePosition.y - gridCenter.y) / cellSize);

            if (gridX >= 0 && gridY >= 0 && gridX < gridWidth && gridY < gridHeight)
            {
                if (!gridSpaces[gridX, gridY]) // Only update counts if the cell was previously free
                {
                    freeCellsCount--;
                    occupiedCellsCount++;
                }
                gridSpaces[gridX, gridY] = true;
                UpdateGridCellColor(gridX, gridY); // Update the color of the grid cell
                itemOccupiedCells[item].Add(squarePosition); // Store the occupied cell position
            }
        }

        // Add the item to the list of items in the grid
        if (!itemsInGrid.Contains(item))
        {
            itemsInGrid.Add(item);
        }
        Debug.Log("Item Placed: Free Cells = " + freeCellsCount + ", Occupied Cells = " + occupiedCellsCount);
    }


    public void RemoveItem(InventoryItem item, List<Vector2> squareWorldPositions)
    {
        ClearItemCells(item); // Clear cells based on squareWorldPositions passed to remove
        itemOccupiedCells.Remove(item); // Remove item from occupied cells tracking

        // Remove the item from the list of items in the grid
        if (itemsInGrid.Contains(item))
        {
            itemsInGrid.Remove(item);
        }
    }

     private void ClearItemCells(InventoryItem item)
    {
        if (itemOccupiedCells.ContainsKey(item))
        {
            List<Vector2> occupiedPositions = itemOccupiedCells[item];
            foreach (Vector2 squarePosition in occupiedPositions)
            {
                int gridX = Mathf.RoundToInt((squarePosition.x - gridCenter.x) / cellSize);
                int gridY = Mathf.RoundToInt((squarePosition.y - gridCenter.y) / cellSize);

                if (gridX >= 0 && gridY >= 0 && gridX < gridWidth && gridY < gridHeight)
                {
                    if (gridSpaces[gridX, gridY]) // Only update counts if the cell was previously occupied
                    {
                        freeCellsCount++;
                        occupiedCellsCount--;
                    }
                    gridSpaces[gridX, gridY] = false;
                    UpdateGridCellColor(gridX, gridY); // Update the color of the grid cell
                }
            }
        }
        Debug.Log("Item Cells Cleared: Free Cells = " + freeCellsCount + ", Occupied Cells = " + occupiedCellsCount);
    }

    // Call this function when an item starts to be moved (e.g., drag started)
    public void StartMovingItem(InventoryItem item)
    {
        if (itemOccupiedCells.ContainsKey(item))
        {
            ClearItemCells(item);
            itemOccupiedCells[item].Clear(); // Prepare for new positions, or remove if you clear completely on move start
            //itemOccupiedCells.Remove(item); // If you want to completely remove tracking until placed again
        }
    }


    void UpdateGridCellColor(int x, int y)
    {
        // Update the color of the grid cell based on its occupied state
        if (gridCellObjects[x, y] != null)
        {
            SpriteRenderer renderer = gridCellObjects[x, y].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = gridSpaces[x, y] ? occupiedColor : freeColor;
            }
        }
    }

    public bool IsCellOccupied(int x, int y)
    {
        // Check if a specific cell is occupied
        if (x >= 0 && y >= 0 && x < gridWidth && y < gridHeight)
        {
            return gridSpaces[x, y];
        }
        return false;
    }

    public void ClearGrid()
    {
        // Clear all grid spaces and reset their colors
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridSpaces[x, y]) // Only update counts if the cell was previously occupied
                {
                    freeCellsCount++;
                    occupiedCellsCount--;
                }
                gridSpaces[x, y] = false;
                UpdateGridCellColor(x, y);
            }
        }

        // Clear the list of items in the grid and occupied cells tracking
        itemsInGrid.Clear();
        itemOccupiedCells.Clear();

        freeCellsCount = totalCells; // Reset free cells count to total after clearing grid
        occupiedCellsCount = 0; // Reset occupied cells count to 0 after clearing grid
    }

    IEnumerator ValidateGridState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f); // Validate the grid every 0.5 seconds

            // Corrected Validation Logic: Just update cell colors based on gridSpaces
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    UpdateGridCellColor(x, y); // Ensure cell color reflects gridSpaces state
                }
            }
        }
    }
    
}