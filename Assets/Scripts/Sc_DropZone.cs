using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_DropZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Sc_GameManager.Instance.HasMessage)
        {
            Debug.Log("Message Delivered!");
            // Win screen or restart
        }
    }
}
