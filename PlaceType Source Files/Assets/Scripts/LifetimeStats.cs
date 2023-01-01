using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifetimeStats : MonoBehaviour
{
    private float lifetimeWPM;
    private float lifetimeCPS;
    private float lifetimeWords;
    private float lifetimeChars;
    private float lifetimeSeconds;

    // Multiplier calculated based on typing stats. The faster, the higher this multiplier goes. Also used by PointsScript
    public static float pointsMultiplier;

    // Text objects
    public TextMeshProUGUI wpmObject;
    public TextMeshProUGUI cpsObject;
    public TextMeshProUGUI wordsObject;
    public TextMeshProUGUI multiplierObject;

    // Private variable to see if lifetime WPM is increasing or decreasing
    private float previousWPM;
    private float previousMultiplier;

    // Arrow and change text objects
    public GameObject[] greenArrows;
    public GameObject[] redArrows;
    public TextMeshProUGUI changeInWPM;
    public TextMeshProUGUI changeInMultiplier;


    // Start is called before the first frame update
    void Start()
    {
        // Set all lifetime stats to 0 to start
        lifetimeWPM = 0;
        lifetimeCPS = 0;
        lifetimeWords = 0;
        lifetimeChars = 0;
        lifetimeSeconds = 0;
        pointsMultiplier = 1.00f;

        // Set all arrows and related text to inactive at start
        greenArrows[0].SetActive(false);
        greenArrows[1].SetActive(false);
        redArrows[0].SetActive(false);
        redArrows[1].SetActive(false);

        changeInWPM.text = "";
        changeInMultiplier.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLifetimeStats(float words, float chars, float seconds)
    {
        // Store previous WPM
        previousWPM = lifetimeWPM;
        previousMultiplier = pointsMultiplier;

        lifetimeWords += words;
        lifetimeChars += chars;
        lifetimeSeconds += seconds;

        // Aggregate stats to calculate lifetime WPM and CPS
        lifetimeWPM = (lifetimeChars / 5) / (lifetimeSeconds / 60);
        lifetimeCPS = lifetimeChars / lifetimeSeconds;

        // Update multiplier based on a formula.
        // 40 WPM is the average typing speed. This results in ~1x multiplier.
        // This doubles to 2x by 80 WPM.
        pointsMultiplier = lifetimeWPM / 40f;

        // If WPM increased from previous, set green arrow to active and update change text
        if (lifetimeWPM >= previousWPM)
        {
            greenArrows[0].SetActive(true);
            greenArrows[1].SetActive(true);
            redArrows[0].SetActive(false);
            redArrows[1].SetActive(false);

            changeInWPM.CrossFadeColor(Color.green, 0, false, true);
            changeInMultiplier.CrossFadeColor(Color.green, 0, false, true);
            changeInWPM.text = "+" + string.Format("{0:.##}", lifetimeWPM - previousWPM);
            changeInMultiplier.text = "+" + string.Format("{0:.##}", pointsMultiplier - previousMultiplier);
        }

        else if (lifetimeWPM < previousWPM)
        {
            greenArrows[0].SetActive(false);
            greenArrows[1].SetActive(false);
            redArrows[0].SetActive(true);
            redArrows[1].SetActive(true);

            changeInWPM.CrossFadeColor(Color.red, 0, false, true);
            changeInMultiplier.CrossFadeColor(Color.red, 0, false, true);
            changeInWPM.text = string.Format("{0:.##}", lifetimeWPM - previousWPM);
            changeInMultiplier.text = string.Format("{0:.##}", pointsMultiplier - previousMultiplier);
        }

        // Update text objects
        wpmObject.text = string.Format("{0:.##}", lifetimeWPM);
        cpsObject.text = string.Format("{0:.##}", lifetimeCPS);
        wordsObject.text = lifetimeWords.ToString();
        multiplierObject.text = "x" + string.Format("{0:.##}", pointsMultiplier);

    }
}
