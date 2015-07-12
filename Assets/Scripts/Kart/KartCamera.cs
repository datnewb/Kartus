using UnityEngine;

public class KartCamera : MonoBehaviour 
{
    [SerializeField]
    private Transform cameraRigRoot;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private float maxHorizontalAngle;

    private float maxCameraDistance;
    private float currentCameraDistance;
    private Vector3 cameraDirection;
    private Vector3 desiredCameraPosition;

    internal float mouseHorizontal;
    internal float mouseVertical;

    void Start()
    {
        maxCameraDistance = cameraTransform.localPosition.magnitude;
        currentCameraDistance = maxCameraDistance;
        cameraDirection = cameraTransform.localPosition.normalized;
        desiredCameraPosition = cameraRigRoot.TransformPoint(maxCameraDistance * cameraDirection);
    }

    void Update()
    {
        //camera rotation
        SetCameraRigRotation();
        //camera position
        SetCameraRigPosition();
        //fade kart if the camera is near the kart
        //or if kart is blocking camera
        FadeKart();
    }

    private void SetCameraRigRotation()
    {
        if (mouseHorizontal + AngleCorrection(cameraRigRoot.localEulerAngles.x) >= maxHorizontalAngle)
            mouseHorizontal = maxHorizontalAngle - AngleCorrection(cameraRigRoot.localEulerAngles.x);
        else if (mouseHorizontal + AngleCorrection(cameraRigRoot.localEulerAngles.x) <= -maxHorizontalAngle)
            mouseHorizontal = -maxHorizontalAngle - AngleCorrection(cameraRigRoot.localEulerAngles.x);

        cameraRigRoot.localEulerAngles = new Vector3(cameraRigRoot.localEulerAngles.x, cameraRigRoot.localEulerAngles.y, -transform.root.localEulerAngles.z);
        cameraRigRoot.localEulerAngles += new Vector3(mouseHorizontal, mouseVertical, 0);

    }

    private void SetCameraRigPosition()
    {
        desiredCameraPosition = cameraRigRoot.TransformPoint(cameraDirection * maxCameraDistance);
        RaycastHit hitInfo;
        if (Physics.Linecast(cameraRigRoot.position, desiredCameraPosition, out hitInfo, ~((1 << 8) | (1 << 9) | (1 << 11))))
        {
            currentCameraDistance = hitInfo.distance - 2.0f;
        }
        else
        {
            currentCameraDistance = maxCameraDistance;
        }
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraDirection * currentCameraDistance, 20 * Time.deltaTime);
    }

    private float AngleCorrection(float angle)
    {
        float newAngle = angle;
        while (newAngle > 180f || newAngle < -180f)
        {
            if (newAngle > 180f)
                newAngle -= 360f;
            else if (newAngle < -180f)
                newAngle += 360f;
        }
        return newAngle;
    }

    private void FadeKart()
    {
        TransparentEffect transparentEffect;
        if (GetComponents<TransparentEffect>().Length == 0)
            transparentEffect = gameObject.AddComponent<TransparentEffect>();
        else
            transparentEffect = GetComponent<TransparentEffect>();

        RaycastHit hitInfo;
        if (Physics.SphereCast(cameraTransform.position, 0.25f, cameraTransform.forward, out hitInfo))
        {
            if (hitInfo.transform.gameObject == gameObject &&
                Mathf.InverseLerp(0, 16, currentCameraDistance * currentCameraDistance) > 0.9f)
                transparentEffect.SetTransparency(0.5f);
            else
                transparentEffect.SetTransparency(Mathf.InverseLerp(0, 16, currentCameraDistance * currentCameraDistance));
        }
        else
            transparentEffect.SetTransparency(Mathf.InverseLerp(0, 16, currentCameraDistance * currentCameraDistance));
    }

    internal Transform GetCameraRigRoot()
    {
        return cameraRigRoot;
    }

    internal Camera GetCamera()
    {
        return cameraTransform.GetComponent<Camera>();
    }
}
