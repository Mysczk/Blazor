using BlazorDemo.Models;
namespace BlazorDemo.Services;
public class TaskService
{
    private List<TaskListModel> taskLists = new List<TaskListModel>();
    public TaskListModel DefaultTaskList { get; private set; }

    public TaskService()
    {
        // Initialize the default task list
        DefaultTaskList = new TaskListModel { Name = "ToDo" };
        taskLists.Add(DefaultTaskList);
    }

    // Method to get all task lists
    public List<TaskListModel> GetTaskLists()
    {
        return taskLists;
    }

    // Method to add a new task list
    private string errorMessage = string.Empty;

    public void AddTaskList(string newTaskListName)
    {
        if (taskLists.Any(tl => tl.Name == newTaskListName))
        {
            errorMessage = $"Task List '{newTaskListName}' už existuje!";
            return;
        }

        taskLists.Add(new TaskListModel { Name = newTaskListName });
        newTaskListName = string.Empty;
    }

    public void RemoveTaskList(TaskListModel taskList)
    {
        taskLists.Remove(taskList);
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


    // Method to add a new task to a specific task list
    public void AddTask(string text, TaskListModel taskList)
    {
        taskList.Tasks.Add(new TaskModel { Text = text, TaskList = taskList });
    }

    // Method to move a task to a different task list
    public void MoveTask(TaskModel task, TaskListModel newTaskList)
    {
        if (task.TaskList != null)
        {
            task.TaskList.Tasks.Remove(task);
        }
        task.TaskList = newTaskList;
        newTaskList.Tasks.Add(task);
    }
}
