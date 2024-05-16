using Microsoft.AspNetCore.Http;

namespace LXP.Core.IServices
{
    public interface IBulkQuestionService
    {
        object ImportQuizData(IFormFile file);
    }
}