using LXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Data.IRepository
{
    public interface ITopicFeedbackRepository
    {
        IEnumerable<FeedbackQuestionDTO> GetAllFeedbackQuestions();
        FeedbackQuestionDTO GetFeedbackQuestionById(Guid id);
        void AddFeedbackResponse(FeedbackResponseDTO feedbackResponse);
        bool AddFeedbackQuestion(FeedbackQuestionDTO question);
        // Add other methods as needed
    }
}
