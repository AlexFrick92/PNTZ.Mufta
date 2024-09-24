# Список сигналов OPC UA для ПЛК1517

Сигналлы сгруппированы по функциональному признаку.
Тип WORD(PLC) = USINT(OPCUA), DWORD(PLC) = UINT(OPCUA). Доступ ко всем сигналам на чтение и на запись. 

Для типа данных время использовать DTL.

## Параметры машин

```
MP_OP_SPS.MP_Load_Cell_Span	                Real            Диапазон значений / Einheitenbereich Messzelle / Range Load Cell Span /
MP_OP_SPS.MP_Load_Span_Digits	            Real            Разрешение измерительной ячейк / Auflцsung Messzelle / Range Load Cell Digit /
MP_OP_SPS.MP_Handle_Length	                Real            Диапазон Длина ручки / Hebelarmlдnge Messzelle / Range Handle Length
MP_OP_SPS.MP_Handle_Length_Digits	        Real            Диапазон длины ручки / Auflцsung Hebelarm Messzelle / Range Handle Length Digit
MP_OP_SPS.MP_TC_PPR	                        Real
MP_OP_SPS.MP_Box_Length	                    Real            Messbereich Muffenlдnge / Range Box Length Measure System
MP_OP_SPS.MP_Box_Length_Digit	            Real    
MP_OP_SPS.MP_Makeup_Length	                Real            Messbereich Verschraublдnge / Range Makeup Length Measure System
MP_OP_SPS.MP_Makeup_Length_Digits	        Real
MP_OP_SPS.MP_Tq_Max	                        Real            Maximales Drehmoment Maschine / Max Torque Value Machine
MP_OP_SPS.MP_Machine_No	                    String[20]      Maschinen Nummer / Machine No.
MP_OP_SPS.MP_Cal_Factor	                    Real            Калибровочный коэффициент
MP_OP_SPS.MP_Cal_User	                    String[20]      Bearbeiter Kalibrierung / User, who did calibration
MP_OP_SPS.MP_Cal_Timestamp	                DTL             Zeitstempel Kalibrierung / Calibration Timestamp
MP_OP_SPS.MP_Makeup_Length_Offset	        Real            Referenzwert Lдngenmassstab Verschraublдnge
```
```
KOM_REG.SPS_TPC_MDAT.CMD_TPC                 UINT        команда от АРМ в ПЛК
KOM_REG.SPS_TPC_MDAT.CMD_PLC                 UINT        команда от ПЛК в АРМ
```


## Загрузка рецепта в ПЛК

```
REZ_TPC_DOWN.REZ_ALLG.HEAD_OPEN_PULSES      REAL        Обороты на открытие навёрточной головки. Поворачивается, поканаверточная головка открыта. 
REZ_TPC_DOWN.REZ_ALLG.TURNS_BREAK           REAL        Turns after Breakout Обороты на развинчивание. Поворачивается пока не завершено развинчивание 
REZ_TPC_DOWN.REZ_ALLG.PLC_PROG_NR           USINT        PLC Programm Nummer	???  
REZ_TPC_DOWN.REZ_ALLG.LOG_NO                USINT        Log Nummer	???  
REZ_TPC_DOWN.REZ_ALLG.Tq_UNIT               USINT        0=Nm; 1=ft-lbs. Единицы измерения момента: Нм, фут-фунт
REZ_TPC_DOWN.REZ_ALLG.Thread_type           USINT        1=Right Hand; 2=Left Hand Направление резьбы: Правосторонняя, левосторонняя
REZ_TPC_DOWN.REZ_ALLG.PIPE_TYPE	            STRING[20]  Pipe Type (ASCII)	Тип трубы 
REZ_TPC_DOWN.REZ_ALLG.Rez_ALLG_RES	        USINT        Резерв
```
```
REZ_TPC_DOWN.REZ_Muffe.Box_Moni_Time	    DINT	    Время наблюдения (10 с)
REZ_TPC_DOWN.REZ_Muffe.Box_Len_Min	        REAL	    Минимальная длина муфты
REZ_TPC_DOWN.REZ_Muffe.Box_Len_Max	        REAL	    Максимальная длина муфты
REZ_TPC_DOWN.REZ_Muffe.RET_MUFFE_RES_00	    REAL		Вероятно резерв
REZ_TPC_DOWN.REZ_Muffe.RET_MUFFE_RES_01	    REAL		Вероятно резерв
```
```
REZ_TPC_DOWN.REZ_MVS.Pre_Moni_Time          DINT	    Время наблюдения?
REZ_TPC_DOWN.REZ_MVS.Pre_Len_Min		    REAL        Минимальная длина
REZ_TPC_DOWN.REZ_MVS.Pre_Len_Max		    REAL    	Максимальная длина
REZ_TPC_DOWN.REZ_MVS.Pre_VS_RES_00		    REAL        Вероятно резерв
REZ_TPC_DOWN.REZ_MVS.Pre_VS_RES_01		    REAL        Вероятно резерв
```
```
REZ_TPC_DOWN.REZ_CAM.MU_Moni_Time	        DINT	    Время наблюдения
REZ_TPC_DOWN.REZ_CAM.MU_Tq_Ref	            REAL	    Базовый крутящий момент
REZ_TPC_DOWN.REZ_CAM.MU_Makeup_Mode	        USINT	    Makeup Mode: 0=By Torque, 1=By Length, 2=ByJValue Режим свинчивания
REZ_TPC_DOWN.REZ_CAM.MU_TqSpeed_Red_1	    REAL	    Снижение скорости при достижении крутящего момента 1
REZ_TPC_DOWN.REZ_CAM.MU_TqSpeed_Red_2	    REAL	    Снижение скорости при достижении крутящего момента 2
REZ_TPC_DOWN.REZ_CAM.MU_Tq_Dump	            REAL        Значение крутящего момента при котором производится останов свинчивания
REZ_TPC_DOWN.REZ_CAM.MU_Tq_Max	            REAL    	Максимальный крутящий момент
REZ_TPC_DOWN.REZ_CAM.MU_Tq_Min	            REAL    	Минимальный крутящий момент
REZ_TPC_DOWN.REZ_CAM.MU_Len_Speed_1	        REAL    	Снижение скорости при достижении длины 1
REZ_TPC_DOWN.REZ_CAM.MU_Len_Speed_2	        REAL    	Снижение скорости при достижении длины 2
REZ_TPC_DOWN.REZ_CAM.MU_Len_Dump	        REAL    	Значение длины свинчивания при котором производится сброс(остановка)
REZ_TPC_DOWN.REZ_CAM.Mu_Len_Min	            REAL    	Минимальная длина свинчивания
REZ_TPC_DOWN.REZ_CAM.Mu_Len_Max	            REAL    	Максимальная длина свинчивания
REZ_TPC_DOWN.REZ_CAM.MU_JVal_Speed_1	    REAL    	Уменьшение скорости при достижениии значения J 1
REZ_TPC_DOWN.REZ_CAM.MU_JVAL_Speed_2	    REAL    	Снижение скорости при достижении значения J2
REZ_TPC_DOWN.REZ_CAM.MU_JVAL_Dump    	    REAL    	Значение J при котором выполняется останов свинчивания
REZ_TPC_DOWN.REZ_CAM.MU_JVal_Min	        REAL    	Минимальное значение J
REZ_TPC_DOWN.REZ_CAM.MU_JVal_Max	        REAL    	Максимальное значение J
REZ_TPC_DOWN.REZ_CAM.MU_Tq_Save	            REAL    	Крутящий момент автосохранения
REZ_TPC_DOWN.REZ_CAM.MU_RES_01       	    REAL    	Вероятно резерв
REZ_TPC_DOWN.REZ_CAM.MU_RES_02	            INT 		Вероятно резерв
```
```
KOM_REG.SPS_TPC_REZ.CMD_TPC                 UINT        команда от АРМ в ПЛК
KOM_REG.SPS_TPC_REZ.CMD_PLC                 UINT        команда от ПЛК в АРМ
```

## Запись оперативных параметров

```
REZ_TPC_UP.ERG_Muffe.PMR_BOX_RESULT	        UINT	        (2=NIO, 1=IO)	Результат измерения муфты?
REZ_TPC_UP.ERG_Muffe.PMR_BOX_LEN_BEGIN	    DTL             Время начала измерения муфты?
REZ_TPC_UP.ERG_Muffe.PMR_BOX_LEN_END	    DTL	            Время окончания?
REZ_TPC_UP.ERG_Muffe.PMR_BOX_LEN_VALUE	    REAL	        Длина муфты?
REZ_TPC_UP.ERG_Muffe.PMR_BOX_RES_00	        REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_Muffe.PMR_BOX_RES_01	        INT		        Вероятно резерв?
```
```
REZ_TPC_UP.ERG_MVS.PMR_Pre_MAKEUP_RESULT	UINT	        (2=NIO, 1=IO)	Результат предварительного свинчивания?
REZ_TPC_UP.ERG_MVS.PMR_Pre_MAKEUP_BEGIN	    DTL             Время начала предварительного свинчивания?
REZ_TPC_UP.ERG_MVS.PMR_Pre_MAKEUP_END	    DTL	            Время окончания предварительного свинчивания?
REZ_TPC_UP.ERG_MVS.PMR_PIPE_POS	            UINT	        (2=NIO, 1=IO)	Результат позиционирования трубы?
REZ_TPC_UP.ERG_MVS.PMR_Pre_MAKEUP_LEN	    REAL	        Длина предварительного свинчивания?
REZ_TPC_UP.ERG_MVS.PMR_PIPE_POS_LEN	        REAL	        Позиция трубы после предварительного свинчивания?
REZ_TPC_UP.ERG_MVS.PMR_PRE_MAKEUP_RES_00	REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_MVS.PMR_PRE_MAKEUP_RES_01	DINT		    Вероятно резерв?
```
```
REZ_TPC_UP.ERG_CAM.PMR_MR_MAKEUP_RESULT	DWORD	        (2=NIO, 1=IO)	Результат силового свинчивания в системе ПЛК. 
                                        Видимо если свинчивания не произведено (недудачное измерение муфты или преднавёртки), то ПЛК присваивает свой результат
REZ_TPC_UP.ERG_CAM.PMR_MR_MAKEUP_BEGIN	    DTL	            Время начала силового свинчивания?
REZ_TPC_UP.ERG_CAM.PMR_MR_MAKEUP_END	    DTL             Время окончания силового свинчивания?
REZ_TPC_UP.ERG_CAM.PMR_MR_TOTAL_RESULT	    UINT	        (2=NIO, 1=IO)	Еще один результат? Вероятно, что это оценка уже по параметрам (момент, длина, заплечник и т.д),
REZ_TPC_UP.ERG_CAM.PMR_MR_MAKEUP_LEN	    REAL	        Длина силового свинчивания
REZ_TPC_UP.ERG_CAM.PMR_MR_TOTAL_MAKEUP_LEN	REAL	        Общая длина (преднавертка + силовое свинчивание)
REZ_TPC_UP.ERG_CAM.PMR_MR_MAKEUP_FIN_TQ     REAL	        Окончательный Момент силовой навертки
REZ_TPC_UP.ERG_CAM.PMR_MR_MAKEUP_FIN_TN	    REAL	        Окончательное количество оборотов силовой навертки
REZ_TPC_UP.ERG_CAM.PMR_MR_TOTAL_MAKEUP_VAL	REAL            Окончательное значение J силовой навертки
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_0	            REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_01	        REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_02	        REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_03	        REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_04	        REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_05	        REAL		    Вероятно резерв?
REZ_TPC_UP.ERG_CAM.PMR_MR_RES_06	        USINT		    Вероятно резерв?
```
```
REZ_TPC_UP.REZ_ZST.GENERAL_TS	            DTL	            Общая
REZ_TPC_UP.REZ_ZST.BOX_TS	                DTL	            Измерение муфты
REZ_TPC_UP.REZ_ZST.PREMAKEUP_TS	            DTL	            Преднавёртка
REZ_TPC_UP.REZ_ZST.MAKEUP_TS	            DTL	            Силовая навёртка
REZ_TPC_UP.REZ_ZST.DATE_14	                DTL		
```
```
KOM_REG.CAM_CMD_PLC                         USINT           команда от ПЛК в АРМ
KOM_REG.CAM_CMD_TPC                         USINT           команда от АРМ в ПЛК
```
```
JointOperationalParams.Torque               REAL[0..4]      Очередь значений момента
JointOperationalParams.Length               REAL[0..4]      Очередь значений длины свинчивания
JointOperationalParams.Turns                REAL[0..4]      Очередь значений оборотов
```


## Системные параметры

```
KOM_REG.CAM_LOG_NO                          UINT            номер. Инкрементируется ПЛК
KOM_REG.CAM_CON_NO                          UINT            номер соединения. Инкрементируется ПЛК
KOM_REG.CAM_OPERATE_MODE                    UINT            Режим. 1 - ручной, 2 - авто
KOM_REG.CAM_TPC_READY                       UINT            Готовность АРМа к работе
KOM_REG.TPC_Heartbeat                       Boolean         Прямоугольный сигнал. Период 2 с. АРМ
KOM_REG.PLC_Heartbeat                       Boolean         Прямоугольный сигнал. Период 2 с. ПЛК
```
