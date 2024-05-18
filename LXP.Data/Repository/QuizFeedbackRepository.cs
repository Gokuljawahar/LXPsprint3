﻿using System;
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



        public Guid AddFeedbackQuestion(QuizFeedbackQuestionDto quizfeedbackquestionDto, List<QuizFeedbackQuestionsOptionDto> options)
        {
            try
            {
                // Normalize question type to uppercase
                var normalizedQuestionType = quizfeedbackquestionDto.QuestionType.ToUpper();

                // Ensure no options are saved for descriptive questions
                if (normalizedQuestionType == FeedbackQuestionTypes.DescriptiveQuestion.ToUpper())
                {
                    options = null;
                }

                if (!ValidateOptionsByFeedbackQuestionType(quizfeedbackquestionDto.QuestionType, options))
                    throw new ArgumentException(
                        "Invalid options for the given question type.",
                        nameof(options)
                    );

                // Create and save the feedback question entity
                var questionEntity = new Quizfeedbackquestion
                {
                    QuizId = quizfeedbackquestionDto.QuizId,
                    QuestionNo = GetNextFeedbackQuestionNo(quizfeedbackquestionDto.QuizId),
                    Question = quizfeedbackquestionDto.Question,
                    QuestionType = normalizedQuestionType,
                    CreatedBy = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Quizfeedbackquestions.Add(questionEntity);
                _dbContext.SaveChanges();

                // Save the options only if the question type is MCQ
                if (normalizedQuestionType == FeedbackQuestionTypes.MultiChoiceQuestion.ToUpper())
                {
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
                }

                return questionEntity.QuizFeedbackQuestionId;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if necessary
                throw new InvalidOperationException("An error occurred while adding the feedback question.", ex);
            }
        }

        public List<QuizFeedbackQuestionNoDto> GetAllFeedbackQuestions()
        {
            return _dbContext.Quizfeedbackquestions
                .Select(q => new QuizFeedbackQuestionNoDto
                {
                    QuizFeedbackQuestionId = q.QuizFeedbackQuestionId,
                    QuizId = q.QuizId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType,
                    Options = _dbContext.Feedbackquestionsoptions
                                    .Where(o => o.QuizFeedbackQuestionId == q.QuizFeedbackQuestionId)
                                    .Select(
                                        o =>
                                            new QuizFeedbackQuestionsOptionDto
                                            {
                                                OptionText = o.OptionText,
                                              
                                            }
                                    )
                                    .ToList()
                }).ToList();
        }


        public int GetNextFeedbackQuestionNo(Guid quizId)
        {
            var lastQuestion = _dbContext.Quizfeedbackquestions
                .Where(q => q.QuizId == quizId)
                .OrderByDescending(q => q.QuestionNo)
                .FirstOrDefault();
            return lastQuestion != null ? lastQuestion.QuestionNo + 1 : 1;
        }


        private void ValidateFeedbackQuestion(QuizFeedbackQuestionDto quizfeedbackquestionDto, List<QuizFeedbackQuestionsOptionDto> options)
        {
            if (quizfeedbackquestionDto.QuestionType == FeedbackQuestionTypes.MultiChoiceQuestion)
            {
                if (options == null || options.Count == 0)
                {
                    throw new ArgumentException("MCQ questions must have at least one option.");
                }
            }
            else if (quizfeedbackquestionDto.QuestionType == FeedbackQuestionTypes.DescriptiveQuestion)
            {
                if (options != null && options.Count > 0)
                {
                    throw new ArgumentException("Descriptive questions should not have options.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid question type.");
            }
        }



        public Guid AddFeedbackQuestionOption(QuizFeedbackQuestionsOptionDto feedbackquestionsoptionDto, Guid quizFeedbackQuestionId)
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

        public List<QuizFeedbackQuestionsOptionDto> GetFeedbackQuestionOptionsById(Guid quizFeedbackQuestionId)
        {
            return _dbContext.Feedbackquestionsoptions
                .Where(o => o.QuizFeedbackQuestionId == quizFeedbackQuestionId)
                .Select(o => new QuizFeedbackQuestionsOptionDto
                {
                    //FeedbackQuestionOptionId = o.FeedbackQuestionOptionId,
                    OptionText = o.OptionText
                }).ToList();
        }

        public QuizFeedbackQuestionNoDto GetFeedbackQuestionById(Guid quizFeedbackQuestionId)
        {
            return _dbContext.Quizfeedbackquestions
                .Where(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId)
                .Select(q => new QuizFeedbackQuestionNoDto
                {
                    QuizFeedbackQuestionId = q.QuizFeedbackQuestionId,
                    QuizId = q.QuizId,
                    QuestionNo = q.QuestionNo,
                    Question = q.Question,
                    QuestionType = q.QuestionType,

                    Options = _dbContext.Feedbackquestionsoptions
                                    .Where(o => o.QuizFeedbackQuestionId == q.QuizFeedbackQuestionId)
                                    .Select(
                                        o =>
                                            new QuizFeedbackQuestionsOptionDto
                                            {
                                                OptionText = o.OptionText,

                                            }
                                    )
                                    .ToList()
                }).FirstOrDefault();
        }

   
        public bool ValidateOptionsByFeedbackQuestionType(string questionType, List<QuizFeedbackQuestionsOptionDto> options)
        {
            questionType = questionType.ToUpper();

            if (questionType == FeedbackQuestionTypes.MultiChoiceQuestion.ToUpper())
            {
                return options != null && options.Count >= 2 && options.Count <= 5;
            }
            return options == null || options.Count == 0;
        }


        public bool UpdateFeedbackQuestion(Guid quizFeedbackQuestionId, QuizFeedbackQuestionDto quizfeedbackquestionDto, List<QuizFeedbackQuestionsOptionDto> options)
        {
            try
            {
                var existingQuestion = _dbContext.Quizfeedbackquestions.FirstOrDefault(q => q.QuizFeedbackQuestionId == quizFeedbackQuestionId);
                if (existingQuestion != null)
                {
                    // Check if the question type is being modified
                    if (!existingQuestion.QuestionType.Equals(quizfeedbackquestionDto.QuestionType, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException("Question type cannot be modified.");
                    }

                    // Validate options based on question type
                    if (!ValidateOptionsByFeedbackQuestionType(existingQuestion.QuestionType, options))
                    {
                        throw new ArgumentException("Invalid options for the given question type.");
                    }

                    // Update the question details
                    existingQuestion.Question = quizfeedbackquestionDto.Question;
                    existingQuestion.ModifiedAt = DateTime.UtcNow;
                    existingQuestion.ModifiedBy = "Admin";
                    _dbContext.SaveChanges();

                    // Remove existing options if question type is MultiChoiceQuestion
                    if (existingQuestion.QuestionType == FeedbackQuestionTypes.MultiChoiceQuestion.ToUpper())
                    {
                        var existingOptions = _dbContext.Feedbackquestionsoptions.Where(o => o.QuizFeedbackQuestionId == quizFeedbackQuestionId).ToList();
                        _dbContext.Feedbackquestionsoptions.RemoveRange(existingOptions);
                        _dbContext.SaveChanges();

                        // Add new options
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
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if necessary
                throw new InvalidOperationException("An error occurred while updating the feedback question.", ex);
            }
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

                    ReorderQuestionNos(existingQuestion.QuizId, existingQuestion.QuestionNo);

                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return false;
        }

        public void ReorderQuestionNos(Guid quizId, int deletedQuestionNo)
        {
            var questionsToUpdate = _dbContext.Quizfeedbackquestions
                                              .Where(q => q.QuizId == quizId && q.QuestionNo > deletedQuestionNo)
                                              .OrderBy(q => q.QuestionNo)
                                              .ToList();

            foreach (var question in questionsToUpdate)
            {
                question.QuestionNo--;
            }
            _dbContext.SaveChanges();
        }

    }
}























































