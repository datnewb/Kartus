using UnityEngine;
using System.Collections;

public class TransparentEffect : MonoBehaviour 
{
    [SerializeField]
    private float transparency;

    void Start()
    {
        SetTransparency(transparency);
    }

    internal void SetTransparency(float transparency)
    {
        this.transparency = transparency;
        if (transparency < 1 && transparency >= 0)
        {
            foreach (Renderer meshRenderer in GetComponentsInChildren<Renderer>())
            {
                meshRenderer.material.SetFloat("_Mode", 2);
                meshRenderer.material.SetInt("_ZWrite", 0);
                meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
                meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                meshRenderer.material.SetColor("_Color", new Color(1, 1, 1, transparency));
            }
        }
        else
        {
            DisableTransparency();
        }
    }

    void OnDestroy()
    {
        DisableTransparency();
    }

    private void DisableTransparency()
    {
        foreach (Renderer meshRenderer in GetComponentsInChildren<Renderer>())
        {
            meshRenderer.material.SetFloat("_Mode", 0);
            meshRenderer.material.SetInt("_ZWrite", 1);
            meshRenderer.material.SetInt("_SrcBlend", 1);
            meshRenderer.material.SetInt("_DstBlend", 0);
            meshRenderer.material.EnableKeyword("_ALPHATEST_ON");
            meshRenderer.material.DisableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.SetColor("_Color", new Color(1, 1, 1, 1));
        }
    }
}
