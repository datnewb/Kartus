using UnityEngine;

public class StatEffect : MonoBehaviour
{
    public float duration;
    internal float currentDuration;
    public bool isStacking;
    internal bool isCarrier;

    internal NetworkView netView;

    internal virtual void Start()
    {
        currentDuration = duration;

        if (GetComponent<Bullet>() != null)
            isCarrier = true;
        else
            isCarrier = false;

        netView = gameObject.AddComponent<NetworkView>();
        netView.observed = this;
    }

    void Update()
    {
        if (!isCarrier)
        {
            currentDuration -= Time.deltaTime;
            if (currentDuration > 0)
                Effect();
            else
                Destroy(this);
        }
    }

    internal virtual void Effect()
    {

    }

    internal virtual void OnDestroy()
    {
        Destroy(netView);
    }

    internal void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_currentDuration = 0;

        if (stream.isWriting)
        {
            net_currentDuration = currentDuration;
            stream.Serialize(ref net_currentDuration);
        }
        else
        {
            stream.Serialize(ref net_currentDuration);
            currentDuration = net_currentDuration;
        }
    }
}
