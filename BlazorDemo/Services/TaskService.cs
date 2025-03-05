using BlazorDemo.Models;
using System.Text.Json;

namespace BlazorDemo.Services
{
    public class TaskService
    {
        private List<TaskListModel> taskLists = new List<TaskListModel>();
        private readonly string _filePath = "Data/tasks.json"; // Cesta k souboru pro ukládání dat

        public TaskListModel DefaultTaskList { get; private set; }
        public TaskService()
        {

            // Pokud neexistuje žádný seznam úkolů, vytvořte výchozí seznam
            if (!taskLists.Any())
            {
                DefaultTaskList = new TaskListModel { Name = "ToDo" };
                taskLists.Add(DefaultTaskList);
            }
            else
            {
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
          //  SaveTasksAsync().Wait(); // Uložte data po přidání seznamu
        }

        // Metoda pro přidání úkolu do seznamu
        public void AddTask(string taskListName, string taskText)
        {
            Console.WriteLine("AddTask in TaskService");
            var taskList = taskLists.FirstOrDefault(tl => tl.Name == taskListName);
            if (taskList != null)
            {
                taskList.Tasks.Add(new TaskModel { Text = taskText });
            //    SaveTasksAsync().Wait(); // Uložte data po přidání úkolu
            }
        }

        // Metoda pro přesunutí úkolu do jiného seznamu
        public void MoveTask(TaskModel task, string targetTaskListName)
        {
            var sourceTaskList = taskLists.FirstOrDefault(tl => tl.Name == task.TaskListName);

            if (sourceTaskList != null)
            {
                var targetTaskList = taskLists.FirstOrDefault(tl => tl.Name == targetTaskListName);
                if (targetTaskList != null)
                {
                    sourceTaskList.Tasks.Remove(task);
                    targetTaskList.Tasks.Add(task);
                    task.TaskListName = targetTaskListName;
                //    SaveTasksAsync().Wait(); // Uložte data po přesunutí úkolu
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
           //     SaveTasksAsync().Wait(); // Uložte data po smazání seznamu
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
          //      SaveTasksAsync().Wait(); // Uložte data po aktualizaci názvu
            }
        }

        // Metoda pro smazání úkolu ze seznamu
        public void RemoveTask(string taskListName, TaskModel task)
        {
            var taskList = taskLists.FirstOrDefault(tl => tl.Name == taskListName);
            if (taskList != null)
            {
                taskList.Tasks.Remove(task);
             //   SaveTasksAsync().Wait(); // Uložte data po smazání úkolu
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