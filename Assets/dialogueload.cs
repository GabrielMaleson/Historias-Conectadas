using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogueload : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
