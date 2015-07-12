using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialMovement : MonoBehaviour 
{
    [SerializeField]
    Text tutorialText;
    [SerializeField]
    Slider progressBar;
    [SerializeField]
    Text progressText;

    [TextArea(2, 4)]
    public string moveForwardMessage;
    [TextArea(2, 4)]
    public string moveBackwardMessage;
    [TextArea(2, 4)]
    public string steeringMessage;
    [TextArea(2, 4)]
    public string checkPointMessage;

    bool moveForwardComplete;
    bool moveBackwardComplete;
    bool steeringComplete;
    bool checkPointsComplete;

    private float time;
    private float currentTime;
    internal int acquiredCheckpoints;

    [SerializeField]
    List<Checkpoint> checkpoints;

    void Start()
    {
        moveForwardComplete = false;
        moveBackwardComplete = false;
        steeringComplete = false;
        checkPointsComplete = false;

        time = 1;
        currentTime = 0;
        acquiredCheckpoints = 0;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        FindObjectOfType<InputManager>().allowShoot = false;

        MovePlayerToCenter();

        if (!moveForwardComplete)
        {
            tutorialText.text = moveForwardMessage;
            progressBar.value = currentTime / time;
            progressText.text = "TIME";
            if (Input.GetAxis("Vertical") > 0)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= time)
                {
                    currentTime = 0;
                    moveForwardComplete = true;
                }
            }
        }
        else if (!moveBackwardComplete)
        {
            tutorialText.text = moveBackwardMessage;
            progressBar.value = currentTime / time;
            progressText.text = "TIME";
            if (Input.GetAxis("Vertical") < 0)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= time)
                {
                    currentTime = 0;
                    moveBackwardComplete = true;
                }
            }
        }
        else if (!steeringComplete)
        {
            tutorialText.text = steeringMessage;
            progressBar.value = currentTime / time;
            progressText.text = "TIME";
            if (Input.GetAxis("Horizontal") != 0)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= time)
                {
                    currentTime = 0;
                    steeringComplete = true;
                    checkpoints[0].gameObject.SetActive(true);
                }
            }
        }
        else if (!checkPointsComplete)
        {
            tutorialText.text = checkPointMessage;
            progressText.text = acquiredCheckpoints + "/" + checkpoints.Count;
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.acquired)
                {
                    if (!checkpoint.added)
                    {
                        acquiredCheckpoints++;
                        checkpoint.added = true;
                    }
                    checkpoint.gameObject.SetActive(false);
                    if (checkpoint != checkpoints[checkpoints.Count - 1])
                    {
                        if (!checkpoints[checkpoints.IndexOf(checkpoint) + 1].acquired)
                            checkpoints[checkpoints.IndexOf(checkpoint) + 1].gameObject.SetActive(true);
                    }
                }
            }
            progressBar.value = (float)acquiredCheckpoints / (float)checkpoints.Count;
            if (progressBar.value >= 1)
                checkPointsComplete = true;
        }
        else
        {
            Invoke("LoadLevel", 2.0f);
        }
    }

    private void MovePlayerToCenter()
    {
        Transform player = FindObjectOfType<InputManager>().transform;
        Vector3 newPosition = Vector3.zero;
        float distance = 75;
        if (player.position.x > distance)
            newPosition.x = -distance;
        else if (player.position.x < -distance)
            newPosition.x = distance;
        if (player.position.z > distance)
            newPosition.z = -distance;
        else if (player.position.z < -distance)
            newPosition.z = distance;
        player.transform.position += newPosition;
    }

    private void LoadLevel()
    {
        FindObjectOfType<GameManager>().LoadLevel("Tutorial 2", 5);
    }
}
