using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;

public class StaticImageTagManager : MonoBehaviour
{
    [Tooltip("List of Sprites to use for images")]
    public List<Sprite> sprites = new List<Sprite>();

    [Tooltip("List of tags corresponding to each sprite")]
    public List<string> spriteTags = new List<string>();

    [Tooltip("Prefab for the UI Image to instantiate")]
    public Image imagePrefab;

    private static StaticImageTagManager instance;
    private static Dictionary<string, Image> activeImages = new Dictionary<string, Image>();

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
        while (sprites.Count > spriteTags.Count)
        {
            spriteTags.Add("");
        }

        while (spriteTags.Count > sprites.Count)
        {
            spriteTags.RemoveAt(spriteTags.Count - 1);
        }
    }

    [YarnCommand("sprite")]
    public static void ShowImage(string spriteTag, string positionName)
    {
        if (instance == null)
        {
            Debug.LogError("No StaticImageTagManager instance found in scene!");
            return;
        }

        // First remove any existing image at this position
        RemoveImage(positionName);

        // Find the sprite by tag
        Sprite foundSprite = null;
        for (int i = 0; i < instance.spriteTags.Count; i++)
        {
            if (instance.spriteTags[i] == spriteTag)
            {
                foundSprite = instance.sprites[i];
                break;
            }
        }

        if (foundSprite == null)
        {
            Debug.LogError($"No sprite found with tag: {spriteTag}");
            return;
        }

        // Find the position GameObject
        GameObject positionObj = GameObject.Find(positionName);
        if (positionObj == null)
        {
            Debug.LogError($"No GameObject found with name: {positionName}");
            return;
        }

        // Create a new UI Image at the target position
        Image newImage = Instantiate(instance.imagePrefab, positionObj.transform);
        newImage.sprite = foundSprite;
        newImage.transform.localPosition = Vector3.zero;
        newImage.gameObject.SetActive(true);

        // Track this active image
        activeImages[positionName] = newImage;
    }

    [YarnCommand("removesprite")]
    public static void RemoveImage(string positionName)
    {
        if (activeImages.TryGetValue(positionName, out Image existingImage))
        {
            Destroy(existingImage.gameObject);
            activeImages.Remove(positionName);
        }
    }
}