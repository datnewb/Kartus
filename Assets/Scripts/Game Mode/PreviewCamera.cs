using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreviewCamera : MonoBehaviour 
{
    public List<Camera> previewCameras;
    private Camera activeCamera = null;
    private int currentCameraIndex;
    private PlayerHandler playerHandler;

    IEnumerator cameraCycle;

    void Start()
    {
        currentCameraIndex = 0;
        playerHandler = null;
    }

    void OnEnable()
    {
        if (activeCamera == null)
            activeCamera = previewCameras[0];

        SetPreviewCamera();

        cameraCycle = CameraCycle();
        StartCoroutine(cameraCycle);
    }

    void OnDisable()
    {
        DisablePreviewCameras();
        StopCoroutine(cameraCycle);
    }

    void Update()
    {
        if (FindObjectOfType<UIPauseMenu>() != null &&
            !FindObjectOfType<UIPauseMenu>().inPauseMenu)
        {
            if (playerHandler == null)
            {
                if (cameraCycle == null)
                {
                    cameraCycle = CameraCycle();
                    StartCoroutine(cameraCycle);
                }

                LookForPlayerHandler();
                PreviewCameraControls();
                SetPreviewCamera();
            }
            else
            {
                if (cameraCycle != null)
                {
                    StopCoroutine(cameraCycle);
                    cameraCycle = null;
                }
                if (playerHandler.kartInstance == null)
                {
                    PreviewCameraControls();
                    SetPreviewCamera();
                }
                else
                {
                    DisablePreviewCameras();
                }
            }
        }
    }

    private void LookForPlayerHandler()
    {
        foreach (PlayerHandler playerHandlerInstance in FindObjectsOfType<PlayerHandler>())
        {
            if (playerHandlerInstance.GetComponent<NetworkView>().isMine)
            {
                playerHandler = playerHandlerInstance;
                break;
            }
        }
    }

    private void PreviewCameraControls()
    {
        if (Input.GetMouseButtonDown(0))
            NextCamera();
        else if (Input.GetMouseButtonDown(1))
            PreviousCamera();
    }

    private void SetPreviewCamera()
    {
        activeCamera = previewCameras[currentCameraIndex];
        foreach (Camera previewCamera in previewCameras)
        {
            if (previewCamera == activeCamera)
                previewCamera.gameObject.SetActive(true);
            else
                previewCamera.gameObject.SetActive(false);
        }
    }

    private void DisablePreviewCameras()
    {
        foreach (Camera previewCamera in previewCameras)
            previewCamera.gameObject.SetActive(false);
    }

    private IEnumerator CameraCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(10.0f);
            NextCamera();
        }
    }

    private void NextCamera()
    {
        currentCameraIndex++;
        if (currentCameraIndex >= previewCameras.Count)
            currentCameraIndex = 0;
        SetPreviewCamera();
    }

    private void PreviousCamera()
    {
        currentCameraIndex--;
        if (currentCameraIndex < 0)
            currentCameraIndex = previewCameras.Count - 1;
        SetPreviewCamera();
    }
}
