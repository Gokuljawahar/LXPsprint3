using LXP.Common.DTO;
using LXP.Data.IRepository;
using LXP.Data.DBContexts;


namespace LXP.Data.Repository
{
    public class QuizRepository : IQuizRepository
    {
        private readonly LXPDbContext _LXPDbContext;

        public QuizRepository(LXPDbContext dbContext)
        {
            _LXPDbContext = dbContext;
        }


        public void CreateQuiz(QuizDto quiz)
        {
            // Validate NameOfQuiz
            if (string.IsNullOrWhiteSpace(quiz.NameOfQuiz))
                throw new Exception("NameOfQuiz cannot be null or empty.");

            // Validate Duration
            if (quiz.Duration <= 0)
                throw new Exception("Duration must be a positive integer.");

            // Validate PassMark
            if (quiz.PassMark <= 0)
                throw new Exception("PassMark must be a positive integer.");

            // Validate AttemptsAllowed
            if (quiz.AttemptsAllowed.HasValue && quiz.AttemptsAllowed <= 0)
                throw new Exception("AttemptsAllowed must be null or a positive integer.");

            var quizEntity = new Quiz
            {
                QuizId = quiz.QuizId,
                CourseId = quiz.CourseId,
                TopicId = quiz.TopicId,
                NameOfQuiz = quiz.NameOfQuiz,
                Duration = quiz.Duration,
                PassMark = quiz.PassMark,
                AttemptsAllowed = quiz.AttemptsAllowed,
                CreatedBy = quiz.CreatedBy,
                CreatedAt = quiz.CreatedAt
            };

            _LXPDbContext.Quizzes.Add(quizEntity);
            _LXPDbContext.SaveChanges();
        }

        public void UpdateQuiz(QuizDto quiz)
        {
            // Validate NameOfQuiz
            if (string.IsNullOrWhiteSpace(quiz.NameOfQuiz))
                throw new Exception("NameOfQuiz cannot be null or empty.");

            // Validate Duration
            if (quiz.Duration <= 0)
                throw new Exception("Duration must be a positive integer.");

            // Validate PassMark
            if (quiz.PassMark <= 0)
                throw new Exception("PassMark must be a positive integer.");

            // Validate AttemptsAllowed
            if (quiz.AttemptsAllowed.HasValue && quiz.AttemptsAllowed <= 0)
                throw new Exception("AttemptsAllowed must be null or a positive integer.");

            var quizEntity = _LXPDbContext.Quizzes.Find(quiz.QuizId);
            if (quizEntity != null)
            {
                quizEntity.NameOfQuiz = quiz.NameOfQuiz;
                quizEntity.Duration = quiz.Duration;
                quizEntity.PassMark = quiz.PassMark;
                quizEntity.AttemptsAllowed = quiz.AttemptsAllowed;

                _LXPDbContext.SaveChanges();
            }
        }

        public void DeleteQuiz(Guid quizId)
        {
            var quizEntity = _LXPDbContext.Quizzes.Find(quizId);
            if (quizEntity != null)
            {
                _LXPDbContext.Quizzes.Remove(quizEntity);
                _LXPDbContext.SaveChanges();
            }
        }

        public IEnumerable<QuizDto> GetAllQuizzes()
        {
            return _LXPDbContext.Quizzes
                .Select(q => new QuizDto
                {
                    QuizId = q.QuizId,
                    CourseId = q.CourseId,
                    TopicId = q.TopicId,
                    NameOfQuiz = q.NameOfQuiz,
                    Duration = q.Duration,
                    PassMark = q.PassMark,
                    AttemptsAllowed = q.AttemptsAllowed
                })
                .ToList();
        }

        public QuizDto GetQuizById(Guid quizId)
        {
            return _LXPDbContext.Quizzes
                .Where(q => q.QuizId == quizId)
                .Select(q => new QuizDto
                {
                    QuizId = q.QuizId,
                    CourseId = q.CourseId,
                    TopicId = q.TopicId,
                    NameOfQuiz = q.NameOfQuiz,
                    Duration = q.Duration,
                    PassMark = q.PassMark,
                    AttemptsAllowed = q.AttemptsAllowed
                })
                .FirstOrDefault();
        }
    }
}



//public void CreateQuiz(Guid quizId, Guid courseId, Guid topicId, string nameOfQuiz, int duration, int passMark, string createdBy, DateTime createdAt)
//{
//    var quizEntity = new Quiz
//    {
//        QuizId = quizId,
//        CourseId = courseId,
//        TopicId = topicId,
//        NameOfQuiz = nameOfQuiz,
//        Duration = duration,
//        PassMark = passMark,
//        CreatedBy = createdBy,
//        CreatedAt = createdAt,
//    };

//    _LXPDbContext.Quizzes.Add(quizEntity);
//    _LXPDbContext.SaveChanges();
//}

//using LXP.Common.DTO;
//using LXP.Data.IRepository;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LXP.Data.Repository
//{
//    public class QuizRepository : IQuizRepository
//    {

//        public void Create(QuizDto quiz)
//        {
//            throw new NotImplementedException();
//        }

//        public void Delete(Guid quizId)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<QuizDto> GetAll()
//        {
//            throw new NotImplementedException();
//        }

//        public QuizDto GetById(Guid quizId)
//        {
//            throw new NotImplementedException();
//        }

//        public void Update(QuizDto quiz)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
// public void CreateQuiz(QuizDto quiz)
// {
//     try
//     {
//         // 1. Check if topic exists (optional)
//         var topicExists = _LXPDbContext.Topics.Any(t => t.TopicId == quiz.TopicId);
//         if (!topicExists)
//         {
//             throw new Exception("Invalid topic ID provided."); // or return an error message
//         }
//     }
//     catch (Exception ex)
//     {
//         throw; // Re-throw the exception for handling at a higher level (optional)
//                // OR
//                // Log the error and return an appropriate message (recommended)
//         Console.WriteLine($"Error creating quiz: {ex.Message}");
//         throw new Exception("Invalid topic ID provided."); // Or a more user-friendly message
//         //Console.WriteLine($"Error creating quiz: {ex.Message}");
//         //return BadRequest("Invalid topic ID provided."); // Or a more descriptive message
//     }


//     // ... other logic


//     var quizEntity = new Quiz
//     {
//         QuizId = quiz.QuizId, // You can generate a random ID here if needed
//         CourseId = quiz.CourseId, // Use your pre-defined fake Course ID here
//         TopicId = quiz.TopicId,  // Use your pre-defined fake Topic ID here
//         NameOfQuiz = quiz.NameOfQuiz,
//         Duration = quiz.Duration,
//         PassMark = quiz.PassMark,
//         CreatedBy = quiz.CreatedBy,
//         CreatedAt = quiz.CreatedAt
//     };

//     _LXPDbContext.Quizzes.Add(quizEntity);
//     _LXPDbContext.SaveChanges();
// }
// public void CreateQuiz(QuizDto quiz)
// {
//     var quizEntity = new Quiz
//     {
//         QuizId = quiz.QuizId,
//         CourseId = quiz.CourseId,
//         TopicId = quiz.TopicId,
//         NameOfQuiz = quiz.NameOfQuiz,
//         Duration = quiz.Duration,
//         PassMark = quiz.PassMark,
//         CreatedBy = quiz.CreatedBy,
//         CreatedAt = quiz.CreatedAt,
//         ModifiedBy = quiz.ModifiedBy,
//         ModifiedAt = quiz.ModifiedAt
//     };

//     _LXPDbContext.Quizzes.Add(quizEntity);
//     _LXPDbContext.SaveChanges();
// }
// public IEnumerable<QuizDto> GetAllQuizzes()
// {
//     return _LXPDbContext.Quizzes
//         .Select(q => new QuizDto
//         {
//             QuizId = q.QuizId,
//             CourseId = q.CourseId,
//             TopicId = q.TopicId,
//             NameOfQuiz = q.NameOfQuiz,
//             Duration = q.Duration,
//             PassMark = q.PassMark,
//             CreatedBy = q.CreatedBy,
//             CreatedAt = q.CreatedAt,
//             ModifiedBy = q.ModifiedBy,
//             ModifiedAt = (DateTime)q.ModifiedAt
//         })
//         .ToList();
// }
// public QuizDto GetQuizById(Guid quizId)
// {
//     return _LXPDbContext.Quizzes
//         .Where(q => q.QuizId == quizId)
//         .Select(q => new QuizDto
//         {
//             QuizId = q.QuizId,
//             CourseId = q.CourseId,
//             TopicId = q.TopicId,
//             NameOfQuiz = q.NameOfQuiz,
//             Duration = q.Duration,
//             PassMark = q.PassMark,
//             CreatedBy = q.CreatedBy,
//             CreatedAt = q.CreatedAt,
//             ModifiedBy = q.ModifiedBy,
//             ModifiedAt = (DateTime)q.ModifiedAt
//         })
//         .FirstOrDefault();
// }

// public void UpdateQuiz(QuizDto quiz)
// {
//     var quizEntity = _LXPDbContext.Quizzes.Find(quiz.QuizId);
//     if (quizEntity != null)
//     {
//         quizEntity.NameOfQuiz = quiz.NameOfQuiz;
//         quizEntity.Duration = quiz.Duration;
//         quizEntity.PassMark = quiz.PassMark;
//         quizEntity.ModifiedBy = quiz.ModifiedBy;
//         quizEntity.ModifiedAt = quiz.ModifiedAt;

//         _LXPDbContext.SaveChanges();
//     }
// }
//public void UpdateQuiz(QuizDto quiz)
// {
//     var quizEntity = _LXPDbContext.Quizzes.Find(quiz.QuizId);
//     if (quizEntity != null)
//     {
//         quizEntity.NameOfQuiz = quiz.NameOfQuiz;
//         quizEntity.Duration = quiz.Duration;
//         quizEntity.PassMark = quiz.PassMark;

//         _LXPDbContext.SaveChanges();
//     }
// }

// public void CreateQuiz(Guid quizId, Guid courseId, Guid topicId, string nameOfQuiz, int duration, int passMark, string createdBy, DateTime createdAt)
// {
//     var quizEntity = new Quiz
//     {
//         QuizId = quizId,
//         CourseId = courseId,
//         TopicId = topicId,
//         NameOfQuiz = nameOfQuiz,
//         Duration = duration,
//         PassMark = passMark,
//         CreatedBy = createdBy,
//         CreatedAt = createdAt,
//     };

//     _LXPDbContext.Quizzes.Add(quizEntity);
//     _LXPDbContext.SaveChanges();
// }