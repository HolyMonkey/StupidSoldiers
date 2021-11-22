using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class YandexSDK : MonoBehaviour {
    
    public static YandexSDK Instance;
    
    [DllImport("__Internal")] private static extern void GetUserData();
    [DllImport("__Internal")] private static extern void ShowFullscreenAd();
    [DllImport("__Internal")] private static extern int ShowRewardedAd(string placement);
    [DllImport("__Internal")] private static extern void GetReward();
    [DllImport("__Internal")] private static extern void AuthenticateUser();
    [DllImport("__Internal")] private static extern void InitPurchases();
    [DllImport("__Internal")] private static extern void Purchase(string id);
    
    private readonly Queue<int> _rewardedAdPlacementsAsInt = new Queue<int>();
    private readonly Queue<string> _rewardedAdsPlacements = new Queue<string>();

    public UserData UserData { get; private set; }

    public event Action<UserData> UserDataReceived;
    public event Action InterstitialAdShown;
    public event Action<string> InterstitialAdFailed;
    public event Action<int> RewardedAdOpened;
    public event Action<string> RewardedAdShown;
    public event Action<int> RewardedAdClosed;
    public event Action<string> RewardedAdError;
    public event Action<string> PurchaseSuccess;
    public event Action<string> PurchaseFailed;
    public event Action Closed;
    

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    public void Authenticate() {
        AuthenticateUser();
    }

    /// <summary>
    /// Don't call frequently. There is a 3 minute delay after each show.
    /// </summary>
    public void ShowInterstitial() {
        ShowFullscreenAd();
    }
    
    public void ShowRewarded(string placement) {
        _rewardedAdPlacementsAsInt.Enqueue(ShowRewardedAd(placement));
        _rewardedAdsPlacements.Enqueue(placement);
    }
    
    /// <summary>
    /// Call this to receive user data
    /// </summary>
    public void RequestUserData() {
        GetUserData();
    }
    
    public void InitializePurchases() {
        InitPurchases();
    }

    public void ProcessPurchase(string id) {
        Purchase(id);
    }
    
    public void StoreUserData(string data) {
        UserData = JsonUtility.FromJson<UserData>(data);
        UserDataReceived?.Invoke(UserData);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    public void OnInterstitialShown() {
        InterstitialAdShown?.Invoke();
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="error"></param>
    public void OnInterstitialError(string error) {
        InterstitialAdFailed?.Invoke(error);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewardedOpen(int placement) {
        RewardedAdOpened?.Invoke(placement);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewarded(int placement) {
        if (placement == _rewardedAdPlacementsAsInt.Dequeue()) {
            RewardedAdShown?.Invoke(_rewardedAdsPlacements.Dequeue());
        }
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewardedClose(int placement) {
        RewardedAdClosed?.Invoke(placement);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewardedError(string placement) {
        RewardedAdError?.Invoke(placement);
        _rewardedAdsPlacements.Clear();
        _rewardedAdPlacementsAsInt.Clear();
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="id"></param>
    public void OnPurchaseSuccess(string id) {
        PurchaseSuccess?.Invoke(id);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="error"></param>
    public void OnPurchaseFailed(string error) {
        PurchaseFailed?.Invoke(error);
    }
    
    /// <summary>
    /// Browser tab has been closed
    /// </summary>
    /// <param name="error"></param>
    public void OnClose() {
        Closed?.Invoke();
    }
}

public struct UserData {
    public string Id;
    public string Name;
    public string AvatarUrlSmall;
    public string AvatarUrlMedium;
    public string AvatarUrlLarge;
}