using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuickRetry : MonoBehaviour
{
    public GameObject typingMission;
    public GameObject quickRetryHint;
    public TextMeshProUGUI tileNumberText;

    // Passed from Typing Manager
    public string nameOfRegion;
    public int sentenceLength;          // Controlled by difficulty of region
    public float earnablePoints;
    public int tileNumber;

    // Becomes true when Quick Retry is available. Also becomes unavailable once user manually chooses another mission
    public static bool quickRetryReady;

    // Animation
    //[SerializeField] private Animator slideAnimation;

    // Start is called before the first frame update
    void Start()
    {
        // Make indicator inactive at start
        quickRetryHint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (quickRetryReady)
        {
            // If player presses R...
            if (Input.GetKeyDown(KeyCode.R))
            {
                quickRetryHint.SetActive(false);
                quickRetryReady = false;
                
                // Create a new typing mission
                GameObject newTypingMission = Instantiate(typingMission);

                // Set stats of new typing mission
                newTypingMission.GetComponent<TypingManager>().SetStats(nameOfRegion, sentenceLength, earnablePoints, tileNumber);
            }
        }        

    }

    // Function called from Typing Manager upon failure to enable Quick Retry for the latest mission
    public void EnableQuickRetry(string region, int length, float points, int tileNum)
    {
        // Update stats, tile number text on indicator
        nameOfRegion = region;
        sentenceLength = length;
        earnablePoints = points;
        tileNumber = tileNum;

        // Update tile number text
        tileNumberText.text = "Tile " + tileNumber.ToString();

        // Make indicator visible
        quickRetryHint.SetActive(true);
        quickRetryReady = true;

        
    }

    // Function called from Typing Manager upon failure to enable Quick Retry for the latest mission
    public void HideQuickRetry()
    {
        quickRetryHint.SetActive(false);
        quickRetryReady = false;

    }

    // Function to launch a typing mission (similar to function found in Dropdown Controller)
    public void StartMission()
    {
        quickRetryHint.SetActive(false);
        GameObject newTypingMission = Instantiate(typingMission);

        // Set stats of new typing mission
        newTypingMission.GetComponent<TypingManager>().SetStats(nameOfRegion, sentenceLength, earnablePoints, tileNumber);
    }
}
