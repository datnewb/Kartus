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

    [SerializeField]
    private Text killsText;
    [SerializeField]
    private Text deathsText;

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
        if (playerHandler != null)
        {
            if (playerHandler.kartInstance != null)
            {
                shield = playerHandler.kartInstance.GetComponent<CharacterShield>();
                health = playerHandler.kartInstance.GetComponent<CharacterHealth>();
                ammo = playerHandler.kartInstance.GetComponent<CharacterAmmo>();
            }
            killsText.text = "K : " + playerHandler.kills;
            deathsText.text = "D : " + playerHandler.deaths;
        }
        else
            LookForPlayerHandler();
    }

    private void SetValues()
    {
        if (shield != null)
        {
            shieldBar.transform.parent.gameObject.SetActive(true);
            shieldBar.gameObject.SetActive(true);
            shieldValueText.gameObject.SetActive(true);

            shieldBar.value = shield.currentShield / shield.maxShield;
            shieldValueText.text = Mathf.RoundToInt(shield.currentShield) + " / " + shield.maxShield;
        }
        else
        {
            shieldBar.transform.parent.gameObject.SetActive(false);
            shieldBar.gameObject.SetActive(false);
            shieldValueText.gameObject.SetActive(false);
        }

        if (health != null)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
            healthBar.gameObject.SetActive(true);
            healthValueText.gameObject.SetActive(true);

            healthBar.value = health.currentHealth / health.maxHealth;
            healthValueText.text = Mathf.RoundToInt(health.currentHealth) + " / " + health.maxHealth;
        }
        else
        {
            healthBar.transform.parent.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(false);
            healthValueText.gameObject.SetActive(false);
        }

        if (ammo != null)
        {
            ammoBar.transform.parent.gameObject.SetActive(true);
            ammoBar.gameObject.SetActive(true);
            ammoValueText.gameObject.SetActive(true);

            ammoBar.value = ammo.currentAmmo / ammo.maxAmmo;
            ammoValueText.text = Mathf.RoundToInt(ammo.currentAmmo) + " / " + ammo.maxAmmo;
        }
        else
        {
            ammoBar.transform.parent.gameObject.SetActive(false);
            ammoBar.gameObject.SetActive(false);
            ammoValueText.gameObject.SetActive(false);
        }
    }
}
