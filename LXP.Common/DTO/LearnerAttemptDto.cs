using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    
        public class LearnerAttemptDto
        {
            public Guid LearnerAttemptId { get; set; }
            public Guid LearnerId { get; set; }
            public Guid QuizId { get; set; }
            public int AttemptCount { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public float Score { get; set; }
            public string CreatedBy { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedAt { get; set; }
        }
    }

