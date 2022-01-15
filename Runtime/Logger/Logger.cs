using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    private static readonly List<string> CategoriesFiltered = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad()
    {
        var loggerSettings = Resources.FindObjectsOfTypeAll<LoggerCategoriesFilter>();
        for (var i = 0; i < loggerSettings.Length; i++)
        {
            for (var j = 0; j < loggerSettings[i].categoriesFiltered.Length; j++)
            {
                CategoriesFiltered.Add(loggerSettings[i].categoriesFiltered[j]);
            }
        }
        
        ShowCategoriesFiltered();
    }

    public static void ShowCategory(string category)
    {
        var anyRemoved = false;
        for (var i = CategoriesFiltered.Count - 1; i >= 0; i--)
        {
            if (CategoriesFiltered[i].Equals(category))
            {
                anyRemoved = true;
                CategoriesFiltered.RemoveAt(i);
            }
        }

        if (anyRemoved)
        {
            ShowCategoriesFiltered();
        }
    }
    
    public static void HideCategory(string category)
    {
        if (!CategoriesFiltered.Contains(category))
        {
            CategoriesFiltered.Add(category);
            ShowCategoriesFiltered();
        }
    }

    private static void ShowCategoriesFiltered()
    {
        var categoriesFiltered = "";
        for (var i = 0; i < CategoriesFiltered.Count; i++)
        {
            var category = CategoriesFiltered[i];
            if (categoriesFiltered == "")
            {
                categoriesFiltered = "'" + category + "'";
            }
            else
            {
                categoriesFiltered += ", '" + category + "'";
            }
        }
        
        if (categoriesFiltered == "")
        {
            categoriesFiltered = "<NONE>";
        }
        Log("[Logger] Categories filtered: " + categoriesFiltered);
    }

    public static void Log(string message, string category = "")
    {
        if (!IsCategoryAllowed(category))
        {
            return;
        }
        Debug.Log(GetFinalMessage(message, category));
    }
    
    public static void LogWarning(string message, string category = "")
    {
        if (!IsCategoryAllowed(category))
        {
            return;
        }
        Debug.LogWarning(GetFinalMessage(message, category));
    }
    
    public static void LogError(string message, string category = "")
    {
        if (!IsCategoryAllowed(category))
        {
            return;
        }
        Debug.LogError(GetFinalMessage(message, category));
    }

    private static bool IsCategoryAllowed(string category)
    {
        if (category == "")
        {
            return true;
        }

        return !CategoriesFiltered.Contains(category);
    }

    private static string GetFinalMessage(string message, string category = "")
    {
        if (category != "")
        {
            return "[" + category + "] " + message;
        }
        
        return message;
    }
}
