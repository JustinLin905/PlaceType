using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInteract : MonoBehaviour
{
    public Image tileImage;
    public Canvas overlayCanvas;
    public GameObject dropdownMenu;
    private GameObject newDropdownMenu;

    // Public variables which hold the properties of each cell. Assigned upon instantiation.
    public string nameOfRegion;
    public int sentenceLength;          
    public float earnablePoints;
    public bool filled;
    public int tileNumber;
    public float costToBuy;

    // Bools which are turn true by GridManager at start of game if tile is randomly selected to be special
    public bool special = false;                // Boosts points earned from tile
    //public bool explosive = false;              // Causes explosion when filled, filling adjacent tiles
    
    private bool alreadyFilled;
    private bool specialPlaying;

    public static bool dropdownShowing;

    // Animations and objects used for animations and moving the tile to front of render
    [SerializeField] private Animator animationController;
    public RectTransform rectTransform;

    // Sound effects
    public AudioSource successSound;

    private void Start()
    {
        dropdownShowing = false;
        filled = false;
        alreadyFilled = false;
        specialPlaying = false;

        // Set all tiles to translucent at start
        tileImage.CrossFadeAlpha(0.3f, 0, true);
    }

    private void Update()
    {
        // If filled, run flip tile function
        if (filled && !alreadyFilled)
        {
            FlipTile();
            alreadyFilled = true;
        }
    }
    private void LateUpdate()
    {
        if (special && !filled && !alreadyFilled && !specialPlaying)
        {
            // Play special flipping animation
            StartCoroutine(PlaySpecialAnimation());
            specialPlaying = true;
        }
    }

    // Function called to instantiate a dropdown menu upon left-clicking tile
    public void ShowDropdown()
    {
        if (!dropdownShowing)
        {
            GameObject newDropdownMenu = Instantiate(dropdownMenu, Input.mousePosition, Quaternion.identity, overlayCanvas.transform);

            // Update dropdown menu text with cell-specific stats
            newDropdownMenu.GetComponent<DropdownController>().SetStats(nameOfRegion, sentenceLength, earnablePoints, filled,  tileNumber, costToBuy, special);

            dropdownShowing = true;
        }
    }

    public void FlipTile()
    {
        filled = true;

        // Disable then enable animator on tile to stop any animations that may be playing already
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<Animator>().enabled = true;

        // Set color to white then crossfade, in order to get accurate color.
        tileImage.CrossFadeAlpha(1, 0, true);

        // Move to front of render so tile shows above the rest
        rectTransform.SetAsLastSibling();

        animationController.Play("Flipping Animation", -1, 0f);
        successSound.Play();
    }

    // Function called from DropdownController -> GridManager -> specific tile to actually crossfade the colour of a tile to another color
    public void ChangeColor(string color)
    {
        // Move to front, Play flipping animation
        rectTransform.SetAsLastSibling();
        animationController.Play("Flipping Animation", -1, 0f);
        successSound.Play();

        // CROSS FADE COLOR IS MIXING THE TWO COLORS FOR SOME REASON
        switch (color)
        {
            case "White":
                tileImage.CrossFadeColor(new Color32(255, 255, 255, 255), 0.5f, true, true);               
                break;

            case "Light Grey":
                tileImage.CrossFadeColor(new Color32(210, 210, 210, 255), 0.5f, true, true);
                break;

            case "Grey":

                tileImage.CrossFadeColor(new Color32(130, 130, 130, 255), 0.5f, true, true);
                break;

            case "Black":
                tileImage.CrossFadeColor(new Color32(24, 24, 24, 255), 0.5f, true, true);
                break;

            case "Pink":
                tileImage.CrossFadeColor(new Color32(255, 167, 209, 255), 0.5f, true, true);
                break;

            case "Red":
                tileImage.CrossFadeColor(new Color32(229, 0, 0, 255), 0.5f, true, true);
                break;

            case "Orange":
                tileImage.CrossFadeColor(new Color32(229, 149, 0, 255), 0.5f, true, true);
                break;

            case "Brown":
                tileImage.CrossFadeColor(new Color32(160, 106, 66, 255), 0.5f, true, true);
                break;

            case "Yellow":
                tileImage.CrossFadeColor(new Color32(229, 204, 0, 255), 0.5f, true, true);
                break;

            case "Light Green":
                tileImage.CrossFadeColor(new Color32(148, 224, 68, 255), 0.5f, true, true);
                break;

            case "Green":
                tileImage.CrossFadeColor(new Color32(2, 190, 1, 255), 0.5f, true, true);
                break;

            case "Turquoise":
                tileImage.CrossFadeColor(new Color32(0, 211, 221, 255), 0.5f, true, true);
                break;

            case "Light Blue":
                tileImage.CrossFadeColor(new Color32(0, 131, 199, 255), 0.5f, true, true);
                break;

            case "Blue":
                tileImage.CrossFadeColor(new Color32(0, 0, 234, 255), 0.5f, true, true);
                break;

            case "Magenta":
                tileImage.CrossFadeColor(new Color32(254, 0, 255, 255), 0.5f, true, true);
                break;

            case "Purple":
                tileImage.CrossFadeColor(new Color32(130, 0, 128, 255), 0.5f, true, true);
                break;

            default:
                break;
        }

        
    }

    // Coroutine to play special "flipping" animation if tile is special
    IEnumerator PlaySpecialAnimation()
    {
        tileImage.CrossFadeAlpha(1, 1, true);
        animationController.Play("Special Animation", -1, 0f);

        yield return new WaitForSeconds(1f);

        tileImage.CrossFadeAlpha(0.3f, 1, true);

        yield return new WaitForSeconds(5.5f);

        specialPlaying = false;
    }
}
