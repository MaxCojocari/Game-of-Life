using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public int eventCounter = 0;
    public bool randomizeAtStart = false;
    public bool start = true;
    public bool buttonStartClicked = false;
    public bool buttonResetClicked = false;
    public Color red;
    public Color yellow;

    [Range(0.001f, 5f)] public float updateRate = 2.5f;
    public Text generationNr;
    public Text rateValue;
    public int counterGeneration;
    public Button buttonStart;
    public Button buttonReset;
    public Toggle toggle;
    public Slider slider;

    [Space]
    public Sprite tile;

    private Cell[,] cells = new Cell[192, 108];
    private int[,] states = new int[192, 108];
    private int[,] rule = new int[192, 108];

    // Start is called before the first frame update
    void Start()
    {
        buttonStart.onClick.AddListener(() => IsStartClicked());
        buttonReset.onClick.AddListener(() => RefreshState());
        IsStartClicked();

        //Create grid and fill it with cells
        CreateGrid(gridSizeX, gridSizeY);
        slider.value = 2.5f;
    }

    private void Update()
    {
        rateValue.text = slider.value.ToString();
        updateRate = slider.value;

        //Check for left click to switch the state of the tile clicked on
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f))
            {
                hit.collider.gameObject.GetComponent<Cell>().SetState(1, 0);
            }
        }

        if (!buttonStartClicked)
        {
            if (toggle.isOn && eventCounter == 0)
            {
                RandomizeGrid();
                eventCounter++;
            }
            InvokeRepeating("UpdateStates", 0.1f, updateRate);
        }
        else
        {
            CancelInvoke("UpdateStates");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(0);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(1);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(2);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //Apply the new mode per each cell
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    cells[x, y].SetMode(3);
                }
            }
        }
    }

    void IsStartClicked()
    {
        buttonStartClicked = !buttonStartClicked;
        buttonStart.GetComponent<Image>().color = buttonStartClicked ? yellow : red;
        buttonStart.GetComponentInChildren<Text>().text = buttonStartClicked ? "Start" : "Stop";
    }

    // Randomly fill the grid by changing the state of cells
    void RandomizeGrid()
    {
        if (toggle.isOn)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    // 0 or 1
                    cells[x, y].SetState(Random.Range(0, 2), 0);
                }
            }
        }
    }

    void RefreshState()
    {
        eventCounter = 0;
        counterGeneration = 0;
        generationNr.text = 0.ToString();
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cells[x, y].SetState(0, 0);
                cells[x, y].age = 0;
                cells[x, y].sprRend.color = new Color(0, 0, 0);
            }
        }
    }

    /// Update the state of each cell on a grid
    void UpdateStates()
    {
        //Loop through all cells in a grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //Get current state of the cell
                int state = cells[x, y].state;

                //Initial result of the update is the previous state of the cell
                int result = state;

                //Initial rule used is the rule that has been used previously
                int rl = cells[x, y].currentRl;

                //Get the living neighbour count of current cell
                int count = GetLivingNeighbours(x, y);

                //Check the rules and apply the correct state based on the result

                if (state == 1 && count < 2) {
                    result = 0;
                    rl = 0;

                    if (x > 40 && x < 122 && y > 25 && y < 83) {
                        result = Random.Range(0, 2);
                        rl = 0;
                    }
                }
                if (state == 1 && (count == 2 || count == 3))
                {
                    result = 1;
                    rl = 1;
                }
                if (state == 1 && count > 3)
                {
                    result = 0;
                    rl = 2;
                }
                if (state == 0 && count == 3)
                {
                    result = 1;
                    rl = 3;
                }

                // New rules
                // 1. Birth exception (strict number of neighbors == 5)
                if (state == 0 && count == 5)
                {
                    result = 1;
                    rl = 1;
                }

                // 2. Dead excess (strict number of neighbors == 6)
                if (state == 1 && count == 6)
                {
                    result = 0;
                }

                // "Bermuda Triangle"
                // If number of neighbors is a "triangle number" and
                // cell is alive, it turns in red
                if (state == 1 && (count == 1 || count == 3 || count == 6))
                {
                    rl = 4;
                }

                //CREATE AN ARRAY AND COPY OVER THE WHOLE THING AND THEN APPLY THE RESULTS LATER
                states[x, y] = result;
                rule[x, y] = rl;
            }
        }

        //Apply the results of rule check for every cell
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                cells[x, y].SetState(states[x, y], rule[x, y]);
            }
        }

        SetAcctualGenerationCount();

    }

    void SetAcctualGenerationCount()
    {
        generationNr.text = counterGeneration.ToString();
        counterGeneration++;
    }

    // Create grid of given size and fill it with cell objects
    void CreateGrid(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Create new gameobject with given name
                GameObject temp = new GameObject("x: " + x + " y: " + y);
                //Add the cell component to the gameobjec
                temp.AddComponent<Cell>().CreateCell(new int[] { x, y }, tile);
                //add the reference to the cell to the array
                cells[x, y] = temp.GetComponent<Cell>();
            }
        }
    }

    // Returns the count of the neighbours that are alive around the current cell
    int GetLivingNeighbours(int x, int y)
    {
        int count = 0;

        //
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                //The way of wrapping the grid on X and Y
                //Based on The Coding Train video about the CA
                int col = (x + i + gridSizeX) % gridSizeX;
                int row = (y + j + gridSizeY) % gridSizeY;

                //If the cell is alive add 1, if it's dead the state is 0 so nothing happens
                count += cells[col, row].state;
            }
        }
        //Remove the current cell from the count
        count -= cells[x, y].state;

        return count;
    }
}
