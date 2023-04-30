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
        ShowTitle();
    }

    void Update() {
        if (LevelManager.Instance.CurrentLevel == null) {
            return;
        }

        var timespan = TimeSpan.FromSeconds(Time.time - levelStartTime);
        ReactUnityBridge.Instance.UpdateTimer($"Time: {timespan.ToString(@"hh\:mm\:ss")}");

        // TODO: Instead of having an end-zone, check if game is over by y position of player?
        // Ideally we could have a cutscene at the end, maybe like a building where can pull off to deliver.
        // TODO: Can also give player a distance meter, maybe at the top of the screen
        float playerPosition = PlayerManager.Instance.PlayerController.transform.position.z;
        float progressPct = Math.Clamp(playerPosition / LevelManager.Instance.CurrentLevel.LevelLengthMeters, 0, 1);
        // Debug.Log(progressPct);
        if (progressPct >= 1) {
            FinishLevel();
        }
    }

    public void ShowTitle() {
        LevelManager.Instance.LoadLevel(0);
        MenuSystem.Instance.ShowTitle();
    }

    public void StartGame() {
        MenuSystem.Instance.ShowDialogue("level1");
    }

    public void StartLevel() {
        levelStartTime = Time.time;
        damages = 0;

        ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");

        // LevelManager.Instance.LoadBoTestLevel();
        LevelManager.Instance.LoadLevel(1);
        Debug.Log($"Level Start!");

        PlayerManager.Instance.SwitchActionMaps("gameplay");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Game);
    }

    public void FinishLevel() {
        levelFinishTime = Time.time;
        var timespan = TimeSpan.FromSeconds(levelFinishTime - levelStartTime);

        // TODO: Show score screen
        Debug.Log($"Level Finished! Time {timespan.ToString(@"hh\:mm\:ss")}");
        // LevelManager.Instance.UnloadCurrentLevel();
    }

    public void ScoreCollision(float collisionSpeed) {
        damages += 1 + (int)collisionSpeed;
        ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");
    }
}
