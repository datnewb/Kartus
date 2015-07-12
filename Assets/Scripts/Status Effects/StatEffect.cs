using UnityEngine;

public class StatEffect : MonoBehaviour
{
    public float duration;
    internal float currentDuration;
    public bool isStacking;
    internal bool isCarrier;

    public GameObject statVisual;
    internal GameObject statVisualInstance;

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

        if (!isCarrier)
            GetComponent<NetworkView>().RPC("CreateVisuals", RPCMode.All);
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

    [RPC]
    internal void CreateVisuals()
    {
        if (statVisual != null)
        {
            statVisualInstance = Instantiate(statVisual, transform.position, transform.rotation) as GameObject;
            statVisualInstance.transform.SetParent(transform);
        }
    }

    [RPC]
    internal void DestroyVisuals()
    {
        if (statVisualInstance != null)
            Destroy(statVisualInstance);
    }

    internal virtual void Effect()
    {

    }

    internal virtual void EndEffect()
    {
        GetComponent<NetworkView>().RPC("DestroyVisuals", RPCMode.All);
        Destroy(netView);
        Destroy(this);
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
