using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    
        public class LearnerAnswerDto
        {
            public Guid LearnerAnswerId { get; set; }
            public Guid LearnerAttemptId { get; set; }
            public Guid QuizQuestionId { get; set; }
            public Guid QuestionOptionId { get; set; }
            public string CreatedBy { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedAt { get; set; }
        }
    }
