using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialShooting : MonoBehaviour 
{
    [SerializeField]
    Text tutorialText;
    [SerializeField]
    GameObject minionDummy;

    [TextArea(2, 4)]
    public string shootMessage;
    [TextArea(2, 4)]
    public string shootGalleryMessage;

    public GameObject progressBarObject;
    public Slider progressBar;
    public Text progressBarText;

    internal bool shootComplete;
    internal bool shootGalleryComplete;

    public GameObject firstTarget;

    private bool targetsInstantiated;
    private int originalTargetNumber;
    public List<Transform> targetPositions;
    public List<GameObject> targets;

    private float currentDummies;

    void Start()
    {
        shootComplete = false;
        shootGalleryComplete = false;
        targetsInstantiated = false;
        originalTargetNumber = targetPositions.Count;
        progressBarObject.SetActive(false);
    }

    void Update()
    {
        if (!shootComplete)
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            inputManager.allowDriving = false;
            inputManager.allowAiming = false;

            tutorialText.text = shootMessage;

            if (firstTarget == null)
                shootComplete = true;
        }
        else if (!shootGalleryComplete)
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            inputManager.allowAiming = true;


            tutorialText.text = shootGalleryMessage;

            if (!targetsInstantiated)
            {
                progressBarObject.SetActive(true);
                foreach (Transform trans in targetPositions)
                    targets.Add(Network.Instantiate(minionDummy, trans.position, trans.rotation, 0) as GameObject);
                targetsInstantiated = true;
            }
            else
            {
                float currentDummies = 0;
                foreach (CharacterTeam characterTeam in FindObjectsOfType<CharacterTeam>())
                {
                    if (characterTeam.team == Team.Speedster)
                        currentDummies++;
                }
                progressBar.value = (originalTargetNumber - currentDummies) / originalTargetNumber;
                progressBarText.text = "TARGETS";

                if (currentDummies <= 0)
                    shootGalleryComplete = true;
            }
        }
        else
        {
            Invoke("LoadLevel", 2.0f);
        }
    }

    private void LoadLevel()
    {
        Application.LoadLevel(0);
    }
}
