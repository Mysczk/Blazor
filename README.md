**Blazor**

# Cvičení 1: Jednoduchý poznámkový blok

## Řešení:

#### Popis
 - Tato Blazor komponenta implementuje jednoduchý poznámkový blok, kde uživatel může přidávat, upravovat a mazat poznámky.

---

### Kód komponenty `Notepad.razor`

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

### Vysvětlení kódu

#### 1. Třída `Note`

Třída `Note` reprezentuje jednotlivou poznámku. Obsahuje 2 vlastnosti:
 - `Text` (string) - obsah poznámky
 - `IsEditing` (bool) - indikace, zda je v režimu úprav

```csharp
private class Note
{
    public string Text { get; set; } = "";
    public bool IsEditing { get; set; } = false;
}
```

#### 2. Seznam poznámek
 - V komponentě se používá seznam `Notes`, který uchovává instance třídy `Note`.
 - Tento seznam představuje hlavní datovou strukturu aplikace:

```csharp
private List<Note> Notes = new();
```

#### 3. Přidání poznámky

 - Metoda AddNote vytvoří novou instanci třídy `Note`, pokud `newNoteText` není prázdný řetězec, a přidá ji do seznamu `Notes`:

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
 
 - Tímto způsobem je každá poznámka uchovávána jako samostatný objekt v seznamu.

#### 4. Odebírání poznámky

 - Metoda `RemoveNote` odstraní konkrétní poznámku ze seznamu:

```csharp
private void RemoveNote(Note note)
{
    Notes.Remove(note);
}
```

 - Při kliknutí na tlačítko 🗑️ dojde k odstranění odpovídající instance `Note` ze seznamu `Notes`.

#### 5. Úprava poznámky

 - Když uživatel klikne na tlačítko ✏️, metoda `EditNote` nastaví vlastnost `IsEditing` na `true`, čímž se aktivuje editační režim:

```csharp
private void EditNote(Note note)
{
    note.IsEditing = true;
}
```

 - Tím se změní vykreslení prvku v seznamu a zobrazí se textové pole namísto statického textu:

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

#### 6. Uložení upravené poznámky

 - Metoda `SaveEdit` deaktivuje editační režim nastavením `IsEditing` na `false`, čímž se uložené změny zobrazí jako běžný text:

```razor
private void SaveEdit(Note note)
{
    note.IsEditing = false;
}
```
---

# Cvičení 2: Galerie obrázků
Vytvoř Blazor stránku, která umožní:
1. **Načíst obrázky**
2. **Vykreslit obrázky**
3. **Prohlédnout konkrétní obrázek**
4. **Klávesové ovládání prohlížení obrázků**
4. **Subúkol**: mansory rozložení pomocí blazoru a čistého css


<details>
  <summary>💡 Nápověda</summary>

- Obrázky načítej pomocí **Service** a vykresluj pomocí cyklu v **HTML** značkách
- Pro otevření obrázku vytvoř overlay přes existující galerii
- pro klávesové ovládání budeš potřebovat registrovat vstupy z klávesnice pomocí **`KeyboardEventArgs`**
</details>

<details>
  <summary>Část řešení</summary>

1. Vytvoříme si razor stránku, kde budeme naší galerii volat
```razor
@page "/gallery"

<h3>Galerie</h3>

<ImageGrid />
```

2. Vytvoříme komponentu `ImageGrid.razor`
    - název je volitelný, ale potřeba upravit i v předešlém kroku

    <details>
        <summary>Kód</summary>

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

    ## Kód je zde trochu delší, ale pojďme si ho rozklíčovat:

    ### `OnInitializedAsync`
    - Spouští se automaticky při inicializaci komponenty.

    ### `OpenImage`
    - Otevře náhled konkrétního obrázku.

    ### `FocusOverlay`
    - Zaostří overlay prvek kvůli ovládání klávesnicí.

    ### `CloseImage`
    - Zavře náhled obrázku (overlay).

    ### `PreviousImage`
    - Posune výběr na předchozí obrázek.

    ### `NextImage`
    - Posune výběr na další obrázek.

    ### `HandleKeyDown`
    - Zpracovává stisknuté klávesy při aktivním overlay.

    ## Stále v naší komponentě udělejme strukturu stránky:


    <details>
        <summary>Kód</summary>


    ```csharp
    @if (images.Count == 0)
    {
        <p>Žádné obrázky nejsou k dispozici.</p>
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

    ## Stále v naší komponentě udělejme vzhled stránky:


    <details>
        <summary>Kód</summary>

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

    ## Stránku máme téměř hotovu, ale nezapoměňme na obrázky:
    - Založíme si ve složce `Services` službu `ImagesService.cs`

    <details>
        <summary>Kód</summary>


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
    - Service najde všechny `.jpg` fotky v dané složce a ukládá jejich cesty do `List<string>`
    </details>

    ## Poslední 2 kroky
    - Aby naše `Service` fungovala jak má je potřeba ji registrivat v `Program.cs` souboru a propojit ji s naší komponentou `ImageGrid.razor`
    ### Registrace:
    ```csharp
    builder.Services.AddSingleton<ImageService>();
    ```

    ### Propojení:
    - Na úplném vrcholu našeho souboru s komponentou použijeme:
    ```blazor
    @inject ImageService ImageService
    ```
    - To nám připojí instanci naší `Service` s naší komponentou 
</details>

# Úkol: Automatický obrázkový karusel v Blazoru

## Zadání

Vytvořte komponentu v Blazoru, která automaticky zobrazuje obrázky z dané složky jako karusel. Obrázky se mají měnit každé 3 sekundy, přičemž uživatel má možnost přecházet na předchozí nebo následující obrázek pomocí tlačítek.

---

## Kód komponenty a jeho vysvětlení

### 1. Importy a prostředí

```razor
@using Microsoft.AspNetCore.Hosting
@using Microsoft.Extensions.Hosting

@inject IWebHostEnvironment env
```

Zde importujeme knihovny potřebné pro přístup k prostředí aplikace. `@inject` nám umožňuje získat přístup k webovému kořeni projektu (`wwwroot`), abychom mohli načítat soubory.

---

### 2. Parametry a proměnné

```razor
[Parameter]
public string WorkFolder { get; set; } = "images/gallery";

private int activeIndex = 0;
private List<string> images = new List<string>();
private System.Timers.Timer? timer;
```

- `WorkFolder` – cesta ke složce s obrázky (relativní vůči `wwwroot`).
- `activeIndex` – index aktuálního obrázku.
- `images` – seznam obrázků (cesty k nim).
- `timer` – časovač pro automatické přepínání obrázků.

---

### 3. Inicializace komponenty

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

- Spojuje cestu ke složce `WorkFolder` s kořenem webu.
- Načítá všechny `.jpg` soubory a převádí je na relativní cesty.
- Spouští časovač, který každé 3 sekundy vyvolá funkci `Next()`.

---

### 4. Funkce pro změnu obrázku

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

- `Next()` – přepne na další obrázek.
- `Previous()` – přepne na předchozí obrázek.
- `StateHasChanged()` – způsobí překreslení UI komponenty.

---

### 5. Uvolnění prostředků

```razor
public void Dispose()
{
    timer?.Dispose();
}
```

Při odstranění komponenty se časovač zruší, aby nedošlo k únikům paměti.

---

### 6. HTML šablona komponenty

```razor
<div class="carousel">
    <button class="prev" @onclick="Previous">❮</button>
    <img src="@images[activeIndex]" class="carousel-image" />
    <button class="next" @onclick="Next">❯</button>
</div>
```

HTML struktura komponenty: tlačítka pro navigaci a obrázek podle aktuálního indexu.

---

### 7. Stylování karuselu

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

Styl definuje vzhled karuselu – velikost, zarovnání, vzhled obrázku a tlačítek.

---

# Úkol: Stopky v Blazoru

## Zadání

Vytvořte komponentu stopek v Blazoru, která bude umožňovat spouštění, zastavení, resetování a zaznamenávání mezičasů (lapů). Komponenta má zobrazovat uplynulý čas a seznam uložených lapů. Má obsahovat tlačítka pro ovládání a být stylovaná jako jednoduchá UI aplikace.

---

## Kód komponenty a jeho vysvětlení

### 1. Hlavička a rozhraní

```razor
@page "/stopwatch"
@implements IDisposable
```

- `@page` definuje URL adresu, kde je komponenta dostupná.
- `IDisposable` se používá pro správné uvolnění zdrojů (časovače).

---

### 2. C# část – logika stopek

```razor
@code {
    private bool isRunning = false;
    private TimeSpan elapsed = TimeSpan.Zero;
    private System.Timers.Timer timer;
    private DateTime startTime;
    private List<string> laps = new();

    private string formattedTime => elapsed.ToString(@"hh\:mm\:ss\.ff");
```

- `isRunning` sleduje, zda jsou stopky spuštěné.
- `elapsed` ukládá celkový uplynulý čas.
- `timer` je časovač, který aktualizuje čas.
- `laps` je seznam zaznamenaných mezičasů.

---

### 3. Inicializace a spuštění

```razor
protected override void OnInitialized()
{
    timer = new System.Timers.Timer(100);
    timer.Elapsed += OnTimerElapsed;
}
```

- Časovač je nastaven na interval 100 ms.
- Každý tik volá metodu `OnTimerElapsed()`.

---

### 4. Spouštění a zastavení stopek

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

- Spustí nebo zastaví stopky a upraví uplynulý čas.

---

### 5. Průběžná aktualizace času

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

- Přepočítává čas a aktualizuje UI.

---

### 6. Reset a lapy

```razor
private void Reset() { ... }
private void AddLap() { ... }
private void RemoveLap(string lap) { ... }
private void ClearLaps() { ... }
```

- `Reset()` – vynuluje stopky.
- `AddLap()` – uloží aktuální čas do seznamu.
- `RemoveLap()` – smaže záznam.
- `ClearLaps()` – vyčistí všechny lapy.

---

### 7. Uvolnění časovače

```razor
public void Dispose()
{
    timer?.Dispose();
}
```

- Zabrání úniku paměti.

---

### 8. HTML šablona

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

- UI pro zobrazení času, ovládání a seznam lapů.

---

### 9. CSS stylování

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

Stylované rozhraní pro hezké a přehledné stopky.

---
