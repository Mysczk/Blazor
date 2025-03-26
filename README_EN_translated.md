**Blazor**

# Exercise 1: Simple Notepad

## Solution:

#### Description
- This Blazor component implements a simple notepad where the user can add, edit, and delete notes.

---

### Component Code `Notepad.razor`

```razor
@page "/notepad"
<PageTitle>Poznámkový blok</PageTitle>
<h3>Poznámkový blok</h3>

<div class="mb-3">
    <TextAreaInput @bind-Value="newNoteText" Rows="3" MaxLength="350" Placeholder="Zadejte poznámku..."/>
</div>
<Button @onclick="AddNote" Color="ButtonColor.Primary" class="mt-2">Přidat</Button>

<div class="mb-3"></div>
<ul class="list-group">
    @foreach (var note in Notes)
    {
        <li class="list-group-item d-flex justify-content-between align-items-center">
            @if (note.IsEditing)
            {
                <TextInput @bind-Value="note.Text" class="form-control me-2" />
                <Button @onclick="() => SaveEdit(note)" Color="ButtonColor.Success" class="me-2">Uložit</Button>
            }
            else
            {
                <div class="note-text">
                    @note.Text
                </div>
                <div>
                    <Button @onclick="() => EditNote(note)" Color="ButtonColor.Warning" class="me-2">✏️</Button>
                    <Button @onclick="() => RemoveNote(note)" Color="ButtonColor.Danger">🗑️</Button>
                </div>
            }
        </li>
    }
</ul>

<style>
    .note-text {
        max-height: 100px; 
        overflow: auto;
        white-space: pre-wrap;
        word-wrap: break-word;
    }
</style>

@code {
    private List<Note> Notes = new();
    private string newNoteText = "";
 
    private void AddNote()
    {
        if (!string.IsNullOrWhiteSpace(newNoteText))
        {
            Notes.Add(new Note { Text = newNoteText });
            newNoteText = "";
        }
    }

    private void RemoveNote(Note note)
    {
        Notes.Remove(note);
    }

    private void EditNote(Note note)
    {
        note.IsEditing = true;
    }

    private void SaveEdit(Note note)
    {
        note.IsEditing = false;
    }

    private class Note
    {
        public string Text { get; set; } = "";
        public bool IsEditing { get; set; } = false;
    }
}
```
### Code explanation

#### 1. Class `Note`

Class `Note` represents every single note. Containst 2 properties:
 - `Text` (string) - note content
 - `IsEditing` (bool) - indication whether it is in edit mode

```csharp
private class Note
{
    public string Text { get; set; } = "";
    public bool IsEditing { get; set; } = false;
}
```

#### 2. List of Notes
- The component uses a `Notes` list that stores instances of the `Note` class.
- This list represents the main data structure of the application:

```csharp
private List<Note> Notes = new();
```

#### 3. Adding a Note

- The AddNote method creates a new instance of the `Note` class if `newNoteText` is not an empty string, and adds it to the `Notes` list:

```csharp

private void AddNote()
{
    if (!string.IsNullOrWhiteSpace(newNoteText))
    {
        Notes.Add(new Note { Text = newNoteText });
        newNoteText = "";
    }
}
```
 
- This way, each note is kept as a separate object in the list.

#### 4. Removing a Note

- The `RemoveNote` method removes a specific note from the list:

```csharp
private void RemoveNote(Note note)
{
    Notes.Remove(note);
}
```

- When the 🗑️ button is clicked, the corresponding `Note` instance is removed from the `Notes` list.

#### 5. Editing a Note

- When the user clicks the ✏️ button, the `EditNote` method sets the `IsEditing` property to `true`, thus activating editing mode:

```csharp
private void EditNote(Note note)
{
    note.IsEditing = true;
}
```

- This will change the rendering of the list item and display a text box instead of static text:

```razor
 @if (note.IsEditing)
    {
        <TextInput @bind-Value="note.Text" class="form-control me-2" />
        <Button @onclick="() => SaveEdit(note)" Color="ButtonColor.Success" class="me-2">Uložit</Button>
    }
else
    {
        <div class="note-text">
            @note.Text
        </div>
    }
```

#### 6. Saving an edited note

- The `SaveEdit` method disables editing mode by setting `IsEditing` to `false`, which displays saved changes as plain text:

```razor
private void SaveEdit(Note note)
{
    note.IsEditing = false;
}
```
---


# Exercise 2: Image Gallery
Create a Blazor page that allows you to:
1. **Load images**
2. **Render images**
3. **View a specific image**
4. **Keyboard control for viewing images**
4. **Subtask**: Mansory layout using Blazor and pure CSS


<details>
  <summary>💡 Help</summary>

- Load images using **Service** and render using a loop in **HTML** tags
- Create an overlay over an existing gallery to open the image
- For keyboard control you will need to register keyboard inputs using **`KeyboardEventArgs`**
</details>

<details>
  <summary>Part of solution</summary>

1. We will create a razor page where we will call our gallery
```razor
@page "/gallery"

<h3>Galerie</h3>

<ImageGrid />
```

2. Create the component `ImageGrid.razor`
    - the name is optional, but needs to be edited in the previous step

    <details>
        <summary>Code</summary>

    ```csharp
    @code {
        private List<string> images = new List<string>();
        private int selectedIndex = -1;
        private ElementReference overlayElement;
        private string? path;


        protected override async Task OnInitializedAsync()
        {
            if (path == null)
            {
                images = await ImageService.GetImagePathsAsync("wwwroot/images/gallery");
            }
            else 
            { 
                images = await ImageService.GetImagePathsAsync(path);
            }
        }

        private async Task OpenImage(int index)
        {
            selectedIndex = index;
            StateHasChanged();
            await FocusOverlay();
        }

        private async Task FocusOverlay()
        {
            if (selectedIndex != -1)
            {
                await Task.Delay(50);
                await overlayElement.FocusAsync();
            }
        }

        private void CloseImage()
        {
            selectedIndex = -1;
        }

        private void PreviousImage()
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
                StateHasChanged();
            }
            else
            {
                selectedIndex = images.Count - 1;
                StateHasChanged();
            }
        }

        private void NextImage()
        {
            if (selectedIndex < images.Count - 1)
            {
                selectedIndex++;
                StateHasChanged();
            }
            else
            {
                selectedIndex = 0;
                StateHasChanged();
            }
        }

        private async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (selectedIndex != -1)
            {
                if (e.Key == "ArrowLeft")
                {
                    PreviousImage();
                }
                else if (e.Key == "ArrowRight")
                {
                    NextImage();
                }
                else if (e.Key == "Escape")
                {
                    CloseImage();
                }
                await InvokeAsync(StateHasChanged);
            }
        }
    }
    ```
    </details>

    ## The code here is a bit longer, but let's break it down:

    ### `OnInitializedAsync`
    - Runs automatically when the component is initialized.

    ### `OpenImage`
    - Opens a preview of a specific image.

    ### `FocusOverlay`
    - Focuses the overlay element for keyboard control.

    ### `CloseImage`
    - Closes the image preview (overlay).

    ### `PreviousImage`
    - Moves the selection to the previous image.

    ### `NextImage`
    - Moves the selection to the next image.

    ### `HandleKeyDown`
    - Handles key presses when the overlay is active.

    ## Still in our component, let's make the page structure:


    <details>
        <summary>Code</summary>


    ```csharp
    @if (images.Count == 0)
    {
        <p>No pictures available.</p>
    }
    else
    {
        <div class="gallery">
            @foreach (var img in images.Select((path, index) => new { path, index })) 
            {
                <div class="gallery-item">
                    <img src="@img.path" @onclick="() => OpenImage(img.index)" /> <!-- Creation of individual image and assigning open method -->
                </div>
            }
        </div>
    }

    @if (selectedIndex != -1)
    {
        <div class="gal-overlay" tabindex="0" @onkeydown="HandleKeyDown" @ref="overlayElement">
            <button class="gal-nav-btn left" @onclick="PreviousImage">&#9665;</button>

            <img src="@images[selectedIndex]" @onclick="CloseImage" />

            <button class="gal-nav-btn right" @onclick="NextImage">&#9655;</button>
        </div>
    }
    ``` 
    </details>

    ## Still in our component, let's make the page look:


    <details>
        <summary>Code</summary>

    ```css
    <style>
        .gallery {
            column-count: 5;
            column-gap: 1rem;
            padding: 1rem;
        }
        @@media (max-width: 1200px) {
            .gallery {
                column-count: 4;
            }
        }

        @@media (max-width: 992px) {
            .gallery {
                column-count: 3;
            }
        }

        @@media (max-width: 768px) {
            .gallery {
                column-count: 2;
            }
        
        }

        .gallery-item {
            break-inside: avoid;
            margin-bottom: 1rem;
            background-color: #f8f8f8;
            border-radius: 0.5rem;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        }

        .gallery-item img {
            width: 100%;
            display: block;
        }

        .gallery img {
            transition: transform 0.2s;
        }

        .gallery img:hover {
            transform: scale(1.05);
        }

        .gal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .gal-overlay img {
            max-width: 80%;
            max-height: 80%;
            border-radius: 10px;
        }

        .gal-nav-btn {
            position: absolute;
            top: 50%;
            transform: translateY(-50%);
            background: rgba(255, 255, 255, 0.5);
            border: none;
            font-size: 24px;
            cursor: pointer;
            padding: 10px;
            border-radius: 50%;
        }

        .gal-nav-btn.left {
            left: 20px;
        }

        .gal-nav-btn.right {
            right: 20px;
        }
    </style>
    ```
    </details>

    ## The page is almost done, but let's not forget about the images:
    - We will create a service `ImagesService.cs` in the `Services` folder

    <details>
        <summary>Code</summary>


    ```csharp

        public class ImageService
        {
            public Task<List<string>> GetImagePathsAsync(string path)
            {
                var images = new List<string>();

                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path, "*.jpg"); 
                    foreach (var file in files)
                    {
                        images.Add($"images/gallery/{Path.GetFileName(file)}");
                    }
                }

                return Task.FromResult(images);
            }
        }
    ```
    - Service finds all `.jpg` photos in a given folder and stores their paths in `List<string>`
    </details>

    ## Last 2 steps
    - For our `Service` to work properly, we need to register it in the `Program.cs` file and link it to our `ImageGrid.razor` component
    ### Registration:
    ```csharp
    builder.Services.AddSingleton<ImageService>();
    ```

    ### Linking:
    - At the very top of our component file, we use:
    ```blazor
    @inject ImageService ImageService
    ```
    - This will link an instance of our `Service` to our component
    </details>

# Task: Automatic Image Carousel in Blazor

## Assignment

Create a component in Blazor that automatically displays images from a given folder as a carousel. The images should change every 3 seconds, with the user being able to navigate to the previous or next image using buttons.

---

## Component code and its explanation

### 1. Imports and environment

```razor
@using Microsoft.AspNetCore.Hosting
@using Microsoft.Extensions.Hosting

@inject IWebHostEnvironment env
```

Here we import the libraries needed to access the application environment. `@inject` allows us to access the project's web root (`wwwroot`) so we can load files.

---

### 2. Parameters and Variables

```razor
[Parameter]
public string WorkFolder { get; set; } = "images/gallery";

private int activeIndex = 0;
private List<string> images = new List<string>();
private System.Timers.Timer? timer;
```

- `WorkFolder` – path to the folder with images (relative to `wwwroot`).
- `activeIndex` – index of the current image.
- `images` – list of images (paths to them).
- `timer` – timer for automatic image switching.

---

### 3. Component initialization

```razor
protected override void OnInitialized()
{
    string fullPath = Path.Combine(env.WebRootPath, WorkFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));
    if (Directory.Exists(fullPath))
    {
        images = Directory.GetFiles(fullPath, "*.jpg").ToList();
        images = images.Select(img => Path.Combine("/", WorkFolder, Path.GetFileName(img))).ToList();
    }

    timer = new System.Timers.Timer(3000); // každé 3 sekundy
    timer.Elapsed += (sender, e) => InvokeAsync(Next);
    timer.AutoReset = true;
    timer.Start();
}
```

- Joins the path to the `WorkFolder` folder to the root of the site.
- Loads all `.jpg` files and converts them to relative paths.
- Starts a timer that calls the `Next()` function every 3 seconds.

---

### 4. Image change function

```razor
private void Next()
{
    activeIndex = (activeIndex + 1) % images.Count;
    timer?.Stop();
    timer?.Start();
    StateHasChanged(); // aktualizace UI
}

private void Previous()
{
    activeIndex = (activeIndex - 1 + images.Count) % images.Count;
    timer?.Stop();
    timer?.Start();
    StateHasChanged();
}
```

- `Next()` – switches to the next image.
- `Previous()` – switches to the previous image.
- `StateHasChanged()` – causes the UI component to redraw.

---

### 5. Disposing of component

```razor
public void Dispose()
{
    timer?.Dispose();
}
```

When the component is removed, the timer is canceled to avoid memory leaks.

---

### 6. Component HTML Template

```razor
<div class="carousel">
    <button class="prev" @onclick="Previous">❮</button>
    <img src="@images[activeIndex]" class="carousel-image" />
    <button class="next" @onclick="Next">❯</button>
</div>
```


HTML structure of the component: navigation buttons and image according to the current index.

---

### 7. Carousel styling

```razor
<style>
    .carousel {
        position: relative;
        width: 800px;
        height: 400px;
        margin: auto;
    }

    .carousel-image {
        width: 100%;
        height: 100%;
        object-fit: cover;
        border-radius: 10px;
    }

    .prev,
    .next {
        position: absolute;
        top: 50%;
        transform: translateY(-50%);
        background: rgba(0, 0, 0, 0.5);
        color: white;
        border: none;
        padding: 10px;
        cursor: pointer;
    }

    .prev {
        left: 10px;
    }

    .next {
        right: 10px;
    }
</style>
```

The style defines the appearance of the carousel – size, alignment, appearance of the image and buttons.

---

# Task: Stopwatch in Blazor

## Assignment

Create a stopwatch component in Blazor that will allow starting, stopping, resetting and recording split times (laps). The component should display the elapsed time and a list of saved laps. It should contain buttons for control and be styled as a simple UI application.

---

## Component code and its explanation

### 1. Header and interface

```razor
@page "/stopwatch"
@implements IDisposable
```

- `@page` defines the URL where the component is available.
- `IDisposable` is used for proper resource (timer) disposal.

---

### 2. C# part – stopwatch logic

```razor
@code {
    private bool isRunning = false;
    private TimeSpan elapsed = TimeSpan.Zero;
    private System.Timers.Timer timer;
    private DateTime startTime;
    private List<string> laps = new();

    private string formattedTime => elapsed.ToString(@"hh\:mm\:ss\.ff");
```

- `isRunning` keeps track of whether the stopwatch is running.
- `elapsed` stores the total elapsed time.
- `timer` is a timer that updates the time.
- `laps` is a list of recorded lap times.

---

### 3. Initialization and Startup

```razor
protected override void OnInitialized()
{
    timer = new System.Timers.Timer(100);
    timer.Elapsed += OnTimerElapsed;
}
```

- The timer is set to an interval of 100 ms.
- Each tick calls the `OnTimerElapsed()` method.

---

### 4. Starting and stopping the stopwatch

```razor
private void ToggleTimer()
{
    if (isRunning)
    {
        timer.Stop();
        elapsed += DateTime.Now - startTime;
    }
    else
    {
        startTime = DateTime.Now;
        timer.Start();
    }
    isRunning = !isRunning;
}
```

- Starts or stops the stopwatch and adjusts the elapsed time.

---

### 5. Continuous time update

```razor
private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
{
    var current = DateTime.Now - startTime + elapsed;
    InvokeAsync(() =>
    {
        elapsed = current;
        StateHasChanged();
    });
}
```

- Recalculates time and updates UI.

---

### 6. Reset and Laps

```razor
private void Reset() { ... }
private void AddLap() { ... }
private void RemoveLap(string lap) { ... }
private void ClearLaps() { ... }
```

- `Reset()` – resets the stopwatch.
- `AddLap()` – saves the current time to the list.
- `RemoveLap()` – deletes the record.
- `ClearLaps()` – clears all laps.

---

### 7. Releasing the timer

```razor
public void Dispose()
{
    timer?.Dispose();
}
```

- Prevents memory leaks.

---

### 8. HTML Template

```razor
<div class="stopwatch">
    <p>Time: @formattedTime</p>
    <button @onclick="ToggleTimer">@(isRunning ? "⏸ Pause" : "▶ Start")</button>
    <button @onclick="Reset">⏹ Reset</button>
    <button @onclick="AddLap" disabled="@(!isRunning)">📍 Add Lap</button>
    <button @onclick="ClearLaps">Clear Laps</button>

    @if (laps.Count > 0)
    {
        <h4>Laps:</h4>
        <ul>
            @foreach (var lap in laps)
            {
                <li>
                    @lap
                    <button @onclick="() => RemoveLap(lap)">🗑</button>
                </li>
            }
        </ul>
    }
</div>
```

- UI for time display, controls and lap list.

---

### 9. CSS styling

```css
<style>
    .stopwatch {
        max-width: 500px;
        margin: 20px auto;
        padding: 20px;
        background: #f5f5f5;
        border-radius: 10px;
        box-shadow: 0 0 10px rgba(0,0,0,0.1);
        font-family: Arial, sans-serif;
    }

    .stopwatch p {
        font-size: 2em;
        text-align: center;
        margin: 10px 0;
    }

    .stopwatch button {
        margin: 5px;
        padding: 8px 12px;
        font-size: 1em;
        border: none;
        border-radius: 5px;
        background-color: #007bff;
        color: white;
        cursor: pointer;
    }

    .stopwatch button:disabled {
        background-color: #aaa;
        cursor: not-allowed;
    }

    .stopwatch ul {
        list-style-type: none;
        padding: 0;
    }

    .stopwatch li {
        margin: 5px 0;
        padding: 5px;
        background-color: #eee;
        display: flex;
        justify-content: space-between;
        align-items: center;
        border-radius: 5px;
    }

    .stopwatch li button {
        background-color: #dc3545;
        padding: 4px 8px;
        font-size: 0.9em;
    }
</style>
```

Stylish interface for a nice and clear stopwatch.

---
