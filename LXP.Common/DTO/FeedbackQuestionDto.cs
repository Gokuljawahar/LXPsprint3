﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    
        public class FeedbackQuestionDto
        {
            public Guid FeedbackQuestionId { get; set; }
            public Guid TopicId { get; set; }
            public int QuestionNo { get; set; }
            public string Question { get; set; } = null!;
            public string QuestionType { get; set; } = null!;
            public string FeedbackType { get; set; } = null!;
            public string CreatedBy { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public string? ModifiedBy { get; set; }
            public DateTime? ModifiedAt { get; set; }
        }
    }
