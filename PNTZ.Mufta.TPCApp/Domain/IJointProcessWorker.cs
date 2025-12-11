using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public interface IJointProcessWorker
    {
        /// <summary>
        /// Оценка оператора
        /// </summary>
        /// <param name="result">1-годная, 2-брак</param>
        void Evaluate(uint result);
        /// <summary>
        /// Труба появилась на позиции силовой навёртки
        /// </summary>
        event EventHandler<JointResult> PipeAppear;
        /// <summary>
        /// Начата запись графиков
        /// </summary>
        event EventHandler<EventArgs> RecordingBegun;
        /// <summary>
        /// Запись графиков остановлена
        /// </summary>
        event EventHandler<JointResult> RecordingFinished;
        /// <summary>
        /// Ожидание оценки оператора
        /// </summary>
        event EventHandler<JointResult> AwaitForEvaluation;
        /// <summary>
        /// Новая точка с данными
        /// </summary>
        event EventHandler<TqTnLenPoint> NewTqTnLenPoint;        
        /// <summary>
        /// Свинчивание завершено
        /// </summary>
        event EventHandler<JointResult> JointFinished;

        bool CyclicallyListen { get; set; }

        void SetActualRecipe(JointRecipe recipe);

    }
}
