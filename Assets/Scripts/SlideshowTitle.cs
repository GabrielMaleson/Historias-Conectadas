using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlideshowController : MonoBehaviour
{
    public Sprite[] slides;
    public Image slideImage;
    public float transitionInterval = 4f; // Time between slides
    public float fadeDuration = 1f; // Duration of fade effect

    private int currentSlideIndex = 0;
    private bool isTransitioning = false;

    void Awake()
    {
        // Set initial slide
        if (slides.Length > 0)
        {
            slideImage.sprite = slides[0];
            slideImage.color = new Color(1, 1, 1, 1); // Ensure it's fully visible
        }

        ResetGame();
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Start automatic slideshow
        StartCoroutine(AutoSlideshow());
    }

    private IEnumerator AutoSlideshow()
    {
        while (true)
        {
            yield return new WaitForSeconds(transitionInterval);

            if (!isTransitioning && slides.Length > 1)
            {
                yield return StartCoroutine(TransitionToNextSlide());
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGame();
    }


    private IEnumerator TransitionToNextSlide()
    {
        isTransitioning = true;

        // Calculate next slide index
        int nextSlideIndex = (currentSlideIndex + 1) % slides.Length;

        // Create a temporary object for the new slide
        GameObject tempImageObj = new GameObject("TempSlide");
        Image tempImage = tempImageObj.AddComponent<Image>();
        tempImage.sprite = slides[nextSlideIndex];
        tempImage.color = new Color(1, 1, 1, 0); // Start transparent

        // Set up RectTransform to match the original image
        RectTransform tempRect = tempImage.GetComponent<RectTransform>();
        RectTransform originalRect = slideImage.GetComponent<RectTransform>();

        // Copy all important RectTransform properties
        tempRect.SetParent(originalRect.parent, false);
        tempRect.anchorMin = originalRect.anchorMin;
        tempRect.anchorMax = originalRect.anchorMax;
        tempRect.pivot = originalRect.pivot;
        tempRect.anchoredPosition = originalRect.anchoredPosition;
        tempRect.sizeDelta = originalRect.sizeDelta;
        tempRect.localScale = originalRect.localScale;
        tempRect.localRotation = originalRect.localRotation;

        tempRect.SetAsLastSibling(); // Make sure it's on top

        // Fade in the new image
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            tempImage.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Complete the transition
        tempImage.color = Color.white;
        slideImage.sprite = slides[nextSlideIndex];
        currentSlideIndex = nextSlideIndex;

        // Clean up
        Destroy(tempImageObj);
        isTransitioning = false;
    }

    // Optional: Manual controls if needed
    public void NextSlide()
    {
        if (!isTransitioning && slides.Length > 1)
        {
            StartCoroutine(TransitionToNextSlide());
        }
    }

    public void PreviousSlide()
    {
        if (!isTransitioning && slides.Length > 1)
        {
            StartCoroutine(TransitionToPreviousSlide());
        }
    }

    private IEnumerator TransitionToPreviousSlide()
    {
        isTransitioning = true;

        // Calculate previous slide index
        int prevSlideIndex = currentSlideIndex - 1;
        if (prevSlideIndex < 0) prevSlideIndex = slides.Length - 1;

        // Create temporary object for the new slide
        GameObject tempImageObj = new GameObject("TempSlide");
        Image tempImage = tempImageObj.AddComponent<Image>();
        tempImage.sprite = slides[prevSlideIndex];
        tempImage.color = new Color(1, 1, 1, 0);

        // Set up RectTransform to match the original image
        RectTransform tempRect = tempImage.GetComponent<RectTransform>();
        RectTransform originalRect = slideImage.GetComponent<RectTransform>();

        // Copy all important RectTransform properties
        tempRect.SetParent(originalRect.parent, false);
        tempRect.anchorMin = originalRect.anchorMin;
        tempRect.anchorMax = originalRect.anchorMax;
        tempRect.pivot = originalRect.pivot;
        tempRect.anchoredPosition = originalRect.anchoredPosition;
        tempRect.sizeDelta = originalRect.sizeDelta;
        tempRect.localScale = originalRect.localScale;
        tempRect.localRotation = originalRect.localRotation;

        tempRect.SetAsLastSibling();

        // Fade in the new image
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            tempImage.color = new Color(1, 1, 1, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Complete the transition
        tempImage.color = Color.white;
        slideImage.sprite = slides[prevSlideIndex];
        currentSlideIndex = prevSlideIndex;

        // Clean up
        Destroy(tempImageObj);
        isTransitioning = false;
    }

    public void ResetGame()
    {
        // 2. Destroy all Don'tDestroyOnLoad objects
        DestroyAllDontDestroyOnLoadObjects();

        // 3. Reset any other game state if needed
        ResetGameState();

        Debug.Log("Game completely reset - Yarn variables cleared and DontDestroyOnLoad objects removed");
    }

    private void DestroyAllDontDestroyOnLoadObjects()
    {
        // Create a temporary GameObject that will be destroyed immediately
        GameObject tempObject = new GameObject("TempDestroyer");
        DontDestroyOnLoad(tempObject);

        // Get the root of the DontDestroyOnLoad scene
        Scene dontDestroyOnLoadScene = tempObject.scene;

        // Get all root objects in the DontDestroyOnLoad scene
        List<GameObject> rootObjects = new List<GameObject>();
        dontDestroyOnLoadScene.GetRootGameObjects(rootObjects);

        // Destroy all objects except our temporary one and the ones we want to keep
        foreach (GameObject obj in rootObjects)
        {
            if (obj != tempObject && obj != gameObject && ShouldDestroyObject(obj))
            {
                Destroy(obj);
                Debug.Log("Destroyed DontDestroyOnLoad object: " + obj.name);
            }
        }

        // Destroy the temporary object
        Destroy(tempObject);
    }

    private bool ShouldDestroyObject(GameObject obj)
    {
        // Add any objects you want to preserve here
        // For example, if you want to keep audio managers or other essential systems:
        // return !obj.CompareTag("EssentialSystem");

        return true; // Destroy all by default
    }

    private void ResetGameState()
    {
        // Add any additional game state reset logic here
        // For example:
        // - Reset player preferences
        // - Clear inventory
        // - Reset quest system
        // - etc.

        // Example: Reset time scale
        Time.timeScale = 1f;

        // Example: Clear all player prefs (use with caution!)
        // PlayerPrefs.DeleteAll();
    }

}