using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // List of all the grid tiles
    public GameObject[] gridTiles;
    private GameObject tileToFlip;

    public PointsScript pointsScript;

    private void Start()
    {
        // Call function to randomly select special tiles
        SelectSpecial();
    }

    public void FillTile(int tileNumber)
    {
        // Get the correct tile
        tileToFlip = gridTiles[tileNumber];

        // Get the TileInteract script on the tile object, and set filled to true
        tileToFlip.GetComponent<TileInteract>().filled = true;

        // Update points text
        pointsScript.UpdatePoints();
    }

    // Function called from Dropdown menu to begin changing tile color
    public void ChangeColor(int tileNumber, string color)
    {
        // Get the correct tile
        tileToFlip = gridTiles[tileNumber];

        // Get the TileInteract script on the tile object, then change color
        tileToFlip.GetComponent<TileInteract>().ChangeColor(color);

        // Update points text
        pointsScript.UpdatePoints();
    }

    // Function to randomly select 10 tiles to be "special", which grant bonuses
    public void SelectSpecial()
    {

        // Randomly choose a tile 0 - 99 to make special tiles. 8 special tiles, 2 explosive tiles
        for (int i = 0; i < 8; i++)
        {
            int randomIndex = Random.Range(0, 100);
            gridTiles[randomIndex].GetComponent<TileInteract>().special = true;
            gridTiles[randomIndex].GetComponent<TileInteract>().earnablePoints *= 3;
        }

        /*
        for (int i = 0; i < 2; i++)
        {
            gridTiles[Random.Range(0, 100)].GetComponent<TileInteract>().explosive = true;
        }
        */
    }
}
