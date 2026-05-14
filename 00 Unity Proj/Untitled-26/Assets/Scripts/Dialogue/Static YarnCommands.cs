using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class Quit
{
    [YarnCommand("quitGame")]
    public static void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
        //Just to make sure its working
    }
}

public class Music
{
    [YarnCommand("musicOn")]
    public static void MusicOn(AudioSource music)
    {
        music.Play();
        Debug.Log("Playing " + music.name);
    }

    [YarnCommand("musicOff")]
    public static void MusicOff(AudioSource music)
    {
        music.Stop();
        Debug.Log("Stopping " + music.name);
    }
}

public class Location
{
    static Image currentLocation;

    [YarnCommand("Swap")]
    public static void Swap(GameObject Location)
    {
        Debug.Log("The current Location is " + currentLocation);
        if (Location != null)
        {
            Image locationSprite = Location.GetComponent<Image>();
            Image lastLocation;


            lastLocation = currentLocation;
            currentLocation = locationSprite;
            currentLocation.enabled = true;
            Debug.Log(Location + " is now active");
            if (lastLocation != null)
            {
                lastLocation.enabled = false;
                Debug.Log(lastLocation + " is no longer active");
            }
        }
        else Debug.LogWarning("GameObject not found for command 'Swap'.");
    }
    [YarnCommand("StopCutscene")]
    public static void StopCutscene()
    {
        Debug.Log("The current cutscene is " + currentLocation);
        if (currentLocation != null)
        {
            Image locationSprite = currentLocation.GetComponent<Image>();
            locationSprite.enabled = false;
            Debug.Log(locationSprite + " is no longer active");
            currentLocation = null;
        }
    }
}

public class Animation
{
    static SpriteRenderer currentEmotion;
    static Animator currentAnimation;

    [YarnCommand("Animate")]
    public static void Animate(GameObject Emotion)
    {
        Debug.Log("The current animation is " + currentEmotion);
        if (Emotion != null)
        {
            SpriteRenderer emotionSprite = Emotion.GetComponent<SpriteRenderer>();
            //Animator emotionAnimation = Emotion.GetComponent<Animator>();
            SpriteRenderer lastEmotion;
            //Animator lastAnimation;


            lastEmotion = currentEmotion;
            //lastAnimation = currentAnimation;
            currentEmotion = emotionSprite;
            //currentAnimation = emotionAnimation;
            currentEmotion.enabled = true;
            //currentAnimation.enabled = true;
            Debug.Log(Emotion + " is now active");
            if (lastEmotion != null)
            {
                lastEmotion.enabled = false;
                //lastAnimation.enabled = false;
                Debug.Log(lastEmotion + " is no longer active");
            }
        }
        else Debug.LogWarning("GameObject not found for command 'Animate'.");
    }

    [YarnCommand("StopAnimation")]
    public static void StopAnimation()
    {
        Debug.Log("The current animation is " + currentEmotion);
        if (currentEmotion != null)
        {
            SpriteRenderer emotionSprite = currentEmotion.GetComponent<SpriteRenderer>();
            //Animator emotionAnimation = currentEmotion.GetComponent<Animator>();
            emotionSprite.enabled = false;
            //emotionAnimation.enabled = false;
            Debug.Log(emotionSprite + " is no longer active");
            currentEmotion = null;
            //currentAnimation = null;
        }
    }
}

public class CollectableFlower
{
    public static event Action flowerCollected;

    [YarnCommand("FlowerCollected")]
    public static void FlowerCollected()
    {
        Debug.Log("flower Collected");
        flowerCollected?.Invoke();
    }
}