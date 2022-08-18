using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameAnalitic /*: MonoBehaviour*/
{
#if VK_GAMES
    private const string ResourceCurrenciesType = "Gold";

    public static void StartGame(int count)
    {
        Dictionary<string, object> countText = new Dictionary<string, object> { {"count", count } };
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game_start", countText);
    }

    public static void StartLevel(int index)
    {
        Dictionary<string, object> level = new Dictionary<string, object> { { "level", index } };
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_start", level);
    }

    public static void EndLevel(int index, int time)
    {
        Dictionary<string, object> userInfo = new Dictionary<string, object> { { "level ", index}, { "time_spent", time } };
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_complete", userInfo);
    }

    public static void RestartLevel(int index, int time)
    {
        Dictionary<string, object> userInfo = new Dictionary<string, object> { {"level",index }, { "time_spent",time } };
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "fail", userInfo);
    }

    public static void SpentMoney(float money, string itemName)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, ResourceCurrenciesType, money, "shop purchace", itemName);
    }

    public static void GainedMoney(float money, string source, string itemId)
    {
        GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, ResourceCurrenciesType, money, source, itemId);
    }

    public static void TotalPlayedTime(float time)
    {
        GameAnalytics.NewDesignEvent("total_playtime", time);
    }

    public static void AddsStart(GAAdType type)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Clicked, type, "VKSdk", "placement") ;
    }

    public static void AdsComplete(GAAdType type)
    {
        GameAnalytics.NewAdEvent(GAAdAction.Show, type, "VKSdk", "placement");
    }
#endif
}
