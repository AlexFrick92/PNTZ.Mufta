# Блок 401 REZ_TPC_DOWN

Это стандартная структура блока

В блоке DB401 находится следующая структура:
```
- REZ_ALLG :    "REZ_ALLG"      Общие данные
- REZ_Muffe :   "REZ_Muffe"     Данные для муфты
- REZ_MVS :     "REZ_MVS"       Данные для преднавёртки
- REZ_CAM :     "REZ_CAM"       Данные для силовой навертки
- ERG_Muffe :   "ERG_Muffe"     Результат измерения муфты     
- ERG_MVS :     "ERG_MVS"       Результат преднавёртки         
- ERG_CAM :     "ERG_CAM"       Результат силовой навёртки      
- ZEITSTEMPEL : "REZ_ZST"       Метки времени.                 
```

**"REZ_ALLG" – Общие данные**
```
HEAD_OPEN_PULSES    REAL        Обороты на открытие навёрточной головки. Поворачивается, 
                                поканаверточная головка открыта. 
TURNS_BREAK         REAL        Turns after Breakout Обороты на развинчивание. 
                                Поворачивается пока не завершено развинчивание 
PLC_PROG_NR         WORD        PLC Programm Nummer	???  
LOG_NO              WORD        Log Nummer	???  
Tq_UNIT             WORD        0=Nm; 1=ft-lbs. Единицы измерения момента: Нм, фут-фунт
Thread_type         WORD        1=Right Hand; 2=Left Hand	
                                Направление резьбы: Правосторонняя, левосторонняя
PIPE_TYPE	        STRING[20]  Pipe Type (ASCII)	Тип трубы 
Rez_ALLG_RES	    WORD        Резерв
```

**"REZ_Muffe" – Данные для муфты**
```
Box_Moni_Time	    DINT	    Время наблюдения (10 с)
Box_Len_Min	        REAL	    Минимальная длина муфты
Box_Len_Max	        REAL	    Максимальная длина муфты
RET_MUFFE_RES_00	REAL		Вероятно резерв
RET_MUFFE_RES_01	REAL		Вероятно резерв
```
**"REZ_MVS" – Данные для преднавёртки**
```
Pre_Moni_Time       DINT	    Время наблюдения?
Pre_Len_Min		    REAL        Минимальная длина
Pre_Len_Max		    REAL    	Максимальная длина
Pre_VS_RES_00		REAL        Вероятно резерв
Pre_VS_RES_01		REAL        Вероятно резерв
```
**"REZ_CAM" – Данные для силовой навертки**
```
MU_Moni_Time	    DINT	    Время наблюдения
MU_Tq_Ref	        REAL	    Базовый крутящий момент
MU_Makeup_Mode	    WORD	    Makeup Mode: 0=By Torque, 1=By Length, 2=ByJValue	
                                Режим свинчивания (программа)
MU_TqSpeed_Red_1	REAL	    Снижение скорости при достижении крутящего момента 1
MU_TqSpeed_Red_2	REAL	    Снижение скорости при достижении крутящего момента 2
MU_Tq_Dump	        REAL        Значение крутящего момента при котором производится останов свинчивания
MU_Tq_Max	        REAL    	Максимальный крутящий момент
MU_Tq_Min	        REAL    	Минимальный крутящий момент
MU_Len_Speed_1	    REAL    	Снижение скорости при достижении длины 1
MU_Len_Speed_2	    REAL    	Снижение скорости при достижении длины 2
MU_Len_Dump	        REAL    	Значение длины свинчивания при котором производится сброс(остановка)
Mu_Len_Min	        REAL    	Минимальная длина свинчивания
Mu_Len_Max	        REAL    	Максимальная длина свинчивания
MU_JVal_Speed_1	    REAL    	Уменьшение скорости при достижениии значения J 1
MU_JVAL_Speed_2	    REAL    	Снижение скорости при достижении значения J2
MU_JVAL_Dump    	REAL    	Значение J при котором выполняется останов свинчивания
MU_JVal_Min	        REAL    	Минимальное значение J
MU_JVal_Max	        REAL    	Максимальное значение J
MU_Tq_Save	        REAL    	Крутящий момент автосохранения
MU_RES_01       	REAL    	Вероятно резерв
MU_RES_02	        INT 		Вероятно резерв
```

**"ERG_Muffe" – Результат измерения муфты**
```
PMR_BOX_RESULT	    DWORD	        (2=NIO, 1=IO)	Результат измерения муфты?
PMR_BOX_LEN_BEGIN	DATE_AND_TIME   Время начала измерения муфты?
PMR_BOX_LEN_END	    DATE_AND_TIME	Время окончания?
PMR_BOX_LEN_VALUE	REAL	        Длина муфты?
PMR_BOX_RES_00	    REAL		    Вероятно резерв?
PMR_BOX_RES_01	    INT		        Вероятно резерв?
```
**"ERG_MVS" – Результат преднавёртки**
```
PMR_Pre_MAKEUP_RESULT	DWORD	        (2=NIO, 1=IO)	Результат предварительного свинчивания?
PMR_Pre_MAKEUP_BEGIN	DATE_AND_TIME   Время начала предварительного свинчивания?
PMR_Pre_MAKEUP_END	    DATE_AND_TIME	Время окончания предварительного свинчивания?
PMR_PIPE_POS	        DWORD	        (2=NIO, 1=IO)	Результат позиционирования трубы?
PMR_Pre_MAKEUP_LEN	    REAL	        Длина предварительного свинчивания?
PMR_PIPE_POS_LEN	    REAL	        Позиция трубы после предварительного свинчивания?
PMR_PRE_MAKEUP_RES_00	REAL		    Вероятно резерв?
PMR_PRE_MAKEUP_RES_01	DINT		    Вероятно резерв?
```
**"ERG_CAM" – Результат силовой навёртки**
```
PMR_MR_MAKEUP_RESULT	DWORD	        (2=NIO, 1=IO)	Результат силового свинчивания в системе ПЛК. 
                                        Видимо если свинчивания не произведено (недудачное измерение муфты или преднавёртки), то ПЛК присваивает свой результат
PMR_MR_MAKEUP_BEGIN	    DATE_AND_TIME	Время начала силового свинчивания?
PMR_MR_MAKEUP_END	    DATE_AND_TIME   Время окончания силового свинчивания?
PMR_MR_TOTAL_RESULT	    DWORD	        (2=NIO, 1=IO)	Еще один результат? Вероятно, что это оценка уже по параметрам (момент, длина, заплечник и т.д),
PMR_MR_MAKEUP_LEN	    REAL	        Длина силового свинчивания
PMR_MR_TOTAL_MAKEUP_LEN	REAL	        Общая длина (преднавертка + силовое свинчивание)
PMR_MR_MAKEUP_FIN_TQ	REAL	        Окончательный Момент силовой навертки
PMR_MR_MAKEUP_FIN_TN	REAL	        Окончательное количество оборотов силовой навертки
PMR_MR_TOTAL_MAKEUP_VAL	REAL            Окончательное значение J силовой навертки
PMR_MR_RES_0	        REAL		    Вероятно резерв?
PMR_MR_RES_01	        REAL		    Вероятно резерв?
PMR_MR_RES_02	        REAL		    Вероятно резерв?
PMR_MR_RES_03	        REAL		    Вероятно резерв?
PMR_MR_RES_04	        REAL		    Вероятно резерв?
PMR_MR_RES_05	        REAL		    Вероятно резерв?
PMR_MR_RES_06	        WORD		    Вероятно резерв?
```
**"REZ_ZST" – Метки времени.**
```
GENERAL_TS	    DATE_AND_TIME	Общая
BOX_TS	        DATE_AND_TIME	Измерение муфты
PREMAKEUP_TS	DATE_AND_TIME	Преднавёртка
MAKEUP_TS	    DATE_AND_TIME	Силовая навёртка
DATE_14	        DATE_AND_TIME		
```