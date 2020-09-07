using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/*  8/19/2020 2:50pm - Tab Navigation Monobehaviour
    Used whenever a tab navigation system is required in the game
    ( switches through different panels depending on what is pressed )
*/
public class TabNav : MonoBehaviour {
    public List<Tabpanel> tabs;

    void Awake() {
        EventManager.StartListening("TriggerMessage", On_TriggerMessage);
    }

    void Start() {
        foreach(Tabpanel tp in tabs) {
            tp.button.onClick.AddListener(() => SwitchPanels(tp)); // I keep forgetting this so I would like to remind you. this is called a LAMBDA EXPRESSION ( 8/19/2020 3:57pm )
            tp.DeactivatePanel();
        }

        SwitchPanels(tabs[0]); // Switches to the first panel/button on the list ( 8/19/2020 5:00pm )
    }

    void OnDestroy() {
        EventManager.StopListening("TriggerMessage", On_TriggerMessage);
    }

    public void SwitchPanels(Tabpanel tabToOpen) {
        foreach(Tabpanel tp in tabs) {
            if (tp.isActive) {
                tp.DeactivatePanel();
            }       
        }

        tabToOpen.ActivatePanel();
    }

    #region Event Listeners

    private void On_TriggerMessage(EventParams eventParams) {
        if (eventParams.messageParam.forceChat)
            SwitchPanels(Tabpanel.ReturnTabFromKey(tabs, "log"));
    }

    #endregion
}

// Holds a button and a panel which correspond to each other. The button will be used to open the panel ( 8/19/2020 2:54pm )
[System.Serializable]
public class Tabpanel {
    public Button button;
    public GameObject panel;

    public string key;

    public bool isActive = false;

    public Tabpanel(Button button, GameObject panel, string key) {
        this.button = button;
        this.panel = panel;
        this.key = key;
    }

    public void ActivatePanel() {
        isActive = true;

        panel.SetActive(true);
        button.interactable = false;
    }

    public void DeactivatePanel() {
        isActive = false;

        panel.SetActive(false);
        button.interactable = true;      
    }

    public static Tabpanel ReturnTabFromKey(List<Tabpanel> tablist, string key) {
        foreach (Tabpanel tab in tablist) {
            if (tab.key == key) {
                return tab;
            }
        }

        Debug.LogError("No such key, " + key + " exists in list of tabs, " + tablist);
        return null;
    }
}