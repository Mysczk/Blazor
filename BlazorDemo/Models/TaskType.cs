namespace BlazorDemo.Models
{
    
    public enum TaskType
    {
        Backlog,      // Čeká na zpracování
        ToDo,         // Naplánované úkoly
        InProgress,   // Právě probíhá
        Review,       // Čeká na schválení
        Done          // Hotovo
    }
}