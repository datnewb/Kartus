using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour 
{
    public float existTime;

    internal virtual void Start()
    {
        Invoke("DestroyPowerUp", existTime);
    }

    internal virtual void Effect(GameObject target)
    {
        DestroyPowerUp();
    }

    internal void DestroyPowerUp()
    {
        Network.Destroy(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        Effect(other.transform.root.gameObject);
    }
}
