using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuScript : MonoBehaviour
{
    public GameObject player;
    public GameObject camera;
    public GameObject UI;
    public Canvas shopMenu;
   [HideInInspector]
    public GameObject achievements;
    PlayerAnimationController _animController;
    public bool started = false;
    private Quaternion targetCameraRotation;
    public GameObject chunkGenerator;
    int renderDistance = 3;
    public List<Material> uiButtonMats;
    public GameObject[] enemiesToLoad;
    public PlayerController controller;
    public GameObject loadingScreen;
    public GameObject inGameUI;

    public TutorialScript tutorial;

    bool startUp = true;
    bool tutorialStarted = false;

    private bool vibrate = false;
   

  
    private void Start()
    {
        if (PlayerPrefs.GetString("vibrate", "true") == "true")
        {
            vibrate = true;
        }
        if (!IsFirstStarting.instance.startUp)
        {
            startUp = false;
        }
        if (startUp)
        {
            loadGame();
        }
        else
        {
            if (!IsFirstStarting.instance.audioManagerStarted)
            {
                AudioManager.instance.initializeAudioManager();
                IsFirstStarting.instance.audioManagerStarted = true;
            }
         
            updateUI();
            targetCameraRotation = Quaternion.Euler(23f, 90f, 0f);
            shopMenu.enabled = false;

      
            if (PlayerPrefs.GetString("restart", "false") == "true")
            {
                StartGame();
            }

        }
      
    }
    /*
     * forces the character through actions that tend to lag on the start, done while loading screen is up
     * 
     */
    private void loadGame()
    {
        PlayerPrefs.SetString("vibrate", "false");
        loadingScreen.SetActive(true);

        controller.loadDeflect();
        controller.loading = true;
        StartGame();
        inGameUI.SetActive(false);

        int x = 10;
        foreach(GameObject g in enemiesToLoad)
        {
            Instantiate(g, new Vector3(x, 1.2f, 0), Quaternion.identity);
            x += 3;
        }

        Invoke("stopLoadingScreen", 2f);

    }
    public void stopLoadingScreen()
    {
        if (vibrate)
        {
            PlayerPrefs.SetString("vibrate", "true");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
     


    }
   


    /**
     * Updates the game each frame.
     */
    private void FixedUpdate()
    {
        // keeps the player from inching off screen
        if (!started)
        {
            player.transform.position = new Vector3(
                -2f, 0.745f, 0);

        }
        else
        {
            // rotates the camera out
            camera.transform.rotation = Quaternion.Lerp(
                camera.transform.rotation, targetCameraRotation, 3.5f * Time.deltaTime);
        }

        DisableScript();
    }

    /**
     * Disables the script once the camera is rotated fully so the update
     * function stops calling and saves memory.
     */
    private void DisableScript()
    {
        if (camera.transform.rotation == targetCameraRotation)
        {
            this.enabled = false;
        }
    }

    /**
     * Transfers over to the achievements menu. Called by onClick()
     * function of the Trophy button at the main menu.
     */
    public void OnTrophyClick()
    {
        achievements.GetComponentInChildren<Canvas>().enabled = true;
        this.GetComponentInChildren<Canvas>().enabled = false;
    }

    /**
     * Enables the tutorial.
     */
    public void EnableTutorial()
    {
        tutorial.enabled = true;
        tutorialStarted = true;
        StartGame();
    }

    /**
     *  Resets the camera and player position the game configuration
     */
    public void StartGame()
    {
        if (!startUp && !tutorialStarted)
        {
            if (PlayerPrefs.GetString("tutorialCompleted", "false") == "false")
            {
                EnableTutorial();
            }
           
        }
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        _animController = player.GetComponent<PlayerAnimationController>();
        UI.GetComponent<Canvas>().enabled = true;
        UI.GetComponent<ScoreCounter>().enabled = true;


        player.GetComponent<Rigidbody>().transform.position = new Vector3(0, 1.15f, 0);
        player.GetComponent<PlayerController>().enabled = true;
        _animController.StartGame();
       camera.GetComponent<CameraFollowScript>().enabled = true;
        this.GetComponentInChildren<Canvas>().enabled = false;
        started = true;
        for(int i =0;i<renderDistance;i++)
        {
            if (tutorialStarted)
            {
                chunkGenerator.GetComponent<ChunkGenerator>().generateChunk(new Vector3(90, 0, 0), Quaternion.Euler(0, 90, 0), true) ;
            }
            else
            {
                chunkGenerator.GetComponent<ChunkGenerator>().generateChunk(new Vector3(90, 0, 0), Quaternion.Euler(0, 90, 0),false);

            }
          
        }
        
        
    }
    public void updateUI()
    {
        foreach (Material material in uiButtonMats)
        {
            material.EnableKeyword("buttonColor");
            material.SetColor("buttonColor", ColorDataBase.GetUIColor());

        }
    }
   
}
