# Блок данных DB700 : KOM_REG_SP Машина 1

В блоке данных находятся переменные для команд - KOM_REG регистры коммуникаций.
Для обмена с ПЛК и АРМ используется еще один блок данных - DB400 : KOM_REG. Он служит буффером. То есть АРМ записывает и читает из DB400, а ПЛК работает с DB700. Данные между этими блками ПЛК копирует.

Есть три участника коммуникаций: ПЛК (SPS), TPC (приложение), EASY (контроллер измерений).
TPC через OPC записывает в этот блок данных команды, ПЛК их обрабатывает и записывает свою команду в этот же блок. TPC может считать эту команду. Коды команд определяют шаг коммуникаций.
Нотация именования переменных следующая:

Например, переменная TPC_CMD_REZ_TPC_SP из блока данных:
- префикс TPC_CMD - означает, что инициатором является TPC.
- суффикс TPC_SP - означает, что сюда команду записывает TPC, PLC_SP - переменную записывает ПЛК.
- REZ - означает предмет команды. REZ - рецептура, MP - параметры машин, CAM - результат соединения

Весь список переменных в блоке:

- TPC_CMD_REZ_TPC_SP - Команда TPC на загрузку рецепта
- TPC_CMD_REZ_PLC_SP - Ответ ПЛК на загрузку рецепта
- TPC_CMD_REZ_RES_00 - резерв
- TPC_CMD_MP_TPC_SP - Команда TPC для обработки параметра машин
- TPC_CMD_MP_PLC_SP - Ответ ПЛК для обработки параметра машин
- TPC_CMD_MP_RES_00 - резерв
- SPS_CMD_CAM_TPC_SP - Команда ПЛК на выгрузку результатов
- SPS_CMD_CAM_PLC_SP - Ответ TPC на выгрузку результатов
- SPS_CMD_CAM_RES_00 - резерв
- SPS_CMD_CAM_RES_01 - резерв
- SPS_CMD_CAM_RES_02 - резерв
- EASY_CMD_MP_EASY_SP - 
- EASY_CMD_MP_RES_0 -
- EASY_CMD_MP_RES_01 -
- EASY_CMD_MP_RES_02 -
- EASY_CMD_MP_SPS_SP -
- EASY_CMD_MP_RES_03 -
- EASY_CMD_MP_RES_04 -
- EASY_CMD_MP_RES_05 -
- EASY_CMD_MP_RES_06 -
- EASY_CMD_MP_RES_07 -
- EASY_CMD_REZ_EASY_SP -
- EASY_CMD_REZ_RES_08 -
- EASY_CMD_REZ_RES_01 -
- EASY_CMD_REZ_RES_02 -
- EASY_CMD_REZ_SPS_SP -
- EASY_CMD_REZ_RES_03 -
- EASY_CMD_REZ_RES_04 -
- EASY_CMD_REZ_RES_05 -
- EASY_CMD_REZ_RES_06 -
- EASY_CMD_REZ_RES_07 -
- EASY_CMD_CAM_EASY_SP1 -
- EASY_CMD_CAM_RES_01 -
- EASY_CMD_CAM_RES_02 -
- EASY_CMD_CAM_RES_03 -
- EASY_CMD_CAM_SPS_SP -
- EASY_CMD_CAM_RES_04 -
- EASY_CMD_CAM_RES_05 -
- EASY_CMD_CAM_RES_06 -
- EASY_CMD_CAM_RES_07 -
- EASY_CMD_CAM_RES_08 -
- EASY_CMD_RES_01 -
- EASY_CMD_RES_02 -
- EASY_CMD_RES_03 -
- EASY_CMD_RES_04 -
- EASY_CMD_RES_05 -
- EASY_CMD_RES_06 -
- EASY_CMD_RES_07 -
- EASY_CMD_RES_08 -
- EASY_CMD_RES_09 -
