**Blazor**

Projekt pro předmět GUI zaměřený .NET Blazor

# Založení projektu pomocí CMD
```
    dotnet new blazor -n [název_projektu] -f net8.0
```
# Založení pomocí Visual studia
* Výběr templatu
![Výber šablony](docs/images/vscreate.png)

* Tvorba projektu
![Popis projektu](docs/images/vscreatetwo.png)

* Technická specifika projektu
![Specifika projektu](docs/images/vscreatethree.png)

---

# Blazor TODO Aplikace

Tato aplikace je jednoduchý **TODO seznam** vytvořený v Blazor Server. Umožňuje uživateli přidávat, upravovat, mazat a přesouvat úkoly mezi seznamy.

## Jak spustit projekt
1. Naklonuj repozitář:
```bash
git clone https://github.com/Mysczk/Blazor.git
cd Blazor/BlazorDemo
```
2. Spusť Blazor Server aplikaci:
```bash
dotnet watch run
```
3. Otevři v prohlížeči *http://localhost:5000*

---

## Funkcionalita
- Přidání nového úkolu
- Přidání nového seznamu úkolů
- Úprava úkolu (název, popis, termín)
- Odstranění úkolu
- Přesouvání úkolů mezi seznamy
- Dynamické rozhraní s [Blazor Bootstrap](https://demos.blazorbootstrap.com/buttons)

---

## Struktura projektu

```plaintext
/BlazorDemo
│── /Components       # UI komponenty a stránky aplikace
│   ├── /Layout       # Rozvržení aplikace
│   │   ├── MainLayout.razor
│   │   ├── NavMenu.razor
│   ├── /Pages        # Stránky aplikace
│   │   ├── Home.razor        # Hlavní stránka
│   │   ├── Todo.razor        # Stránka s Todo aplikací
│   ├── /Shared       # Sdílené komponenty
│   │   ├── TaskItem.razor        # Komponenta pro jednotlivý úkol
│   │   ├── TaskList.razor        # Komponenta pro seznam úkolů
│   │   ├── AddTaskForm.razor     # Komponenta pro přidání úkolu
│   │   ├── TaskItemDetails.razor # Sidebar s detaily úkolu
│   ├── _Imports.razor  # Globální importy Razor komponent
│   ├── App.razor       # Root aplikace
│   ├── Routes.razor    # Definice routování
│── /Data              # Ukládání a správa dat
│   ├── tasks.json     # JSON soubor s uloženými úkoly
│── /Models            # Datové modely
│   ├── TaskListModel.cs  # Model pro seznam úkolů 
│   ├── TaskModel.cs      # Model pro jednotlivý úkol
│── /Services          # Aplikační logika a služby
│   ├── TaskService.cs # Správa úkolů (přidávání, mazání, přesouvání)
│── /wwwroot           # Statické soubory (CSS, obrázky)
│   ├── styles.css     # Vlastní styly aplikace
│── appsettings.Development.json  # Nastavení pro vývoj
│── appsettings.json               # Konfigurace aplikace
│── BlazorDemo.csproj    # Projektový soubor
│── Program.cs           # Hlavní vstupní bod aplikace
│── Blazor.sln           # Solution soubor

```

---

## Sequence diagram

```mermaid
sequenceDiagram
    participant User as Uživatel
    participant AddTaskForm as AddTaskForm.razor
    participant TaskList as TaskList.razor
    participant TaskService as TaskService.cs

    User->>TaskList: Klikne na tlačítko "+ Přidat úkol"
    TaskList-->>User: Zobrazí AddTaskForm komponentu
    User->>AddTaskForm: Napíše Task název a klikne na "Přidat úkol"
    AddTaskForm->>TaskList: await OnTaskAdded.InvokeAsync(newTaskText);
    TaskList->>TaskService: TaskService.AddTask(taskListModel.Name, newTaskText);
    TaskService-->>TaskList: Aktualizuje seznam úkolů
    TaskList-->>User: Zobrazí aktualizovaný seznam úkolů
```

---

## Ukázky kódu pro přidání seznamu úkolů

### Komponenta `AddTaskListForm.razor`
```razor
<div class="add-task-form">
    <input @bind="newTaskListName" placeholder="Zadejte název seznamu" @bind:event="oninput" @onkeypress="HandleKeyPress" />
    <Button @onclick="AddTaskList" Color="ButtonColor.Primary">Přidat seznam</Button>

    @if (!string.IsNullOrEmpty(errorMessage))
    {
        <p class="error">@errorMessage</p>
    }
</div>

@code {
    [Parameter]
    public EventCallback<string> OnTaskListAdded { get; set; }

    [Parameter]
    public List<TaskListModel> ExistingTaskLists { get; set; } = new();

    private string newTaskListName = string.Empty;
    private string? errorMessage;

    private async Task AddTaskList()
    {
        if (string.IsNullOrWhiteSpace(newTaskListName))
        {
            errorMessage = "Název seznamu nemůže být prázdný.";
            return;
        }

        if (ExistingTaskLists.Any(tl => tl.Name.Equals(newTaskListName, StringComparison.OrdinalIgnoreCase)))
        {
            errorMessage = $"Seznam '{newTaskListName}' již existuje!";
            return;
        }

        await OnTaskListAdded.InvokeAsync(newTaskListName);
        newTaskListName = string.Empty;
        errorMessage = null; // Resetujeme chybu
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await AddTaskList();
        }
    }
}
```
### Řešení přidání seznamu úkolů na stránce `Todo.razor`
```razor
    <div class="add-task-list-column">
        @if (showAddTaskListInput)
        {
            <AddTaskListForm OnTaskListAdded="HandleTaskListAdded" ExistingTaskLists="@GetTaskLists()" />
        }
        else
        {
            <Button @onclick="ShowAddTaskListInput" 
                    Color="ButtonColor.Primary" 
                    Size="ButtonSize.Medium"
                    Class="w-100 d-flex align-items-center justify-content-center">
                <Icon Name="IconName.Plus" Class="me-2" /> Přidat seznam
            </Button>
        }
    </div>

@code {
    private void HandleTaskListAdded(string newTaskListName)
    {
        TaskService.AddTaskList(newTaskListName);
        HideAddTaskListInput();
    }
}
```
### Přidání úkolu v `TaskService.cs`
```csharp
 public class TaskService
    {
        private List<TaskListModel> taskLists = new List<TaskListModel>(); // List všech seznamů úkolů | List of all task lists
        public void AddTaskList(string name)
        {
            taskLists.Add(new TaskListModel { Name = name });
        }
    }
```
---

# Cvičení 1: Jednoduchý poznámkový blok

### Zadání
Vytvoř novou Blazor stránku, která umožní uživateli:
1. **Přidat** novou poznámku.
2. **Smazat** existující poznámku.
3. **Upravit** existující poznámku.


<details>
  <summary>💡 Nápověda</summary>

- Použij **`@bind`** k obousměrnému svázání vstupu.
- Ulož poznámky do **`List<string>`** a vykresli je pomocí **`@foreach`**.

</details>

