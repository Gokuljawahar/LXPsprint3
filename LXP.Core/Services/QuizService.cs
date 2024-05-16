


using LXP.Common.DTO;
using LXP.Core.IServices;
using LXP.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Core.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public void CreateQuiz(QuizDto quiz)
        {
            _quizRepository.CreateQuiz(quiz);
        }

        public void UpdateQuiz(QuizDto quiz)
        {
            _quizRepository.UpdateQuiz(quiz);
        }


        public void DeleteQuiz(Guid quizId)
        {
            _quizRepository.DeleteQuiz(quizId);
        }

        public IEnumerable<QuizDto> GetAllQuizzes()
        {
            return _quizRepository.GetAllQuizzes();
        }

        public QuizDto GetQuizById(Guid quizId)
        {
            return _quizRepository.GetQuizById(quizId);
        }


    }
}



//public void CreateQuiz(Guid quizId, Guid courseId, Guid topicId, string nameOfQuiz, int duration, int passMark, string createdBy, DateTime createdAt)
//{
//    _quizRepository.CreateQuiz(quizId, courseId, topicId, nameOfQuiz, duration, passMark, createdBy, createdAt);
//}



//public void CreateQuiz(Guid quizId, Guid courseId, Guid topicId, string nameOfQuiz, int duration, int passMark, string createdBy, DateTime createdAt)
//{
//    _quizRepository.CreateQuiz(quizId, courseId, topicId, nameOfQuiz, duration, passMark, createdBy, createdAt);
//}
