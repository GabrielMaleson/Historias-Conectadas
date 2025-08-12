using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;
using TMPro; // Add this namespace for TextMeshPro

public class StaticImageTagManager : MonoBehaviour
{
    [System.Serializable]
    public class PositionData
    {
        public string positionName;
        public Transform positionTransform;
    }

    [Tooltip("TextMeshProUGUI component for objectives")]
    private TextMeshProUGUI ObjectiveObj;

    public GameObject objective;

    [Tooltip("List of Sprites to use for images")]
    public List<Sprite> sprites = new List<Sprite>();

    [Tooltip("List of tags corresponding to each sprite")]
    public List<string> spriteTags = new List<string>();

    [Tooltip("Prefab for the UI Image to instantiate")]
    public Image imagePrefab;

    [Tooltip("List of positions where images can be placed")]
    public List<PositionData> positions = new List<PositionData>();

    private static StaticImageTagManager instance;
    private static Dictionary<string, Image> activeImages = new Dictionary<string, Image>();

    private void Awake()
    {
        objective = GameObject.FindGameObjectWithTag("Objective");
        ObjectiveObj = objective.GetComponent<TextMeshProUGUI>();
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
        // Keep sprite tags and sprites lists synchronized
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

        // Find the position in our list
        Transform positionTransform = null;
        foreach (var positionData in instance.positions)
        {
            if (positionData.positionName == positionName)
            {
                positionTransform = positionData.positionTransform;
                break;
            }
        }

        if (positionTransform == null)
        {
            Debug.LogError($"No position found with name: {positionName}");
            return;
        }

        // Create a new UI Image at the target position
        Image newImage = Instantiate(instance.imagePrefab, positionTransform);
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

    [YarnCommand("objective")]
    public static void ObjectiveUpdate(string objectivetext)
    {
        if (instance.ObjectiveObj == null)
        {
            Debug.LogError("StaticImageTagManager instance or ObjectiveObj not found!");
            return;
        }
        instance.ObjectiveObj.text = objectivetext;
    }
}