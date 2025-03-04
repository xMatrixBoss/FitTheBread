using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class GridInventorySystem : MonoBehaviour
{
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float cellSize = 1f;
    public Vector2 gridCenter = Vector2.zero;
    public GameObject gridCellPrefab;
    public GameObject winScreen;
    public TextMeshProUGUI winTimeText;
    private Timer timer;

    public Color freeColor = Color.white;
    public Color occupiedColor = Color.red;

    private List<InventoryItem>[,] gridSpaces;
    private GameObject[,] gridCellObjects;
    private Dictionary<InventoryItem, List<Vector2>> itemOccupiedCells = new Dictionary<InventoryItem, List<Vector2>>();

    void Start()
    {
        InitializeGrid();
        CreateGridVisuals();
        StartCoroutine(ValidateGridState());
        timer = FindObjectOfType<Timer>();
    }

    void InitializeGrid()
    {
        gridSpaces = new List<InventoryItem>[gridWidth, gridHeight];
        gridCellObjects = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                gridSpaces[x, y] = new List<InventoryItem>();
            }
        }
    }

    void CreateGridVisuals()
    {
        if (gridCellPrefab == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 cellPosition = new Vector2(gridCenter.x + x * cellSize, gridCenter.y + y * cellSize);
                GameObject cell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, transform);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1f);
                gridCellObjects[x, y] = cell;
                UpdateGridCellColor(x, y);
            }
        }
    }

    public void StartMovingItem(InventoryItem item)
    {
        RemoveItem(item, item.GetSquareWorldPositions());
    }

    public Vector2 SnapToGrid(Vector2 position)
    {
        int gridX = Mathf.RoundToInt((position.x - gridCenter.x) / cellSize);
        int gridY = Mathf.RoundToInt((position.y - gridCenter.y) / cellSize);
        gridX = Mathf.Clamp(gridX, 0, gridWidth - 1);
        gridY = Mathf.Clamp(gridY, 0, gridHeight - 1);
        return new Vector2(gridCenter.x + gridX * cellSize, gridCenter.y + gridY * cellSize);
    }

    public bool CanPlaceItem(List<Vector2> positions)
    {
        foreach (Vector2 pos in positions)
        {
            int gridX = Mathf.RoundToInt((pos.x - gridCenter.x) / cellSize);
            int gridY = Mathf.RoundToInt((pos.y - gridCenter.y) / cellSize);
            if (gridX < 0 || gridY < 0 || gridX >= gridWidth || gridY >= gridHeight)
                return false;
            if (gridSpaces[gridX, gridY].Count > 0)
                return false;
        }
        return true;
    }

    public void PlaceItem(InventoryItem item, List<Vector2> squareWorldPositions)
    {
        RemoveItem(item, itemOccupiedCells.ContainsKey(item) ? itemOccupiedCells[item] : new List<Vector2>());
        
        itemOccupiedCells[item] = new List<Vector2>();

        foreach (Vector2 pos in squareWorldPositions)
        {
            int gridX = Mathf.RoundToInt((pos.x - gridCenter.x) / cellSize);
            int gridY = Mathf.RoundToInt((pos.y - gridCenter.y) / cellSize);
            
            if (gridX >= 0 && gridY >= 0 && gridX < gridWidth && gridY < gridHeight)
            {
                gridSpaces[gridX, gridY].Add(item);
                itemOccupiedCells[item].Add(pos);
                UpdateGridCellColor(gridX, gridY);
            }
        }
        CheckWinCondition();
    }

    public void RemoveItem(InventoryItem item, List<Vector2> squareWorldPositions)
    {
        if (itemOccupiedCells.ContainsKey(item))
        {
            foreach (Vector2 pos in itemOccupiedCells[item])
            {
                int gridX = Mathf.RoundToInt((pos.x - gridCenter.x) / cellSize);
                int gridY = Mathf.RoundToInt((pos.y - gridCenter.y) / cellSize);
                
                if (gridX >= 0 && gridY >= 0 && gridX < gridWidth && gridY < gridHeight)
                {
                    gridSpaces[gridX, gridY].Remove(item);
                    UpdateGridCellColor(gridX, gridY);
                }
            }
            itemOccupiedCells.Remove(item);
        }
    }

    void UpdateGridCellColor(int x, int y)
    {
        if (gridCellObjects[x, y] != null)
        {
            SpriteRenderer renderer = gridCellObjects[x, y].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = IsCellOccupied(x, y) ? occupiedColor : freeColor;
            }
        }
    }

    bool IsCellOccupied(int x, int y)
    {
        return gridSpaces[x, y].Count > 0;
    }

    void CheckWinCondition()
    {
        foreach (var cell in gridSpaces)
        {
            if (cell.Count == 0) return;
        }
        
        InventoryItem[] items = FindObjectsOfType<InventoryItem>();
        foreach (var item in items)
        {
            if (item.isDragging) return;
        }
        
        timer.ToggleTimer(true);
        winScreen.SetActive(true);
        winTimeText.text = $"Your time: {timer.GetElapsedTime()}";
    }

    IEnumerator ValidateGridState()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    UpdateGridCellColor(x, y);
                }
            }
        }
    }
}

