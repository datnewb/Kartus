using UnityEngine;
using System.Collections.Generic;

public enum Kart
{
    Napalm
}

public enum Gender
{
    Male,
    Female
}


public class CharacterList : MonoBehaviour
{
    public List<GameObject> drivers;
    public List<GameObject> kartObjects;
}
