using System.Linq.Expressions;

namespace Promatis.Core.Expressions
{
    /// <summary>
    /// Вспомогательный класс, содержащий методы по замене параметров в узлах деревьев выражений
    /// </summary>
    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _targetParameter;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ParameterReplaceVisitor"/>
        /// </summary>
        /// <param name="param">Выражение с параметром</param>
        public ParameterReplaceVisitor(ParameterExpression param)
        {
            _targetParameter = param;
        }

        /// <summary>
        /// Заменяет параметр внутри выражения на указанный
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="parameter">Параметр</param>
        /// <returns>Выражение с замененным параметром</returns>
        public static Expression ReplaceAllParameter(Expression expression, ParameterExpression parameter)
        {
            return new ParameterReplaceVisitor(parameter).Visit(expression);
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression p) => base.VisitParameter(_targetParameter);
    }
}
