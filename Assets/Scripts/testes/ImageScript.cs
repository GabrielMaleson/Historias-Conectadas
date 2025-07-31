using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class TaggedImage
{
    public Image image;
    public string imageTag;
}

public class ImageTagManager : MonoBehaviour
{
    public List<TaggedImage> taggedImages = new List<TaggedImage>();

    // Optional: Add a method to find an image by its tag
    public Image GetImageByTag(string tag)
    {
        foreach (var taggedImage in taggedImages)
        {
            if (taggedImage.imageTag == tag)
            {
                return taggedImage.image;
            }
        }
        return null;
    }
}