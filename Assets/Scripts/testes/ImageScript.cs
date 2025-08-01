using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;

public class StaticImageTagManager : MonoBehaviour
{
    [Tooltip("List of UI Images")]
    public List<Image> images = new List<Image>();

    [Tooltip("List of tags corresponding to each image")]
    public List<string> imageTags = new List<string>();

    private static StaticImageTagManager instance;
    private static Dictionary<string, Image> activeSprites = new Dictionary<string, Image>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple StaticImageTagManager instances found. Only one should exist.");
        }
    }

    private void OnValidate()
    {
        while (images.Count > imageTags.Count)
        {
            imageTags.Add("");
        }

        while (imageTags.Count > images.Count)
        {
            imageTags.RemoveAt(imageTags.Count - 1);
        }
    }

    [YarnCommand("sprite")]
    public static void PlaceSprite(string spriteTag, string positionName)
    {
        if (instance == null)
        {
            Debug.LogError("No StaticImageTagManager instance found in scene!");
            return;
        }

        // First remove any existing sprite at this position
        RemoveSprite(positionName);

        // Find the image by tag
        Image foundImage = null;
        for (int i = 0; i < instance.imageTags.Count; i++)
        {
            if (instance.imageTags[i] == spriteTag)
            {
                foundImage = instance.images[i];
                break;
            }
        }

        if (foundImage == null)
        {
            Debug.LogError($"No image found with tag: {spriteTag}");
            return;
        }

        // Find the position GameObject
        GameObject positionObj = GameObject.Find(positionName);
        if (positionObj == null)
        {
            Debug.LogError($"No GameObject found with name: {positionName}");
            return;
        }

        // Create a new instance of the image at the target position
        Image newImage = Instantiate(foundImage, positionObj.transform);
        newImage.transform.localPosition = Vector3.zero;
        newImage.gameObject.SetActive(true);

        // Track this active sprite
        activeSprites[positionName] = newImage;
    }

    [YarnCommand("removeSprite")]
    public static void RemoveSprite(string positionName)
    {
        if (activeSprites.TryGetValue(positionName, out Image existingImage))
        {
            Destroy(existingImage.gameObject);
            activeSprites.Remove(positionName);
        }
    }
}