using System.Collections.Generic;
using UnityEngine;

public class Logger
{
    #region CATEGORIES FILTERS
    private static readonly List<string> CategoriesFiltered = new ();

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
                UnityEngine.Debug.Log("[Logger] File found: " + loggerCategoriesFilters[i].name);
                LoadAssetCategory(loggerCategoriesFilters[i]);
            }
        }
        
        var file = Resources.Load<LoggerCategoriesFilter>("Logger Categories Filter");
        if (file)
        {
            anyFileFound = true;
            UnityEngine.Debug.Log("[Logger] File found: " + file.name);
            LoadAssetCategory(file);
        }
        
        if(!anyFileFound)
        {
            UnityEngine.Debug.Log("[Logger] No 'Logger Categories Filter' file found in the Resources folder.");
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

    private static void ShowCategory(string category)
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

    private static void HideCategory(string category)
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
        UnityEngine.Debug.Log("[Logger] Categories filtered: " + categoriesFiltered);
    }
    
    private static bool IsCategoryAllowed(string category)
    {
        if (category == string.Empty)
        {
            return true;
        }

        return !CategoriesFiltered.Contains(category);
    }
    #endregion

    private readonly string categoryName;
    
    public static Logger Create<T>()
    {
        var newLogger = new Logger(typeof(T).FullName);
        return newLogger;
    }
    
    public Logger(string fullName)
    {
        categoryName = fullName;
    }

    public void ShowLogs()
    {
        ShowCategory(categoryName);
    }

    public void HideLogs()
    {
        HideCategory(categoryName);
    }
    
    public void Info(string message)
    {
        if (!IsCategoryAllowed(categoryName))
        {
            return;
        }
        UnityEngine.Debug.Log(GetFinalMessage(categoryName, message));
    }
    
    public void Debug(string message)
    {
        if (!IsCategoryAllowed(categoryName))
        {
            return;
        }
        UnityEngine.Debug.Log(GetFinalMessage(categoryName, message));
    }

    public void Warning(string message)
    {
        if (!IsCategoryAllowed(categoryName))
        {
            return;
        }
        UnityEngine.Debug.LogWarning(GetFinalMessage(categoryName, message));
    }

    public void Error(string message)
    {
        if (!IsCategoryAllowed(categoryName))
        {
            return;
        }
        UnityEngine.Debug.LogError(GetFinalMessage(categoryName, message));
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
