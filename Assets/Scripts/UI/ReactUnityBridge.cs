using System.Collections;
using System.Collections.Generic;
using ReactUnity;
using ReactUnity.Reactive;
using UnityEngine;

public class ReactUnityBridge : Singleton<ReactUnityBridge> {
    [NonNullField]
    public UIRouter Router;

    public ReactiveValue<string> route = new ReactiveValue<string>();

    // Sample usage
    [NonNullField]
    public DialogueSystem DialogueSystem;
    public ReactiveValue<string> dialogueSpeaker = new ReactiveValue<string>();
    public ReactiveValue<string> dialogueText = new ReactiveValue<string>();
    // TODO: This requires less code, but not sure how to type it on JS side.
    public ReactiveValue<DialogueData> dialogueData = new ReactiveValue<DialogueData>();

    // HUD texts
    public ReactiveValue<string> timerText = new ReactiveValue<string>();
    public ReactiveValue<string> damagesText = new ReactiveValue<string>();

    protected override void Awake() {
        base.Awake();
        ReactRendererBase reactRenderer = GetComponentInChildren<ReactUnity.UGUI.ReactRendererUGUI>();
        Router.OnRouteUpdate += OnRouteUpdate;
        reactRenderer.Globals["route"] = route;

        // Sample usage
        DialogueSystem.OnDialogueUpdate += OnDialogueUpdate;
        reactRenderer.Globals["dialogueSpeaker"] = dialogueSpeaker;
        reactRenderer.Globals["dialogueText"] = dialogueText;
        reactRenderer.Globals["dialogueData"] = dialogueData;

        reactRenderer.Globals["timerText"] = timerText;
        reactRenderer.Globals["damagesText"] = damagesText;

    }

    void OnRouteUpdate(object sender, string data) {
        route.Value = data;
    }

    // Sample usage
    void OnDialogueUpdate(object sender, DialogueData data) {
        dialogueSpeaker.Value = data.speaker;
        dialogueText.Value = data.text;
        // dialogueData.Value = data;
    }

    public void UpdateTimer(string text) {
        timerText.Value = text;
    }

    public void UpdateDamages(string text) {
        damagesText.Value = text;
    }

}
