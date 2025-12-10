# Создание тестового окна для контрола

## Шаги создания

1. **Создать XAML файл** `[ControlName]TestWindow.xaml` в папке `TestWindows/`
   - Namespace: `PNTZ.Mufta.Showcase.TestWindows`
   - Добавить reference на assembly с контролом: `xmlns:control="clr-namespace:...;assembly=PNTZ.Mufta.TPCApp"`
   - Разместить контрол в Grid с Border для оформления

2. **Создать code-behind** `[ControlName]TestWindow.xaml.cs`
   - Наследуется от `Window`
   - Минимальная реализация: только `InitializeComponent()` в конструкторе

3. **Зарегистрировать в галерее** `MainWindow.xaml.cs`
   - Добавить в `InitializeControls()` новый `ControlInfo` с `WindowType = typeof([ControlName]TestWindow)`
   - Указать Name, Category, Description для отображения в списке
   - Category и Description подбирать самостоятельно по контексту контрола
