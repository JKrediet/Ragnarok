using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using UnityEngine.Video;

public class Connection : MonoBehaviourPunCallbacks
{
    public static Connection Instance;

    [SerializeField] GameObject menu_PlayerPrefab, menu_RoomPrefab;
    [SerializeField] Transform menu_PlayerParent, menu_RoomParent;

    string roomName;
    [SerializeField] TextMeshProUGUI roomNameUI;
    TextMeshProUGUI seedText;
    [SerializeField] GameObject startButton;
    [SerializeField] int seed;
    [SerializeField] GameObject loadingScreen, videoplayer;
    [SerializeField] VideoClip[] videos;
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
        if(PhotonNetwork.CreateRoom(roomName))
        {

        }
        else
        {
            PhotonNetwork.CreateRoom(roomName + Random.Range(0, 1000));
        }
    }
    public override void OnJoinedRoom()
    {
        UpdatePlayerList();
        roomNameUI.text = PhotonNetwork.CurrentRoom.Name;
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
            Destroy(room.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject tempRoomObject = Instantiate(menu_RoomPrefab, menu_RoomParent);
            tempRoomObject.GetComponent<JoinRoom>().roomName = roomList[i].Name;
            tempRoomObject.GetComponent<JoinRoom>().GiveName();
            if (roomList[i].PlayerCount == 0)
            {
                Destroy(tempRoomObject.gameObject);
            }
        }
    }
    public void RecieveSeed(Text _seed)
    {
        if (_seed.text.Length > 0)
        {
            seed = int.Parse(_seed.text);
        }
    }
    public void StartGame()
    {
        if (seed == 0)
        {
            seed = Random.Range(0, 99999);
        }
        GetComponent<PhotonView>().RPC("SincSeed", RpcTarget.All, seed);
        GetComponent<PhotonView>().RPC("LoadingScreen", RpcTarget.All);
        Invoke("StartLevel", 5);
    }
    [PunRPC]
    void SincSeed(int _seed)
    {
        //after seed sinced
        PlayerPrefs.SetInt("Seed", _seed);
    }
    [PunRPC]
    void LoadingScreen()
    {
        int roll = Random.Range(0, videos.Length);
        VideoPlayer videoHost = videoplayer.GetComponent<VideoPlayer>();
        videoHost.clip = videos[roll];
        videoHost.Play();
        loadingScreen.SetActive(true);
    }
    void StartLevel()
    {
        PhotonNetwork.LoadLevel(1);
    }
    #region names
    public void GiveNickname(TextMeshProUGUI _nickname)
    {
        if(_nickname.text.Length > 3)
        {
            PhotonNetwork.NickName = _nickname.text;
        }
    }
    public void GiveRoomName(TextMeshProUGUI _roomName)
    {
        roomName = _roomName.text;
        roomNameUI.text = roomName;
    }
    #endregion
}
