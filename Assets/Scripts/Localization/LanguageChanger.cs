using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Agava.YandexGames;

public class LanguageChanger : MonoBehaviour
{
    private void Awake()
    {
        if (YandexGamesSdk.Environment.i18n.tld == "com.tr")
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2];
        else if (YandexGamesSdk.Environment.i18n.lang == "ru")
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        else if (YandexGamesSdk.Environment.i18n.lang == "com")
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        else
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }
}
