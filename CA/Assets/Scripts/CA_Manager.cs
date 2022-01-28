using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CA_Manager : MonoBehaviour
{
    [Header ("Grid settings")]
    public int res_x;
    public int res_y;

    public float size;
    public CA_Cell[] grid_cells;
    public int numCells;

    [Header("Visualisation Settings")]
    public GameObject cell_obj;

    public Material matA;
    public Material matD;

    [Header("CA Settings")]
    [Range (0.0f, 1.0f)]
    public float random_treshold;

    private bool[] states_current;
    private bool[] states_updte;
    public bool runCA;
    public float interval;
    private float timer;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        CreateGrid();
        VisualiseGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            RandomiseAllCells();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            runCA = true;
        }
        if(runCA == true)
        {
            if(timer>= interval)
            { 
                RunCA();
                timer = 0;
            }
            timer += Time.deltaTime;
            
        }
    }
    #region Grid

    public void CreateGrid()
    {
        numCells = res_x * res_y;
        grid_cells = new CA_Cell[numCells];
        states_current = new bool[numCells];
        states_updte = new bool[numCells];

        int counter = 0;

        for (int y=0; y<res_y; y++)
        {
            var y_pos = y * size;

            for (int x=0; x < res_x; x++)
            {
                var x_pos = x * size;
                CA_Cell cell = new CA_Cell();
                cell.ID = counter;
                cell.position = new Vector2(x_pos, y_pos);
                cell.normilised_coords = new Vector2(x, y);

                grid_cells[counter] = cell;
                counter++;

            }
        }
    }
    public void VisualiseGrid()
    {
        for (int i=0; i<numCells; i++)
        {
            var cell = grid_cells[i];
            var pos = cell.position;
            GameObject new_cell = GameObject.Instantiate(cell_obj, transform);
            new_cell.transform.position = new Vector3(pos.x, pos.y, 0);
            new_cell.transform.localScale = new Vector3(size, size, size);
            new_cell.name = "cell_" + i.ToString();
            new_cell.GetComponent<MeshRenderer>().sharedMaterial = matD;
        }
    }
    #endregion
    #region CA
    public void RandomiseAllCells()
    {
        for(int i =0; i< numCells; i++)
        {
            RandomiseCellState(i);
        }

        UpdateAllColors();
    }

    public void RandomiseCellState(int ID)
    {
        float number = UnityEngine.Random.Range(0.0f, 1.0f);
        if(number > random_treshold)
        {
            states_current[ID] = true;
        }
        else
        {
            states_current[ID] = false;
        }
    }
    public void UpdateAllColors()
    {
        for(int i=0; i<numCells; i++)
        {
            UpdateSingleCellColor(i);
        }
    }
    public void UpdateSingleCellColor(int ID)
    {
        if(states_current[ID] == true)
        {
            transform.GetChild(ID).GetComponent<MeshRenderer>().sharedMaterial = matA;
        }
        else
        {
            transform.GetChild(ID).GetComponent<MeshRenderer>().sharedMaterial = matD;
        }
    }
    public void RunCA()
    {
        for(int i=0; i<numCells; i++)
        {
            
            var state = states_current[i];
            var n_count = CountNeighbours(i);
             if(state==true)
            {
                if(n_count == 2 || n_count == 3)
                {
                    states_updte[i] = true;
                }
                else
                {
                    states_updte[i] = false;
                }
            }
             else
            {
                if(n_count==3)
                {
                    states_updte[i] = true;
                }
                else
                {
                    states_updte[i] = false;
                }
            }
        }
        (states_current, states_updte) = (states_updte, states_current);
        UpdateAllColors();
    }
    public int CountNeighbours(int ID)
    {
        var cell = grid_cells[ID];
        var coords = cell.normilised_coords;
        int neighbour_counter = 0;

        for (int y=-1;y<2; y++)
        {
            var ny = coords.y + y;
            if(ny<0 || ny>=res_y)
            {
                continue;
            }

            for (int x = -1; x < 2; x++)
            {
                var nx = coords.x + x;
                if (nx < 0 || nx>= res_x)
                {
                    continue;
                }
                if(ny==0 && nx==0)
                {
                    continue;
                }

                int n_id = (int)ny * res_x + (int)nx;
                if(states_current[n_id]== true)
                {
                    neighbour_counter++;
                }
            }
        }
        return neighbour_counter;
    }

    #endregion
}
