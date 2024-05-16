using LXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Data.IRepository
{
    public interface IQuizRepository
    {

        QuizDto GetQuizById(Guid quizId);
        IEnumerable<QuizDto> GetAllQuizzes();

        void CreateQuiz(QuizDto quiz);
        void UpdateQuiz(QuizDto quiz);
        void DeleteQuiz(Guid quizId);


    }
}