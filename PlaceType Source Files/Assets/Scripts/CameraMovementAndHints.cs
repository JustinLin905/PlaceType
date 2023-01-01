using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraMovementAndHints : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetPosition;

    private bool drag = false;
    private bool cameraResetting = false;

    private float zoom = 1075f;

    // Indicator which appears when user pans far away from Canvas
    public TextMeshProUGUI pressFHint;
    private bool hintShowing;

    // Help hint that shows when you first enter the game
    public TextMeshProUGUI firstHint;

    // White screen which fades out when game starts
    public Image whiteScreen;


    private void Start()
    {
        ResetPosition = Camera.main.transform.position;
        pressFHint.CrossFadeAlpha(0, 0, true);
        hintShowing = false;

        StartCoroutine(FadeOutWhiteScreen());
        StartCoroutine(FadeFirstHint());
    }

    private void Update()
    {
        if (cameraResetting)
        {
            // Lerp camera to Reset Position
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, ResetPosition, Time.deltaTime * 5);

            // Hide hint
            pressFHint.CrossFadeAlpha(0, 0.5f, true);
            hintShowing = false;

            StartCoroutine(ResetTimer());
        }

        OnMouseScroll();

        
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (drag == false)
            {
                drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

        }
        
        else
        {
            drag = false;
        }

        if (drag && HasMouseMoved())
        {
            Camera.main.transform.position = Origin - Difference * 1f;

            // If user is far away from Canvas, show hint
            if (Vector3.Distance(Camera.main.transform.position, ResetPosition) > 3000)
            {
                if (!hintShowing)
                {
                    pressFHint.CrossFadeAlpha(1, 0.5f, true);
                    hintShowing = true;
                }
            }
            else
            {
                if (hintShowing)
                {
                    pressFHint.CrossFadeAlpha(0, 0.5f, true);
                    hintShowing = false;
                }
            }
        }

        // Press button to reset camera to default position
        if (Input.GetKeyDown(KeyCode.F) && !cameraResetting)
        {
            cameraResetting = true;
        }
        

    }

    // Function to handle zooming using scroll wheel
    private void OnMouseScroll()
    {
        float zoomChangeAmount = 80f;

        if (Input.mouseScrollDelta.y > 0 && zoom > 100)
        {
            zoom -= zoomChangeAmount * Time.deltaTime * 250f;

            if (zoom < 100)
            {
                zoom = 100;
            }
        }
        else if (Input.mouseScrollDelta.y < 0 && zoom < 2000)
        {
            zoom += zoomChangeAmount * Time.deltaTime * 250f;
        }

        // Set camera size to zoom
        Camera.main.orthographicSize = zoom;

        // NEED TO ADD CLAMPING
    }

    bool HasMouseMoved()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }

    IEnumerator ResetTimer()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1);

        //Camera.main.transform.position = ResetPosition;

        cameraResetting = false;
    }

    IEnumerator FadeOutWhiteScreen()
    {
        whiteScreen.gameObject.SetActive(true);
        whiteScreen.CrossFadeAlpha(0, 1f, true);

        yield return new WaitForSeconds(1f);

        whiteScreen.gameObject.SetActive(false);
    }

    IEnumerator FadeFirstHint()
    {
        yield return new WaitForSeconds(15f);
        firstHint.CrossFadeAlpha(0, 2, true);
    }

}
