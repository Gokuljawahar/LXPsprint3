using System.Threading.Tasks;
using LXP.Common.DTO;
using Microsoft.AspNetCore.Http;

namespace LXP.Data.IRepository
{
    public interface IBulkQuestionRepository
    {
        List<QuizQuestion> AddQuestions(List<QuizQuestion> quizQuestions);
        void AddOptions(List<QuestionOption> questionOptions, Guid quizQuestionId);
        // Add other repository methods as needed
    }
}