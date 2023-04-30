using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRouter : Singleton<UIRouter> {
    public enum Route {
        Title,
        Pause,
        LevelStart,
        LevelEnd,
        Game,
        Dialogue,
    }

    public Route DebugRoute = Route.Game;

    public event EventHandler<string> OnRouteUpdate;

    public void SwitchRoutes(Route routeName) {
        OnRouteUpdate(this, RouteNameToPath(routeName));
    }

    void OnValidate() {
        if (!Application.isPlaying) { return; }
        OnRouteUpdate?.Invoke(this, RouteNameToPath(DebugRoute));
    }

    string RouteNameToPath(Route routeName) {
        string routePath = "";
        switch (routeName) {
            case Route.Title:
                routePath = "/title";
                break;
            case Route.Pause:
                routePath = "/pause";
                break;
            case Route.LevelStart:
                routePath = "/level_start";
                break;
            case Route.LevelEnd:
                routePath = "/level_end";
                break;
            case Route.Game:
                routePath = "/hud";
                break;
            case Route.Dialogue:
                routePath = "/dialogue";
                break;
        }
        return routePath;
    }
}
