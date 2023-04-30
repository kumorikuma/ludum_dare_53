using System.Collections;
using System.Collections.Generic;
using ReactUnity;
using ReactUnity.Reactive;
using UnityEngine;

public class ReactUnityBridge : Singleton<ReactUnityBridge> {
    [NonNullField]
    public UIRouter Router;

    public ReactiveValue<string> route = new ReactiveValue<string>();

    // Conversation
    public ReactiveValue<string> conversationKey = new ReactiveValue<string>();

    // HUD texts
    public ReactiveValue<string> timerText = new ReactiveValue<string>();
    public ReactiveValue<string> damagesText = new ReactiveValue<string>();

    protected override void Awake() {
        base.Awake();
        ReactRendererBase reactRenderer = GetComponentInChildren<ReactUnity.UGUI.ReactRendererUGUI>();
        Router.OnRouteUpdate += OnRouteUpdate;
        reactRenderer.Globals["route"] = route;

        // Dialog
        reactRenderer.Globals["conversationKey"] = conversationKey;

        // HUD
        reactRenderer.Globals["timerText"] = timerText;
        reactRenderer.Globals["damagesText"] = damagesText;

        // Scores

        // Upgrades

    }

    void OnRouteUpdate(object sender, string data) {
        route.Value = data;
    }

    public void UpdateTimer(string text) {
        timerText.Value = text;
    }

    public void UpdateDamages(string text) {
        damagesText.Value = text;
    }

    public static void StartGameClicked() {
        GameManager.Instance.StartGame();
    }

    public static void DialogueFinished() {
        Debug.Log("DialogueFinished");
        GameManager.Instance.StartLevel();
    }

    public static void TestDebug() {
        Debug.Log("Interop Test Works");
    }

}
