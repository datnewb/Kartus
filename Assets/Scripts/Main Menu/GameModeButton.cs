using UnityEngine;
using UnityEngine.UI;

public class GameModeButton : MonoBehaviour
{
    [SerializeField]
    internal string title;
    [SerializeField]
    [TextArea(3, 10)]
    internal string description;

    private MenuCreateGame createGameMenu;

    void Start()
    {
        createGameMenu = FindObjectOfType<MenuCreateGame>();
        GetComponent<Button>().onClick.AddListener(() => { SetGameMode(); });
    }

    public void SetGameMode()
    {
        createGameMenu.gameModeTitle.text = title;
        createGameMenu.gameModeDesc.text = description;

        createGameMenu.selectedGameMode = this;
    }
}
