using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Message;
using TMPro;

/*  8/21/2020 5:09pm - Messagelog Class
    Handles Messages to convey information to the player and whatever

*/
public class Messagelog : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI messagePrefab = null;
    [SerializeField] private int messageLimit = 10;

    [SerializeField] private List<Message> messageList;

    #region Unity Functions

    void Awake() {
        EventManager.StartListening("TriggerMessage", On_TriggerMessage);
        messageList = RetrieveChatLog();
    }

    void Start() {
        
    }

    void OnDestroy() {
        EventManager.StopListening("TriggerMessage", On_TriggerMessage);
    }

    #endregion

    #region Functions

    public void CreateMessage(Message msg) {
        // If the number of concurrent messages exceeds the message limit, delete the first message ( 8/21/2020 8:28pm )
        if (messageList.Count >= messageLimit) {
            Destroy(messageList[0].textObject.gameObject);
            messageList.RemoveAt(0);
        }

        messageList.Add(msg);
        CreateTextObject(msg);

        LevelManager.instance.messageLog.Add(msg);
    }

    private List<Message> RetrieveChatLog() {
        List<Message> outputList = new List<Message>();

        if (LevelManager.instance.messageLog.Count <= messageLimit && LevelManager.instance.messageLog.Count > 0) {
            foreach(Message msg in LevelManager.instance.messageLog)
                outputList.Add(msg);

        } else if (LevelManager.instance.messageLog.Count > messageLimit) {
            foreach(Message msg in LevelManager.instance.messageLog.GetRange(LevelManager.instance.messageLog.Count - messageLimit, messageLimit))
                outputList.Add(msg);
        }

        if (outputList.Count > 0)
            foreach(Message msg in outputList)
                CreateTextObject(msg);

        return outputList;
    }

    // Creates the actual text object seen in-game. Is child of the current object ( 8/21/2020 8:42pm )
    private void CreateTextObject(Message msg) {
        msg.textObject = Instantiate(messagePrefab.gameObject, transform).GetComponent<TextMeshProUGUI>();

        msg.textObject.text = msg.text;
        msg.textObject.color = msg.color;
    }

    #endregion

    #region Event Listeners

    private void On_TriggerMessage(EventParams eventParams) {
        CreateMessage(eventParams.messageParam);
    }

    #endregion
}

[System.Serializable]
public class Message {
    public string text;
    public Color32 color;
    public TextMeshProUGUI textObject;

    public bool forceChat = false; // Will open the chat box automatically when enabled ( 9/6/2020 7:24pm )

    public Message(string text, Color32 color, bool forceChat = false) {
        this.text = text;
        this.color = color;
        this.forceChat = forceChat;
    }

    #region Predefined Strings
    
    public static Message ItemCraft(string item) => new Message($"Player crafted a(n) {item}.", Black, true);

    public static Message ItemFound(string item) => new Message($"Player found a(n) {item}.", Black, true);

    public static Message EnemyEncounter(string enemy) => new Message($"Player encountered a(n) {enemy}!", Red, true);

    public static Message EnemyDeafeated(string enemy) => new Message($"Player defeated the {enemy}!", Green, true);

    #endregion

    public static Color32 Black = new Color32(0, 0, 0 , 255);
    public static Color32 Red = new Color32(100, 0, 0, 255);
    public static Color32 Green = new Color32(0, 100, 0, 255);
}
