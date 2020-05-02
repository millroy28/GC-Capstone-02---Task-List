using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace GC_Capstone_02___Task_List
{
    class Program
    {
        static void Main(string[] args)
        {   //Populates our task lists with some content
            List<Task> taskList = GenerateStartingTaskList();
            List<Task> deletedTasks = GenerateStartingDeletedTaskList();

            //sets console window width
            Console.WindowWidth = 127;
            //Greets user and prompts User for Username
            string user = GetUserName();

            bool continueRun = true;
            do
            {   //Calls first Menu      
                SetConsole(user);
                SelectAction(user, taskList, deletedTasks);

                //Prompts again for quit
                continueRun = !GetYesOrNo("Do you really wish to quit? y/n");
            } while (continueRun);

            Console.Clear();
            Console.WriteLine("Goodbye, " + user!);
        }
        public static void SelectAction(string user, List<Task> taskList, List<Task> deletedTasks)
        {   //THIS IS WHERE MENU OPTIONS ARE SET
            //Sends string array to display menu method

            string[] menuItems = { "List all tasks", "List only incomplete", "List all before specified date", "List all for a teammate", "List deleted tasks", "Add task", "Edit Task", "Mark task complete", "Delete Task", "Quit" };
            int menuChoice = DisplayMenu(menuItems);

            switch(menuChoice)
            {
                case 1:
                    ListAllTasks(user, taskList, deletedTasks);
                    break;
                case 2:
                    ListIncompleteTasks(user, taskList, deletedTasks);
                    break;
                case 3:
                    ListTasksDueBefore(user, taskList, deletedTasks);
                    break;
                case 4:
                    ListTaskByPerson(user, taskList, deletedTasks);
                    break;
                case 5:
                    ListDeletedTasks(user, taskList, deletedTasks);
                    break;
                case 6:
                    AddTask(user, taskList, deletedTasks);
                    break;
                case 7:
                    EditTask(user, taskList, deletedTasks);
                    break;
                case 8:
                    MarkTaskComplete(user, taskList, deletedTasks);
                    break;
                case 9:
                    DeleteTask(user, taskList, deletedTasks);
                    break;
                case 10:
                    break;
            }
        }
        public static void AddTask(string user, List<Task> taskList, List<Task> deletedTasks)
        {   // Prompts for several fields and adds a task to the tasklist screen
            
            //Setting Screen and prompting/getting user input
            SetConsole(user);
            Console.WriteLine();
            Console.WriteLine("     Add a Task:");
            Console.WriteLine("     ===========");
            Console.WriteLine();


            int nextTaskID = taskList.Count + deletedTasks.Count + 1;   //Avoids duplicate task IDs due to deleted tasks being removed from main list, +1 because 1 indexed
            string name = GetUserString("Enter the name of the person to whom this task will be assigned: ");
            string description = GetUserString("Enter the task to be completed: ");
            
            DateTime date = DateTime.Now;
            bool validInput = false;
            while (!validInput)
            {
                validInput = DateTime.TryParse(GetUserString("Enter the due date for this task in mm/dd/yyyy format: "), out date);
            }

            //Add new task
            taskList.Add(new Task(nextTaskID, name, description, date, user));

            SetConsole(user);
            ListAllTasks(user, taskList, deletedTasks);
        }
        public static void EditTask(string user, List<Task> taskList, List<Task> deletedTasks)
        {   //gets user choice on which task to edit:
            int taskChoice = ChooseTaskToEdit(user, taskList);
            bool contRun = true;
            while (contRun)
            {

                SetConsole(user);
                Task.PrintTaskDisplayHeader();
                foreach (Task task in taskList)
                {
                    if (task.TaskID == taskChoice)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Task.PrintTaskToConsole(task);
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
                Console.WriteLine("     Edit a Task:");
                Console.WriteLine("     ===========");
                Console.WriteLine();

                string[] editableFields = { "Who task is assigned to", "Task description", "Due Date" };
                int fieldChoice = DisplayMenu(editableFields);
                Console.WriteLine();
                switch(fieldChoice)
                {
                    case 1:
                        string newTeammate = GetUserString("Please enter a new teammate to assign this task to: ");
                        taskList = Task.ChangeAssignment(taskList, taskChoice, newTeammate, user);  
                        break;
                    case 2:
                        string newDescription = GetUserString("Please enter a new description for this task: ");
                        taskList = Task.ChangeDescription(taskList, taskChoice, newDescription, user);
                        break;
                    case 3:
                        DateTime newDueDate = DateTime.Now;
                        bool validInput = false;
                        while (!validInput)
                        {
                            validInput = DateTime.TryParse(GetUserString("Enter the new due date for this task in mm/dd/yyyy format: "), out newDueDate);
                        }
                        taskList = Task.ChangeDueDate(taskList, taskChoice, newDueDate, user);
                        break;
                }
                contRun = GetYesOrNo("Would you like to edit another field? y/n: ");
            }

            SetConsole(user);
            ListAllTasks(user, taskList, deletedTasks);
        }
        public static int ChooseTaskToComplete (string user, List<Task> taskList)
        {
            //Displays list of all tasks, prompts which task to edit by task ID, Asks for which field to edit
            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach (Task task in taskList)
            {
                if (!task.TaskCompletionStatus)
                {
                    Task.PrintTaskToConsole(task);
                }
            }

            Console.WriteLine();
            Console.WriteLine("     Mark a task complete:");
            Console.WriteLine("     =====================");

            //Prompts for user integer and checks against live task IDs
            int[] taskIDs = Task.GetIncompleteTaskIDs(taskList);
            int choice = 0;
            bool validInput = false;
            while (!validInput)
            {
                choice = GetUserInt("Choose the task to mark complete by entering the Task ID:", 0, 9999999);
                if (taskIDs.Contains(choice))
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("I'm sorry, that is not a current Task ID. Please try again.");
                }

            }
            return choice;
        }
        public static int ChooseTaskToDelete (string user, List<Task> taskList)
        {
            //Displays list of all tasks, prompts which task to edit by task ID, Asks for which field to edit
            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach (Task task in taskList)
            {
                Task.PrintTaskToConsole(task);
            }
            Console.WriteLine();
            Console.WriteLine("     Delete a task:");
            Console.WriteLine("     ==============");
            

            //Prompts for user integer and checks against live task IDs
            int[] taskIDs = Task.GetTaskIDs(taskList);
            int choice = 0;
            bool validInput = false;
            while (!validInput)
            {
                choice = GetUserInt("Choose the task to delete by entering the Task ID:", 0, 9999999);
                if (taskIDs.Contains(choice))
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("I'm sorry, that is not a current Task ID. Please try again.");
                }

            }
            return choice;
        }
        public static int ChooseTaskToEdit (string user, List<Task> taskList)
        {
            //Displays list of all tasks, prompts which task to edit by task ID, Asks for which field to edit
            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach (Task task in taskList)
            {
                Task.PrintTaskToConsole(task);
            }
            Console.WriteLine();
            Console.WriteLine("     Edit a Task:");
            Console.WriteLine("     ==============");
            

            //Prompts for user integer and checks against live task IDs
            int[] taskIDs = Task.GetTaskIDs(taskList);
            int choice = 0;
            bool validInput = false;
            while (!validInput)
            {
                choice = GetUserInt("Choose the task to edit by entering the Task ID:", 0, 9999999);
                if (taskIDs.Contains(choice))
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("I'm sorry, that is not a current Task ID. Please try again.");
                }

            }
            return choice;
        }
        public static void MarkTaskComplete(string user, List<Task> taskList, List<Task> deletedTasks)
        {
            bool check = true;
            while(check)
            { 
                int taskChoice = ChooseTaskToComplete(user, taskList);
                SetConsole(user);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine();
                Console.WriteLine("     Mark a task complete:");
                Console.WriteLine("     =====================");
                Console.WriteLine();
                Console.ResetColor();

                foreach (Task task in taskList)
                {
                    if (task.TaskID == taskChoice)
                    {
                        Task.PrintTaskDisplayHeader();
                        Task.PrintTaskToConsole(task);
                    }
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                check = GetYesOrNo($"Are you sure you want to mark Task {taskChoice} complete? y/n: ");
                Console.ResetColor();
                if(check)
                {
                    taskList = Task.MarkComplete(taskList, taskChoice, user);
                    check = false;
                }
            
            }

            SetConsole(user);
            ListAllTasks(user, taskList, deletedTasks);
        }
        public static void DeleteTask(string user, List<Task> taskList, List<Task> deletedTasks)
        {
            bool check = true;
            while(check)
            { 
                int taskChoice = ChooseTaskToDelete(user, taskList);
                SetConsole(user);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine("     Delete a task:");
                Console.WriteLine("     ==============");
                Console.WriteLine();
                Console.ResetColor();

                foreach (Task task in taskList)
                {
                    if (task.TaskID == taskChoice)
                    {
                        Task.PrintTaskDisplayHeader();
                        Task.PrintTaskToConsole(task);
                    }
                }


                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Are you sure you want to delete task {taskChoice}?");
                Console.ResetColor();
                check = GetYesOrNo("This cannot be undone... y/n: ");
                if(check)
                {
                    deletedTasks = Task.DeleteTaskListAdd(taskList, deletedTasks, taskChoice);
                    taskList = Task.DeleteFromTaskList(taskList, taskChoice);
                    check = false;
                }
            
            }

            SetConsole(user);
            ListAllTasks(user, taskList, deletedTasks);
        }
        public static void ListTaskByPerson(string user, List<Task> taskList, List<Task> deletedTask)
        {   //First section gets unique list of names, presents it as menu to user to get choice
            string[] names = Task.GetTeamNames(taskList);
            SetConsole(user);
            Console.WriteLine("     List tasks by teammate. Please select from the following teammates:");
            int choice = DisplayMenu(names)-1;

            //This section displays the filtered tasks
            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach (Task task in taskList)
            {
                if(task.AssignedTo == names[choice])
                { 
                    Task.PrintTaskToConsole(task); 
                }
            }
            SelectAction(user, taskList, deletedTask);
        }
        public static void ListAllTasks(string user, List<Task> taskList, List<Task> deletedTasks)
        {
            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach(Task task in taskList)
            {
                Task.PrintTaskToConsole(task);
            }
            SelectAction(user, taskList, deletedTasks);
        }
        public static void ListIncompleteTasks(string user, List<Task> taskList, List<Task> deletedTasks)
        {
            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach (Task task in taskList)
            {
                if (!task.TaskCompletionStatus)
                {
                    Task.PrintTaskToConsole(task);
                }
            }
            SelectAction(user, taskList, deletedTasks);
        }
        public static void ListTasksDueBefore(string user, List<Task> taskList, List<Task> deletedTasks)
        {
            SetConsole(user);
            Console.WriteLine("     To display incomplete tasks due before a specified date,");
            string prompt = "     please enter a date in the mm/dd/yyyy format: ";

            DateTime date = DateTime.Now;
            bool validInput = false;
            while (!validInput)
            {
                validInput = DateTime.TryParse(GetUserString(prompt), out date);
            }

            SetConsole(user);
            Task.PrintTaskDisplayHeader();
            foreach (Task task in taskList)
            {
                if (!task.TaskCompletionStatus && task.TaskDueDate < date)
                
                Task.PrintTaskToConsole(task);
            }
            SelectAction(user, taskList, deletedTasks);
        }
        public static void ListDeletedTasks(string user, List<Task> taskList, List<Task> deletedTasks)
        {
            SetConsole(user);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("     DELETED TASKS");
            Console.ResetColor();
            Task.PrintTaskDisplayHeader();
            foreach (Task task in deletedTasks)
            {
                Task.PrintTaskToConsole(task);
            }
            SelectAction(user, taskList, deletedTasks);
        }
        public static int DisplayMenu(string[] menuItems)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("   ========================================");
            Console.WriteLine("   Please select from the folowing options:");
            for(int i = 0; i < menuItems.Length; i++)
            {
                Console.WriteLine($"    {i+1}.\t{menuItems[i]}");
            }
            int selection = GetUserInt("", 1, menuItems.Length);
            return selection;
        }
        public static void SetConsole(string user)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{user}\t\t\t\t\tGC Task List\t\t\t\t\t{DateTime.Now.ToLongDateString()}");
            Console.WriteLine();
            Console.ResetColor();
        }
        public static string GetUserName()
        {
            //Greets user and prompts for username
            Console.WriteLine("Welcome to the Grand Circus Task List!");
            Console.WriteLine("======================================");
            string userName = GetUserString("Please enter your name: ");
            return userName;
        }
        public static List<Task> GenerateStartingTaskList()
        {
            List<Task> tasks = new List<Task>()
            {
                new Task(1,"Roy Miller", "Create task list", DateTime.Parse("05/04/2020"), true, "Roy", DateTime.Parse("05/01/2020")),
                new Task(2,"Amy Ansel", "New water cooler", DateTime.Parse("05/15/2020"), false, "Mary", DateTime.Parse("05/01/2020")),
                new Task(5,"Mary Wentz", "Update birthday calendar", DateTime.Parse("05/08/2020"),false, "Mary", DateTime.Parse("05/01/2020")),
                new Task(6,"Mary Wentz", "Test task tracker", DateTime.Parse("05/08/2020"), true, "Roy", DateTime.Parse("05/01/2020")),
                new Task(8,"Ando Clandier", "Scan last year's invoices", DateTime.Parse("06/16/2020"),false, "Mary", DateTime.Parse("05/01/2020")),
                new Task(9,"Ananda Blonde", "Find new vendor for signage", DateTime.Parse("07/03/2020"),false, "Mary", DateTime.Parse("05/01/2020"))
            };
            return tasks;
        }
        public static List<Task> GenerateStartingDeletedTaskList()
        {
            List<Task> deletedTasks = new List<Task>()
            {
                new Task(3,"Cranny Hamelson", "Purchase scanner", DateTime.Parse("05/08/2020"),false, "Mary", DateTime.Parse("05/01/2020")),
                new Task(4,"Cranny Hamelson", "Find new vendor for signage", DateTime.Parse("07/03/2020"), false,"Mary", DateTime.Parse("05/01/2020")),
                new Task(7,"Mary Wentz", "Fire Cranny", DateTime.Parse("05/08/2020"),false, "Mary", DateTime.Parse("05/01/2020"))
            };
            return deletedTasks;
        }
        public static string GetUserString(string prompt)
        {   //asks user for input with prompt string
            //validates against empty string
            while(true)
            {
                Console.WriteLine();
                Console.WriteLine(prompt);
                string userInput = Console.ReadLine();

                if(String.IsNullOrWhiteSpace(userInput))
                { 
                    Console.WriteLine();
                    Console.WriteLine("I'm sorry, input cannot be empty.");
                }
                else
                {
                    return userInput;
                }
            }
        }
        public static int GetUserInt(string prompt, int lowerBound, int upperBound)
        {   //Prompts user for an int between two values, validates, returns
            while (true)
            {
                try
                {
                    Console.WriteLine(prompt);
                    int userNum = int.Parse(Console.ReadLine());
                    if (userNum >= lowerBound && userNum <= upperBound)
                    {
                        return userNum;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Input out of bounds. A whole number between {lowerBound} and {upperBound} is required: ");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine();
                    Console.WriteLine("I'm sorry, your input wasn't an integer.");
                }
                catch (Exception)
                {
                    Console.WriteLine();
                    Console.WriteLine("I'm sorry, your input wasn't valid.");
                }

            }

        }
        public static bool GetYesOrNo(string prompt)
        {
            //Prompts user for y/n; returns true for y and false for n
            while (true)
            {
                Console.WriteLine(prompt);
                string input = Console.ReadLine().ToLower();

                if (input == "y")
                    return true;
                else if (input == "n")
                    return false;
                else
                    Console.WriteLine("I'm sorry, I didn't get that.");
            }
        }
    }
}
