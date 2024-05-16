
using LXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Core.IServices
{
    public interface IQuizService
    {
        QuizDto GetQuizById(Guid quizId);
        IEnumerable<QuizDto> GetAllQuizzes();
        //void CreateQuiz(Guid quizId, Guid courseId, Guid topicId, string nameOfQuiz, int duration, int passMark, string createdBy, DateTime createdAt);

        void CreateQuiz(QuizDto quiz);
        void UpdateQuiz(QuizDto quiz);
        void DeleteQuiz(Guid quizId);
    }
}