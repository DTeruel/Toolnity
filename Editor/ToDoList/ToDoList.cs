#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolnity
{
    [CreateAssetMenu(fileName = "[ToDo] Tasks", menuName = "Toolnity/ToDo List")]
    public class ToDoList : ScriptableObject
    {
        public List<ToDoElement> tasks = new List<ToDoElement>();
        public List<ToDoElement> completedTasks = new List<ToDoElement>();
        public List<string> parameters1 = new List<string>();
        public List<string> parameters2 = new List<string>();
        public bool configurationEnabled;
        public bool showCompletedTasks;
        public bool showFilters;
        public bool filter1Enabled;
        public bool filter2Enabled;
        public int filterIndexParameter1;
        public int filterIndexParameter2;

        public ToDoList()
        {
            parameters1.Add("Design");
            parameters1.Add("Art");
            parameters1.Add("Programming");
            parameters1.Add("Audio");
            parameters1.Add("Animation");
            parameters1.Add("VFX");
            
            parameters2.Add("Normal");
            parameters2.Add("Very High");
            parameters2.Add("High");
            parameters2.Add("Low");
            parameters2.Add("Very Low");
        }
    }

    [Serializable]
    public class ToDoElement
    {
        public string description;
        public int parameter2Id;
        public int parameter1Id;

        public ToDoElement()
        {
            description = "";
            parameter2Id = 0;
            parameter1Id = 0;
        }
    }
}
#endif