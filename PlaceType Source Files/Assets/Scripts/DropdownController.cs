using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownController : MonoBehaviour
{

    public GameObject typingMission;

    // Passed from tile interact
    public string nameOfRegion;
    public int sentenceLength;          // Controlled by difficulty of region
    public float earnablePoints;
    public bool filled;
    public int tileNumber;
    public float costToBuy;

    public bool special;

    //public bool explosive;

    // Text objects on dropdown menu
    public Text title;
    public Text specialTileText;
    public Text missionDescription;
    public Text costText;
    public Text recolorCostText;
    

    // Button GameObjects
    public GameObject missionButton;
    public GameObject purchaseButton;
    public GameObject recolorButton;
    
    public GameObject checkmarkPopup;
    public Text costPopup;

    // GameObjects found in scene to access scripts outside of the Dropdown Menu prefab
    private GameObject gridObject;


    public void Start()
    {
        gridObject = GameObject.Find("Grid UI");
        checkmarkPopup.SetActive(false);

        // If tile is not special, don't show special text
        if (!special)
        {
            specialTileText.gameObject.SetActive(false);
        }     
    }   

    private void Update()
    {
        // Destroy dropdown if Right mouse is pressed
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(gameObject);
            TileInteract.dropdownShowing = false;
        }

        // If mouse is out of dropdown, close upon left click
        if (!IsMouseInDropdown())
        {

            // Need to destroy the dropdown menu once player clicks outside of it
            if (TileInteract.dropdownShowing && Input.GetMouseButtonDown(0))
            {
                Destroy(gameObject);
                TileInteract.dropdownShowing = false;
            }


        }

        // If player scrolls, also destroy the dropdown menu
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Destroy(gameObject);
            TileInteract.dropdownShowing = false;
        }

        // If player presses F key, also destroy dropdown menu
        if (Input.GetKeyDown(KeyCode.F))
        {
            Destroy(gameObject);
            TileInteract.dropdownShowing = false;
        }
    }

    // Function to detect if mouse is inside the dropdown menu
    private bool IsMouseInDropdown()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Function to launch a typing mission
    public void StartMission()
    {
        // Set this to false to disable Quick Retry if it was available
        QuickRetry.quickRetryReady = false;
        
        Destroy(gameObject);
        GameObject newTypingMission = Instantiate(typingMission);

        // Set stats of new typing mission
        newTypingMission.GetComponent<TypingManager>().SetStats(nameOfRegion, sentenceLength, earnablePoints, tileNumber);
    }

    // Function to buy tile by pressing button
    public void BuyTile()
    {
        // If player has enough points, buy tile
        if (PointsScript.points >= costToBuy)
        {
            if (!filled)
            {
                // Deduct points, change text
                filled = true;
                PointsScript.points -= costToBuy;
                costText.text = "Tile Filled";

                // Flip tile
                gridObject.GetComponent<GridManager>().FillTile(tileNumber);

                // Add 1 to tiles flipped
                PointsScript.tilesFilled++;

                // Destroy dropdown menu
                Destroy(gameObject);
                TileInteract.dropdownShowing = false;
            }

            else
            {
                costText.text = "Tile Filled";
                StartCoroutine(FadeRed(costText));
            }

        }

        else
        {
            // Turn text red if not enough points
            StartCoroutine(FadeRed(costText));
        }
    }

    // Function to change the color of a tile via the dropdown menu, after the tile has been filled
    public void SelectColor(string color)
    {
        if (PointsScript.points >= costToBuy)
        {
            PointsScript.points -= costToBuy;
            gridObject.GetComponent<GridManager>().ChangeColor(tileNumber, color);                    

            // Show checkmark popup
            StartCoroutine(ShowCheckmark());
        }

        else
        {
            // Turn text red if not enough points
            StartCoroutine(FadeRed(recolorCostText));
        }
        
    }

    // Set stats function. Updates text shown on dropdown menu. Called from TileInteract
    public void SetStats(string region, int length, float points, bool tileFilled, int tileNum, float cost, bool isSpecial)
    {
        nameOfRegion = region;
        sentenceLength = length;
        earnablePoints = points;
        filled = tileFilled;
        tileNumber = tileNum;
        costToBuy = cost;
        special = isSpecial;

        // Update text shown on dropdown menu
        missionDescription.text = "Typing Mission\n" + nameOfRegion + "\n" + sentenceLength + " words";

        // Set relevant text to title and cost text
        title.text = "Tile " + tileNumber;
        costText.text = costToBuy.ToString() + " PlacePoints";

        // Show special stuff if the tile is special
        if (special)
        {
            title.color = new Color32(255, 255, 160, 255);
        }

        // Activate Mission or Recolor button based on tile filled stats
        if (!filled)
        {
            missionButton.SetActive(true);
            purchaseButton.SetActive(true);
            recolorButton.SetActive(false);
        }

        else
        {
            missionButton.SetActive(false);
            purchaseButton.SetActive(false);
            recolorButton.SetActive(true);

            // Update cost text
            recolorCostText.text = costToBuy.ToString() + " PlacePoints";
        }
    }

    // Coroutine to turn text red if not enough points
    IEnumerator FadeRed(Text costText)
    {
        costText.CrossFadeColor(Color.red, 0.25f, false, true);
        yield return new WaitForSeconds(0.6f);
        costText.CrossFadeColor(Color.white, 0.25f, false, true);
    }

    // Coroutine to play checkmark popup animation
    IEnumerator ShowCheckmark()
    {
        // Activate checkmark gameobject and update cost text
        checkmarkPopup.SetActive(true);
        costPopup.text = "-" + costToBuy.ToString() + " PlacePoints";

        // Play animation attached to checkmark
        checkmarkPopup.GetComponent<Animator>().Play("Checkmark Pop Up", -1, 0f);

        yield return new WaitForSeconds(0.9f);

        checkmarkPopup.SetActive(false);
    }
}
