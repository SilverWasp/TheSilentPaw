using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PickUpMessage : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Sc_GameManager.Instance.HasMessage = true;
            Destroy(gameObject);
        }
    }
}
