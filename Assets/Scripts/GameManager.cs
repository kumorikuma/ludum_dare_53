using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Controls overall game stuff like scoring, game start, game over

public class GameManager : Singleton<GameManager> {

    [NonNullField]
    [SerializeField]
    PlayerController playerController;

    // Game stats
    private int money;

    // Level stats
    private float levelStartTime;
    private float levelFinishTime;
    private int damages;

    void Start() {
        StartLevel();
    }

    void Update() {
        var timespan = TimeSpan.FromSeconds(Time.time - levelStartTime);
        ReactUnityBridge.Instance.UpdateTimer($"Time: {timespan.ToString(@"hh\:mm\:ss")}");
    }

    public void StartLevel() {
        levelStartTime = Time.time;
        damages = 0;

        ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");

        LevelManager.Instance.LoadBoTestLevel();
        Debug.Log($"Level Start!");
    }

    public void FinishLevel() {
        levelFinishTime = Time.time;
        var timespan = TimeSpan.FromSeconds(levelFinishTime - levelStartTime);

        // TODO: Show score screen
        Debug.Log($"Level Finished! Time {timespan.ToString(@"hh\:mm\:ss")}");
        LevelManager.Instance.UnloadCurrentLevel();
    }

    public void ScoreCollision(float collisionSpeed) {
        damages += 1 + (int)collisionSpeed;
        ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");
    }
}
