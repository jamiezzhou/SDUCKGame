using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsText;
    private bool controlClicked = false;

    public GameObject helpText;
    private bool helpClicked = false;

    public void Start()
    {
        controlsText.SetActive(controlClicked);
        helpText.SetActive(helpClicked);
    }


    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Controls() {
        if (!controlClicked)
        {
            controlClicked = true;
        }
        else {
            controlClicked = false;
        }
        controlsText.SetActive(controlClicked);
    }

    public void Help()
    {
        if (!helpClicked)
        {
            helpClicked = true;
        }
        else
        {
            helpClicked = false;
        }
        helpText.SetActive(helpClicked);
    }
}
