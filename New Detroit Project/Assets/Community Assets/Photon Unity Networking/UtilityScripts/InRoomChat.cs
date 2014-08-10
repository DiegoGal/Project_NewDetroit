using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour 
{
    public Rect GuiRect = new Rect(0,0, 250,300);
    public bool IsVisible = true;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;
    private bool teamChat = false;
    private bool focused = false;

    public static PhotonPlayer[] players; //recive value at rol selection
    public static readonly string ChatRPC = "Chat";

    public void Start()
    {
        if (this.AlignBottom)
        {
            this.GuiRect.y = ((float)Screen.height)/2.5f;
        }
    }

    public void OnGUI()
    {
        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
            return;
        }
        /*
        if (!focused && !teamChat && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.T)
        {
            teamChat = true;
            GUI.FocusControl("ChatInput");
            focused = true;
            inputLine = "";
            return;
        }
        */
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)
            {
                if (!string.IsNullOrEmpty(this.inputLine))
                {
                    if (!teamChat)
                        this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                    else
                    {
                        for (int i = 0 + LocalGameManager.myTeam; i < players.Length; i = i + 2)
                        {
                            if (players[i] != null)
                                this.photonView.RPC("Chat", players[i], "[T]" + this.inputLine);
                        }
                        teamChat = false;
                    }
                    
                    this.inputLine = "";
                    GUI.FocusControl("");
                    focused = false;
                    return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
                }
                else
                {
                    if (!focused)
                    {
                        GUI.FocusControl("ChatInput");
                    }
                    else
                    {
                        GUI.FocusControl("");
                        teamChat = false;
                    }
                    focused = !focused;
                }
            }
            if (Event.current.keyCode == KeyCode.Backspace)
                {
                    if (!focused && !teamChat)
                    {
                        teamChat = true;                                                                        
                    }
                }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();
        for (int i = 0; i < messages.Count; i++)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        if (!focused && teamChat)
        {
            focused = true;
            GUI.FocusControl("ChatInput");       
        }
        else
         inputLine = GUILayout.TextField(inputLine);
        /*
        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
        {
            if (!teamChat)
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            else
            {
                for (int i = 0 + LocalGameManager.myTeam; i < players.Length; i = i + 2)
                    this.photonView.RPC("Chat", players[i], "[T]" + this.inputLine);
                teamChat = false;
            }
            this.inputLine = "";
            GUI.FocusControl("");
            focused = false;
        }        
        */
        GUILayout.EndHorizontal();
        GUILayout.EndArea();        
    }

    [RPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi != null && mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.name))
            {
                senderName = mi.sender.name;
            }
            else
            {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add(senderName +": " + newLine);
    }

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }
}
