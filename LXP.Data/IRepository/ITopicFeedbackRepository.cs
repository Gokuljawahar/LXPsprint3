﻿using LXP.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Data.IRepository
{
    public interface ITopicFeedbackRepository
    {
        IEnumerable<TopicFeedbackQuestionNoDTO> GetAllFeedbackQuestions();
        TopicFeedbackQuestionNoDTO GetFeedbackQuestionById(Guid id);
        void AddFeedbackResponse(TopicFeedbackResponseDTO feedbackResponse);
        bool AddFeedbackQuestion(TopicFeedbackQuestionDTO question, List<FeedbackOptionDTO> options);
        bool DeleteFeedbackQuestion(Guid id);
        bool UpdateFeedbackQuestion(Guid id, TopicFeedbackQuestionDTO question, List<FeedbackOptionDTO> options);
        // Add other methods as needed
    }
}
