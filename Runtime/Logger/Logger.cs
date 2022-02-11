using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    private static readonly List<string> CategoriesFiltered = new List<string>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad()
    {
        CategoriesFiltered.Clear();

        bool anyFileFound = false;
        var loggerCategoriesFilters = Resources.FindObjectsOfTypeAll<LoggerCategoriesFilter>();
        if (loggerCategoriesFilters.Length > 0)
        {
            anyFileFound = true;
            for (var i = 0; i < loggerCategoriesFilters.Length; i++)
            {
                Log("[Logger] File found: " + loggerCategoriesFilters[i].name);
                LoadAssetCategory(loggerCategoriesFilters[i]);
            }
        }
        
        var file = Resources.Load<LoggerCategoriesFilter>("Logger Categories Filter");
        if (file)
        {
            anyFileFound = true;
            Log("[Logger] File found: " + file.name);
            LoadAssetCategory(file);
        }
        
        if(!anyFileFound)
        {
            Log("[Logger] No 'Logger Categories Filter' file found in the Resources folder.");
        }

        ShowCategoriesFiltered();
    }

    private static void LoadAssetCategory(LoggerCategoriesFilter loggerCategoriesFilter)
    {
        for (var i = 0; i < loggerCategoriesFilter.categoriesFiltered.Length; i++)
        {
            var category = loggerCategoriesFilter.categoriesFiltered[i];
            if (!CategoriesFiltered.Contains(category))
            {
                CategoriesFiltered.Add(category);
            }
        }
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
        var categoriesFiltered = string.Empty;
        for (var i = 0; i < CategoriesFiltered.Count; i++)
        {
            var category = CategoriesFiltered[i];
            if (categoriesFiltered == string.Empty)
            {
                categoriesFiltered = "'" + category + "'";
            }
            else
            {
                categoriesFiltered += ", '" + category + "'";
            }
        }
        
        if (categoriesFiltered == string.Empty)
        {
            categoriesFiltered = "<NONE>";
        }
        Log("[Logger] Categories filtered: " + categoriesFiltered);
    }

    public static void Log(string message)
    {
        Log(string.Empty, message);
    }

    public static void Log(string category, string message)
    {
        if (!IsCategoryAllowed(category))
        {
            return;
        }
        Debug.Log(GetFinalMessage(category, message));
    }

    public static void LogWarning(string message)
    {
        LogWarning(string.Empty, message);
    }

    public static void LogWarning(string category, string message)
    {
        if (!IsCategoryAllowed(category))
        {
            return;
        }
        Debug.LogWarning(GetFinalMessage(category, message));
    }

    public static void LogError(string message)
    {
        LogError(string.Empty, message);
    }

    public static void LogError(string category, string message)
    {
        if (!IsCategoryAllowed(category))
        {
            return;
        }
        Debug.LogError(GetFinalMessage(category, message));
    }

    private static bool IsCategoryAllowed(string category)
    {
        if (category == string.Empty)
        {
            return true;
        }

        return !CategoriesFiltered.Contains(category);
    }

    private static string GetFinalMessage(string category, string message)
    {
        if (category != string.Empty)
        {
            return "[" + category + "] " + message;
        }
        
        return message;
    }
}
