using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsScript : MonoBehaviour
{
    public static float points;
    private float pointsEarnedBeforeMultiplier;
    private float pointsToAdd;
    public static int tilesFilled;
    public TextMeshProUGUI pointsText;

    // Variable used to show transaction animation with accurate numbers
    private float previousPoints;

    // Objects to show popup
    public Canvas overlayCanvas;
    public GameObject popup;
    private GameObject newPopup;
    

    // Start is called before the first frame update
    void Start()
    {
        points = 0;
        tilesFilled = 0;
    }

    public void AddPoints(float pointsReward)
    {
        // Add points, adjusted using multiplier
        pointsEarnedBeforeMultiplier = pointsReward;
        pointsToAdd = pointsEarnedBeforeMultiplier * LifetimeStats.pointsMultiplier;     
        points += pointsToAdd;

        // Keep track of total tiles filled
        tilesFilled++;

        // Update points text
        pointsText.text = points.ToString();

        // Show popup with summary of points earned
        GameObject newPopup = Instantiate(popup);
        newPopup.transform.SetParent(overlayCanvas.transform, false);
        newPopup.GetComponent<PopupController>().UpdateStats(pointsEarnedBeforeMultiplier, pointsToAdd, tilesFilled);
    }

    public void UpdatePoints()
    {
        // Update points text, show to two decimal places
        pointsText.text = points.ToString("0.00");
    }
}
