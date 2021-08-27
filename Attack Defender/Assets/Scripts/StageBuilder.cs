using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
    public GameObject cell;
    public Material firstMaterial;
    public Material secondMaterial;

    GameObject[,] mapCells;

    bool _gameStarted = false;
    public bool gameStarted
    {
        get { return _gameStarted; }
        set { _gameStarted = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load wall prefab for testing purposes
        var wall = GameConst.LoadPrefab(AssetPath.DefenseStructure, "Wall");

        // Load cannon prefab for testing purposes
        var cannon = GameConst.LoadPrefab(AssetPath.DefenseUnit, "Cannon");

        // Initialize Map Array
        mapCells = new GameObject[GameConst.mapGridSizeX, GameConst.mapGridSizeZ];

        // Create map grid
        for (int z = 0; z < GameConst.mapGridSizeZ; z++)
        {
            for (int x = 0; x < GameConst.mapGridSizeX; x++)
            {
                // Add Map Cell
                GameObject newCell = Instantiate(cell, new Vector3(x * GameConst.mapCellSize.x, 0, z * GameConst.mapCellSize.z), Quaternion.identity);
                mapCells[x, z] = newCell;

                // Get MapCell component
                var mapCell = newCell.GetComponentInChildren<MapCell>();

                // change cube size
                mapCell.ResizeObject(GameConst.mapCellSize);

                // change material
                if (z % 2 == 0)
                {
                    if (x % 2 == 0)
                    {
                        newCell.GetComponentInChildren<Renderer>().material = secondMaterial;
                    }
                    else
                    {
                        newCell.GetComponentInChildren<Renderer>().material = firstMaterial;
                    }
                }
                else
                {
                    if (x % 2 == 0)
                    {
                        newCell.GetComponentInChildren<Renderer>().material = firstMaterial;
                    }
                    else
                    {
                        newCell.GetComponentInChildren<Renderer>().material = secondMaterial;
                    }
                }

                if (x < GameConst.mapGridSizeX - 1)
                {
                    if (z == 0 && x == 5)
                    {
                        // Add a single cannon on cell x0,z5 cell just for testing
                        var clone = Instantiate(cannon);
                        var clonePos = GameConst.mapCellSize / 2;
                        clonePos.y = GameConst.mapCellSize.y;

                        // Set the map cell as a parent
                        clone.transform.parent = newCell.transform;
                        clone.transform.localPosition = clonePos;
                        clone.transform.Rotate(Vector3.up, 90f);

                        // Automatic get structure
                        mapCell.structure = newCell.GetComponentInChildren<DefenseStructure>();
                    }
                    else
                    {
                        // Add a simple wall on every cell just for testing
                        var clone = Instantiate(wall);
                        var clonePos = GameConst.mapCellSize / 2;
                        clonePos.y = GameConst.mapCellSize.y;

                        // Set the map cell as a parent
                        clone.transform.parent = newCell.transform;
                        clone.transform.localPosition = clonePos;
                        clone.transform.Rotate(Vector3.up, 90f);

                        // Automatic get structure
                        mapCell.structure = newCell.GetComponentInChildren<DefenseStructure>();
                    }
                }
            }
        }

        // Start raid
        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public MapCell GetMapCell(int x, int z)
    {
        return this.mapCells[x - 1, z - 1].GetComponentInChildren<MapCell>();
    }
}
