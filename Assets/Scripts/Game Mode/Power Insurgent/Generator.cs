﻿using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour 
{
    IEnumerator towerSearchCoroutine;

    void Start()
    {
        towerSearchCoroutine = TowerSearch();
        StartCoroutine(towerSearchCoroutine);

        GetComponent<CharacterHealth>().isInvulnerable = true;
    }

    IEnumerator TowerSearch()
    {
        while (true)
        {
            int towersInTeam = 0;
            foreach (Tower tower in FindObjectsOfType<Tower>())
            {
                if (tower.GetComponent<CharacterTeam>().team == GetComponent<CharacterTeam>().team)
                    towersInTeam++;
            }
            if (towersInTeam <= 0)
            {
                GetComponent<CharacterHealth>().isInvulnerable = false;
                StopCoroutine(towerSearchCoroutine);
            }
            yield return null;
        }
    }
}
