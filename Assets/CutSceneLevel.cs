using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CutSceneLevel : MonoBehaviour
{
    [SerializeField] private Transform RunToPoint;
    [SerializeField] private GameObject[] TalkPanels;
    [SerializeField] private Animator ceoAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed = 5.0f;
    private bool dialogStarted = false;
    private int dialogIndex = 0;

    void Start()
    {
        // Start moving the player towards the point
        StartCoroutine(MovePlayerToPosition());
    }

    IEnumerator MovePlayerToPosition()
    {
        while (Vector3.Distance(player.transform.position, RunToPoint.position) > 0.1f)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, RunToPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        ceoAnimator.SetTrigger("WakeUp");
        TalkPanels[dialogIndex].SetActive(true);
        dialogStarted = true;
    }


    void Update()
    {
        if (dialogStarted)
        {
            if (Input.GetKeyDown("space"))
            {
                dialogIndex++;
                ActivateNextDialog();
            }
        }
    }


    void ActivateNextDialog()
    {

        if (dialogIndex != 0)
        {
            TalkPanels[dialogIndex - 1].SetActive(false);
        }

        if (dialogIndex == TalkPanels.Length)
        {
            SceneManager.LoadScene("LVL-4");
            return;
        }

        TalkPanels[dialogIndex].SetActive(true);
    }
}