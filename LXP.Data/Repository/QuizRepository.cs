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

        public Guid? GetQuizIdByTopicId(Guid topicId)
        {
            var quizId = _LXPDbContext?.Quizzes.Where(q => q.TopicId == topicId).Select(q => q.QuizId).FirstOrDefault();
            return quizId != Guid.Empty? quizId : (Guid?)null;
        }
    }
}

