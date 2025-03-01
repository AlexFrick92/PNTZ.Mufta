# Сведения о программе ПЛК400

Система навертки состоит из узлов:

1. Подача муфт, муфтовый лифт
2. Смазка муфт, измерение длины муфты
3. Предварительная навертка муфты
4. Силовая навертка муфты, с системой измерения момента затяжки

## Силовая навертка

Аппарат силовой навертки состоит из:

**Наверточной головки** - 3х кулачковой системы зажима муфты. Главный привод головки состоит из трех гидромоторов, которые через планетарную передачу развивают момент затяжки до 67791 Nm. Направление навинчивания можно изменять. Это значит, что можно как развинчивать, так и свинчивать трубу. А так же можно свинчивать как правую так и левую резьбу. 
Кулачки можно менять для разных типоразмеров труб.

Сначала кулачки должны зажать муфту. Механизм исполнен таким образом, что сначала тормозная система блокирует вращение кулачков, и они сводятся. Как только кулачки зажали муфту, усиление превосходит ограничение тормозной системы и начинается свинчиваение. Благодаря этому этот процесс происходит бесступенчато, а усилие сжатия муфты в кулачках всегда необходимое.

В системе наверточной головки установлены фотодатчики для определения исходной точки угла поворота 0.

**Контер-ключ** - 3х кулачковой системы зажима трубы. Контр-ключ перемещается продольно вместе с трубой. Два датчика интегрированы в контр-ключе и создают основу измерения крутящего момента. 

**Измерительный контроллер** - контроллер Easy, который выполняет считывание сигналов:

1. Обороты наверточной головки
2. Перемещение станины контр-ключа
3. Момент затяжки на контр-ключе

Дискретный сигнал выход:

1. Дамп - сброс усилия на наверточной головке

Установка состоит из двух машин силовой навёртки, двух машин предварителной навёртки, и подачи муфт. 

Для каждой машины установлен свой АРМ. На АРМ выполняется программа. Для коммуникации между программой и ПЛК созданы блоки. Блоки названы и содержат похожий код для каждой из двух машин. Почему они не использовали один и тот же блок но с разными дб – не известно. Рассмотрим алгоритм на примере машины 1.

В программе встречаются следующие сокращения, или имена:
- TPC – (Torque Process Controller) Программное обеспечение установленное на АРМSPS – ПЛК 400
- SPS - ПЛК
- REZ – Рецепт (программа). Данные, необходимые для соединения (силовой навертки)
- MP – Machin Parameter. Конфигурация машины. Базовые настройки, неизменные во время работы 
- BA – Betriebsarten – Режим работы 
- kraftschrauber, CAM, MU – Makeup, Силовая навёртка
- vorschrauber, MVS - Преднавёртка 
- ML - Муфтовый лифт 
- MB – Смазка муфт

## Структура программы в контексте обмена данными

![-](OB1.drawio.svg)

### OB1
Вся программа находится в блоке OB1. Хотя в ПЛК есть и другие OB, Они используются для диагностики и не содержат вложенных блоков

### FB799 : (1)FB_Komm

Общий блок коммуникаций для машины 1. Здесь собраны и вызываются все блоки коммуникаций между TPC, SPS И EASY. 

### [FB710 : FB_TPC_REZ](Рецепт.md)
Блок загрузки рецепта (задания) из TPC в ПЛК. Алгоритм загрузки рецепта состоит из нескольких шагов, каждый шаг подтверждается командой от TPC или ПЛК.

### [FB711 : FB_TPC_MP](Параметры%20машин.md)
Блок коммуникаций для параметров машин. Параметры машин - это параметры настроки датчиков, энкодеров, тензометрии.
Параметры машин не загружаются из TPC в ПЛК. Параметры загружаются из панели оператора в ПЛК. Но чтобы ПЛК принял эти параметры, на TPC появляется окно, где оператор проверяет параметры и подтверждает загрузку. После подтверждения эти параметры сохраняются и на TPC. Зачем? Это нужно выяснить.

### [FB720 : FB_Com_Kraft](Оперативные%20параметры.md)
Блок коммуникаций в процессе силового свинчивания. В самом блоке формируются команды, но условия для этих команд создаются снаружи блока, т.е. в блоке (1)FB_Komm.

### FB_TPC2PLC, FB_PLC2TPC
Блок сервисных коммуникаций: хартбит, режимы работы

## Задачи

- Состав рецепта (рабочих данных)
- Процедура загрузки рецепта в ПЛК
- Состав оперативных параметров
- Состав результата соединения
- Процедура получения результата соединения