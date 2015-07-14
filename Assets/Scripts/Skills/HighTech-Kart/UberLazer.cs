using UnityEngine;
using System.Collections;

public class UberLazer : MonoBehaviour 
{
    LineRenderer lazerLineRenderer;
    internal Vector3 startPoint;
    internal Vector3 endPoint;

    void Start()
    {
        lazerLineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        GetComponent<NetworkView>().RPC("UpdateLazer", RPCMode.All, startPoint, endPoint);

        lazerLineRenderer.SetPosition(0, startPoint);
        lazerLineRenderer.SetPosition(1, endPoint);
    }

    [RPC]
    internal void UpdateLazer(Vector3 startPoint, Vector3 endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }
}
