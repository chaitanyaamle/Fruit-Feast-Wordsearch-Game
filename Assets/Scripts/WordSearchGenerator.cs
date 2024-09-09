using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WordSearchGenerator : MonoBehaviour
{
    public GameObject letterTilePrefab;
    public GridLayoutGroup gridLayout;
    public int rows = 6;
    public int columns = 8;
    private char[,] grid;
    private List<string> words = new List<string> { "MANGO", "BANANA", "PINEAPPLE", "STRAWBERRY", "PEAR" };
    private int maxAttempts = 100; // Maximum attempts to place each word

    void Start()
    {
        ConfigureGridLayout();
        GenerateGrid();
    }

    void ConfigureGridLayout()
    {
        // Set the number of rows and columns
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
    }

    void GenerateGrid()
    {
        grid = new char[rows, columns];
        InitializeGrid();
        PlaceWordsInGrid();
        FillEmptySpacesWithRandomLetters();
        CreateGridUI();
    }

    void InitializeGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j] = ' ';
            }
        }
    }

    void PlaceWordsInGrid()
    {
        foreach (var word in words)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxAttempts)
            {
                int direction = Random.Range(0, 4); // 0 = horizontal, 1 = vertical, 2 = diagonal bottom-right, 3 = diagonal top-right
                int row = Random.Range(0, rows);
                int col = Random.Range(0, columns);

                if (direction == 0 && col + word.Length <= columns)
                {
                    if (CanPlaceWordHorizontally(row, col, word))
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            grid[row, col + i] = word[i];
                        }
                        placed = true;
                    }
                }
                else if (direction == 1 && row + word.Length <= rows)
                {
                    if (CanPlaceWordVertically(row, col, word))
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            grid[row + i, col] = word[i];
                        }
                        placed = true;
                    }
                }
                else if (direction == 2 && row + word.Length <= rows && col + word.Length <= columns)
                {
                    if (CanPlaceWordDiagonallyBottomRight(row, col, word))
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            grid[row + i, col + i] = word[i];
                        }
                        placed = true;
                    }
                }
                else if (direction == 3 && row - word.Length >= -1 && col + word.Length <= columns)
                {
                    if (CanPlaceWordDiagonallyTopRight(row, col, word))
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            grid[row - i, col + i] = word[i];
                        }
                        placed = true;
                    }
                }

                attempts++;
            }

            if (!placed)
            {
                Debug.LogWarning($"Failed to place the word '{word}' after {maxAttempts} attempts.");
            }
        }
    }

    bool CanPlaceWordHorizontally(int row, int col, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (grid[row, col + i] != ' ')
            {
                return false;
            }
        }
        return true;
    }

    bool CanPlaceWordVertically(int row, int col, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (grid[row + i, col] != ' ')
            {
                return false;
            }
        }
        return true;
    }

    bool CanPlaceWordDiagonallyBottomRight(int row, int col, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (grid[row + i, col + i] != ' ')
            {
                return false;
            }
        }
        return true;
    }

    bool CanPlaceWordDiagonallyTopRight(int row, int col, string word)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (grid[row - i, col + i] != ' ')
            {
                return false;
            }
        }
        return true;
    }

    void FillEmptySpacesWithRandomLetters()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] == ' ')
                {
                    grid[i, j] = (char)('A' + Random.Range(0, 26));
                }
            }
        }
    }

    void CreateGridUI()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var letterTile = Instantiate(letterTilePrefab, gridLayout.transform);
                letterTile.GetComponentInChildren<TextMeshProUGUI>().text = grid[i, j].ToString();

                // Ensure the tile has a BoxCollider2D for detecting clicks
                if (letterTile.GetComponent<BoxCollider2D>() == null)
                {
                    letterTile.AddComponent<BoxCollider2D>();
                }
            }
        }
    }
}
