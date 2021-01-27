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
        public List<string> assigners = new List<string>();
        public List<string> priorities = new List<string>();
        public bool configurationEnabled;
        public bool showCompletedTasks;

        public ToDoList()
        {
            assigners.Add("Design");
            assigners.Add("Art");
            assigners.Add("Programming");
            assigners.Add("Audio");
            assigners.Add("Animation");
            assigners.Add("VFX");
            
            priorities.Add("Normal");
            priorities.Add("Very High");
            priorities.Add("High");
            priorities.Add("Low");
            priorities.Add("Very Low");
        }
    }

    [Serializable]
    public class ToDoElement
    {
        public string description;
        public int priorityId;
        public int assignerId;

        public ToDoElement()
        {
            description = "";
            priorityId = 0;
            assignerId = 0;
        }
    }
}
#endif