using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace GC_Capstone_02___Task_List
{
    class Task
    {
        //Private fields
        private int taskID;
        private string assignedTo;
        private string taskDescription;
        private DateTime taskDueDate;
        private bool taskCompletionStatus;
        private string lastEditedBy;
        private DateTime lastEditedTimestamp;

        //Public Properties
        public int TaskID
        {
            get
            {
                return taskID;
            }
            set
            {
                taskID = value;
            }
        }
        public string AssignedTo
        {
            get
            {
                return assignedTo;
            }
            set
            {
                assignedTo = value;
            }
        }
        public string TaskDescription
        {
            get
            {
                return taskDescription;
            }
            set
            {
                taskDescription = value;
            }
        }
        public DateTime TaskDueDate
        {
            get
            {
                return taskDueDate;
            }
            set
            {
                taskDueDate = value;
            }
        }
        public bool TaskCompletionStatus
        {
            get
            {
                return taskCompletionStatus;
            }
            set
            {
                taskCompletionStatus = value;
            }
        }
        public string LastEditedBy
        {
            get
            {
                return lastEditedBy;
            }
            set
            {
                lastEditedBy = value;
            }
        }
        public DateTime LastEditedTimeStamp
        {
            get
            {
                return lastEditedTimestamp;
            }
            set
            {
                lastEditedTimestamp = value;
            }
        }

        //Constructors
        public Task() //default
        {
            taskID = 0;
            assignedTo = "";
            taskDescription = "";
            taskDueDate = DateTime.Parse("01/01/2000");
            taskCompletionStatus = false;
            lastEditedBy = "";
            lastEditedTimestamp = DateTime.Now;
        }
        public Task(int _taskID, string _assignedTo, string _taskDescription, DateTime _taskDueDate, bool _taskCompletionStatus, string _lastEditedBy, DateTime _lastEditedTimeStamp) //Taking in all fields including status
        {
            taskID = _taskID;
            assignedTo = _assignedTo;
            taskDescription = _taskDescription;
            taskDueDate = _taskDueDate;
            taskCompletionStatus = _taskCompletionStatus;
            lastEditedBy = _lastEditedBy;
            lastEditedTimestamp = _lastEditedTimeStamp;
        }
        public Task(int _taskID, string _assignedTo, string _taskDescription, DateTime _taskDueDate, string _lastEditedBy) //Taking in all fields except status
        {
            taskID = _taskID;
            assignedTo = _assignedTo;
            taskDescription = _taskDescription;
            taskDueDate = _taskDueDate;
            taskCompletionStatus = false;
            lastEditedBy = _lastEditedBy;
            lastEditedTimestamp = DateTime.Now;
        }

        //Methods
        public static void PrintTaskDisplayHeader()
        {
            Console.WriteLine("Task ID  Task Assigned To  Task Description                               Due By    Status       Last Edited By  Last Edited On");
            Console.WriteLine("=======  ================  ===========================================  =========   ===========  ==============  ==============");
        }
        public static void PrintTaskToConsole(Task thisTask)
        {
            string completed = "";
            if(thisTask.taskCompletionStatus)
            {
                completed = "Complete";
            }
            else
            {
                completed = "Incomplete";
            }

            Console.WriteLine("{0,3}      {1,-16}  {2,-40}  {3,12}   {4,-12}  {5,-15}  {6,12}", thisTask.taskID, thisTask.assignedTo, thisTask.taskDescription, thisTask.taskDueDate.ToShortDateString(), completed, thisTask.LastEditedBy, thisTask.lastEditedTimestamp.ToShortDateString());
        }
        public static string[] GetTeamNames (List<Task> tasks)
        {
            List<string> names = new List<string>();
            foreach (Task task in tasks)
            {
                names.Add(task.assignedTo);
            }
            string[] distinctNames = names.Distinct().ToArray();
            return distinctNames;

        }
        public static int[] GetTaskIDs (List<Task> tasks)
        {
            List<int> taskIDs = new List<int>();
            foreach (Task task in tasks)
            {
                taskIDs.Add(task.taskID);
            }
            int[] distinctIDs = taskIDs.Distinct().ToArray();
            return distinctIDs;
        }
        public static int[] GetIncompleteTaskIDs (List<Task> tasks)
        {
            List<int> taskIDs = new List<int>();
            foreach (Task task in tasks)
            {
                if (!task.taskCompletionStatus)
                {
                    taskIDs.Add(task.taskID);
                }
            }
            int[] distinctIDs = taskIDs.Distinct().ToArray();
            return distinctIDs;
        }

        public static List<Task> ChangeAssignment (List<Task> tasks, int taskID, string newTeammate, string user)
        {
            foreach (Task task in tasks)
            {
                if (task.taskID == taskID)
                {
                    task.assignedTo = newTeammate;
                    task.lastEditedBy = user;
                    task.lastEditedTimestamp = DateTime.Now;
                }
            }
            return tasks;            

        }
        public static List<Task> ChangeDescription(List<Task> tasks, int taskID, string newDescription, string user)
        {
            foreach (Task task in tasks)
            {
                if (task.taskID == taskID)
                {
                    task.taskDescription = newDescription;
                    task.lastEditedBy = user;
                    task.lastEditedTimestamp = DateTime.Now;
                }
            }
            return tasks;

        }
        public static List<Task> ChangeDueDate(List<Task> tasks, int taskID, DateTime newDueDate, string user)
        {
            foreach (Task task in tasks)
            {
                if (task.taskID == taskID)
                {
                    task.taskDueDate = newDueDate;
                    task.lastEditedBy = user;
                    task.lastEditedTimestamp = DateTime.Now;
                }
            }
            return tasks;

        }
        public static List<Task> MarkComplete(List<Task> tasks, int taskID, string user)
        {
            foreach (Task task in tasks)
            {
                if (task.taskID == taskID)
                {
                    task.taskCompletionStatus = true;
                    task.lastEditedBy = user;
                    task.lastEditedTimestamp = DateTime.Now;
                }
            }
            return tasks;
        }
        public static List<Task> DeleteTaskListAdd(List<Task> taskList, List<Task> deletedTasks, int taskID)
        {
            foreach (Task task in taskList)
            {
                if(task.taskID == taskID)
                {
                    string delAssignedTo = task.assignedTo;
                    string delDescription = task.taskDescription;
                    DateTime delDueDate = task.taskDueDate;
                    bool delStatus = task.taskCompletionStatus;
                    string delEditedBy = task.lastEditedBy;
                    DateTime delTimeStamp = task.lastEditedTimestamp;
                                        
                    deletedTasks.Add(new Task(taskID, delAssignedTo, delDescription, delDueDate, delStatus,  delEditedBy, delTimeStamp));
                }
            }
            return deletedTasks;
        }
        public static List<Task> DeleteFromTaskList(List<Task> taskList, int taskID)
        {
            for (int i = 0; i < taskList.Count; i++)
            {
                if (taskList[i].taskID == taskID)
                {
                    taskList.RemoveAt(i);
                }
            }
            return taskList;
        }
    }
}
