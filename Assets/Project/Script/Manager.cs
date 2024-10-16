using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Manager 
{
    public static DelayManager Delay { get { return DelayManager.Instance; } }
    public static UIManager UI {  get { return UIManager.Instance; } } 
    public static LobbyManager Lobby { get { return LobbyManager.Instance; } }
}
