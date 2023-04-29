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
    private int damage;

    void Start() {
        StartLevel();
    }

    public void StartLevel() {
        levelStartTime = Time.time;
        Debug.Log($"Level Start!");
    }

    public void FinishLevel() {
        levelFinishTime = Time.time;
        var timespan = TimeSpan.FromSeconds(levelFinishTime - levelStartTime);

        // TODO: Show score screen
        Debug.Log($"Level Finished! Time {timespan.ToString(@"hh\:mm\:ss")}");
    }

    public void ScoreCollision(float collisionSpeed) {
        damage += 1 + (int)collisionSpeed;
        Debug.Log($"Collision! Damage {damage}");
    }
}
