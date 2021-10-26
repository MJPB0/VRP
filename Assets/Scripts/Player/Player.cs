using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour
{
    [SerializeField] private float CurrentHealth;
    [SerializeField] private float MaxHealth;

    [SerializeField] List<UsableItem> Equipment;
    private Hand[] Hands;

    private void Start()
    {
        Equipment = new List<UsableItem>();
        Hands = GetComponentsInChildren<Hand>();
        if (Hands.Length != 2) 
            Debug.LogError("Player doesn't have two hands!");
    }
}
