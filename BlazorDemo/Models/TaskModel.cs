namespace BlazorDemo.Models;

public class TaskModel
{
    public string Text { get; set; } = string.Empty; // Task name
    public TaskListModel TaskList { get; set; } // Reference to the task list it belongs to
}