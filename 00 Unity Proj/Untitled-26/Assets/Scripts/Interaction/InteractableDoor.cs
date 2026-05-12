using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableDoor : MonoBehaviour
{

    public Transform teleportLocation;

    public UnityEvent<Vector3> doorEntered;

    public bool fadePuzzleTransition = false;
    public Image transitionCanvasColor;
    public float fadeInFadeOutInBetween = 0.5f;
    public float currentFadeInFadeOutTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

            fadePuzzleTransition = true;
            //Player.Instance.TeleportPlayer(teleportLocation.position);
            //doorEntered.Invoke(teleportLocation.position);

        }
    }

    private void FixedUpdate()
    {
        
            if (fadePuzzleTransition)
            {

                //transitionCanvasColor = GameObject.Find("TransitionCanvas").GetComponent<Image>();
                transitionCanvasColor.gameObject.SetActive(true);

                if (transitionCanvasColor != null && transitionCanvasColor.color.a < 1)
                {

                    Color lowerAlphaColor = transitionCanvasColor.color;
                    
                    Debug.Log("PRECHANGE A: " + lowerAlphaColor.a);

                    lowerAlphaColor.a += Time.deltaTime;

                    Debug.Log("POSTCHANGE A: " + lowerAlphaColor.a);

                    transitionCanvasColor.color = lowerAlphaColor;

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
                        
                        Player.Instance.TeleportPlayer(teleportLocation.position);
                        doorEntered.Invoke(teleportLocation.position);
                    }

                }

            }
            else
            {

                if (transitionCanvasColor != null && transitionCanvasColor.color.a >= 0)
                {

                    Color newColor = transitionCanvasColor.color;

                    newColor.a = newColor.a - Time.deltaTime;

                    transitionCanvasColor.color = newColor; //fade out over time

                }
                else if (transitionCanvasColor.gameObject.activeInHierarchy && transitionCanvasColor.color.a <= 0)
                {
                    transitionCanvasColor.gameObject.SetActive(false);
                    currentFadeInFadeOutTime = 0;
                }
            }

    }

}
