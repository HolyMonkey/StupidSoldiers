using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Agava.YandexGames;
using Lean.Localization;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] private LeanLocalization _leanLocalization;

    private void Awake()
    {
        switch (YandexGamesSdk.Environment.i18n.tld)
        {
            case "com":
                _leanLocalization.CurrentLanguage = "English";
                break;
            case "com.tr":
                _leanLocalization.CurrentLanguage = "Turkish";
                break;
            case "ru":
                _leanLocalization.CurrentLanguage = "Russian";
                break;
            default:
                _leanLocalization.CurrentLanguage = "English";              
                break;
        }
    }
}
