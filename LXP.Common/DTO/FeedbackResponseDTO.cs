using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXP.Common.DTO
{
    public class FeedbackResponseDTO
    {
        public Guid Id { get; set; }    
        public Guid LearnerId { get; set; }
        public Guid TopicFeedbackQuestionId { get; set; }
        public Guid OptionId {  get; set; } 
        public string Response { get; set; }
        // Add any other properties you need
    }
}
