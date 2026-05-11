using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResetPuzzle : MonoBehaviour
{

    public static event Action resetPuzzle;

    public static bool fadePuzzleTransition = false;
    public Image resetCanvasColor;
    public float fadeInFadeOutInBetween = 0.5f;
    public float currentFadeInFadeOutTime = 0f;

    public static void OnReset()
    {
        Debug.Log("Resetting Puzzle");
        fadePuzzleTransition = true;
    }

    private void FixedUpdate()
    {

        if (fadePuzzleTransition)
        {
            //transitionCanvasColor = GameObject.Find("TransitionCanvas").GetComponent<Image>();
            resetCanvasColor.gameObject.SetActive(true);

            if (resetCanvasColor != null && resetCanvasColor.color.a < 1)
            {
                Color newColor = resetCanvasColor.color;

                newColor.a = newColor.a + Time.deltaTime;

                resetCanvasColor.color = newColor; //fade in over time

            }
            else
            {
                //timer in between fade in and out to give some visual rest
                if (currentFadeInFadeOutTime < fadeInFadeOutInBetween)
                {
                    currentFadeInFadeOutTime += Time.deltaTime;
                }
                else
                {
                    fadePuzzleTransition = false;
                    // Notify GameStateManager that we've detected a puzzle switch.
                    resetPuzzle.Invoke();
                }

            }

        }
        else
        {

            if (resetCanvasColor != null && resetCanvasColor.color.a >= 0)
            {

                Color newColor = resetCanvasColor.color;

                newColor.a = newColor.a - Time.deltaTime;

                resetCanvasColor.color = newColor; //fade out over time

            }
            else if (resetCanvasColor != null && resetCanvasColor.gameObject.activeInHierarchy && resetCanvasColor.color.a <= 0)
            {
                resetCanvasColor.gameObject.SetActive(false);
                currentFadeInFadeOutTime = 0;
            }
        }

    }

}
