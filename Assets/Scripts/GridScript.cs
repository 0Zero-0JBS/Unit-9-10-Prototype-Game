using UnityEngine;

public class GridScript : MonoBehaviour
{
    public Transform[,] grid = new Transform [10, 20];

    [Header("Grid Alignment")]
    public Vector2 gridOrigin = new Vector2(-1.93f, -4.23f);
    public float cellSize = 0.34f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2Int Round(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x - gridOrigin.x) / cellSize);
        int y = Mathf.RoundToInt((pos.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public bool IsInsideBorder(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 10 && pos.y >= 0 && pos.y < 20;
    }

    public void UpdateGrid(Transform tetromino)
    {
        for (int y = 0; y < 20; y++)
            for (int x = 0; x < 10; x++)
                if (grid[x, y] != null && grid[x, y].parent == tetromino)
                    grid[x, y] = null;

        foreach (Transform block in tetromino)
        {
            Vector2Int pos = Round(block.position);
            if (pos.y < 20) grid[pos.x, pos.y] = block;
        }
    }

    public Transform GetTransformAtGridPosition(Vector2Int pos)
    {
        if(pos.y > 20-1)
        {
            return null;
        }
        return grid[pos.x, pos.y];
    }

    public bool IsValidPosition(Transform tetromino)
    {
        foreach(Transform block in tetromino)
        {
            Vector2Int pos = Round(block.position);
            if (!IsInsideBorder(pos))
            {
                return false;
            }

            if (GetTransformAtGridPosition(pos) != null && GetTransformAtGridPosition(pos).parent != tetromino)
            {
                return false;
            }
        }
        return true;
    }
}
