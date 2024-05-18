using System;
using System.Collections.Generic;
using System.Linq;
using LXP.Common.DTO;
using LXP.Data;
using LXP.Data.DBContexts;
using LXP.Data.IRepository;

namespace LXP.Data.Repository
{
    public static class FeedbackQuestionTypes
    {
        public const string MultiChoiceQuestion = "MCQ";
        public const string DescriptiveQuestion = "Descriptive";
    }
    public class QuizFeedbackRepository : IQuizFeedbackRepository
    {
        private readonly LXPDbContext _dbContext;

        public QuizFeedbackRepository(LXPDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Guid AddFeedbackQuestion(QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options)
        {
            var questionEntity = new Quizfeedbackquestion
            {
                QuizId = quizfeedbackquestionDto.QuizId,
                QuestionNo = GetNextFeedbackQuestionNo(quizfeedbackquestionDto.QuizId),
                Question = quizfeedbackquestionDto.Question,
                QuestionType = quizfeedbackquestionDto.QuestionType,
                CreatedBy = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Quizfeedbackquestions.Add(questionEntity);
            _dbContext.SaveChanges();

            if (options != null && options.Count > 0)
            {
                foreach (var option in options)
                {
                    var optionEntity = new Feedbackquestionsoption
                    {
                        QuizFeedbackQuestionId = questionEntity.QuizFeedbackQuestionId,
                        OptionText = option.OptionText,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "Admin"
                    };
                    _dbContext.Feedbackquestionsoptions.Add(optionEntity);
                }
                _dbContext.SaveChanges();
            }

            return questionEntity.QuizFeedbackQuestionId;
        }

        public List<QuizfeedbackquestionNoDto> GetAllFeedbackQuestions()
        {
            return _dbContext.Quizfeedbackquestions
                .Select(q => new QuizfeedbackquestionNoDto
                {
                    QuizFeedbackQuestionId = q.QuizFeedbackQuestionId,
                    QuizId = q.QuizId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType
                }).ToList();
        }

        public void DecrementFeedbackQuestionNos(Guid deletedQuestionId)
        {
            var deletedQuestion = _dbContext.Quizfeedbackquestions.FirstOrDefault(q => q.QuizFeedbackQuestionId == deletedQuestionId);
            if (deletedQuestion != null)
            {
                var questionsToUpdate = _dbContext.Quizfeedbackquestions
                    .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
                    .OrderBy(q => q.QuestionNo)
                    .ToList();
                foreach (var question in questionsToUpdate)
                {
                    question.QuestionNo--;
                }
                _dbContext.SaveChanges();
            }
        }

        public int GetNextFeedbackQuestionNo(Guid quizId)
        {
            var lastQuestion = _dbContext.Quizfeedbackquestions
                .Where(q => q.QuizId == quizId)
                .OrderByDescending(q => q.QuestionNo)
                .FirstOrDefault();
            return lastQuestion != null ? lastQuestion.QuestionNo + 1 : 1;
        }

        //public int GetNextQuestionNo(Guid quizId)
        //{
        //    try
        //    {
        //        return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException(
        //            "An error occurred while retrieving the next question number.",
        //            ex
        //        );
        //    }
        //}

        public Guid AddFeedbackQuestionOption(FeedbackquestionsoptionDto feedbackquestionsoptionDto, Guid quizFeedbackQuestionId)
        {
            var optionEntity = new Feedbackquestionsoption
            {
                QuizFeedbackQuestionId = quizFeedbackQuestionId,
                OptionText = feedbackquestionsoptionDto.OptionText,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };
            _dbContext.Feedbackquestionsoptions.Add(optionEntity);
            _dbContext.SaveChanges();

            return optionEntity.FeedbackQuestionOptionId;
        }

        public List<FeedbackquestionsoptionDto> GetFeedbackQuestionOptionsById(Guid quizFeedbackQuestionId)
        {
            return _dbContext.Feedbackquestionsoptions
                .Where(o => o.QuizFeedbackQuestionId == quizFeedbackQuestionId)
                .Select(o => new FeedbackquestionsoptionDto
                {
                    //FeedbackQuestionOptionId = o.FeedbackQuestionOptionId,
                    OptionText = o.OptionText
                }).ToList();
        }

        public QuizfeedbackquestionNoDto GetFeedbackQuestionById(Guid quizFeedbackQuestionId)
        {
            return _dbContext.Quizfeedbackquestions
                .Where(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId)
                .Select(q => new QuizfeedbackquestionNoDto
                {
                    QuizFeedbackQuestionId = q.QuizFeedbackQuestionId,
                    QuizId = q.QuizId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType
                }).FirstOrDefault();
        }

        public bool ValidateOptionsByFeedbackQuestionType(string questionType, List<FeedbackquestionsoptionDto> options)
        {
            // Implement validation logic based on question type
            if (questionType == "MCQ")
            {
                return options != null && options.Count >= 2 && options.Count <= 5;
            }
            return true; // For descriptive questions, options are not required
        }

        public bool UpdateFeedbackQuestion(Guid quizFeedbackQuestionId, QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options)
        {
            var existingQuestion = _dbContext.Quizfeedbackquestions.FirstOrDefault(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId);
            if (existingQuestion != null)
            {
                existingQuestion.Question = quizfeedbackquestionDto.Question;
                existingQuestion.QuestionType = quizfeedbackquestionDto.QuestionType;
                existingQuestion.ModifiedAt = DateTime.UtcNow;
                existingQuestion.ModifiedBy = "Admin";
                _dbContext.SaveChanges();

                var existingOptions = _dbContext.Feedbackquestionsoptions.Where(o => o.QuizFeedbackQuestionId == quizFeedbackQuestionId).ToList();
                _dbContext.Feedbackquestionsoptions.RemoveRange(existingOptions);
                _dbContext.SaveChanges();

                if (options != null && options.Count > 0)
                {
                    foreach (var option in options)
                    {
                        var optionEntity = new Feedbackquestionsoption
                        {
                            QuizFeedbackQuestionId = quizFeedbackQuestionId,
                            OptionText = option.OptionText,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "Admin"
                        };
                        _dbContext.Feedbackquestionsoptions.Add(optionEntity);
                    }
                    _dbContext.SaveChanges();
                }

                return true;
            }
            return false;
        }


        public bool DeleteFeedbackQuestion(Guid quizFeedbackQuestionId)
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var existingQuestion = _dbContext.Quizfeedbackquestions.FirstOrDefault(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId);
                if (existingQuestion != null)
                {
                   
                    var relatedOptions = _dbContext.Feedbackquestionsoptions
                                                    .Where(o => o.QuizFeedbackQuestionId == quizFeedbackQuestionId)
                                                    .ToList();

                    if (relatedOptions.Any())
                    {
                        _dbContext.Feedbackquestionsoptions.RemoveRange(relatedOptions);
                    }

                    _dbContext.Quizfeedbackquestions.Remove(existingQuestion);
                    _dbContext.SaveChanges();

                 
                    DecrementFeedbackQuestionNos(quizFeedbackQuestionId);

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


        //public bool DeleteFeedbackQuestion(Guid quizFeedbackQuestionId)
        //{
        //    using var transaction = _dbContext.Database.BeginTransaction();
        //    try
        //    {
        //        var existingQuestion = _dbContext.Quizfeedbackquestions.FirstOrDefault(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId);
        //        if (existingQuestion != null)
        //        {
        //            // Find related FeedbackQuestionsOptions and delete them
        //            var relatedOptions = _dbContext.Feedbackquestionsoptions
        //                                            .Where(o => o.QuizFeedbackQuestionId == quizFeedbackQuestionId)
        //                                            .ToList();

        //            if (relatedOptions.Any())
        //            {
        //                _dbContext.Feedbackquestionsoptions.RemoveRange(relatedOptions);
        //            }

        //            _dbContext.Quizfeedbackquestions.Remove(existingQuestion);
        //            _dbContext.SaveChanges();
        //            DecrementFeedbackQuestionNos(quizFeedbackQuestionId);
        //            transaction.Commit();
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        transaction.Rollback();
        //        // Log the exception (ex) here if necessary
        //        throw;
        //    }
        //    return false;
        //}


        //public bool DeleteFeedbackQuestion(Guid quizFeedbackQuestionId)
        //{
        //    var existingQuestion = _dbContext.Quizfeedbackquestions.FirstOrDefault(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId);
        //    if (existingQuestion != null)
        //    {
        //        _dbContext.Quizfeedbackquestions.Remove(existingQuestion);
        //        _dbContext.SaveChanges();
        //        DecrementFeedbackQuestionNos(quizFeedbackQuestionId);
        //        return true;
        //    }
        //    return false;
        //}
    }
}






















































//using LXP.Common.DTO;
//using LXP.Data.IRepository;
//using LXP.Data.DBContexts;

//namespace LXP.Data.Repository
//{

//    public static class FeedbackQuestionTypes
//    {      
//        public const string MultiChoiceQuestion = "MCQ";
//        public const string DescriptiveQuestion = "Descriptive";
//    }

//    public class QuizFeedbackRepository : IQuizFeedbackRepository
//    {
//        private readonly LXPDbContext _dbContext;

//        public QuizFeedbackRepository(LXPDbContext dbContext)
//        {
//            _dbContext = dbContext;    
//        }

//        public Guid AddFeedbackQuestion(QuizfeedbackquestionDto quizfeedbackquestionDto, List<FeedbackquestionsoptionDto> options)
//        {
            
//        }
//    }
//}
