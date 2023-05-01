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

    // Score screen
    public ReactiveValue<int> scoreLevel = new ReactiveValue<int>();
    public ReactiveValue<float> scoreTime = new ReactiveValue<float>();
    public ReactiveValue<float> scoreTimeLimit = new ReactiveValue<float>();
    public ReactiveValue<int> scoreDamages = new ReactiveValue<int>();
    public ReactiveValue<int> scoreDeliveries = new ReactiveValue<int>();
    public ReactiveValue<int> scoreDeliveriesGoal = new ReactiveValue<int>();
    public ReactiveValue<int> scoreEarnings = new ReactiveValue<int>();

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

        // Score screen
        reactRenderer.Globals["scoreLevel"] = scoreLevel;
        reactRenderer.Globals["scoreTime"] = scoreTime;
        reactRenderer.Globals["scoreTimeLimit"] = scoreTimeLimit;
        reactRenderer.Globals["scoreDamages"] = scoreDamages;
        reactRenderer.Globals["scoreDeliveries"] = scoreDeliveries;
        reactRenderer.Globals["scoreDeliveriesGoal"] = scoreDeliveriesGoal;
        reactRenderer.Globals["scoreEarnings"] = scoreEarnings;

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

    public void UpdateScores(int level, float time, float timeLimit, int damages, int deliveries, int deliveriesGoal, int earnings) {
        scoreLevel.Value = level;
        scoreTime.Value = time;
        scoreTimeLimit.Value = timeLimit;
        scoreDamages.Value = damages;
        scoreDeliveries.Value = deliveries;
        scoreDeliveriesGoal.Value = deliveriesGoal;
        scoreEarnings.Value = earnings;
    }

    public static void StartGameClicked() {
        GameManager.Instance.StartGame();
    }

    public static void DialogueFinished() {
        Debug.Log("DialogueFinished");
        GameManager.Instance.StartLevel();
    }

    public static void NextLevelClicked() {
        GameManager.Instance.NextLevel();
    }

    public static void TestDebug() {
        Debug.Log("Interop Test Works");
    }

}
