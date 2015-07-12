using UnityEngine;
using UnityEngine.UI;

public class MenuLoadingScreen : MonoBehaviour
{
    [SerializeField]
    internal Slider progressBar;
    [SerializeField]
    internal Text waitingForOthersText;
    internal bool allFinished;

    void Start()
    {
        OnEnable();
    }

    void OnEnable()
    {
        waitingForOthersText.text = "";
        if (GetComponent<MenuLobby>().kartPreview != null)
            Destroy(GetComponent<MenuLobby>().kartPreview);
    }

    public void SetProgressBar(AsyncOperation asyncOp)
    {
        progressBar.value = asyncOp.progress;
        if (progressBar.value >= 0.9)
            waitingForOthersText.text = "WAITING FOR OTHER PLAYERS";
    }

    void OnDisable()
    {
        GetComponent<MainMenuHandler>().loadingScreenCanvas.enabled = false;
    }
}
