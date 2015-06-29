using UnityEngine;
using UnityEngine.UI;

public class UICharacterStats : MonoBehaviour 
{
    [SerializeField]
    private Slider shieldBar;
    [SerializeField]
    private Text shieldValueText;

    [SerializeField]
    private Slider healthBar;
    [SerializeField]
    private Text healthValueText;

    [SerializeField]
    private Slider ammoBar;
    [SerializeField]
    private Text ammoValueText;

    PlayerHandler playerHandler;
    CharacterShield shield;
    CharacterHealth health;
    CharacterAmmo ammo;

    void Start()
    {
        LookForPlayerHandler();
    }

    void Update()
    {
        if (playerHandler == null)
            LookForPlayerHandler();
        GetStats();
        SetValues();
    }

    private void LookForPlayerHandler()
    {
        foreach (PlayerHandler playerHandlerInstance in FindObjectsOfType<PlayerHandler>())
        {
            if (playerHandlerInstance.GetComponent<NetworkView>().isMine)
            {
                playerHandler = playerHandlerInstance;
                break;
            }
        }
    }

    private void GetStats()
    {
        if (playerHandler.kartInstance != null)
        {
            shield = playerHandler.kartInstance.GetComponent<CharacterShield>();
            health = playerHandler.kartInstance.GetComponent<CharacterHealth>();
            ammo = playerHandler.kartInstance.GetComponent<CharacterAmmo>();
        }
        else
        {
            shield = playerHandler.kart.GetComponent<CharacterShield>();
            health = playerHandler.kart.GetComponent<CharacterHealth>();
            ammo = playerHandler.kart.GetComponent<CharacterAmmo>();
        }
    }

    private void SetValues()
    {
        shieldBar.value = shield.currentShield / shield.maxShield;
        shieldValueText.text = Mathf.RoundToInt(shield.currentShield) + " / " + shield.maxShield;

        healthBar.value = health.currentHealth / health.maxHealth;
        healthValueText.text = Mathf.RoundToInt(health.currentHealth) + " / " + health.maxHealth;

        ammoBar.value = ammo.currentAmmo / ammo.maxAmmo;
        ammoValueText.text = Mathf.RoundToInt(ammo.currentAmmo) + " / " + ammo.maxAmmo;
    }
}
