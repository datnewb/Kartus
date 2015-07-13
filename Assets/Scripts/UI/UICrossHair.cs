using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICrossHair : MonoBehaviour 
{
    [SerializeField]
    private Image crossHairUI;

    [SerializeField]
    private Sprite defaultCrossHair;
    [SerializeField]
    private Sprite skillAimCrossHair;
    [SerializeField]
    private Sprite cantAttackCrossHair;

    private InputManager inputManager;

    void Update()
    {
        if (inputManager == null)
            inputManager = FindObjectOfType<InputManager>();
        else
        {
            if (!inputManager.allowShoot)
                crossHairUI.sprite = cantAttackCrossHair;
            else
            {
                PlayerHandler localPlayerHandler = null;
                foreach (PlayerHandler playerHandler in FindObjectsOfType<PlayerHandler>())
                {
                    if (playerHandler.GetComponent<NetworkView>().isMine)
                    {
                        localPlayerHandler = playerHandler;
                        break;
                    }
                }
                if (localPlayerHandler != null)
                {
                    if (localPlayerHandler.kartInstance != null)
                    {
                        foreach (Skill skill in localPlayerHandler.kartInstance.GetComponents<Skill>())
                        {
                            if (skill.isAiming)
                            {
                                crossHairUI.sprite = skillAimCrossHair;
                                break;
                            }
                            else
                                crossHairUI.sprite = defaultCrossHair;

                        }
                    }
                    else
                        crossHairUI.sprite = null;
                }
            }
        }
    }
}
