using Photon.Pun;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool SettingUI = false;

    GameObject ScoreBoard;
    public GameObject pauseMenuUI;
    public GameObject setthingMenuUI;

    public AudioMixer audioMixer;
    public float MouseSens = 5f;

    public PlayerMovement SettingInfo;
    int playerCount;

    void Awake()
    {
        MouseSens = 5f;
        SettingInfo = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        ScoreBoard = GameObject.Find("Canvas").transform.Find("ScoreBoard").gameObject;
    }

    //PAUSE MENU PART
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            ScoreBoard.SetActive(true);
            UpdateScoreBoard();
                
        }
        else
        {
            ScoreBoard.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused && !SettingUI)
            {
                Resume();
            }
            else if(GameIsPaused && SettingUI)
            {
                Resume();
            }
            else if(!GameIsPaused && !SettingUI)
            {
                Pause();
            }
            else if(!GameIsPaused && SettingUI)
            {
                Pause();
            }
        }      
    }

    void Resume()
    {
        SettingUI = false;
        pauseMenuUI.SetActive(false);
        setthingMenuUI.SetActive(false);
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        SettingUI = false;
        pauseMenuUI.SetActive(true);
        setthingMenuUI.SetActive(false);
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SettingOpen()
    {
        SettingUI = true;
        setthingMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
    }

    public void SettingClose()
    {
        SettingUI = false;
        setthingMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //SETTINGS MENU PART

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetMouseSensitive(float sens)
    {
        MouseSens = sens;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    void UpdateScoreBoard()
    {
        playerCount = PhotonNetwork.PlayerList.Length;

        var playerNames = new StringBuilder();
        
        foreach(var player in PhotonNetwork.PlayerList)
        {
            playerNames.Append(player.NickName + "\n");
        }

        string output = "Player Count:" + playerCount.ToString() + "\n" + playerNames.ToString();
        ScoreBoard.transform.Find("Text").GetComponent<Text>().text = output;
    }
}
