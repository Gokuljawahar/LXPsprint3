using LXP.Common;
using LXP.Common.DTO;
using LXP.Data.DBContexts;
using LXP.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using LXP.Data.DBContexts;
using LXP.Data;
namespace LXP.Core.Repositories
{
    public class BulkQuestionRepository : IBulkQuestionRepository
    {
        private readonly LXPDbContext _dbContext;

        public BulkQuestionRepository(LXPDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<QuizQuestion> AddQuestions(List<QuizQuestion> questions)
        {
            _dbContext.QuizQuestions.AddRange(questions);
            _dbContext.SaveChanges();
            return questions;
        }

        public void AddOptions(List<QuestionOption> questionOptions, Guid quizQuestionId)
        {
            var existingQuestion = _dbContext.QuizQuestions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionId);
            if (existingQuestion != null)
            {
                foreach (var option in questionOptions)
                {
                    option.QuizQuestionId = quizQuestionId;
                    _dbContext.QuizFeedbackQuestionOptions.Add(option);
                }
                _dbContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Quiz question with ID {quizQuestionId} does not exist.");
            }
        }


    }
}