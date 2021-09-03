using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Header("Launcher")]
    public static Launcher Instance;

    List<RoomInfo> fullRoomList = new List<RoomInfo>();
    List<RoomListItem> roomListItems = new List<RoomListItem>();

    [SerializeField] TMP_InputField roomManeInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text RoomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject MasterView;
    [SerializeField] GameObject ClientView;
    [SerializeField] int LevelIndex = 0;
    PhotonView PV;
    [Header("Display Map Name")]
    public TMP_Text DisplayMap;
    [SerializeField] string[] MapName;
    [SerializeField] Sprite[] MapImage;

    public TMP_Text TextDisplay;
    public Image ImageDisplay;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        Debug.Log("Connectting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManger.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomManeInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomManeInputField.text);
        MenuManger.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManger.Instance.OpenMenu("room");
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in PlayerListContent)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        MasterView.SetActive(PhotonNetwork.IsMasterClient);
        ClientView.SetActive(!PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        MasterView.SetActive(PhotonNetwork.IsMasterClient);
        ClientView.SetActive(!PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManger.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        if (LevelIndex != 0)
        {
            MenuManger.Instance.OpenMenu("loading");
            PhotonNetwork.LoadLevel(LevelIndex);
        }
        else
        {
            DisplayMap.text = "select Level first";
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManger.Instance.OpenMenu("loading");
        Debug.Log("Leave room");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManger.Instance.OpenMenu("loading");       
    }

    public override void OnLeftRoom()
    {
        MenuManger.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo updatedRoom in roomList)
        {
            RoomInfo existingRoom = fullRoomList.Find(x => x.Name.Equals(updatedRoom.Name)); // Check to see if we have that room already
            if (existingRoom == null) // WE DO NOT HAVE IT
            {
                fullRoomList.Add(updatedRoom); // Add the room to the full room list
            }
            else if (updatedRoom.RemovedFromList) // WE DO HAVE IT, so check if it has been removed
            {
                fullRoomList.Remove(existingRoom); // Remove it from our full room list
            }
        }
        RenderRoomList();
    }

    void RenderRoomList()
    {
        RemoveRoomList();
        foreach (RoomInfo roomInfo in fullRoomList)
        {
            RoomListItem roomListItem = Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>();
            roomListItem.SetUp(roomInfo);
            roomListItems.Add(roomListItem);
        }
    }

    void RemoveRoomList()
    {
        foreach (RoomListItem roomListItem in roomListItems)
        {
            Destroy(roomListItem.gameObject);
        }
        roomListItems.Clear();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && LevelIndex != 0)
        {
            PV.RPC("Changestate", RpcTarget.Others, LevelIndex);
        }            
        Instantiate(PlayerListItemPrefab, PlayerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }

    public void LevelSelect(int Inputindex)
    {
        DisplayMap.text = MapName[Inputindex];
        LevelIndex = Inputindex;
        PV.RPC("Changestate", RpcTarget.Others, Inputindex);
    }

    [PunRPC]
    void Changestate(int state)
    {
        LevelIndex = state;
        Debug.Log("This message from PRV it work the map is : " + LevelIndex);
        TextDisplay.text = MapName[LevelIndex];
        ImageDisplay.sprite = MapImage[LevelIndex];
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
