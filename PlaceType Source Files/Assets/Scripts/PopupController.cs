using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    // Passed from PointsScript. Other variables used are public static and do not need to be passed
    public float pointsEarned;
    public float pointsToAdd;

    // Variables used to show count up animation
    private float addingEarned;
    private float addingTotal;
    private float earnedCounter;
    private float totalCounter;

    // Text objects on popup
    public Text calculationText;
    public Text pointsToAddText;
    public Text totalPointsText;
    public Text tilesFilledText;

    // Function called from points script to show accurate summary of the points earned from the previous mission
    public void UpdateStats(float pointsReward, float pointsAdded, int totalTiles)
    {
        // Update popup with accurate stats
        pointsEarned = pointsReward;
        pointsToAdd = pointsAdded;

        // Update text
        calculationText.text = pointsEarned.ToString() + "  x  " + LifetimeStats.pointsMultiplier.ToString();
        StartCoroutine(CountUpPoints());
        tilesFilledText.text = totalTiles.ToString();

        // Start coroutine to destroy popup after certain time
        StartCoroutine(DestroyPopup());
    }

    // Coroutine to destroy popup after certain time
    IEnumerator DestroyPopup()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    // Coroutine to show points counting up quickly
    IEnumerator CountUpPoints()
    {
        yield return new WaitForSeconds(3f);

        // Calculate how much to add up each tick from 0
        addingEarned = pointsToAdd / 100f;
        addingTotal = PointsScript.points / 100f;

        // Loop adding above values to counter variables until they match the current values
        while (true)
        {
            if (earnedCounter >= pointsEarned || totalCounter >= PointsScript.points)
            {
                earnedCounter = pointsEarned;
                totalCounter = PointsScript.points;
                pointsToAddText.text = "+          " + earnedCounter.ToString();
                totalPointsText.text = "             " + totalCounter.ToString();
                break;
            }

            earnedCounter += addingEarned;
            totalCounter += addingTotal;
            pointsToAddText.text = "+          " + earnedCounter.ToString();
            totalPointsText.text = "             " + totalCounter.ToString();

            yield return new WaitForSeconds(0.01f);
        }
    }
}
