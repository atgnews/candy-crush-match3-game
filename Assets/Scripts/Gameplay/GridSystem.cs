using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CandyCrush.Core;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Manages the 8x8 game grid and candy interactions
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        private const int GRID_WIDTH = 8;
        private const int GRID_HEIGHT = 8;
        private const float CANDY_SIZE = 1f;

        [SerializeField] private Transform gridContainer;
        [SerializeField] private Candy candyPrefab;
        [SerializeField] private float gridStartX = -3.5f;
        [SerializeField] private float gridStartY = 3.5f;

        private Candy[,] grid = new Candy[GRID_WIDTH, GRID_HEIGHT];
        private List<Candy> candyPool = new List<Candy>();
        private HashSet<Candy> matchedCandies = new HashSet<Candy>();

        public int Width => GRID_WIDTH;
        public int Height => GRID_HEIGHT;

        private void Awake()
        {
            if (gridContainer == null)
                gridContainer = transform;
        }

        /// <summary>
        /// Initialize grid with random candies
        /// </summary>
        public void InitializeGrid(CandyType[] levelGrid = null)
        {
            ClearGrid();

            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    CandyType candyType;

                    if (levelGrid != null && y * GRID_WIDTH + x < levelGrid.Length)
                    {
                        candyType = levelGrid[y * GRID_WIDTH + x];
                    }
                    else
                    {
                        candyType = GetRandomCandyType();
                    }

                    SpawnCandy(x, y, candyType);
                }
            }
        }

        /// <summary>
        /// Spawn a candy at grid position
        /// </summary>
        private void SpawnCandy(int x, int y, CandyType type)
        {
            Vector3 worldPos = GridToWorldPosition(x, y);

            Candy candy;
            if (candyPool.Count > 0)
            {
                candy = candyPool[0];
                candyPool.RemoveAt(0);
                candy.gameObject.SetActive(true);
            }
            else
            {
                candy = Instantiate(candyPrefab, gridContainer);
            }

            candy.transform.position = worldPos;
            candy.Initialize(x, y, type);
            grid[x, y] = candy;
        }

        /// <summary>
        /// Get candy at grid position
        /// </summary>
        public Candy GetCandy(int x, int y)
        {
            if (IsValidPosition(x, y))
                return grid[x, y];
            return null;
        }

        /// <summary>
        /// Check if position is within grid bounds
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < GRID_WIDTH && y >= 0 && y < GRID_HEIGHT;
        }

        /// <summary>
        /// Convert grid coordinates to world position
        /// </summary>
        public Vector3 GridToWorldPosition(int x, int y)
        {
            return new Vector3(
                gridStartX + x * CANDY_SIZE,
                gridStartY - y * CANDY_SIZE,
                0
            );
        }

        /// <summary>
        /// Convert world position to grid coordinates
        /// </summary>
        public bool WorldToGridPosition(Vector3 worldPos, out int x, out int y)
        {
            x = Mathf.RoundToInt(worldPos.x - gridStartX);
            y = Mathf.RoundToInt(gridStartY - worldPos.y);
            return IsValidPosition(x, y);
        }

        /// <summary>
        /// Swap two adjacent candies
        /// </summary>
        public bool TrySwapCandies(int x1, int y1, int x2, int y2)
        {
            if (!IsValidPosition(x1, y1) || !IsValidPosition(x2, y2))
                return false;

            // Check if adjacent
            if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1)
                return false;

            Candy candy1 = grid[x1, y1];
            Candy candy2 = grid[x2, y2];

            // Swap
            grid[x1, y1] = candy2;
            grid[x2, y2] = candy1;

            candy1.SetGridPosition(x2, y2);
            candy2.SetGridPosition(x1, y1);

            return true;
        }

        /// <summary>
        /// Find all matches on the grid
        /// </summary>
        public List<List<Candy>> FindMatches()
        {
            List<List<Candy>> matches = new List<List<Candy>>();
            matchedCandies.Clear();

            // Check horizontal matches
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    if (!matchedCandies.Contains(grid[x, y]))
                    {
                        List<Candy> horizontalMatch = FindConsecutiveCandies(x, y, 1, 0);
                        if (horizontalMatch.Count >= 3)
                        {
                            matches.Add(horizontalMatch);
                            foreach (var candy in horizontalMatch)
                                matchedCandies.Add(candy);
                        }
                    }
                }
            }

            // Check vertical matches
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    if (!matchedCandies.Contains(grid[x, y]))
                    {
                        List<Candy> verticalMatch = FindConsecutiveCandies(x, y, 0, 1);
                        if (verticalMatch.Count >= 3)
                        {
                            matches.Add(verticalMatch);
                            foreach (var candy in verticalMatch)
                                matchedCandies.Add(candy);
                        }
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Find consecutive candies in a direction
        /// </summary>
        private List<Candy> FindConsecutiveCandies(int startX, int startY, int dirX, int dirY)
        {
            List<Candy> consecutive = new List<Candy>();
            Candy startCandy = grid[startX, startY];

            if (startCandy.CandyType == CandyType.None)
                return consecutive;

            consecutive.Add(startCandy);

            // Search forward
            int x = startX + dirX;
            int y = startY + dirY;
            while (IsValidPosition(x, y) && grid[x, y].CandyType == startCandy.CandyType)
            {
                consecutive.Add(grid[x, y]);
                x += dirX;
                y += dirY;
            }

            // Search backward
            x = startX - dirX;
            y = startY - dirY;
            while (IsValidPosition(x, y) && grid[x, y].CandyType == startCandy.CandyType)
            {
                consecutive.Insert(0, grid[x, y]);
                x -= dirX;
                y -= dirY;
            }

            return consecutive;
        }

        /// <summary>
        /// Clear matched candies
        /// </summary>
        public void ClearMatches(List<List<Candy>> matches)
        {
            foreach (var match in matches)
            {
                foreach (var candy in match)
                {
                    if (IsValidPosition(candy.GridX, candy.GridY))
                    {
                        candy.AnimateClear();
                        grid[candy.GridX, candy.GridY] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Apply gravity to candies
        /// </summary>
        public void ApplyGravity()
        {
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = GRID_HEIGHT - 1; y >= 0; y--)
                {
                    if (grid[x, y] == null)
                    {
                        // Find candy above
                        for (int checkY = y - 1; checkY >= 0; checkY--)
                        {
                            if (grid[x, checkY] != null && grid[x, checkY].CandyType != CandyType.None)
                            {
                                // Move candy down
                                grid[x, y] = grid[x, checkY];
                                grid[x, checkY] = null;
                                grid[x, y].SetGridPosition(x, y);
                                grid[x, y].Fall(GridToWorldPosition(x, y));
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill empty spaces with new candies
        /// </summary>
        public void FillEmptySpaces()
        {
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    if (grid[x, y] == null || grid[x, y].CandyType == CandyType.None)
                    {
                        SpawnCandy(x, y, GetRandomCandyType());
                    }
                }
            }
        }

        /// <summary>
        /// Get random candy type
        /// </summary>
        private CandyType GetRandomCandyType()
        {
            return (CandyType)Random.Range(1, 7);
        }

        /// <summary>
        /// Clear all candies from grid
        /// </summary>
        private void ClearGrid()
        {
            foreach (var candy in grid)
            {
                if (candy != null)
                {
                    candy.gameObject.SetActive(false);
                    candyPool.Add(candy);
                }
            }
            grid = new Candy[GRID_WIDTH, GRID_HEIGHT];
        }
    }
}
