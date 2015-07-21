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
            CreateVisuals();
    }

    void Update()
    {
        if (!isCarrier)
        {
            currentDuration -= Time.deltaTime;
            if (currentDuration > 0)
                Effect();
            else
                EndEffect();
        }
    }

    internal void CreateVisuals()
    {
        if (statVisual != null)
        {
            statVisualInstance = Network.Instantiate(statVisual, transform.position, transform.rotation, 0) as GameObject;
            statVisualInstance.AddComponent<NetworkView>().observed = statVisualInstance.transform;
            statVisualInstance.transform.SetParent(transform);
        }
    }

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
        DestroyVisuals();
        Destroy(netView);
        Destroy(this);
    }

    internal void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        float net_currentDuration = 0;
        float net_duration = 0;

        if (stream.isWriting)
        {
            net_currentDuration = currentDuration;
            net_duration = duration;
            stream.Serialize(ref net_currentDuration);
            stream.Serialize(ref net_duration);
        }
        else
        {
            stream.Serialize(ref net_currentDuration);
            stream.Serialize(ref net_duration);
            currentDuration = net_currentDuration;
            duration = net_duration;
        }
    }
}
