using UnityEngine;
using System.Collections.Generic;

public class PowerUpDropper : MonoBehaviour 
{
    public List<GameObject> possibleDrops;
    public int percentChanceNoDrop;

    public void DropPowerUp()
    {
        int chance = Random.Range(0, 100);
        if (chance > percentChanceNoDrop)
        {
            for (int currentChance = percentChanceNoDrop, currentPowerUp = 0; 
                currentChance <= 100 && currentPowerUp < possibleDrops.Count; 
                currentChance += (100 - percentChanceNoDrop) / possibleDrops.Count, currentPowerUp++)
            {
                if (chance <= currentChance)
                {
                    Network.Instantiate(possibleDrops[currentPowerUp], transform.position, Quaternion.identity, 0);
                    break;
                }
            }
        }
    }
}
