# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PNTZ.Mufta.TPCApp is a WPF desktop application for monitoring and controlling pipe joint threading operations (муфтонавёртка). The application interfaces with PLCs via OPC UA, manages threading recipes, and records joint operation results to local SQLite and remote PostgreSQL databases.

**Target Framework**: .NET Framework 4.6.2

**Note**: The project was written for .NET 6, but there are issues with DevExpress controls on .NET 8, and .NET 7 is no longer supported by Microsoft (as noted in readme.md).

## Working Mode with Console Commands

When working with git and other console commands:
- **DO NOT** execute commands automatically using the Bash tool
- **DO** provide command text for manual execution by the user
- **DO** explain what each command does
- **Format**: Provide commands in code blocks with explanations

**Exception**: Commands may be executed automatically only if the user explicitly requests it (e.g., "execute this command", "run it for me").

**Example interaction:**
```
User: How do I check git status?
Assistant: Use this command:
  git status
This will show you the current state of your working directory and staging area.
```

## Git Commit Messages

When asked to provide commit messages, use this format:

**Structure:**
1. **Summary line**: One sentence describing the essence (max 10 words)
2. **Details list**: Bulleted clarifications (max 7 points)

**Content Guidelines:**
- **DO** focus on **what** changed and **why** (functionality, not file names)
- **DO** describe business logic and feature changes
- **DO NOT** list file names (git tracks this automatically)
- **DO NOT** include technical file structure details

**When to mention files:**
- Renaming/removing critical files
- Breaking changes to important configurations
- Refactoring with significant structural impact

**Example format:**
```
Добавлен JointProcessChartView в галерею Showcase

- Создано окно для проверки разметки контрола
- Зарегистрирован в галерее (категория "Графики")
- Реализована базовая структура для тестирования
```

**Not recommended:**
```
Добавлены файлы JointProcessChartViewTestWindow.xaml и .xaml.cs

- Создан TestWindows\JointProcessChartViewTestWindow.xaml
- Создан TestWindows\JointProcessChartViewTestWindow.xaml.cs
- Изменён MainWindow.xaml.cs
- Обновлён PNTZ.Mufta.Showcase.csproj
```

## Building and Testing

### Build Commands
```bash
# Build entire solution
msbuild PNTZ.Mufta.sln /p:Configuration=Debug /p:Platform="Any CPU"

# Build release version
msbuild PNTZ.Mufta.sln /p:Configuration=Release /p:Platform="Any CPU"

# Build main app only
msbuild PNTZ.Mufta.TPCApp\PNTZ.Mufta.TPCApp.csproj /p:Configuration=Debug

# Build test project
msbuild PNTZ.Mufta.TPCApp.Test\PNTZ.Mufta.TPCApp.Test.csproj /p:Configuration=Debug
```

**Note**: Debug builds use x64 platform target, Release builds use AnyCPU.

### Running Tests
```bash
# Run all tests using NUnit
nunit3-console PNTZ.Mufta.TPCApp.Test\bin\Debug\PNTZ.Mufta.TPCApp.Test.dll

# Run tests in Visual Studio Test Explorer or use:
dotnet test PNTZ.Mufta.sln
```

### Running the Application
```bash
# Run from build output
PNTZ.Mufta.TPCApp\bin\Debug\PNTZ.Mufta.TPCApp.exe
```

## Architecture

### Application Lifecycle (StagedApplication Pattern)

The application uses a staged startup pattern from the Desktop library:

1. **BeforeInit** (App.cs:54-95): IoC container setup, dependency registration, DpConnect framework configuration
2. **Init** (App.cs:97-103): DpBuilder builds PLC connections, creates MainView and MainViewModel
3. **AfterInit** (App.cs:105-108): Post-initialization hooks

Entry point: `Program.cs:Main()` → `App.Start()`

### Dependency Injection (DryIoc)

The application uses DryIoc container for IoC. Key registrations in `App.cs:BeforeInit()`:

- **Repositories**: `LocalRepository` (singleton)
- **DpConnect**: `OpcUaConnection`, `ContainerizedConnectionManager`, `ContainerizedWorkerManager`
- **DpWorkers**: `JointProcessDpWorker`, `RecipeDpWorker`, `MachineParamFromPlc`, `HeartbeatCheck`, `SensorStatusDpWorker`
- **ViewModels**: `MainViewModel`
- **Infrastructure**: `ILogger`, `IDpBuilder`, `IDpBinder`

### DpConnect Framework

DpConnect is a custom framework for PLC communication via OPC UA:

- **IDpBuilder**: Builds connections and workers from XML configuration (`DpConnect/DpConfig.xml`)
- **IDpConnectionManager**: Manages PLC connection lifecycle
- **IDpWorkerManager**: Manages DpWorker instances that handle specific PLC data
- **IDpValue<T>**: Represents a data point bound to PLC tags, with `ValueUpdated` events
- **IDpWorker**: Interface for workers that process PLC data

**DpWorkers** in this project:
- `JointProcessDpWorker`: Main worker for joint threading process monitoring
- `RecipeDpWorker`: Handles recipe upload/download to/from PLC
- `MachineParamFromPlc`: Reads machine parameters
- `HeartbeatCheck`: Monitors PLC connection health
- `SensorStatusDpWorker`: Monitors sensor states

### Data Layer Architecture

**Dual Repository Pattern**: Local (SQLite) + Remote (PostgreSQL)

- **LocalRepository** (primary interface):
  - Local SQLite databases for recipes and results
  - Recipes: `Repository/RecipesData.db`
  - Results: `Repository/ResultsData.db`
  - Provides sync methods to coordinate with RemoteRepository

- **RemoteRepository**:
  - PostgreSQL database (connection: "PostgresDb" in App.config)
  - Used for centralized storage across multiple stations

**Database Contexts** (LINQ2DB):
- `JointRecipeContext` → `JointRecipeTable`
- `JointResultContext` → `JointResultTable`
- `RemoteRepositoryContext` → both tables on PostgreSQL

**Sync Strategy**:
- Recipes sync bidirectionally based on timestamps
- Results upload from local to remote (`PushResults`)
- Results can be downloaded for analysis (`PullResults`)

### Domain Model

**Core Entities**:

- **JointRecipe**: Threading recipe with validation parameters for different joint modes
  - `JointMode`: Torque, Length, TorqueLength, JValue, TorqueJValue, TorqueShoulder
  - `ThreadType`: Enum for thread types
  - Configured via `Domain/JointRecipe.xml`

- **JointResult**: Result of a single threading operation
  - Includes timestamp, evaluation, mode, target parameters
  - Contains result data from CAM, Muffe, MVS systems
  - Stores serialized TqTnLen points (torque-turns-length graph data)

- **TqTnLenPoint**: Real-time data point during threading
  - Torque, Turns, Length, TurnsPerMinute, TimeStamp

- **MachineParam**: Machine parameters from PLC

**PLC Data Structures** (in `DpConnect/Struct/`):
- `ERG_CAM`, `ERG_Muffe`, `ERG_MVS`: Result structures from different systems
- `REZ_CAM`, `REZ_Muffe`, `REZ_MVS`, `REZ_ALLG`: Recipe structures
- `OperationalParam`: Real-time operational parameters
- `ZEITSTEMPEL`: Timestamp structure

### MVVM Architecture

**ViewModels** (in `ViewModel/`):
- `MainViewModel`: Navigation hub, manages view switching and CLI commands
- `JointProcessViewModel`: Real-time joint monitoring, configured via `ViewModel/JointProcessViewModel.xml`
- `RecipeViewModel`: Recipe management (new, edit, download to PLC)
- `ResultsViewModel`: Results viewing and filtering
- `MachinParamViewModel`: Machine parameter monitoring
- `StatusBarViewModel`: Connection status, heartbeat, recipe status

**Views** (in `View/`):
- `MainView`: Main window with navigation
- `Joint/JointProcessView`: Real-time threading visualization
- `Recipe/RecipeView`: Recipe management UI
- `Results/JointResultsView`: Results browser
- `MP/MachineParamView`: Machine parameters display
- `Control/RealtimeChartView`: DevExpress chart for real-time data

**Navigation**: `MainViewModel` uses `RelayCommand` to switch between views via `MainContent` property.

### CLI Commands (MainViewModel.cs:127-134)

The application has an integrated CLI accessible from the UI:

- `start` - Open PLC connections
- `stop` - Close PLC connections
- `rr_init` - Initialize remote repository tables
- `rr_syncrecipes` - Sync recipes between local and remote
- `rr_pushresults` - Upload results to remote repository
- `rr_pullresults [recipeName]` - Download results from remote
- `rr_clearresults` - Clear local results database
- `rr_fetchresults` - List remote recipe names

### Joint Process Workflow

1. **PipeAppear Event**: Pipe detected on station
2. **RecordingBegun Event**: Threading starts, `TqTnLenPoint` collection begins
3. Real-time data points accumulated via `NewTqTnLenPoint` event
4. **RecordingFinished Event**: Threading complete, awaiting evaluation
5. **AwaitForEvaluation Event**: Operator prompted to evaluate (OK/NOK)
6. `Evaluate(uint result)` called with operator input
7. **JointFinished Event**: Result saved to LocalRepository

Implemented in `JointProcessDpWorker` and coordinated by `JointProcessViewModel`.

### Configuration Files

- `App.config`: Connection strings, logging configuration
- `DpConnect/DpConfig.xml`: OPC UA connection and DpWorker bindings
- `Domain/JointRecipe.xml`: Recipe validation rules
- `ViewModel/JointProcessViewModel.xml`: Process view settings
- `ViewModel/MainViewModel.xml`: Startup settings (ConnectOnStartup)
- `View/ViewConfig.xml`: UI view configuration
- `nlog.config`: NLog logging configuration

## External Dependencies

**Key Libraries**:
- **DevExpress WPF v22.2**: Charts and UI controls
- **linq2db 5.4.1**: LINQ to database provider
- **Npgsql 4.1.11**: PostgreSQL driver
- **System.Data.SQLite**: SQLite database
- **NLog 5.0.4**: Logging framework
- **Promatis Libraries**: Core utilities, IoC (DryIoc), OPC UA client
- **NUnit 3.13.3**: Testing framework
- **Moq 4.17.2**: Mocking for tests

**External Projects** (referenced via relative paths):
- `Toolbox/` - Toolkit, Desktop, DpConnect libraries
- `DpConnect/` - DpConnect core, DpConnect.OpcUa
- `Tools/` - Promatis.Opc.UA.Client, Promatis.IoC.DryIoc

## Code Patterns

### Custom Validation
The project uses a custom validation system:
- `ComparableValidationPropertyAttribute`: Marks properties for validation
- `ComparableValueValidator`: Validates values against min/max/target with tolerance
- Used extensively in `JointRecipe` for validating threading parameters

### XML Configuration Loading
Domain objects use `DomainObjectXmlConfigurator` to load validation rules from XML files, allowing runtime configuration of validation without recompiling.

### Event-Driven PLC Communication
DpConnect uses event-driven patterns. DpWorkers subscribe to `IDpValue<T>.ValueUpdated` events to react to PLC data changes in real-time.

### Data Smoothing
`Toolbox/Smoothing/MovingAverage` is used to smooth torque readings for better visualization and noise reduction.

## Common Tasks

### Adding a New DpWorker
1. Create class implementing `IDpWorker` interface
2. Define `IDpValue<T>` properties for PLC data points
3. Implement `DpBound()` method to subscribe to value updates
4. Register in `App.cs:BeforeInit()` with container
5. Add configuration to `DpConnect/DpConfig.xml`

### Adding a New View
1. Create XAML view in `View/` subdirectory
2. Create corresponding ViewModel inheriting `BaseViewModel`
3. Register ViewModel navigation command in `MainViewModel`
4. Add navigation button in `MainView.xaml`

### Modifying Recipe Structure
1. Update `JointRecipe.cs` domain model
2. Update `JointRecipeTable.cs` database table
3. Update `Domain/JointRecipe.xml` for new validation rules
4. Update corresponding `REZ_*` structs if PLC communication affected
5. Regenerate or update database schema

### Working with Results
Results are stored as `JointResultTable` with serialized graph data. To work with graph points:
- Deserialize `TqTnLenPoints` field (serialized as string)
- Points contain torque, turns, length, RPM over time
- Use for charting or analysis

## Communication Style

When discussing code changes with the user:
- **DO** provide a short plan of changes without showing code
- **DO** ask clarifying questions before implementation
- **DO NOT** show code examples until explicitly requested
- **DO** wait for approval before making changes

При обсуждении изменений кода:
- **Предлагай** короткий план изменений без показа кода
- **Задавай** уточняющие вопросы перед реализацией
- **НЕ показывай** примеры кода, пока не попросят
- **Жди** одобрения перед внесением изменений
