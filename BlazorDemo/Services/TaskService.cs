using BlazorDemo.Models;
using System.Text.Json;

namespace BlazorDemo.Services
{
    public class TaskService
    {
        private List<TaskListModel> taskLists = new List<TaskListModel>(); // List všech seznamů úkolů
        private readonly string _filePath = "Data/tasks.json"; // Cesta k souboru pro ukládání dat

        public TaskListModel DefaultTaskList { get; private set; } // Výchozí seznam úkolů
        public TaskService()
        {

            // Pokud neexistuje žádný seznam úkolů, vytvořte výchozí seznam
            if (!taskLists.Any())
            {
                DefaultTaskList = new TaskListModel { Name = "ToDo" }; // Vytvoření výchozího seznamu úkolů
                taskLists.Add(DefaultTaskList);      // Přidání výchozího seznamu do seznamu úkolů
            }
            else
            {
                // Nastavení výchozího seznamu úkolů pokud již existuje
                DefaultTaskList = taskLists.FirstOrDefault(tl => tl.Name == "ToDo") ?? taskLists.First(); 
            }
        }

        // Metoda pro získání všech seznamů úkolů
        public List<TaskListModel> GetTaskLists()
        {
            return taskLists;
        }

        // Metoda pro přidání nového seznamu úkolů
        public void AddTaskList(string name)
        {
            taskLists.Add(new TaskListModel { Name = name });
        }

        // Metoda pro přidání úkolu do seznamu
        public void AddTask(string taskListName, string taskText)
        {
            var taskList = taskLists.FirstOrDefault(tl => tl.Name == taskListName);
            if (taskList != null)
            {
                taskList.Tasks.Add(new TaskModel { Text = taskText });
            }
        }

        // Metoda pro přesunutí úkolu do jiného seznamu
        public void MoveTask(TaskModel task, string targetTaskListName)
        {
            var sourceTaskList = taskLists.FirstOrDefault(tl => tl.Name == task.TaskListName);

            if (sourceTaskList != null)
            {
                sourceTaskList.Tasks.Remove(task);
                var targetTaskList = taskLists.FirstOrDefault(tl => tl.Name == targetTaskListName);
                if (targetTaskList != null)
                {
                    targetTaskList.Tasks.Add(task);
                    task.TaskListName = targetTaskListName;
                }
            }
        }

        // Metoda pro smazání seznamu úkolů
        public void RemoveTaskList(string name)
        {
            var taskList = taskLists.FirstOrDefault(tl => tl.Name == name);
            if (taskList != null)
            {
                taskLists.Remove(taskList);
            }
        }

        public void UpdateTaskListName(TaskListModel updatedTaskList)
        {
            if (string.IsNullOrWhiteSpace(updatedTaskList.Name))
            {
                throw new ArgumentException("Název task listu nemůže být prázdný.");
            }

            if (taskLists.Any(tl => tl != updatedTaskList && tl.Name == updatedTaskList.Name))
            {
                throw new InvalidOperationException($"Task List s názvem '{updatedTaskList.Name}' už existuje.");
            }

            var existingTaskList = taskLists.FirstOrDefault(t => t == updatedTaskList);
            if (existingTaskList != null)
            {
                existingTaskList.Name = updatedTaskList.Name;
            }
        }

        // Metoda pro smazání úkolu ze seznamu
        public void RemoveTask(string taskListName, TaskModel task)
        {
            var taskList = taskLists.FirstOrDefault(tl => tl.Name == taskListName);
            if (taskList != null)
            {
                taskList.Tasks.Remove(task);
            }
        }

        // Metoda pro ukládání dat do JSON souboru
        public async Task SaveTasksAsync()
        {
            var options = new JsonSerializerOptions { WriteIndented = true }; // Formátování JSONu
            string json = JsonSerializer.Serialize(taskLists, options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        // Metoda pro načítání dat z JSON souboru
        public async Task LoadTasksAsync()
        {
            if (File.Exists(_filePath))
            {
                string json = await File.ReadAllTextAsync(_filePath);
                taskLists = JsonSerializer.Deserialize<List<TaskListModel>>(json) ?? new List<TaskListModel>();
            }
        }
    }
}