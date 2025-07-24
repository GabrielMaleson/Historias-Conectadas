using UnityEngine;
using Yarn.Unity;
public class FadeCamera: MonoBehaviour 
{

    [YarnCommand("fade_camera")]
    public static void FadesCamera()
    {
        Debug.Log("Fading the camera!");
    }
}