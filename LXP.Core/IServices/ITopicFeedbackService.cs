using LXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Core.IServices
{
    public interface ITopicFeedbackService
    {
        IEnumerable<FeedbackQuestionDTO> GetAllFeedbackQuestions();
        FeedbackQuestionDTO GetFeedbackQuestionById(Guid id);
        void SubmitFeedbackResponse(FeedbackResponseDTO feedbackResponse);
        bool AddFeedbackQuestion(FeedbackQuestionDTO question);
        // Add other methods as needed
    }
}
