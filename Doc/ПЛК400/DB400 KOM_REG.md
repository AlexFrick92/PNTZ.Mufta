# DB400 KOM_REG

Блок данных с регистрами обмена между АРМ (TPC) и ПЛК.

Блок данных имеет следующую структуру:

```
SPS_TPC_REZ         SPS-TPC     команды ПЛК/АРМ для загрузки рецепта
    CMD_TPC         DWORD
    CMD_PLC         DWORD
    Reserve_01      DWORD
    Reserve_02      DWORD
    Reserve_03      DWORD
    Reserve_04      DWORD
    Reserve_05      DWORD
    Reserve_06      DWORD
    Reserve_07      DWORD
    Reserve_08      DWORD
    Reserve_09      DWORD
    Reserve_10      DWORD
    Reserve_11      WORD

SPS_TPC_MDAT        SPS-TPC     команды ПЛК/АРМ для загрузки параметров машин
    CMD_TPC         DWORD
    CMD_PLC         DWORD
    Reserve_01      DWORD
    Reserve_02      DWORD
    Reserve_03      DWORD
    Reserve_04      DWORD
    Reserve_05      DWORD
    Reserve_06      DWORD
    Reserve_07      DWORD
    Reserve_08      DWORD
    Reserve_09      DWORD
    Reserve_10      DWORD
    Reserve_11      WORD

CAM_CMD_TPC         DWORD       команда АРМ для оперативных параметров
CAM_CMD_PLC         DWORD       команда ПЛК для оперативных параметров
CAM_Watchdog        DWORD       Бит жизни. Устанавливается в 1 АРМ. ПЛК сбрасывает. 
CAM_LOG_NO          DWORD       АРМ в рецепте указывает значение, а ПЛК делает инкремент
CAM_CON_NO          DWORD       АРМ в рецепте указывает значение, а ПЛК делает инкремент
CAM_OPERATE_MODE    DWORD       Режим ПЛК. 1 - ручной, 2 - авто
CAM_TPC_READY       DWORD       Готовность АРМ к работе
```