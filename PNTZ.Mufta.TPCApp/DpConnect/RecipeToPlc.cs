﻿using System;
using System.Threading.Tasks;

using DpConnect;

using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.DpConnect.Struct;

using Promatis.Core.Logging;


namespace PNTZ.Mufta.TPCApp.DpConnect
{
    internal class RecipeToPlc : IDpWorker
    {
        public IDpValue<uint> DpTpcCommand { get; set; }
        public IDpValue<uint> DpPlcCommand { get; set; }
        public IDpValue<REZ_ALLG> Dp_REZ_ALLG { get; set; }
        public IDpValue<REZ_Muffe> Dp_REZ_Muffe { get; set; }
        public IDpValue<REZ_MVS> Dp_REZ_MVS { get; set; }
        public IDpValue<REZ_CAM> Dp_REZ_CAM { get; set; }

        ILogger logger;

        public bool LoadingProcedureStarted { get; private set; }

        public RecipeToPlc(ILogger logger)
        {
            this.logger = logger;
        }
        public void DpBound()
        {

        }
        public async Task LoadRecipeAsync(JointRecipe recipe)
        {
            if (LoadingProcedureStarted)
                throw new Exception("Рецепт уже загружается");

            LoadingProcedureStarted = true;

            TaskCompletionSource<uint> awaitCommandFeedback;

            var timeout = Task.Delay(TimeSpan.FromSeconds(10));

            logger.Info("Загрузка рецепта...");

            awaitCommandFeedback = new TaskCompletionSource<uint>();
            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            logger.Info("Отправляем 10");

            DpTpcCommand.Value = 10;

            var first = await Task.WhenAny(awaitCommandFeedback.Task, timeout);

            if (first == awaitCommandFeedback.Task && awaitCommandFeedback.Task.Result != 20)
            {
                LoadingProcedureStarted = false;
                throw new Exception($"Не удалось загрузить. Неверный ответ ({awaitCommandFeedback.Task.Result}) на команду 10");
            }

            logger.Info("Ответ ПЛК: " + awaitCommandFeedback.Task.Result);

            awaitCommandFeedback = new TaskCompletionSource<uint>();
            DpPlcCommand.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            logger.Info("Пишем данные!");

            Dp_REZ_ALLG.Value = new REZ_ALLG().FromRecipe(recipe);

            Dp_REZ_Muffe.Value = new REZ_Muffe().FromRecipe(recipe);

            Dp_REZ_MVS.Value = new REZ_MVS().FromRecipe(recipe);   

            Dp_REZ_CAM.Value = new REZ_CAM().FromRecipe(recipe);   
            

            logger.Info("Данные отправлены, ждем чуть-чуть");

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            logger.Info("Отправляем 40");

            DpTpcCommand.Value = 40;

            first = await Task.WhenAny(awaitCommandFeedback.Task, timeout);

            if (first == awaitCommandFeedback.Task)
            {
                if (awaitCommandFeedback.Task.Result != 50)
                {
                    LoadingProcedureStarted = false;
                    throw new Exception($"Не удалось загрузить. Неверный ответ ({awaitCommandFeedback.Task.Result}) на команду 40");
                }
            }
            else if (first == timeout)
            {
                LoadingProcedureStarted = false;
                throw new Exception("Не удалось загрузить по таймауту");
            }

            logger.Info("Ответ ПЛК: " + awaitCommandFeedback.Task.Result);

            logger.Info("Отправляем 0");

            DpTpcCommand.Value = 0;

            LoadingProcedureStarted = false;

            logger.Info("Рецепт загружен!");
        }
    }
}