using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator startGameAnimation;
    public Image startButton;
    public Image quitButton;
    public TextMeshProUGUI quitText;
    public TextMeshProUGUI hintText;

    private bool gameStarted;
    private bool ready;
    private int currentHint;
    private int nextHint;

    private bool hintDelay = true;

    // Start is called before the first frame update
    void Start()
    {
        gameStarted = false;
        ready = false;

        StartCoroutine(ReadyDelay());
        StartCoroutine(ShowHintText());
    }

    // Update is called once per frame
    void Update()
    {
        // If the player presses space, start scene change coroutine
        if (Input.GetKeyDown(KeyCode.Space) && ready && !gameStarted)
        {
            gameStarted = true;
            StartCoroutine(StartGame());
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && ready && !gameStarted)
        {
            Application.Quit();
        }

        if (!hintDelay)
        {
            StartCoroutine(CycleHints());
        }
    }

    // Coroutine to delay ability to start game for 5 seconds upon launch
    IEnumerator ReadyDelay()
    {
        yield return new WaitForSeconds(5);
        ready = true;
    }

    IEnumerator StartGame()
    {
        // Fade out start button
        startButton.CrossFadeAlpha(0, 1, true);
        quitButton.CrossFadeAlpha(0, 1, true);
        quitText.CrossFadeAlpha(0, 1, true);
        
        startGameAnimation.Play("Start Game Animation", -1, 0f);

        yield return new WaitForSeconds(1);

        startButton.gameObject.SetActive(false);

        // Wait 6 seconds
        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("GameScene");
    }


    // Code relating to hints shown in main menu
    List<string> hintsList = new List<string> {
        "Welcome to PlaceType, a typing and art game.",
        "Upon starting a game, 8 tiles are randomly selected to be <color=#ffffa0>\"Special\".</color> These tiles grant 3 times the points when filled via Typing Mission.",
        "There are two ways to fill a tile: via a Typing Mission, which earns points, or via purchase, which spends points.",
        "Tiles can be changed into 16 vibrant colours! Each colour change costs PlacePoints.",
        "There are 100 tiles in the Canvas arranged in a 10x10 grid. Try creating your own pixel art by colouring each tile!",
        "Making a mistake during a Typing Mission causes it to stop immediately! Don't worry though, you can try again right after by closing it and pressing [R].",
        "Raising your average typing speed (WPM) also increases your points multiplier.",
        "It might be a good idea to build up a large points multiplier before filling in <color=#ffffa0>Special</color> tiles, to maximize earnings!",
        "It becomes more difficult to fill tiles as the tile number increases. Harder tiles also grant more points.",
        "Press [Escape] to open the menu and close the game.",
        "If you drag too far away from the Canvas, you can press [F] to teleport back.",
        "Failing a typing mission does not change your typing stats or points multiplier.",
        "Pressing [Escape] then [Shift] will toggle on grid coordinates to help you create pixel art."};


    IEnumerator ShowHintText()
    {
        // Set invisible at start
        hintText.CrossFadeAlpha(0, 0, true);

        yield return new WaitForSeconds(7);

        // Set text to the first hint in the list 
        hintText.text = hintsList[0];
        currentHint = 0;

        hintText.CrossFadeAlpha(1, 2, true);

        yield return new WaitForSeconds(6);

        hintDelay = false;
    }

    IEnumerator CycleHints()
    {
        hintDelay = true;
        hintText.CrossFadeAlpha(0, 2, true);

        yield return new WaitForSeconds(2.5f);

        // Choose a tip from the list that is not the one which is currently shown
        do
        {
            nextHint = Random.Range(1, hintsList.Count);

        } while (nextHint == currentHint);



        // Now update the current tip to the new index and update the text object
        currentHint = nextHint;
        hintText.text = hintsList[currentHint];

        // Fade in text to make it visible
        hintText.CrossFadeAlpha(1, 2, true);

        // Wait 7 seconds before calling coroutine again
        yield return new WaitForSeconds(7);
        hintDelay = false;
    }
}
