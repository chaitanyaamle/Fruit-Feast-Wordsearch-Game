using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WordSearchInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private List<TextMeshProUGUI> selectedTiles = new List<TextMeshProUGUI>();
    private List<string> words = new List<string> { "MANGO", "BANANA", "PINEAPPLE", "STRAWBERRY", "PEAR" };
    private bool isDragging = false;
    private LineRenderer currentLineRenderer;
    public GameObject lineRendererPrefab;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    public Canvas canvas; // Reference to the Canvas
    public List<GameObject> fruitObjects;
    public string gameOverSceneName = "GameOverScene"; // Name of your game over scene

    void Update()
    {
        HandleTouchInput();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print("INITIALIZE DOWN");
        selectedTiles.Clear();
        isDragging = true;
        CreateNewLineRenderer();
    }

    public void OnDrag(PointerEventData eventData)
    {
        HandlePointerEvent(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print("INITIALIZE UP");
        isDragging = false;
        CheckWord();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnPointerDown(CreatePointerEventData(touch));
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnDrag(CreatePointerEventData(touch));
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnPointerUp(CreatePointerEventData(touch));
                    break;
            }
        }
    }

    PointerEventData CreatePointerEventData(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;
        return eventData;
    }

    void HandlePointerEvent(PointerEventData eventData)
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out mousePos);

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = eventData.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            TextMeshProUGUI textElement = result.gameObject.GetComponent<TextMeshProUGUI>();
            if (textElement != null && !selectedTiles.Contains(textElement))
            {
                selectedTiles.Add(textElement);
                textElement.color = Color.yellow; // Highlighting the tile
                UpdateLineRenderer();
            }
        }
    }

    void CreateNewLineRenderer()
    {
        if (currentLineRenderer != null)
        {
            lineRenderers.Add(currentLineRenderer);
        }
        GameObject lineObj = Instantiate(lineRendererPrefab, canvas.transform); // Instantiate within Canvas
        currentLineRenderer = lineObj.GetComponent<LineRenderer>();
        currentLineRenderer.positionCount = 0;
    }

    void UpdateLineRenderer()
    {
        if (currentLineRenderer == null) return;

        currentLineRenderer.positionCount = selectedTiles.Count;
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            currentLineRenderer.SetPosition(i, selectedTiles[i].transform.position);
        }
    }

    void CheckWord()
    {
        print("checking...");
        string selectedWord = "";
        foreach (var tile in selectedTiles)
        {
            selectedWord += tile.text;
        }

        if (words.Contains(selectedWord))
        {
            foreach (var tile in selectedTiles)
            {
                tile.color = Color.green; // Correct word color
            }
            currentLineRenderer.startColor = Color.green;
            currentLineRenderer.endColor = Color.green;
            print("true");

            TriggerFruitFall(selectedWord);

            // Check if all fruits are found
            if (fruitObjects.Count == 0)
            {
                LoadGameOverScene();
            }
        }
        else
        {
            foreach (var tile in selectedTiles)
            {
                tile.color = Color.white; // Incorrect word color
            }
            Destroy(currentLineRenderer.gameObject);
            print("false");
        }

        currentLineRenderer = null;
    }

    void TriggerFruitFall(string word)
    {
        for (int i = fruitObjects.Count - 1; i >= 0; i--)
        {
            GameObject fruit = fruitObjects[i];
            if (fruit.name.ToUpper() == word)
            {
                Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = fruit.AddComponent<Rigidbody2D>();
                }
                rb.gravityScale = 1; // Set gravity to make the fruit fall

                // Remove the fruit from the list and destroy the object after falling
                fruitObjects.RemoveAt(i);
                Destroy(fruit, 2.0f); // Delay to give time for the fall animation
                break; // Ensure only the matching fruit falls
            }
        }
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    bool ArrayContains(TextMeshProUGUI[] array, TextMeshProUGUI element)
    {
        foreach (TextMeshProUGUI item in array)
        {
            if (item == element)
                return true;
        }
        return false;
    }
}
