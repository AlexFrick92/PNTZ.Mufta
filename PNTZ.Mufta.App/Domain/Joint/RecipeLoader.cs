using System;
using System.Threading;
using System.Threading.Tasks;

using Promatis.Core.Logging;

using DpConnect.Interface;
using DevExpress.Charts.Model;

namespace PNTZ.Mufta.App.Domain.Joint
{
    /// <summary>
    ///  Класс загружает рецепт в ПЛК. Загрузка рецепта выполняется обменом комманд
    /// </summary>
    public class RecipeLoader : IDpProcessor
    {
        private readonly ILogger _logger;

        private bool _recipeLoading;

        public RecipeLoader(ILogger logger)
        {
            _logger = logger;
        }
        public string Name { get; set; } = "Cam1RecipeLoader";

        public void OnDpInitialized()
        {
            //DpJointRecipe.ValueUpdated += (s, v) => Console.WriteLine(v.TURNS_BREAK + " " + v.HEAD_OPEN_PULSES);
        }
        public void LoadRecipe(JointRecipe recipe)
        {
            _logger.Info("Загрузка рецепта...");
            DpJointRecipe.Value = recipe;

            _logger.Info($"Рецепт загружен");
        }        

        public async Task LoadRecipeAsync(JointRecipe recipe)
        {
            if (_recipeLoading)
                throw new Exception("Рецепт уже загружается");

            _recipeLoading = true;

            TaskCompletionSource<uint> awaitCommandFeedback;

            var timeout = Task.Delay(TimeSpan.FromSeconds(10));


            _logger.Info("Загрузка рецепта...");            

            awaitCommandFeedback = new TaskCompletionSource<uint>();
            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            _logger.Info("Отправляем 10");

            SetLoadCommand.Value = 10;

            var first = await Task.WhenAny(awaitCommandFeedback.Task, timeout);                                     

            if(first == awaitCommandFeedback.Task && awaitCommandFeedback.Task.Result != 20)
            {
                _recipeLoading = false;
                throw new Exception($"Не удалось загрузить. Неверный ответ ({awaitCommandFeedback.Task.Result}) на команду 10");
            }

            _logger.Info("Ответ ПЛК: " + awaitCommandFeedback.Task.Result);

            awaitCommandFeedback = new TaskCompletionSource<uint>();
            CommandFeedback.ValueUpdated += (s, v) => awaitCommandFeedback.TrySetResult(v);

            _logger.Info("Пишем данные!");

            DpJointRecipe.Value = recipe;

            _logger.Info("Данные отправлены, ждем чуть-чуть");

            await Task.Delay(TimeSpan.FromMilliseconds(500));
            
            _logger.Info("Отправляем 40");

            SetLoadCommand.Value = 40;

            first = await Task.WhenAny(awaitCommandFeedback.Task, timeout);

            if (first == awaitCommandFeedback.Task)
            {
                if (awaitCommandFeedback.Task.Result != 50)
                {
                    _recipeLoading = false;
                    throw new Exception($"Не удалось загрузить. Неверный ответ ({awaitCommandFeedback.Task.Result}) на команду 40");
                }
            }
            else if (first == timeout)
            {
                _recipeLoading = false;
                throw new Exception("Не удалось загрузить по таймауту");
            }

            _logger.Info("Ответ ПЛК: " + awaitCommandFeedback.Task.Result);

            _logger.Info("Отправляем 0");

            SetLoadCommand.Value = 0;

            _recipeLoading = false;

            _logger.Info("Рецепт загружен!");
        }       

        #region DataPoints
        public IDpValue<JointRecipe> DpJointRecipe { get; set; }

        public IDpValue<uint> SetLoadCommand { get; set; }

        public IDpValue<uint> CommandFeedback { get; set; }

        #endregion
    }
}