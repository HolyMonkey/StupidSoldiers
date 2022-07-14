using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Agava.YandexGames;
using Lean.Localization;

public class LanguageChanger : MonoBehaviour
{
    [SerializeField] private LeanLocalization _leanLocalization;

    private void Start()
    {
        switch (YandexGamesSdk.Environment.i18n.lang)
        {
            case "en":
                _leanLocalization.SetCurrentLanguage("English");
                break;
            case "tr":
                _leanLocalization.SetCurrentLanguage("Turkish");
                break;
            case "ru":
                _leanLocalization.SetCurrentLanguage("Russian");
                break;
            default:
                _leanLocalization.SetCurrentLanguage("English");
                break;
        }

        Debug.Log(YandexGamesSdk.Environment.i18n.lang);
    }
}
