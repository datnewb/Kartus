using UnityEngine;
using System.Collections.Generic;

public enum KartEnum
{
    Napalm,
    Adventurer,
    Genocide,
    HighTech
}

public enum Gender
{
    Male,
    Female
}

public class CharacterList : MonoBehaviour
{
    public List<GameObject> drivers;
    public List<Kart> karts;
}

[System.Serializable]
public class Kart
{
    public KartEnum kartEnumValue;
    public List<GameObject> variations;

    public string kartName;
    [TextArea(3, 5)]
    public string kartDescription;
}
