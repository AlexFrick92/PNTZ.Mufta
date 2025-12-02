using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Promatis.Core.Logging;

namespace PNTZ.Mufta.TPCApp.Domain
{
    /// <summary>
    /// Оценка
    /// </summary>
    internal class JointEvaluation
    {
        private ILogger _logger;
        public JointEvaluation(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Evaluate(JointResult result)
        {
            try
            {
                _logger.Info("Оценка свинчивания...");

                if (CheckConditionsByMode(result))
                {
                    result.ResultTotal = 1;
                    return true;
                }
                else
                {
                    result.ResultTotal = 2;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при выполнении оценки: " + ex.Message);
                return false;
            }
        }

        private bool CheckConditionsByMode(JointResult result)
        {
            switch (result.Recipe.JointMode)
            {
                case JointMode.Torque:
                    return EvaluateTorque(result);

                case JointMode.TorqueShoulder:                    
                    var evaluation = EstimateShoulderTorque(result) 
                        & EvaluateTorque(result) 
                        & EvaluateShoulder(result);
                    return false; //evaluation;

                case JointMode.TorqueLength:
                    return EvaluateTorque(result) && EvaluateLength(result);

                case JointMode.Length:
                    return EvaluateLength(result);

                default:
                    throw new OperationCanceledException($"Оценка данного режима не поддерживается. Выбранный режим: {result.Recipe.JointMode}");
            }
        }

        private bool EvaluateTorque(JointResult result)
        {
            _logger.Info($"Оценка крутящего момента...");
            _logger.Info($"Крутящий момент: {result.FinalTorque} Нм, допустимый диапазон: {result.Recipe.MU_Tq_Min} - {result.Recipe.MU_Tq_Max} Нм");

            if (result.FinalTorque < result.Recipe.MU_Tq_Min || result.FinalTorque > result.Recipe.MU_Tq_Max)
            {
                _logger.Info($"Отклонено! Крутящий момент {result.FinalTorque} Нм вне допустимого диапазона.");
                return false;
            }
            else
            {
                _logger.Info($"Крутящий момент {result.FinalTorque} Нм в пределах допустимого диапазона.");
                return true;
            }
        }

        private bool EvaluateLength(JointResult result)
        {
            _logger.Info("Оценка длины...");
            _logger.Info($"Длина: {result.FinalLength * 1000} м, допустимый диапазон: {result.Recipe.MU_Len_Min} - {result.Recipe.MU_Len_Max} м");
            if ((result.FinalLength * 1000) < result.Recipe.MU_Len_Min || (result.FinalLength * 1000) > result.Recipe.MU_Len_Max)
            {
                _logger.Info($"Отклонено! Длина {(result.FinalLength * 1000)} м вне допустимого диапазона.");
                return false;
            }
            else
            {
                _logger.Info($"Длина {(result.FinalLength * 1000)} м в пределах допустимого диапазона.");
                return true;
            }
        }
        
        private bool EvaluateShoulder(JointResult result)
        {
            _logger.Info("Оценка плеча...");
            _logger.Info($"Плечо: {result.FinalShoulderTorque} Нм, допустимый диапазон: {result.Recipe.MU_TqShoulder_Max} - {result.Recipe.MU_TqShoulder_Min} Нм");
            if (result.FinalShoulderTorque < result.Recipe.MU_TqShoulder_Max || result.FinalShoulderTorque > result.Recipe.MU_TqShoulder_Min)
            {
                _logger.Info($"Отклонено! Плечо {result.FinalShoulderTorque} Нм вне допустимого диапазона.");
                return false;
            }
            else
            {
                _logger.Info($"Плечо {result.FinalShoulderTorque} Нм в пределах допустимого диапазона.");
                return true;
            }
        }

        private bool EstimateShoulderTorque(JointResult result)
        {
            ShoulderPointDetector detector = new ShoulderPointDetector(result.Series);
            int? shoulderIndex = detector.DetectShoulderPoint();
            if (shoulderIndex != null)
            {
                float shoulderTorque = result.Series[shoulderIndex.Value].Torque;
                float shoulderTurns = result.Series[shoulderIndex.Value].Turns;
                _logger.Info($"Точка заплечника обнаружена на индексе {shoulderIndex.Value} с моментом {shoulderTorque} Нм.");
                result.FinalShoulderTorque = shoulderTorque;
                result.FinalShoulderTurns = shoulderTurns;
                return true;
            }
            else
            {
                _logger.Info("Точка заплечника не обнаружена");
                return false;
            }
        }
    }
}
