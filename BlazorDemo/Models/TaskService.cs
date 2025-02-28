namespace BlazorDemo.Models
{
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
        public void AddTaskList(string name)
        {
            taskLists.Add(new TaskListModel { Name = name });
        }

        // Method to add a new task to a specific task list
        public void AddTask(string text, TaskListModel taskList)
        {
            taskList.Tasks.Add(new TaskModel { Text = text, TaskList = taskList });
        }

        // Method to move a task to a different task list
        public void MoveTask(TaskModel task, TaskListModel newTaskList)
        {
            task.TaskList.Tasks.Remove(task);
            task.TaskList = newTaskList;
            newTaskList.Tasks.Add(task);
        }
    }
}