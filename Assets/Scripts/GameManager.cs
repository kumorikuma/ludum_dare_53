using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Controls overall game stuff like scoring, game start, game over

public class GameManager : Singleton<GameManager> {
    public bool DEVELOPMENT_MODE = false;

    [NonNullField]
    [SerializeField]
    PlayerController playerController;

    // Game stats
    private int money;

    // Level stats
    private bool levelInProgress = false;
    private int levelIndex;
    private float levelStartTime;
    private float levelFinishTime;
    private float timeLimit;
    private int damages;
    private int deliveries;

    void Start() {
        ShowTitle();
        // StartLevel();
    }

    void Update() {
        if (!levelInProgress) {
            return;
        }

        var timespan = TimeSpan.FromSeconds(Time.time - levelStartTime);
        ReactUnityBridge.Instance.UpdateTimer($"Time: {timespan.ToString(@"hh\:mm\:ss")}");

        // TODO: Instead of having an end-zone, check if game is over by y position of player?
        // Ideally we could have a cutscene at the end, maybe like a building where can pull off to deliver.
        // TODO: Can also give player a distance meter, maybe at the top of the screen
        float playerPosition = PlayerManager.Instance.PlayerController.transform.position.z;
        float progressPct = Math.Clamp(playerPosition / LevelManager.Instance.CurrentLevel.GetLevelLengthMeters(), 0, 1);
        // Debug.Log(progressPct);

        if (playerPosition > LevelManager.Instance.GetEndPosition()) {
            if (LevelManager.Instance.CurrentLevel.name == "Last Level") {
                Debug.Log($"End game!!");
            } else {
                Debug.Log($"Finished Level: {playerPosition} > {LevelManager.Instance.GetEndPosition()}");
                FinishLevel();
            }
        }
    }

    public void ShowTitle() {
        LevelManager.Instance.LoadLevel(0);
        MenuSystem.Instance.ShowTitle();
    }

    public void StartGame() {
        MenuSystem.Instance.ShowDialogue("game_intro");
    }

    public void StartLevel() {
        levelInProgress = true;
        levelIndex += 1;
        levelStartTime = Time.time;
        damages = 0;

        ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");

        // LevelManager.Instance.LoadBoTestLevel();
        LevelManager.Instance.LoadLevel(levelIndex);
        Debug.Log($"Level Start!");

        // ??
        // playerController.Reset();

        MenuSystem.Instance.UnpauseGame();
        SoundSystem.Instance.PlayLevelMusic(levelIndex);
    }

    public void FinishLevel() {
        levelInProgress = false;
        levelFinishTime = Time.time;
        var timespan = TimeSpan.FromSeconds(levelFinishTime - levelStartTime);

        // Show score screen
        Debug.Log($"Level Finished! Time {timespan.ToString(@"hh\:mm\:ss")}");
        // TODO: Get time limit and delivery goal from LevelData
        var earnings = ComputeEarnings();
        money += earnings;
        MenuSystem.Instance.ShowLevelEnd(levelIndex, levelFinishTime - levelStartTime, 60, damages, deliveries, 0, earnings);

        // Take away control from the user
    }

    public void NextLevel() {
        StartLevel();
    }

    public void ScoreCollision(float collisionSpeed) {
        damages += 1 + (int)collisionSpeed;
        ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");
    }

    public void ScorePackage() {
        deliveries += 1;
        SoundSystem.Instance.PlayClip("deliverySuccess");
    }

    private int ComputeEarnings() {
        int levelBase = 100;
        int timeBonus = (int)Mathf.Floor((levelFinishTime - levelStartTime) / 10f - timeLimit) * 50;
        int deliveriesBonus = deliveries * 100;
        return levelBase + timeBonus + deliveriesBonus - damages;
    }

}
