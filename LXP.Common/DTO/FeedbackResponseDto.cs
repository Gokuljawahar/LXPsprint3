using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
   
        public class FeedbackResponseDto
    {
            public Guid FeedbackResponseId { get; set; }
            public Guid FeedbackQuestionId { get; set; }
            public Guid LearnerId { get; set; }
            public string Response { get; set; } = null!;
            public Guid FeedbackQuestionOptionoptionId { get; set; }
            public string CreatedBy { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedAt { get; set; }
        }
    }

