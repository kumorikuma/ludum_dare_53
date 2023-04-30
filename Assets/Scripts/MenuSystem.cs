using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : Singleton<MenuSystem> {

    void Start() {
    }

    public void ShowTitle() {
        PlayerManager.Instance.SwitchActionMaps("menu");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Title);
    }

    public void ShowDialogue(string conversationKey) {
        PlayerManager.Instance.SwitchActionMaps("menu");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Dialogue);
        ReactUnityBridge.Instance.conversationKey.Value = conversationKey;
    }

    public void ShowLevelEnd() {
        PlayerManager.Instance.SwitchActionMaps("menu");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.LevelEnd);
    }

    public void ShowUpgrades() {
        // TODO
    }

    public void PauseGame() {
        Time.timeScale = 0;
        PlayerManager.Instance.SwitchActionMaps("menu");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Pause);
    }

    public void UnpauseGame() {
        Time.timeScale = 1;
        PlayerManager.Instance.SwitchActionMaps("gameplay");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Game);
    }
}
