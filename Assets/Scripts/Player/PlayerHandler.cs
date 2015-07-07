﻿using UnityEngine;
using System.Collections.Generic;

public class PlayerHandler : MonoBehaviour
{
    internal PlayerInfo playerInfo;
    internal SpawnPoint spawnPoint;

    public GameObject kart;
    public GameObject driver;
    internal GameObject kartInstance;

    internal int kills;
    internal int deaths;

    void Start()
    {
        foreach (SpawnPoint spawnPoint in FindObjectsOfType<SpawnPoint>())
        {
            if (spawnPoint.position == playerInfo.position)
            {
                this.spawnPoint = spawnPoint;
                spawnPoint.Assign();
                break;
            }
        }
    }

    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            SpawnKart();
            if (!spawnPoint.IsAssigned())
                Destroy(this);
        }
    }

    private void SpawnKart()
    {
        if (kartInstance == null && kart != null)
        {
            kartInstance = Network.Instantiate(kart, spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as GameObject;
            kartInstance.GetComponent<CharacterTeam>().team = spawnPoint.GetComponent<CharacterTeam>().team;
            kartInstance.GetComponent<Driver>().driverInstance = Instantiate(driver, kartInstance.transform.position, kartInstance.transform.rotation) as GameObject;
            kartInstance.GetComponent<Driver>().driverSeat = kartInstance.transform;
        }
    }

    void OnDisconnectedFromServer()
    {
        if (kartInstance != null)
            Network.Destroy(kartInstance);

        spawnPoint.Unassign();
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int net_kills = 0;
        int net_deaths = 0;
        if (stream.isWriting)
        {
            net_kills = kills;
            net_deaths = deaths;

            stream.Serialize(ref net_kills);
            stream.Serialize(ref net_deaths);
        }
        else if (stream.isReading)
        {
            stream.Serialize(ref net_kills);
            stream.Serialize(ref net_deaths);

            kills = net_kills;
            deaths = net_deaths;
        }
    }
}
