using UnityEngine;
using UnityEngine.UI;

public class UIGeneratorStats : MonoBehaviour 
{
    [SerializeField]
    private Slider speedsterGeneratorHealthBar;
    [SerializeField]
    private Slider roadkillGeneratorHealthBar;

    void Update()
    {
        UpdateGeneratorHealthBar();
    }

    private void UpdateGeneratorHealthBar()
    {
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
