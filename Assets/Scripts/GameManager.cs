using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Controls overall game stuff like scoring, game start, game over

public class GameManager : Singleton<GameManager> {
    public bool DEVELOPMENT_MODE = false;
    public static float TOTAL_TIME = 3 * 60f;

    [NonNullField]
    [SerializeField]
    PlayerController playerController;

    // Game stats
    private int money;
    private float timeRemaining = TOTAL_TIME;
    private float distanceTravelled = 0f;
    private float totalDistance = 0f;

    // Level stats
    private bool levelInProgress = false;
    private int levelIndex;
    private float levelStartTime;
    private float levelFinishTime;
    private float timeLimit;
    private int damages;
    private int deliveries;
    private float distanceTravelledThisLevel = 0f;

    // Others
    Material cameraMat;

    void Start() {
        cameraMat = PlayerManager.Instance.CameraController.GetComponentInChildren<MeshRenderer>().material;

        ShowTitle();
        // StartLevel();
    }

    void Update() {
        if (!levelInProgress) {
            return;
        }

        timeRemaining -= Time.deltaTime;

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

        distanceTravelledThisLevel = Mathf.Max(0f, playerPosition - LevelManager.Instance.GetLevelOffset());

        UpdateSunset();
        UpdateHud();
    }

    public void ShowTitle() {
        LevelManager.Instance.LoadLevel(0);
        MenuSystem.Instance.ShowTitle();
    }

    public void StartGame() {
        MenuSystem.Instance.ShowDialogue("game_intro");
        timeRemaining = TOTAL_TIME;
        totalDistance = LevelManager.Instance.GetTotalDistance();
    }

    public void StartLevel() {
        levelInProgress = true;
        levelIndex += 1;
        levelStartTime = Time.time;
        damages = 0;
        distanceTravelledThisLevel = 0f;

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

        distanceTravelled += distanceTravelledThisLevel;

        // Show score screen
        var timespan = TimeSpan.FromSeconds(levelFinishTime - levelStartTime);
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
        var damage = 1 + (int)collisionSpeed;
        money -= damage;
        damages += damages;
        // TODO: popup damage text
        // ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");
    }

    public void ScorePackage() {
        money += 100;
        deliveries += 1;
        SoundSystem.Instance.PlayClip("deliverySuccess");
    }

    private int ComputeEarnings() {
        int levelBase = 100;
        int timeBonus = (int)Mathf.Floor((levelFinishTime - levelStartTime) / 10f - timeLimit) * 50;
        int deliveriesBonus = deliveries * 100;
        return levelBase + timeBonus + deliveriesBonus - damages;
    }

    private void UpdateSunset() {
        var sunsetValue = Mathf.Clamp(1f - timeRemaining / TOTAL_TIME, 0f, 1f);
        cameraMat.SetFloat("_Sunset", sunsetValue);
    }

    private void UpdateHud() {
        Debug.Log($"distanceTravelled {distanceTravelled} distanceTravelledThisLevel {distanceTravelledThisLevel}");
        ReactUnityBridge.Instance.UpdateHud(timeRemaining, money, distanceTravelled + distanceTravelledThisLevel, totalDistance);
    }

}
