using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public Player Player;

    [SerializeField] private Slider PlayerHealthSlider;

    private void Start()
    {
        PlayerHealthSliderSetup();
    }

    private void PlayerHealthSliderSetup()
    {
        PlayerHealthSlider.minValue = 0;
        //PlayerHealthSlider.maxValue = Player.MaxHealth;
        //PlayerHealthSlider.value = Player.CurrentHealth;
    }

    public void UpdatePlayerHealthUI()
    {
        //PlayerHealthSlider.value = Player.CurrentHealth;
    }
}
