using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    
        public class FeedbackQuestionOptionDto
        {
            public Guid OptionId { get; set; }
            public Guid FeedbackQuestionId { get; set; }
            public string Option { get; set; } = null!;
            public string CreatedBy { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedAt { get; set; }
        }
    }
