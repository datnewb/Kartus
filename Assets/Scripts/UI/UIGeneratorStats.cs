using UnityEngine;
using UnityEngine.UI;

public class UIGeneratorStats : MonoBehaviour 
{
    [SerializeField]
    private Canvas powerInsurgentCanvas;

    [SerializeField]
    private Slider speedsterGeneratorHealthBar;
    [SerializeField]
    private Slider roadkillGeneratorHealthBar;

    void Update()
    {
        if (FindObjectOfType<GameManager>() != null)
        {
            if (FindObjectOfType<GameManager>().gameStarted)
                UpdateGeneratorHealthBar();
            else
                powerInsurgentCanvas.enabled = false;
        }
    }

    private void UpdateGeneratorHealthBar()
    {
        powerInsurgentCanvas.enabled = true;

        if (FindObjectsOfType<Generator>() != null &&
            FindObjectsOfType<Generator>().Length > 1)
        {
            foreach (Generator generator in FindObjectsOfType<Generator>())
            {
                if (generator.GetComponent<CharacterTeam>().team == Team.Speedster)
                    speedsterGeneratorHealthBar.value = generator.GetComponent<CharacterHealth>().currentHealth / generator.GetComponent<CharacterHealth>().maxHealth;
                else if (generator.GetComponent<CharacterTeam>().team == Team.Roadkill)
                    roadkillGeneratorHealthBar.value = generator.GetComponent<CharacterHealth>().currentHealth / generator.GetComponent<CharacterHealth>().maxHealth;
            }
        }
        else
        {
            speedsterGeneratorHealthBar.transform.parent.gameObject.SetActive(false);
            speedsterGeneratorHealthBar.gameObject.SetActive(false);
            roadkillGeneratorHealthBar.transform.parent.gameObject.SetActive(false);
            roadkillGeneratorHealthBar.gameObject.SetActive(false);
        }
    }
}
