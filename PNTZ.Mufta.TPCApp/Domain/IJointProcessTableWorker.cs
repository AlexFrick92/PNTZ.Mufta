using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public interface IJointProcessTableWorker
    {
        /// <summary>
        /// Оценка оператора
        /// </summary>
        /// <param name="result">1-годная, 2-брак</param>
        void Evaluate(uint result);
        /// <summary>
        /// Труба появилась на позиции силовой навёртки
        /// </summary>
        event EventHandler<JointResultTable> PipeAppear;
        /// <summary>
        /// Начата запись графиков
        /// </summary>
        event EventHandler<EventArgs> RecordingBegun;
        /// <summary>
        /// Запись графиков остановлена
        /// </summary>
        event EventHandler<JointResultTable> RecordingFinished;
        /// <summary>
        /// Ожидание оценки оператора
        /// </summary>
        event EventHandler<JointResultTable> AwaitForEvaluation;
        /// <summary>
        /// Новая точка с данными
        /// </summary>
        event EventHandler<TqTnLenPoint> NewTqTnLenPoint;
        /// <summary>
        /// Свинчивание завершено
        /// </summary>
        event EventHandler<JointResultTable> JointFinished;

        bool CyclicallyListen { get; set; }

        void SetActualRecipe(JointRecipeTable recipe);
    }
}
