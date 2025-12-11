# Анализ архитектуры PNTZ.Mufta.TPCApp

> Документ создан на основе анализа кодовой базы с помощью Gemini CLI
> Дата: 2025-12-10

## Содержание

- [Обзор проекта](#обзор-проекта)
- [Архитектура приложения](#архитектура-приложения)
- [ViewModels](#viewmodels)
- [Workflow процесса свинчивания](#workflow-процесса-свинчивания)
- [События JointProcessDpWorker](#события-jointprocessdpworker)
- [Ключевые архитектурные решения](#ключевые-архитектурные-решения)

---

## Обзор проекта

**PNTZ.Mufta.TPCApp** - WPF desktop-приложение для мониторинга и управления операциями свинчивания муфт на трубы (pipe joint threading operations).

### Основные характеристики

- **Framework**: .NET Framework 4.6.2
- **UI**: WPF с DevExpress компонентами (v22.2)
- **Коммуникация**: OPC UA для связи с ПЛК
- **Базы данных**:
  - Локальная: SQLite (рецепты и результаты)
  - Удалённая: PostgreSQL (централизованное хранилище)
- **DI Container**: DryIoc
- **Логирование**: NLog 5.0.4

### Ключевые компоненты структуры

- **Application Lifecycle**: Staged startup pattern (BeforeInit → Init → AfterInit)
- **Dependency Injection**: DryIoc для управления зависимостями
- **DpConnect Framework**: Кастомный фреймворк для OPC UA взаимодействия с ПЛК
- **Data Layer**: Dual Repository Pattern (Local + Remote с синхронизацией)
- **Domain Model**: JointRecipe, JointResult, TqTnLenPoint, MachineParam
- **MVVM Architecture**: Чёткое разделение Views и ViewModels

---

## Архитектура приложения

### 1. Staged Startup Pattern

**BeforeInit** (`App.cs:54-95`)
- Настройка IoC контейнера
- Регистрация зависимостей
- Конфигурация DpConnect фреймворка

**Init** (`App.cs:97-103`)
- DpBuilder строит PLC-подключения
- Создание MainView и MainViewModel

**AfterInit** (`App.cs:105-108`)
- Post-initialization hooks

### 2. Dependency Injection (DryIoc)

Ключевые регистрации в `App.cs:BeforeInit()`:

**Repositories**
- `LocalRepository` (singleton)

**DpConnect**
- `OpcUaConnection`
- `ContainerizedConnectionManager`
- `ContainerizedWorkerManager`

**DpWorkers**
- `JointProcessDpWorker`
- `RecipeDpWorker`
- `MachineParamFromPlc`
- `HeartbeatCheck`
- `SensorStatusDpWorker`

**ViewModels**
- `MainViewModel`

**Infrastructure**
- `ILogger`, `IDpBuilder`, `IDpBinder`

### 3. DpConnect Framework

Кастомный фреймворк для PLC-коммуникации через OPC UA:

- **IDpBuilder**: Строит подключения и workers из XML конфигурации (`DpConnect/DpConfig.xml`)
- **IDpConnectionManager**: Управляет жизненным циклом PLC-подключений
- **IDpWorkerManager**: Управляет экземплярами DpWorker
- **IDpValue<T>**: Представляет data point, связанный с PLC-тегами, с событиями `ValueUpdated`
- **IDpWorker**: Интерфейс для workers, обрабатывающих PLC-данные

### 4. Data Layer Architecture

**Dual Repository Pattern**: Local (SQLite) + Remote (PostgreSQL)

**LocalRepository** (primary interface):
- Локальные SQLite базы для рецептов и результатов
- Recipes: `Repository/RecipesData.db`
- Results: `Repository/ResultsData.db`
- Методы синхронизации с RemoteRepository

**RemoteRepository**:
- PostgreSQL база (connection: "PostgresDb" в App.config)
- Централизованное хранилище для нескольких станций

**Database Contexts** (LINQ2DB):
- `JointRecipeContext` → `JointRecipeTable`
- `JointResultContext` → `JointResultTable`
- `RemoteRepositoryContext` → обе таблицы на PostgreSQL

**Sync Strategy**:
- Рецепты синхронизируются двунаправленно (по timestamps)
- Результаты загружаются с локальной на удалённую (`PushResults`)
- Результаты могут быть скачаны для анализа (`PullResults`)

### 5. Domain Model

**Core Entities**:

**JointRecipe** - Рецепт свинчивания с параметрами валидации
- `JointMode`: Torque, Length, TorqueLength, JValue, TorqueJValue, TorqueShoulder
- `ThreadType`: Enum типов резьбы
- Конфигурация: `Domain/JointRecipe.xml`

**JointResult** - Результат одной операции свинчивания
- Timestamp, evaluation, mode, target parameters
- Данные от систем CAM, Muffe, MVS
- Сериализованные TqTnLen точки (график момент-обороты-длина)

**TqTnLenPoint** - Точка данных в реальном времени
- Torque, Turns, Length, TurnsPerMinute, TimeStamp

**MachineParam** - Параметры машины от ПЛК

**PLC Data Structures** (в `DpConnect/Struct/`):
- `ERG_CAM`, `ERG_Muffe`, `ERG_MVS`: Структуры результатов
- `REZ_CAM`, `REZ_Muffe`, `REZ_MVS`, `REZ_ALLG`: Структуры рецептов
- `OperationalParam`: Оперативные параметры в реальном времени
- `ZEITSTEMPEL`: Структура timestamp

---

## ViewModels

### Основные ViewModels

#### MainViewModel
Навигационный хаб приложения. Управляет переключением между основными экранами и CLI-командами.

#### StatusBarViewModel
Управляет строкой состояния:
- Статус подключения к ПЛК
- Heartbeat индикатор
- Информация о загруженном рецепте
- Состояние датчиков
- Текущий этап процесса свинчивания

#### JointProcessViewModel
Управляет экраном процесса свинчивания в реальном времени:
- Отображение данных с датчиков на графиках
- Запуск и остановка записи процесса
- Отображение итогового результата
- Кнопки оценки для оператора ("Годное"/"Брак")

#### RecipeViewModel
Управляет экраном работы с рецептами:
- Загрузка, отображение, фильтрация рецептов
- Создание, редактирование, удаление рецептов
- Загрузка выбранного рецепта в ПЛК

#### ResultsViewModel
Управляет экраном просмотра истории результатов:
- Фильтрация завершённых процессов свинчивания
- Детальные графики и параметры каждого результата

#### MachinParamViewModel
Отображает технические параметры, полученные с машины (ПЛК).

### ViewModels-обёртки

#### JointRecipeViewModel
Обёртка для модели `JointRecipe`. Предоставляет свойства рецепта для привязки к элементам интерфейса.

#### JointResultViewModel
Обёртка для модели `JointResult`. Предоставляет данные о результате завершённого свинчивания для отображения (итоговые значения, вердикт).

#### TqTnLenPointViewModel
Обёртка для модели `TqTnLenPoint`, представляющей одну точку данных (момент, обороты, длина) на графике.

### ViewModels диалоговых окон

#### NewRecipeViewModel
ViewModel для диалогового окна создания нового рецепта.

#### RemoveRecipeViewModel
ViewModel для диалогового окна подтверждения удаления рецепта.

### ViewModels элементов управления

#### ChartViewModel
Переиспользуемая ViewModel для элемента управления "График":
- Внешний вид, источник данных
- Оси, диапазоны
- Коллекции константных линий и фоновых областей

#### ChartSeriesViewModel
Описывает одну серию данных (линию) на графике:
- Цвет, толщина
- Привязка к данным

#### ConstantLineViewModel
Описывает константную линию на графике (например, для отображения предельных значений момента или длины).

#### StripViewModel
Описывает выделенную цветом область (полосу) на графике для обозначения допустимого диапазона значений.

### Составные ViewModels для процесса свинчивания

#### JointProcessChartViewModel
Компонует и управляет четырьмя графиками на экране процесса свинчивания:
1. Момент / Обороты
2. Скорость / Обороты
3. Момент / Длина
4. Момент / Время

Настраивает графики согласно текущему рецепту и входящим данным.

#### JointProcessDataViewModel
Отображает текстовые и числовые данные процесса свинчивания:
- Текущие показания датчиков
- Таймер
- Финальные значения
- Статусы проверки параметров (момент, длина и т.д.)

---

## Workflow процесса свинчивания

Процесс свинчивания управляется классом `JointProcessDpWorker`, который работает как **машина состояний**, взаимодействуя с ПЛК по протоколу OPC UA.

### Последовательность шагов

#### 1. Инициализация и ожидание

- `JointProcessDpWorker` подписывается на OPC-теги от ПЛК
- Ключевой тег для управления процессом: `DpPlcCommand`
- Worker ожидает команду `10` ("труба на позиции") от ПЛК

#### 2. Начало процесса (PipeAppear)

**Триггер**: ПЛК присылает команду `10`

**Действия**:
- `JointProcessDpWorker` инициирует новый объект `JointResult` для сбора данных
- Генерируется событие `PipeAppear`
- Сигнализирует о начале нового цикла

#### 3. Передача данных в реальном времени (NewTqTnLenPoint)

**Поток данных**:
1. ПЛК непрерывно обновляет OPC-тег `DpParam`
   - Содержит оперативные данные: момент, длину, обороты
2. Каждое обновление тега вызывает метод `SetLastPoint` в `JointProcessDpWorker`
3. `SetLastPoint` преобразует сырые данные в объект `TqTnLenPoint`
4. Генерируется событие `NewTqTnLenPoint`

**Обработка в UI**:
- `JointProcessViewModel` подписан на событие `NewTqTnLenPoint`
- При получении события обновляет свойство `ActualPoint` в `JointProcessDataViewModel`
- UI автоматически обновляется: графики, цифровые индикаторы

#### 4. Запись процесса (RecordingBegun)

**Триггер**: Обмен командами готовности
- `20` от TPCApp
- `30` от ПЛК (Навинчивание) или `25` (Развинчивание)
- Отправка команды `38` (Начать запись)

**Действия**:
- `JointProcessDpWorker` вызывает метод `RecordOperationParams`
- Начинается сбор всех поступающих точек `TqTnLenPoint` в коллекцию `jointResult.Series`
- Генерируется событие `RecordingBegun`

**Реакция ViewModel**:
- `JointProcessViewModel` вызывает метод `BeginNewJointing()`
- Сбрасывает таймеры и предыдущие результаты в UI

#### 5. Завершение процесса (RecordingFinished)

**Триггер**: Один из случаев
- ПЛК присылает команду `40` ("свинчивание завершено")
- Истекло время ожидания (`TimeoutException`)
- Процесс отменён вручную (`OperationCanceledException`)
- ПЛК прислал неожиданную команду во время записи

**Действия**:
- `JointProcessDpWorker` прекращает запись точек
- Считывает итоговые результаты из ПЛК
- Генерируется событие `RecordingFinished` с полным объектом `JointResult`

**Реакция ViewModel**:
- `JointProcessViewModel` вызывает метод `FinishJointing(result)`
- Останавливает таймер
- Отображает итоговые результаты в `JointProcessDataViewModel`

#### 6. Оценка результата (AwaitForEvaluation)

**Триггер**: Автоматическая оценка (`JointEvaluation.Evaluate`) определила результат как брак

**Действия**:
- Генерируется событие `AwaitForEvaluation`
- Оператору требуется вручную оценить результат
- Оператор выбирает: "Годное" (OK) или "Брак" (NOK)

#### 7. Завершение цикла (JointFinished)

**Триггер**: Всегда вызывается в блоке `finally` основного цикла

**Значение**:
- Событие генерируется независимо от результата (успех, ошибка, отмена)
- Означает, что один полный цикл (от появления трубы до завершения всех операций) окончен

### Диаграмма потока данных

```
ПЛК (OPC UA)
    ↓
DpPlcCommand → Команда "10" → PipeAppear Event
    ↓
DpParam (обновляется непрерывно)
    ↓
SetLastPoint() → TqTnLenPoint → NewTqTnLenPoint Event
    ↓                                    ↓
JointProcessDpWorker              JointProcessViewModel
    ↓                                    ↓
RecordOperationParams()        JointProcessDataViewModel.ActualPoint
    ↓                                    ↓
jointResult.Series              UI: Графики + Индикаторы
    ↓
RecordingFinished Event → JointResult
    ↓
FinishJointing(result) → Отображение итогов
    ↓
(если брак) AwaitForEvaluation → Ручная оценка
    ↓
JointFinished → Цикл завершён
```

---

## События JointProcessDpWorker

Класс `JointProcessDpWorker` генерирует следующие события для управления процессом свинчивания муфты:

### 1. PipeAppear

- **Тип**: `EventHandler<JointResult>`
- **Когда**: Сразу после получения команды `10` от ПЛК
- **Значение**: Труба подана на станок и готова к началу навинчивания

### 2. RecordingBegun

- **Тип**: `EventHandler<EventArgs>`
- **Когда**: В момент начала фактической записи телеметрии (момент, обороты, длина)
- **Условия**: После получения команды `30` (Навинчивание) или `25` (Развинчивание) и отправки `38` (Начать запись)

### 3. RecordingFinished

- **Тип**: `EventHandler<JointResult>`
- **Когда**: По завершении записи телеметрии
- **Возможные причины**:
  - ПЛК прислал команду `40` (успешное окончание)
  - Истекло время ожидания (`TimeoutException`)
  - Процесс отменён вручную (`OperationCanceledException`)
  - ПЛК прислал неожиданную команду во время записи

### 4. AwaitForEvaluation

- **Тип**: `EventHandler<JointResult>`
- **Когда**: Если после завершения автоматическая оценка (`JointEvaluation.Evaluate`) определила результат как брак
- **Значение**: Требуется ручная оценка результата оператором

### 5. JointFinished

- **Тип**: `EventHandler<JointResult>`
- **Когда**: В самом конце всего цикла обработки (в блоке `finally`)
- **Значение**: Событие вызывается независимо от результата (успех/ошибка/отмена). Означает окончание полного цикла.

### 6. NewTqTnLenPoint

- **Тип**: `EventHandler<TqTnLenPoint>`
- **Когда**: Непрерывно, при каждом обновлении `DpParam.ValueUpdated` от ПЛК
- **Данные**: Новая точка с текущими значениями Torque, Turns, Length

### Карта событий

```
Время →

PipeAppear
    ↓
[NewTqTnLenPoint] (непрерывно)
    ↓
RecordingBegun
    ↓
[NewTqTnLenPoint] (записываются в Series)
    ↓
RecordingFinished
    ↓
AwaitForEvaluation (опционально, если брак)
    ↓
JointFinished (всегда)
```

---

## Ключевые архитектурные решения

### 1. Разделение ответственности

**Backend (DpWorker)**:
- Инкапсулирует всю логику взаимодействия с ПЛК
- Машина состояний для управления протоколом
- Обработка команд и данных от OPC UA

**Frontend (ViewModel)**:
- Пассивная роль: только реагирует на события
- Отображение данных в UI
- Минимум бизнес-логики

### 2. Событийная модель

Связь между backend и frontend построена на **событиях C#**:
- `NewTqTnLenPoint` - поток данных в реальном времени
- `RecordingBegun` - начало процесса
- `RecordingFinished` - завершение процесса
- `AwaitForEvaluation` - запрос ручной оценки
- `JointFinished` - финал цикла

**Преимущества**:
- Слабая связанность компонентов
- Гибкость системы
- Легко добавлять новых подписчиков на события

### 3. Машина состояний

Логика в `JointProcessDpWorker` представляет собой **асинхронную машину состояний**:
- Последовательное выполнение шагов протокола обмена с ПЛК
- Обработка исключений и таймаутов
- Гарантированное выполнение финального события (`finally` блок)

### 4. Dual Repository Pattern

**Локальная + Удалённая база**:
- Локальная (SQLite): быстрая работа, независимость от сети
- Удалённая (PostgreSQL): централизованное хранилище, аналитика
- Синхронизация: рецепты двунаправленно, результаты загружаются

**Преимущества**:
- Надёжность (работа при отсутствии сети)
- Производительность (локальные операции быстрые)
- Централизация данных для аналитики

### 5. DpConnect Framework

Кастомный фреймворк для промышленных систем:
- Абстракция над OPC UA
- XML-конфигурация подключений
- Workers для специализированных задач
- Событийная модель обновления данных

**Преимущества**:
- Переиспользуемость кода
- Упрощение интеграции с ПЛК
- Типобезопасность через `IDpValue<T>`

---

## Заключение

Архитектура проекта **PNTZ.Mufta.TPCApp** представляет собой классический пример **промышленной SCADA/MES системы** с чётким разделением слоёв, событийной моделью взаимодействия и надёжной обработкой состояний.

Ключевые сильные стороны:
- **Чёткое разделение ответственности** (DpWorker ↔ ViewModel)
- **Событийная архитектура** для слабой связанности
- **Машина состояний** для надёжного управления процессом
- **Dual Repository** для надёжности и производительности
- **MVVM** для поддерживаемого UI-кода

Проект демонстрирует зрелую архитектуру, подходящую для критичных промышленных приложений.
