using LXP.Common.DTO;
using LXP.Data.DBContexts;
using LXP.Data.IRepository;

namespace LXP.Data.Repository
{
    public class TopicFeedbackRepository : ITopicFeedbackRepository
    {
        private readonly LXPDbContext _context;

        public TopicFeedbackRepository(LXPDbContext context)
        {
            _context = context;
        }

        public bool AddFeedbackQuestion(TopicFeedbackQuestionDTO question, List<FeedbackOptionDTO> options)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));



            int questionCount = _context.Topicfeedbackquestions.Count(q => q.TopicId == question.TopicId);

            int questionNo = questionCount + 1;

            var feedbackQuestion = new Topicfeedbackquestion
            {
                TopicId = Guid.Parse("e3a895e4-1b3f-45b8-9c0a-98f9c0fa4996"),
                QuestionNo = questionNo,
                Question = question.Question,
                QuestionType = question.QuestionType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            if (question.QuestionType == "MCQ" && question.Options != null)
            {
                foreach (var option in options)
                {
                    feedbackQuestion.Feedbackquestionsoptions.Add(new Feedbackquestionsoption
                    {
                        OptionText = option.OptionText
                    });
                }
            }

            _context.Topicfeedbackquestions.Add(feedbackQuestion);
            _context.SaveChanges();

            return true;
        }


        public IEnumerable<TopicFeedbackQuestionNoDTO> GetAllFeedbackQuestions()
        {
            return _context.Topicfeedbackquestions
                .Select(q => new TopicFeedbackQuestionNoDTO
                {
                    TopicFeedbackId = q.TopicFeedbackQuestionId,
                    TopicId = q.TopicId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType
                })
                .ToList();
        }

        public TopicFeedbackQuestionNoDTO GetFeedbackQuestionById(Guid id)
        {
            var question = _context.Topicfeedbackquestions.FirstOrDefault(q => q.TopicFeedbackQuestionId == id);
            if (question == null)
                return null;

            return new TopicFeedbackQuestionNoDTO
            {
                TopicFeedbackId = question.TopicFeedbackQuestionId,
                TopicId = question.TopicId,
                QuestionNo = question.QuestionNo,
                Question = question.Question,
                QuestionType = question.QuestionType
            };
        }

        public void AddFeedbackResponse(TopicFeedbackResponseDTO feedbackResponse)
        {
            var response = new Feedbackresponse
            {
                TopicFeedbackQuestionId = feedbackResponse.TopicFeedbackQuestionId,
                LearnerId = feedbackResponse.LearnerId,
                Response = feedbackResponse.Response,
                OptionId = feedbackResponse.OptionId,

            };

            _context.Feedbackresponses.Add(response);
            _context.SaveChanges();
        }

        public bool UpdateFeedbackQuestion(Guid id, TopicFeedbackQuestionDTO question, List<FeedbackOptionDTO> options)
        {
            var existingQuestion = _context.Topicfeedbackquestions.FirstOrDefault(q => q.TopicFeedbackQuestionId == id);
            if (existingQuestion != null)
            {
                existingQuestion.Question = question.Question;
                existingQuestion.QuestionType = question.QuestionType;
                existingQuestion.ModifiedAt = DateTime.UtcNow;
                existingQuestion.ModifiedBy = "Admin";
                _context.SaveChanges();

                var existingOptions = _context.Feedbackquestionsoptions.Where(o => o.TopicFeedbackQuestionId == id).ToList();
                _context.Feedbackquestionsoptions.RemoveRange(existingOptions);
                _context.SaveChanges();

                if (options != null && options.Count > 0)
                {
                    foreach (var option in options)
                    {
                        var optionEntity = new Feedbackquestionsoption
                        {
                            TopicFeedbackQuestionId = id,
                            OptionText = option.OptionText,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "Admin"
                        };
                        _context.Feedbackquestionsoptions.Add(optionEntity);
                    }
                    _context.SaveChanges();
                }

                return true;
            }
            return false;

        }

        public bool DeleteFeedbackQuestion(Guid id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var existingQuestion = _context.Topicfeedbackquestions.FirstOrDefault(q => q.TopicFeedbackQuestionId == id);
                if (existingQuestion != null)
                {

                    var relatedOptions = _context.Feedbackquestionsoptions
                                                    .Where(o => o.TopicFeedbackQuestionId == id)
                                                    .ToList();
                    if (relatedOptions.Any())
                    {
                        _context.Feedbackquestionsoptions.RemoveRange(relatedOptions);
                    }

                    _context.Topicfeedbackquestions.Remove(existingQuestion);
                    _context.SaveChanges();

                    ReorderQuestionNo(existingQuestion.TopicId, existingQuestion.QuestionNo);

                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
            return false;

        }

        //private void DecrementQuestionNos(Guid deletedQuestionId)
        //{
        //    var deletedQuestion = _context.Topicfeedbackquestions.FirstOrDefault(q => q.TopicFeedbackQuestionId == deletedQuestionId);
        //    if (deletedQuestion != null)
        //    {
        //        var questionsToUpdate = _context.Topicfeedbackquestions
        //            .Where(q => q.TopicId == deletedQuestion.TopicId && q.QuestionNo > deletedQuestion.QuestionNo)
        //            .OrderBy(q => q.QuestionNo)
        //            .ToList();
        //        _context.SaveChanges();

            
        //    }
        //}

        private void ReorderQuestionNo(Guid topicId, int deletedQuestionNo)
        {
            var subsequentQuestions = _context.Topicfeedbackquestions.Where(q => q.TopicId == topicId && q.QuestionNo > deletedQuestionNo).ToList();
            foreach(var question in subsequentQuestions)
            {
                question.QuestionNo--;
            }
            _context.SaveChanges();
        }

    }
}
