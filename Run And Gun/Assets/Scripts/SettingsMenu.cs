using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool SettingUI = false;

    [SerializeField] GameObject ScoreboardListItemPrefab;
    [SerializeField] Transform ScoreboardListContent;

    GameObject ScoreBoard;
    public GameObject pauseMenuUI;
    public GameObject setthingMenuUI;
    public GameObject PostProcess;

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
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
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
        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in ScoreboardListContent)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(ScoreboardListItemPrefab, ScoreboardListContent).GetComponent<scoreBoardScript>().Setup(players[i]);
        }
    } 

    public void PostPocessEnable(bool status)
    {
        if(status == true)
        {
            PostProcess.gameObject.SetActive(true);
        }
        else if(status == false)
        {
            PostProcess.gameObject.SetActive(false);
        }
    }
}
