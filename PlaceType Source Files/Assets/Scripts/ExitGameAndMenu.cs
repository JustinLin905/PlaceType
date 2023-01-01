using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExitGameAndMenu : MonoBehaviour
{
    [SerializeField] private Animator popupAnimation;
    public GameObject popup;
    public TextMeshProUGUI xAxis;
    public TextMeshProUGUI yAxis;

    private bool popupShowing;
    private bool slidingOut;

    private bool coordinatesShowing;

    // Start is called before the first frame update
    void Start()
    {
        popup.SetActive(false);
        popupShowing = false;
        slidingOut = false;

        // Make coordinates invisible at first
        coordinatesShowing = false;
        xAxis.CrossFadeAlpha(0, 0, true);
        yAxis.CrossFadeAlpha(0, 0, true);
    }

    // Update is called once per frame
    void Update()
    {
        // If user presses escape and popup is not showing
        if (Input.GetKeyDown(KeyCode.Escape) && !popupShowing && !slidingOut)
        {
            StartCoroutine(ShowPopup());
        }

        // If user presses escape + enter while popup is showing, close game
        if (Input.GetKey(KeyCode.Escape) && Input.GetKey(KeyCode.Return) && popupShowing && !slidingOut)
        {
            Application.Quit();
        }

        // If user presses Tab while popup is showing, dismiss popup
        if (Input.GetKeyDown(KeyCode.Tab) && popupShowing && !slidingOut)
        {
            StartCoroutine(DismissPopup());
        }

        // If user presses Shift while menu showing, toggle coordinates on and off
        if (Input.GetKeyDown(KeyCode.LeftShift) && popupShowing && !slidingOut)
        {
            StartCoroutine(ToggleCoordinates());
        }
    }

    IEnumerator ShowPopup()
    {
        popup.SetActive(true);
        slidingOut = true;
        popupAnimation.Play("Exit Game Popup Slide In", -1, 0f);
        popupShowing = true;

        yield return new WaitForSeconds(1);

        slidingOut = false;     
    }

    IEnumerator DismissPopup()
    {
        slidingOut = true;
        popupShowing = false;
        popupAnimation.Play("Exit Game Popup Slide Out", -1, 0f);
        yield return new WaitForSeconds(1);
        popup.SetActive(false);
        slidingOut = false;
    }

    IEnumerator ToggleCoordinates()
    {
        if (coordinatesShowing)
        {
            coordinatesShowing = false;
            xAxis.CrossFadeAlpha(0, 1, true);
            yAxis.CrossFadeAlpha(0, 1, true);
            yield return new WaitForSeconds(1);
        }

        else
        {
            coordinatesShowing = true;
            xAxis.CrossFadeAlpha(1, 1, true);
            yAxis.CrossFadeAlpha(1, 1, true);
            yield return new WaitForSeconds(1);
        }
    }
}
