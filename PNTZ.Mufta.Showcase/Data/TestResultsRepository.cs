using LinqToDB;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PNTZ.Mufta.Showcase.Data
{
    /// <summary>
    /// Упрощенный репозиторий для загрузки реальных результатов из SQLite в тестовом окне
    /// </summary>
    public class TestResultsRepository
    {
        private readonly string _resultsConnectionString;

        /// <summary>
        /// Конструктор с указанием пути к файлу базы данных
        /// </summary>
        /// <param name="dbFilePath">Полный путь к файлу ResultsData.db</param>
        public TestResultsRepository(string dbFilePath)
        {
            _resultsConnectionString = $"Data Source={dbFilePath};Mode=ReadOnly";
        }

        /// <summary>
        /// Получить список результатов из базы данных
        /// </summary>
        /// <param name="filter">Необязательный фильтр для результатов</param>
        /// <returns>Список JointResultTable</returns>
        public List<JointResultTable> GetResults(Expression<Func<JointResultTable, bool>> filter = null)
        {
            using (var db = new JointResultContext(_resultsConnectionString))
            {
                var query = db.Results.AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                return query.OrderByDescending(r => r.FinishTimeStamp).ToList();
            }
        }

        /// <summary>
        /// Получить результат по ID
        /// </summary>
        /// <param name="id">ID результата</param>
        /// <returns>JointResult или null</returns>
        public JointResult GetResultById(Guid id)
        {
            using (var db = new JointResultContext(_resultsConnectionString))
            {
                var resultTable = db.Results.FirstOrDefault(r => r.Id == id);
                return resultTable?.ToJointResult();
            }
        }

        /// <summary>
        /// Получить список уникальных имен рецептов из результатов
        /// </summary>
        /// <returns>Список имен рецептов</returns>
        public List<string> GetRecipeNames()
        {
            using (var db = new JointResultContext(_resultsConnectionString))
            {
                return db.Results
                    .Select(r => r.Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();
            }
        }

        /// <summary>
        /// Получить количество результатов в базе
        /// </summary>
        /// <returns>Количество записей</returns>
        public int GetResultsCount()
        {
            using (var db = new JointResultContext(_resultsConnectionString))
            {
                return db.Results.Count();
            }
        }
    }
}
