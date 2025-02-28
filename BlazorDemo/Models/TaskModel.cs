namespace BlazorDemo.Models;
public class TaskModel
{
    public string Text { get; set; } = string.Empty; // Název úkolu
    public TaskType Type { get; set; } = TaskType.ToDo; // Typ úkolu (stav)
}
