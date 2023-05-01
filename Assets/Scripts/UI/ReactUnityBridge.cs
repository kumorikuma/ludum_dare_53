using System;
using System.Collections;
using System.Collections.Generic;
using ReactUnity;
using ReactUnity.Reactive;
using UnityEngine;

public class ReactUnityBridge : Singleton<ReactUnityBridge> {
    [NonNullField]
    public UIRouter Router;

    public ReactiveValue<string> route = new ReactiveValue<string>();

    public ReactiveValue<int> continueValue = new ReactiveValue<int>();

    // Conversation
    public ReactiveValue<string> conversationKey = new ReactiveValue<string>();

    // HUD texts
    public ReactiveValue<string> timerText = new ReactiveValue<string>();
    public ReactiveValue<int> money = new ReactiveValue<int>();
    public ReactiveValue<float> distance = new ReactiveValue<float>();
    public ReactiveValue<float> totalDistance = new ReactiveValue<float>();

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

        // To advance the UI
        reactRenderer.Globals["continue"] = continueValue;

        // Dialog
        reactRenderer.Globals["conversationKey"] = conversationKey;

        // HUD
        reactRenderer.Globals["timerText"] = timerText;
        reactRenderer.Globals["money"] = money;
        reactRenderer.Globals["distance"] = distance;
        reactRenderer.Globals["totalDistance"] = totalDistance;

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

    public void UpdateHud(float timeRemaining, int money, float distance, float totalDistance) {
        var timeRemainingText = TimeSpan.FromSeconds(timeRemaining).ToString(@"m\:ss\.ff");

        this.timerText.Value = timeRemainingText;
        this.money.Value = money;
        this.distance.Value = distance;
        this.totalDistance.Value = totalDistance;
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
        GameManager.Instance.AdvanceGameProgress();
    }

    public static void DialogueFinished() {
        Debug.Log("DialogueFinished");
        GameManager.Instance.AdvanceGameProgress();
    }

    public static void NextLevelClicked() {
        GameManager.Instance.AdvanceGameProgress();
    }

    public static void TestDebug() {
        Debug.Log("Interop Test Works");
    }

    // Kinda hacky
    public void Continue() {
        continueValue.Value = 1;
    }
    public static void ResetContinue() {
        ReactUnityBridge.Instance.continueValue.Value = 0;
    }
}
