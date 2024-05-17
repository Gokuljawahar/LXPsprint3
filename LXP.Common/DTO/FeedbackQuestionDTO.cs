using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    public class FeedbackQuestionDTO
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public int QuestionNo { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public List<string> Options { get; set; }
        // Add any other properties you need
    }
}
