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
    private int gameProgress = 0;
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
    private float levelStartPosition = 0f;
    private float distanceTravelledThisLevel = 0f;

    // Others
    Material cameraMat;

    void Start() {
        cameraMat = PlayerManager.Instance.CameraController.GetComponentInChildren<MeshRenderer>().material;

        ResetGame();
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
            Debug.Log($"Finished Level: {playerPosition} > {LevelManager.Instance.GetEndPosition()}");
            FinishLevel();
        }

        // Fake distance travelled to account for offset
        var actualLevelLength = (LevelManager.Instance.GetEndPosition() - levelStartPosition);
        var actualDistanceTravelled = playerPosition - levelStartPosition;
        distanceTravelledThisLevel = actualDistanceTravelled / actualLevelLength * LevelManager.Instance.CurrentLevel.GetLevelLengthMeters();

        UpdateSunset();
        UpdateHud();
    }

    public void ShowTitle() {
        LevelManager.Instance.LoadLevel(0);
        MenuSystem.Instance.ShowTitle();
    }

    public void StartLevel(int level) {
        levelInProgress = true;
        levelIndex = level;
        levelStartTime = Time.time;
        damages = 0;
        levelStartPosition = PlayerManager.Instance.PlayerController.transform.position.z;
        distanceTravelledThisLevel = 0f;

        LevelManager.Instance.LoadLevel(levelIndex);
        Debug.Log($"Level Start!");

        // ??
        // playerController.Reset();

        MenuSystem.Instance.UnpauseGame();
    }

    public void FinishLevel() {
        levelInProgress = false;
        levelFinishTime = Time.time;

        distanceTravelled += distanceTravelledThisLevel;

        // Show score screen
        var timespan = TimeSpan.FromSeconds(levelFinishTime - levelStartTime);
        Debug.Log($"Level Finished! Time {timespan.ToString(@"hh\:mm\:ss")}");

        // if (LevelManager.Instance.CurrentLevel.name == "Last Level") {
        //     FinishLevel();
        //     Debug.Log($"End game!!");
        //     AdvanceGameProgress();
        // }
        AdvanceGameProgress();
        // TODO: Get time limit and delivery goal from LevelData
        // var earnings = ComputeEarnings();
        // money += earnings;

        // Take away control from the user
    }

    public void AdvanceGameProgress() {
        gameProgress += 1;
        switch (gameProgress) {
            case 0:
                // Title
                break;
            case 1:
                // Intro dialogue
                SoundSystem.Instance.PlayLevelMusic(1);
                MenuSystem.Instance.ShowDialogue("game_intro");
                break;
            case 2:
                // Level 1
                StartLevel(1);
                break;
            case 3:
                // Level 2 dialogue
                MenuSystem.Instance.ShowDialogue("delivery_intro");
                playerController.DeliveryUnlocked = true;
                break;
            case 4:
                // Level 2 - delivery powerup
                StartLevel(2);
                break;
            case 5:
                // Level 3 dialogue
                MenuSystem.Instance.ShowDialogue("bonus_speed");
                playerController.BonusSpeed += 30;
                break;
            case 6:
                // Level 3
                SoundSystem.Instance.PlayLevelMusic(2);
                StartLevel(3);
                break;
            case 7:
                // Level 4
                MenuSystem.Instance.ShowDialogue("jump_intro");
                playerController.JumpUnlocked = true;
                break;
            case 8:
                // Level 4
                SoundSystem.Instance.PlayLevelMusic(3);
                StartLevel(4);
                break;
            case 9:
                // Game end
                if (timeRemaining <= 0) {
                    MenuSystem.Instance.ShowDialogue("ending_bad");
                } else {
                    MenuSystem.Instance.ShowDialogue("ending_good");
                }

                SoundSystem.Instance.PlayLevelMusic(0);
                SoundSystem.Instance.SetVolume(0.5f);
                break;
            case 10:
                // Score screen
                MenuSystem.Instance.ShowLevelEnd(levelIndex, timeRemaining, 0, damages, deliveries, 0, money);
                break;
        }
    }

    public void ResetGame() {
        SoundSystem.Instance.SetVolume(1.0f);
        gameProgress = 0;
        timeRemaining = TOTAL_TIME;
        totalDistance = LevelManager.Instance.GetTotalDistance();
        playerController.Reset();
        ShowTitle();
    }

    public void ScoreCollision(float collisionSpeed) {
        damages += 1;
        // var damage = 1 + (int)collisionSpeed;
        // money -= damage;
        // damages += damages;
        // TODO: popup damage text
        // ReactUnityBridge.Instance.UpdateDamages($"Damages: ${damages}");
    }

    public void ScorePackage() {
        money += 100;
        deliveries += 1;
        SoundSystem.Instance.PlayClip("deliverySuccess");
        playerController.BonusSpeed += 5;
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
        ReactUnityBridge.Instance.UpdateHud(timeRemaining, deliveries, distanceTravelled + distanceTravelledThisLevel, totalDistance);
    }

}
