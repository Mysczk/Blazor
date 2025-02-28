
namespace BlazorDemo.Models
{
    public class TaskService
    {
        private List<TaskModel> tasks = new List<TaskModel>();

        // Metoda pro získání všech úkolů
        public List<TaskModel> GetTasks()
        {
            return tasks;
        }

        // Metoda pro přidání nového úkolu
        public void AddTask(string text, TaskType type)
        {
            tasks.Add(new TaskModel { Text = text, Type = type });
        }

        // Metoda pro přesunutí úkolu do jiného stavu
        public void MoveTask(TaskModel task, TaskType newType)
        {
            task.Type = newType;
        }
    }
}