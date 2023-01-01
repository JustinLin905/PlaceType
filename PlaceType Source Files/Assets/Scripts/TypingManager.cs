using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypingManager : MonoBehaviour
{
    // Reference sentence generator script
    public RandomSentence randomSentence;

    // Reference to done button script
    public ButtonController buttonController;

    // GameObjects found in scene to access scripts outside of the Typing Mission prefab
    private GameObject pointsObject;
    private GameObject lifetimeObject;
    private GameObject gridObject;
    private GameObject retryObject;

    public Canvas typingCanvas;
    public GameObject popupsContainer;
    public Text sentenceObject;

    // Stuff to fade in at start
    public Text typingMission;
    public Text regionText;
    public Text description;
    public Text[] statsText;

    public Text wpmObject;
    public Text cpsObject;
    public Text wordsCompleteObject;
    public Text secondsElapsedObject;

    // Prefabs to instantiate
    public GameObject decayingText;
    public GameObject completeText;
    public GameObject failingLetter;
    public GameObject failedMissionText;
    public GameObject doneButton;
    public GameObject hint;

    public Image typingCursor;

    // Variables accessed upon instatiation by other scripts
    public string nameOfRegion;
    public int sentenceLength;          // Controlled by difficulty of region
    public float earnablePoints;
    public int tileNumber;

    private string sentenceToType;
    private string editingSentence;
    private bool started;
    private bool finished;
    private bool failed;
    private bool readyToExit;

    // Stats variables
    private float secondsElapsed;
    private float charactersComplete;
    private float wpm;
    private float cps;
    private int wordsComplete;

    [Header("Animation")]
    [SerializeField] private Animator typingAnimator;


    // Function to be accessed by Dropdown menu to set difficulty, etc. when typing mission is created
    public void SetStats(string regionName, int length, float pointsToEarn, int tileNum)
    {
        nameOfRegion = regionName;
        sentenceLength = length;
        earnablePoints = pointsToEarn;
        tileNumber = tileNum;
    }

    // Start is called before the first frame update
    void Start()
    {
        started = false;
        finished = false;
        failed = false;
        readyToExit = false;

        // Get components in scene such as points manager script and tile manager script
        pointsObject = GameObject.Find("Points UI");
        lifetimeObject = GameObject.Find("STATS");
        gridObject = GameObject.Find("Grid UI");
        retryObject = GameObject.Find("Quick Retry");

        // The sentence length will adjust based on difficulty region
        sentenceToType = randomSentence.GenerateSentence(sentenceLength);

        // Update description to reflect changing stats
        regionText.text = nameOfRegion;
        description.text = sentenceLength.ToString() + " words\nPotential Points to Earn: " + earnablePoints.ToString();

        // Set sentenceObject to the sentence that was just generated
        sentenceObject.text = "<color=#858585>" + sentenceToType + "</color>";

        // Start coroutine to fade text in
        StartCoroutine(FadeInText());

        // Start coroutine to continuously flash the typing cursor
        StartCoroutine(FlashCursor());

        // Start coroutine to show hint after inactivity
        StartCoroutine(ShowStartHint());

        // Update words complete text
        wordsCompleteObject.text = wordsComplete.ToString() + "/" + sentenceLength.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (!finished)
        {
            CheckInput();
        }

        if (readyToExit)
        {
            // If user presses the spacebar or enter...
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                // Close typing mission
                StartCoroutine(CloseMission());
            }

        }
    }

    private void CheckInput()
    {

        // Check if input key is space first
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSpace();
        }

        else if (Input.anyKeyDown)
        {
            string keysPressed = Input.inputString;

            if (keysPressed.Length == 1)
            {
                EnterLetter(keysPressed);
            }
        }

    }

    private void EnterLetter(string typedLetter)
    {
        // If user hasn't started typing yet...
        if (!started)
        {
            // Start coroutine to keep track of stats
            StartCoroutine(CalculateStats());

            // Make all stats text white
            for (int i = 0; i < statsText.Length; i++)
            {
                statsText[i].CrossFadeColor(new Color32(255, 255, 255, 255), 0, false, true);
            }

            

            started = true;
        }

        // Check if the letter is in the sentence
        if (IsCorrectLetter(typedLetter))
        {
            RemoveLetter();

            // Pass letter to Decaying Letter coroutine
            StartCoroutine(ShowDecayingLetter(typedLetter));

            IsWordComplete();
        }

        else
        {
            StartCoroutine(FailMission(typedLetter));
        }
    }

    private bool IsCorrectLetter(string letter)
    {
        // Check if the letter is the first index of the reamining sentence
        return sentenceToType.IndexOf(letter) == 0;
    }

    private void RemoveLetter()
    {
        // Remove first letter of sentence
        string newString = sentenceToType.Remove(0, 1);

        UpdateRemainingSentence(newString);
    }

    private void UpdateRemainingSentence(string newString)
    {
        // Add 1 to characters complete
        charactersComplete++;

        sentenceToType = newString;

        sentenceObject.text = sentenceToType;
    }

    private void IsWordComplete()
    {
        if (sentenceToType.Length == 0)
        {
            finished = true;

            // Show complete text
            StartCoroutine(CompleteMission());
        }
    }

    // Special function to check for spaces
    private void CheckSpace()
    {

        if (char.IsWhiteSpace(sentenceToType, 0))
        {

            RemoveSpace();

            IsWordComplete();
        }

        else
        {
            StartCoroutine(FailMission("space"));
        }
    }

    // Special function to remove space, idk it's buggy right now
    private void RemoveSpace()
    {
        // When the user presses space, that means the word is complete. Add one to variable
        wordsComplete++;

        string newString = sentenceToType.Remove(0, 2);

        UpdateRemainingSentence(newString);
    }
    
    IEnumerator FadeInText()
    {
        // Set everything to invisible instantly at start
        typingMission.CrossFadeAlpha(0, 0, false);
        regionText.CrossFadeAlpha(0, 0, false);
        description.CrossFadeAlpha(0, 0, false);
        sentenceObject.CrossFadeAlpha(0, 0, false);

        for (int i = 0; i < statsText.Length; i++)
        {
            statsText[i].CrossFadeAlpha(0, 0, false);
        }

        // Start by fading in the mission text
        typingMission.CrossFadeAlpha(1, 1.5f, false);
        regionText.CrossFadeAlpha(1, 1.5f, false);

        sentenceObject.CrossFadeAlpha(1, 1.5f, false);

        // Wait 1 sec
        yield return new WaitForSeconds(1);

        description.CrossFadeAlpha(1, 1f, false);

        // Wait 0.5 sec
        yield return new WaitForSeconds(0.5f);

        // Fade in stats and set to grey to start
        for (int i = 0; i < statsText.Length; i++)
        {
            statsText[i].CrossFadeColor(new Color32(133, 133, 133, 255), 1, false, true);
        }
    }

    IEnumerator CompleteMission()
    {

        // Set wordsCompleted equal to sentence length
        wordsComplete = sentenceLength;    

        // Allow user to exit menu now with space or enter
        readyToExit = true;

        GameObject newCompleteText = Instantiate(completeText);
        newCompleteText.transform.SetParent(popupsContainer.transform, false);
        
        // Wait 2 seconds, then show in the done button
        yield return new WaitForSeconds(2);

        
        GameObject newDoneButton = Instantiate(doneButton);
        newDoneButton.transform.SetParent(popupsContainer.transform, false);      
        
    }

    IEnumerator ShowDecayingLetter(string letter)
    {

        GameObject newDecayingText = Instantiate(decayingText);
        newDecayingText.transform.SetParent(popupsContainer.transform, false);
        newDecayingText.GetComponent<Text>().text = letter;

        // Wait 0.25 seconds
        yield return new WaitForSeconds(0.25f);

        // Destroy the text object
        Destroy(newDecayingText);
    }

    IEnumerator FlashCursor()
    {
        // Set as invisible to start
        typingCursor.CrossFadeAlpha(0, 0, false);

        // Wait for 2 seconds before starting
        yield return new WaitForSeconds(2);

        while (!finished)
        {
            // Fade in cursor to full opacity over 0.5 seconds
            typingCursor.CrossFadeAlpha(1, 0.5f, false);

            // Wait 0.5 seconds
            yield return new WaitForSeconds(0.5f);

            // Fade out cursor to 0 over 0.5 seconds
            typingCursor.CrossFadeAlpha(0, 0.5f, false);

            // Wait 0.5 seconds
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator FailMission(string letter)
    {
        // Set finished and failed to true
        finished = true;
        failed = true;

        

        // Instantiate failing letter
        GameObject newFailingLetter = Instantiate(failingLetter);
        newFailingLetter.transform.SetParent(popupsContainer.transform, false);
        newFailingLetter.GetComponent<Text>().text = letter;

        // Cross fade colour to red over 1 second
        newFailingLetter.GetComponent<Text>().CrossFadeColor(new Color32(255, 150, 150, 255), 1, false, false);

        // Fade sentence text to same colour
        sentenceObject.CrossFadeColor(new Color32(255, 150, 150, 255), 0.5f, false, false);

        // Wait 0.5 seconds then allow user to exit
        yield return new WaitForSeconds(0.5f);
        readyToExit = true;

        yield return new WaitForSeconds(1.5f);

        // Instantiate failed mission text
        GameObject newFailedMission = Instantiate(failedMissionText);
        newFailedMission.transform.SetParent(popupsContainer.transform, false);

        // Create done button after 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        GameObject newDoneButton = Instantiate(doneButton);
        newDoneButton.transform.SetParent(popupsContainer.transform, false);
    }

    // Coroutine to keep track of words per minute
    IEnumerator CalculateStats()
    {
        // Set wpm and seconds to 0
        wpm = 0;
        secondsElapsed = 0;

        // Loop while typing is not finished
        while (!finished)
        {
            // Update every 0.5 seconds
            yield return new WaitForSeconds(0.5f);

            secondsElapsed += 0.5f;

            // Calculate current words per minute using words complete and seconds elapsed
            wpm = (charactersComplete / 5) / (secondsElapsed / 60);

            // Calculate current characters per second
            cps = charactersComplete / secondsElapsed;

            // Round variables to 2 decimal places
            wpm = Mathf.Round(wpm * 100) / 100;
            cps = Mathf.Round(cps * 100) / 100;


            wpmObject.text = wpm.ToString();
            cpsObject.text = cps.ToString();
            wordsCompleteObject.text = wordsComplete.ToString() + "/" + sentenceLength.ToString();
            secondsElapsedObject.text = secondsElapsed.ToString();

            
        }

        // Once finished, if the user has not failed, update lifetime stats
        if (!failed)
        {
            // Update text objects
            wpmObject.text = wpm.ToString();
            cpsObject.text = cps.ToString();
            wordsCompleteObject.text = wordsComplete.ToString() + "/" + sentenceLength.ToString();
            secondsElapsedObject.text = secondsElapsed.ToString();

            // Update lifetime stats
            lifetimeObject.GetComponent<LifetimeStats>().UpdateLifetimeStats(wordsComplete, charactersComplete, secondsElapsed);
        }
    }

    // Coroutine to show start typing hint if after 10 seconds, the user has not started typing
    IEnumerator ShowStartHint()
    {
        // Wait 10 seconds
        yield return new WaitForSeconds(10);

        // If the user has not started typing, show the start hint
        if (!started)
        {
            // Instantiate start hint
            GameObject newHint = Instantiate(hint);
            newHint.transform.SetParent(popupsContainer.transform, false);
        }

    }

    // Coroutine to fade out and close the typing mission
    IEnumerator CloseMission()
    {
        // Set to false to prevent multiple close calls
        readyToExit = false;

        // Set showing dropdownShowing to false
        TileInteract.dropdownShowing = false;

        // If user WAS SUCCESSFUL in mission only:
        if (!failed)
        {
            // Call function from other script to fill tile
            gridObject.GetComponent<GridManager>().FillTile(tileNumber);

            // Add points
            pointsObject.GetComponent<PointsScript>().AddPoints(earnablePoints);

            // Make Quick Retry indicator disappear if it is still active
            retryObject.GetComponent<QuickRetry>().HideQuickRetry();
        }

        // If the user FAILED the mission only:
        else
        {
            // Pass data about this mission to Quick Retry script, allowing user to jump back to this mission
            retryObject.GetComponent<QuickRetry>().EnableQuickRetry(nameOfRegion, sentenceLength, earnablePoints, tileNumber);
        }

        // Play sliding away animation
        typingAnimator.Play("Slide Away", -1, 0f);

        // Wait short period to let animation finsih before destroying
        yield return new WaitForSeconds(1f);
        

        Destroy(typingCanvas.gameObject);

        // Stop all other coroutines
        StopAllCoroutines();
    }
}
