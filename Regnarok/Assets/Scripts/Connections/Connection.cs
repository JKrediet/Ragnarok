using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class Connection : MonoBehaviourPunCallbacks
{
    public static Connection Instance;

    [SerializeField] GameObject menu_PlayerPrefab, menu_RoomPrefab;
    [SerializeField] Transform menu_PlayerParent, menu_RoomParent;

    string roomName;
    [SerializeField] TextMeshProUGUI roomNameUI;
    [SerializeField] GameObject startButton;
    #region base join
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby()
    {
        MenuManager.menuSwitch.ChangeMenu("MainMenu");
    }
    #endregion
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomName);
    }
    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public void UpdatePlayerList()
    {
        foreach (Transform child in menu_PlayerParent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject tempPlayerObject = Instantiate(menu_PlayerPrefab, menu_PlayerParent);
            tempPlayerObject.GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    public void LoadRooms()
    {
        MenuManager.menuSwitch.ChangeMenu("Join");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform room in menu_RoomParent)
        {
            Destroy(room);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject tempRoomObject = Instantiate(menu_RoomPrefab, menu_RoomParent);
            tempRoomObject.GetComponent<JoinRoom>().roomName = roomList[i].Name;
            tempRoomObject.GetComponent<JoinRoom>().GiveName();
        }
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
    #region names
    public void GiveNickname(TextMeshProUGUI _nickname)
    {
        PhotonNetwork.NickName = _nickname.text;
    }
    public void GiveRoomName(TextMeshProUGUI _roomName)
    {
        roomName = _roomName.text;
        roomNameUI.text = roomName;
    }
    #endregion
}
