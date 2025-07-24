using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    public bool DoorOpened = false; 
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterDoor()
    {
        if (DoorOpened)
            SceneManager.LoadScene("TestRoom");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        InventoryItem item = collision.GetComponent<InventoryItem>();

        if (item != null && item.itemTags.Contains("Door Key"))
        {
            if (!DoorOpened)
            {
                DoorOpened = true;
                Debug.Log("Door opened with key!");
            }
        }
    }
}
