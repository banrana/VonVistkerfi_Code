using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public ThirdPersonCamera camera;
    private bool isPaused = false;
    private bool isRestarting = false;
    public bool IsRestarting{ get { return isRestarting; } set { isRestarting = value; } }

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        HideAndLockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }

    void Stop()
    {
        pauseMenu.SetActive(true);
        camera.enabled= false;
        //Time.timeScale = 0f;
        isPaused = true;
        ShowAndUnlockCursor();
    }

    public void Play()
    {
        pauseMenu.SetActive(false);
        camera.enabled= true;
        //Time.timeScale = 1f;
        isPaused = false;
        HideAndLockCursor();
    }
    public void RestartGame()
    {
        IsRestarting = true;
        //Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void HideAndLockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void ShowAndUnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
