using UnityEngine;

public class Checkpoint : MonoBehaviour 
{
    internal bool acquired;
    internal bool added;

    void Start()
    {
        acquired = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (gameObject.activeInHierarchy)
        {
            if (other.transform.root.gameObject.GetComponent<KartController>() != null)
            {
                acquired = true;
            }
        }
    }
}
