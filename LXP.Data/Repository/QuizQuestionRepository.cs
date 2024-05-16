using LXP.Common.DTO;
using LXP.Data.IRepository;
using LXP.Data.DBContexts;

namespace LXP.Data.Repository
{
    public static class QuestionTypes
    {
        public const string MultiSelectQuestion = "MSQ";
        public const string MultiChoiceQuestion = "MCQ";
        public const string TrueFalseQuestion = "T/F";
    }

    public class QuizQuestionRepository : IQuizQuestionRepository
    {
        private readonly LXPDbContext _LXPDbContext;

        public QuizQuestionRepository(LXPDbContext dbContext)
        {
            _LXPDbContext =
                dbContext
                ?? throw new ArgumentNullException(nameof(dbContext), "DB context cannot be null.");
        }

        public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
                    throw new ArgumentException(
                        "Question cannot be null or empty.",
                        nameof(quizQuestionDto.Question)
                    );

                if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
                    throw new ArgumentException(
                        "QuestionType cannot be null or empty.",
                        nameof(quizQuestionDto.QuestionType)
                    );

                quizQuestionDto.QuestionType = quizQuestionDto.QuestionType.ToUpper();

                if (!IsValidQuestionType(quizQuestionDto.QuestionType))
                    throw new ArgumentException(
                        "Invalid question type.",
                        nameof(quizQuestionDto.QuestionType)
                    );

                if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
                    throw new ArgumentException(
                        "Invalid options for the given question type.",
                        nameof(options)
                    );
                if (!ValidateOptions(quizQuestionDto.QuestionType, options))
                    throw new ArgumentException(
                        "Invalid options for the given question type.",
                        nameof(options)
                    );

                var quizQuestionEntity = new QuizQuestion
                {
                    QuizId = quizQuestionDto.QuizId,
                    //QuizId = Guid.Parse("4db699e3-6867-47f9-9bf6-841c221038a3"),
                    Question = quizQuestionDto.Question,
                    QuestionType = quizQuestionDto.QuestionType,
                    QuestionNo = GetNextQuestionNo(quizQuestionDto.QuizId),
                    CreatedBy = "SystemUser",
                    CreatedAt = DateTime.UtcNow
                };

                _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
                _LXPDbContext.SaveChanges();

                foreach (var option in options)
                {
                    var questionOptionEntity = new QuestionOption
                    {
                        QuizQuestionId = quizQuestionEntity.QuizQuestionId,
                        Option = option.Option,
                        IsCorrect = option.IsCorrect,
                        CreatedBy = "SystemUser",
                        CreatedAt = DateTime.UtcNow
                    };

                    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
                }

                _LXPDbContext.SaveChanges();

                return quizQuestionEntity.QuizQuestionId;
            }
            catch (ArgumentException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while adding the quiz question.",
                    ex
                );
            }
        }

        private bool IsValidQuestionType(string questionType)
        {
            return questionType == QuestionTypes.MultiSelectQuestion
                || questionType == QuestionTypes.MultiChoiceQuestion
                || questionType == QuestionTypes.TrueFalseQuestion;
        }


        public bool UpdateQuestion(
    Guid quizQuestionId,
    QuizQuestionDto quizQuestionDto,
    List<QuestionOptionDto> options
         )
        {
            try
            {
                var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
                if (quizQuestionEntity == null)
                    return false;

                if (quizQuestionDto.QuestionType.ToUpper() != quizQuestionEntity.QuestionType)
                {
                    throw new InvalidOperationException("Question type cannot be updated.");
                }


                // Update the question and question type
                quizQuestionEntity.Question = quizQuestionDto.Question;

                _LXPDbContext.SaveChanges();

                // Remove existing options
                var existingOptions = _LXPDbContext.QuestionOptions
                    .Where(o => o.QuizQuestionId == quizQuestionId)
                    .ToList();
                _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

                // Add new options
                foreach (var option in options)
                {
                    var questionOptionEntity = new QuestionOption
                    {
                        QuizQuestionId = quizQuestionEntity.QuizQuestionId,
                        Option = option.Option,
                        IsCorrect = option.IsCorrect,
                        CreatedBy = quizQuestionEntity.CreatedBy,
                        CreatedAt = quizQuestionEntity.CreatedAt
                    };

                    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
                }

                // Validate options based on the existing question type
                if (!ValidateOptionsByQuestionType(quizQuestionEntity.QuestionType, options))
                    throw new ArgumentException(
                        "Invalid options for the given question type.",
                        nameof(options)
                    );

                _LXPDbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while updating the quiz question.",
                    ex
                );
            }
        }

        public bool DeleteQuestion(Guid quizQuestionId)
        {
            try
            {
                var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
                if (quizQuestionEntity == null)
                    return false;

                _LXPDbContext.QuestionOptions.RemoveRange(
                    _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId)
                );
                _LXPDbContext.QuizQuestions.Remove(quizQuestionEntity);
                _LXPDbContext.SaveChanges();

                ReorderQuestionNos(quizQuestionEntity.QuizId, quizQuestionEntity.QuestionNo);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while deleting the quiz question.",
                    ex
                );
            }
        }
        private void ReorderQuestionNos(Guid quizId, int deletedQuestionNo)
        {
            var subsequentQuestions = _LXPDbContext.QuizQuestions
                .Where(q => q.QuizId == quizId && q.QuestionNo > deletedQuestionNo)
                .ToList();
            foreach (var question in subsequentQuestions)
            {
                question.QuestionNo--;
            }
            _LXPDbContext.SaveChanges();
        }


        public List<QuizQuestionNoDto> GetAllQuestions()
        {
            try
            {
                return _LXPDbContext.QuizQuestions
                    .Select(
                        q =>
                            new QuizQuestionNoDto
                            {
                                QuizId = q.QuizId,
                                QuizQuestionId = q.QuizQuestionId,
                                Question = q.Question,
                                QuestionType = q.QuestionType,
                                QuestionNo = q.QuestionNo,

                                Options = _LXPDbContext.QuestionOptions
                                    .Where(o => o.QuizQuestionId == q.QuizQuestionId)
                                    .Select(
                                        o =>
                                            new QuestionOptionDto
                                            {
                                                Option = o.Option,
                                                IsCorrect = o.IsCorrect
                                            }
                                    )
                                    .ToList()
                            }
                    )
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while retrieving all quiz questions.",
                    ex
                );
            }
        }
        public QuizQuestionNoDto GetQuestionById(Guid quizQuestionId)
        {
            try
            {
                return _LXPDbContext.QuizQuestions
                    .Where(q => q.QuizQuestionId == quizQuestionId)
                    .Select(q =>
                        new QuizQuestionNoDto
                        {
                            QuizId = q.QuizId,
                            QuizQuestionId = q.QuizQuestionId,
                            Question = q.Question,
                            QuestionType = q.QuestionType,
                            QuestionNo = q.QuestionNo,

                            Options = _LXPDbContext.QuestionOptions
                                .Where(o => o.QuizQuestionId == q.QuizQuestionId)
                                .Select(o =>
                                    new QuestionOptionDto
                                    {
                                        Option = o.Option,
                                        IsCorrect = o.IsCorrect
                                    }
                                )
                                .ToList()
                        }
                    )
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while retrieving the quiz question by ID.",
                    ex
                );
            }
        }





        public int GetNextQuestionNo(Guid quizId)
        {
            try
            {
                return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while retrieving the next question number.",
                    ex
                );
            }
        }

        public void DecrementQuestionNos(Guid deletedQuestionId)
        {
            try
            {
                var deletedQuestion = _LXPDbContext.QuizQuestions.Find(deletedQuestionId);
                if (deletedQuestion == null)
                    return;

                var subsequentQuestions = _LXPDbContext.QuizQuestions
                    .Where(
                        q =>
                            q.QuizId == deletedQuestion.QuizId
                            && q.QuestionNo > deletedQuestion.QuestionNo
                    )
                    .ToList();
                foreach (var question in subsequentQuestions)
                {
                    question.QuestionNo--;
                }
                _LXPDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while decrementing question numbers.",
                    ex
                );
            }
        }

        public Guid AddQuestionOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
        {
            try
            {
                var questionOptionEntity = new QuestionOption
                {
                    QuizQuestionId = quizQuestionId,
                    Option = questionOptionDto.Option,
                    IsCorrect = questionOptionDto.IsCorrect,
                    CreatedBy = "SystemUser",
                    CreatedAt = DateTime.UtcNow
                };

                _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
                _LXPDbContext.SaveChanges();

                return questionOptionEntity.QuestionOptionId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while adding a question option.",
                    ex
                );
            }
        }

        public List<QuestionOptionDto> GetQuestionOptionsById(Guid quizQuestionId)
        {
            try
            {
                return _LXPDbContext.QuestionOptions
                    .Where(o => o.QuizQuestionId == quizQuestionId)
                    .Select(
                        o => new QuestionOptionDto { Option = o.Option, IsCorrect = o.IsCorrect }
                    )
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An error occurred while retrieving question options by ID.",
                    ex
                );
            }
        }

        public bool ValidateOptionsByQuestionType(
            string questionType,
            List<QuestionOptionDto> options
        )
        {
            switch (questionType)
            {
                case QuestionTypes.MultiSelectQuestion:
                    return options.Count >= 5
                        && options.Count <= 8
                        && options.Count(o => o.IsCorrect) >= 2
                        && options.Count(o => o.IsCorrect) <= 3;
                case QuestionTypes.MultiChoiceQuestion:
                    return options.Count == 4 && options.Count(o => o.IsCorrect) == 1;
                case QuestionTypes.TrueFalseQuestion:
                    return options.Count == 2 && options.Count(o => o.IsCorrect) == 1;
                default:
                    return false;
            }
        }
        public bool ValidateOptions(string questionType, List<QuestionOptionDto> options)
        {
            if (questionType == QuestionTypes.TrueFalseQuestion)
            {
                return ValidateTrueFalseOptions(options);
            }
            else
            {
                return ValidateUniqueOptions(options)
                    && ValidateOptionsByQuestionType(questionType, options);
            }
        }

        private bool ValidateTrueFalseOptions(List<QuestionOptionDto> options)
        {
            // Check if there are exactly two options
            if (options.Count != 2)
                return false;

            // Check if one option is true and the other is false (case-insensitive)
            var trueOption = options.Any(o => o.Option.ToLower() == "true");
            var falseOption = options.Any(o => o.Option.ToLower() == "false");

            return trueOption && falseOption;
        }

        private bool ValidateUniqueOptions(List<QuestionOptionDto> options)
        {
            // Check if all options are unique (case-insensitive)
            var distinctOptions = options.Select(o => o.Option.ToLower()).Distinct().Count();
            return distinctOptions == options.Count;
        }
    }
}

//public List<QuizQuestionNoDto> GetAllQuestions()
//{
//    try
//    {
//        return _LXPDbContext.QuizQuestions
//            .Select(
//                q =>
//                    new QuizQuestionNoDto
//                    {
//                        QuizId = q.QuizId,
//                        Question = q.Question,
//                        QuestionType = q.QuestionType,
//                        QuestionNo = q.QuestionNo,

//                        Options = _LXPDbContext.QuestionOptions
//                            .Where(o => o.QuizQuestionId == q.QuizQuestionId)
//                            .Select(
//                                o =>
//                                    new QuestionOptionDto
//                                    {
//                                        Option = o.Option,
//                                        IsCorrect = o.IsCorrect
//                                    }
//                            )
//                            .ToList()
//                    }
//            )
//            .ToList();
//    }
//    catch (Exception ex)
//    {
//        throw new InvalidOperationException(
//            "An error occurred while retrieving all quiz questions.",
//            ex
//        );
//    }
//}

//public bool UpdateQuestion(
//    Guid quizQuestionId,
//    QuizQuestionDto quizQuestionDto,
//    List<QuestionOptionDto> options
//)
//{
//    try
//    {
//        var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//        if (quizQuestionEntity == null)
//            return false;

//        if (!ValidateOptions(quizQuestionDto.QuestionType, options))
//            throw new ArgumentException("Invalid options for the given question type.", nameof(options));


//        quizQuestionEntity.Question = quizQuestionDto.Question;
//        quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;

//        _LXPDbContext.SaveChanges();

//        var existingOptions = _LXPDbContext.QuestionOptions
//            .Where(o => o.QuizQuestionId == quizQuestionId)
//            .ToList();
//        _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//        foreach (var option in options)
//        {
//            var questionOptionEntity = new QuestionOption
//            {
//                QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                Option = option.Option,
//                IsCorrect = option.IsCorrect,
//                CreatedBy = quizQuestionEntity.CreatedBy,
//                CreatedAt = quizQuestionEntity.CreatedAt
//            };

//            _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//        }

//        _LXPDbContext.SaveChanges();

//        return true;
//    }
//    catch (Exception ex)
//    {
//        throw new InvalidOperationException(
//            "An error occurred while updating the quiz question.",
//            ex
//        );
//    }
//}
/* for testing
 * public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
{
try
{
var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
if (quizQuestionEntity == null)
    return false;

// Update the question
quizQuestionEntity.Question = quizQuestionDto.Question;

_LXPDbContext.SaveChanges();

// Get existing options
var existingOptions = _LXPDbContext.QuestionOptions
    .Where(o => o.QuizQuestionId == quizQuestionId)
    .ToList();

// Update existing options, add new options, or remove options
foreach (var option in options)
{
    var existingOption = existingOptions.FirstOrDefault(o => o.QuestionOptionId == option.QuestionOptionId);
    if (existingOption != null)
    {
        // Update existing option
        if (option.Option != null)
            existingOption.Option = option.Option;
        if (option.IsCorrect != existingOption.IsCorrect)
            existingOption.IsCorrect = option.IsCorrect;
    }
    else
    {
        // Add new option
        var questionOptionEntity = new QuestionOption
        {
            QuizQuestionId = quizQuestionEntity.QuizQuestionId,
            Option = option.Option,
            IsCorrect = option.IsCorrect,
            CreatedBy = quizQuestionEntity.CreatedBy,
            CreatedAt = quizQuestionEntity.CreatedAt
        };

        _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
    }
}

// Remove options that are not in the updated list
var optionsToRemove = existingOptions.Where(o => !options.Any(opt => opt.QuestionOptionId == o.QuestionOptionId)).ToList();
_LXPDbContext.QuestionOptions.RemoveRange(optionsToRemove);

// Validate options based on the existing question type
if (!ValidateOptionsByQuestionType(quizQuestionEntity.QuestionType, options))
    throw new ArgumentException("Invalid options for the given question type.", nameof(options));

_LXPDbContext.SaveChanges();

return true;
}
catch (Exception ex)
{
throw new InvalidOperationException("An error occurred while updating the quiz question.", ex);
}
}
 * 
 * 
 * */

/* for testing 2 
 * public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
{
try
{
var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
if (quizQuestionEntity == null)
    return false;

// Update the question
quizQuestionEntity.Question = quizQuestionDto.Question;

_LXPDbContext.SaveChanges();

// Get existing options
var existingOptions = _LXPDbContext.QuestionOptions
    .Where(o => o.QuizQuestionId == quizQuestionId)
    .ToList();

// Update existing options or add new options (if allowed for the question type)
foreach (var option in options)
{
    var existingOption = existingOptions.FirstOrDefault(o => o.QuestionOptionId == option.QuestionOptionId);
    if (existingOption != null)
    {
        // Update existing option
        if (option.Option != null)
            existingOption.Option = option.Option;
        if (option.IsCorrect != existingOption.IsCorrect)
            existingOption.IsCorrect = option.IsCorrect;
    }
    else if (quizQuestionEntity.QuestionType == QuestionTypes.MultiSelectQuestion)
    {
        // Add new option for MSQ type only
        var questionOptionEntity = new QuestionOption
        {
            QuizQuestionId = quizQuestionEntity.QuizQuestionId,
            Option = option.Option,
            IsCorrect = option.IsCorrect,
            CreatedBy = quizQuestionEntity.CreatedBy,
            CreatedAt = quizQuestionEntity.CreatedAt
        };

        _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
    }
}

// Remove options that are not in the updated list
var optionsToRemove = existingOptions.Where(o => !options.Any(opt => opt.QuestionOptionId == o.QuestionOptionId)).ToList();
_LXPDbContext.QuestionOptions.RemoveRange(optionsToRemove);

// Validate options based on the existing question type
if (!ValidateOptionsByQuestionType(quizQuestionEntity.QuestionType, options))
    throw new ArgumentException("Invalid options for the given question type.", nameof(options));

_LXPDbContext.SaveChanges();

return true;
}
catch (Exception ex)
{
throw new InvalidOperationException("An error occurred while updating the quiz question.", ex);
}
}
 * 
 * 
 * 
 */



//public class QuizQuestionRepository : IQuizQuestionRepository
//{
//    private readonly LXPDbContext _LXPDbContext;

//    public QuizQuestionRepository(LXPDbContext dbContext)
//    {
//        _LXPDbContext = dbContext;
//    }

//




//    public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//    {
//        var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//        if (quizQuestionEntity != null)
//        {
//            quizQuestionEntity.Question = quizQuestionDto.Question;
//            quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;

//            _LXPDbContext.SaveChanges();

//            // Update options for the question
//            var existingOptions = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//            _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//            // Add options for the question
//            foreach (var option in options)
//            {
//                var questionOptionEntity = new QuestionOption
//                {
//                    QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                    Option = option.Option,
//                    IsCorrect = option.IsCorrect,
//                    CreatedBy = quizQuestionEntity.CreatedBy, // Preserve the original CreatedBy value
//                    CreatedAt = quizQuestionEntity.CreatedAt // Preserve the original CreatedAt value
//                };

//                _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//            }

//            _LXPDbContext.SaveChanges();

//            return true;
//        }
//        return false;
//    }

//    public bool DeleteQuestion(Guid quizQuestionId)
//    {
//        var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//        if (quizQuestionEntity != null)
//        {
//            _LXPDbContext.QuestionOptions.RemoveRange(_LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId));
//            _LXPDbContext.QuizQuestions.Remove(quizQuestionEntity);
//            _LXPDbContext.SaveChanges();

//            ReorderQuestionNos(quizQuestionEntity.QuizId, quizQuestionEntity.QuestionNo);
//            return true;
//        }
//        return false;
//    }

//

//    public List<QuizQuestionDto> GetAllQuestions()
//    {
//        return _LXPDbContext.QuizQuestions
//            .Select(q => new QuizQuestionDto
//            {
//                QuizId = q.QuizId,
//                Question = q.Question,
//                QuestionType = q.QuestionType,
//                Options = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == q.QuizQuestionId)
//                    .Select(o => new QuestionOptionDto
//                    {
//                        Option = o.Option,
//                        IsCorrect = o.IsCorrect
//                    })
//                    .ToList()
//            })
//            .ToList();
//    }

//    public int GetNextQuestionNo(Guid quizId)
//    {
//        return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
//    }

//    public void DecrementQuestionNos(Guid deletedQuestionId)
//    {
//        var deletedQuestion = _LXPDbContext.QuizQuestions.Find(deletedQuestionId);
//        if (deletedQuestion != null)
//        {
//            var subsequentQuestions = _LXPDbContext.QuizQuestions
//                .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
//                .ToList();
//            foreach (var question in subsequentQuestions)
//            {
//                question.QuestionNo--;
//            }
//            _LXPDbContext.SaveChanges();
//        }
//    }

//    public Guid AddQuestionOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
//    {
//        var questionOptionEntity = new QuestionOption
//        {
//            QuizQuestionId = quizQuestionId,
//            Option = questionOptionDto.Option,
//            IsCorrect = questionOptionDto.IsCorrect,
//            CreatedBy = "SystemUser",
//            CreatedAt = DateTime.UtcNow
//        };

//        _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//        _LXPDbContext.SaveChanges();

//        return questionOptionEntity.QuestionOptionId;
//    }

//    public List<QuestionOptionDto> GetQuestionOptionsById(Guid quizQuestionId)
//    {
//        return _LXPDbContext.QuestionOptions
//            .Where(o => o.QuizQuestionId == quizQuestionId)
//            .Select(o => new QuestionOptionDto
//            {
//                Option = o.Option,
//                IsCorrect = o.IsCorrect


//            })
//            .ToList();
//    }
//using LXP.Common.DTO;
//using LXP.Data.IRepository;
//using LXP.Data;
//using LXP.Common.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LXP.Common;
//using LXP.Data.DBContexts;

//namespace LXP.Data.Repository
//{
//    public class QuizQuestionRepository : IQuizQuestionRepository
//    {
//        private readonly LXPDbContext _LXPDbContext;

//        public QuizQuestionRepository(LXPDbContext dbContext)
//        {
//            _LXPDbContext = dbContext;
//        }
//        public static class QuestionTypes
//        {
//            public const string MultiSelectQuestion = "MSQ";
//            public const string MultiChoiceQuestion = "MCQ";
//            public const string TrueFalseQuestion = "T/F";
//        }
//        public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
//                    throw new ArgumentException("Question cannot be null or empty.");

//                if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
//                    throw new ArgumentException("QuestionType cannot be null or empty.");

//                // Validate the question type
//                if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//                    throw new Exception("Invalid question type.");

//                // Validate the options based on the question type
//                if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//                    throw new Exception("Invalid options for the given question type.");

//                var quizQuestionEntity = new QuizQuestion
//                {
//                    QuizId = quizQuestionDto.QuizId,
//                    Question = quizQuestionDto.Question,
//                    QuestionType = quizQuestionDto.QuestionType,
//                    QuestionNo = GetNextQuestionNo(quizQuestionDto.QuizId), // Assign the next question number
//                    CreatedBy = "SystemUser",
//                    CreatedAt = DateTime.UtcNow
//                };

//                _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
//                _LXPDbContext.SaveChanges();

//                // Add options for the question
//                foreach (var option in options)
//                {
//                    var questionOptionEntity = new QuestionOption
//                    {
//                       // QuestionOptionId = Guid.NewGuid(),
//                        QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                        Option = option.Option,
//                        IsCorrect = option.IsCorrect,
//                        CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                        CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                    };

//                    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//                }

//                _LXPDbContext.SaveChanges();

//                return quizQuestionEntity.QuizQuestionId;
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException("An error occurred while adding the quiz question.", ex);

//        }
//        }

//        private bool IsValidQuestionType(string questionType)
//        {
//            return questionType == QuestionTypes.MultiSelectQuestion ||
//                   questionType == QuestionTypes.MultiChoiceQuestion ||
//                   questionType == QuestionTypes.TrueFalseQuestion;
//        }


//        public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//        {
//            var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//            if (quizQuestionEntity != null)
//            {
//                quizQuestionEntity.Question = quizQuestionDto.Question;
//                quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;

//                _LXPDbContext.SaveChanges();

//                // Update options for the question
//                var existingOptions = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//                _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//                // Add options for the question
//                foreach (var option in options)
//                {
//                    var questionOptionEntity = new QuestionOption
//                    {
//                        QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                        Option = option.Option,
//                        IsCorrect = option.IsCorrect,
//                        CreatedBy = quizQuestionEntity.CreatedBy, // Preserve the original CreatedBy value
//                        CreatedAt = quizQuestionEntity.CreatedAt // Preserve the original CreatedAt value
//                    };

//                    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//                }

//                _LXPDbContext.SaveChanges();

//                return true;
//            }
//            return false;
//        }

//        public bool DeleteQuestion(Guid quizQuestionId)
//        {
//            var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//            if (quizQuestionEntity != null)
//            {
//                _LXPDbContext.QuestionOptions.RemoveRange(_LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId));
//                _LXPDbContext.QuizQuestions.Remove(quizQuestionEntity);
//                _LXPDbContext.SaveChanges();

//                ReorderQuestionNos(quizQuestionEntity.QuizId, quizQuestionEntity.QuestionNo);
//                return true;
//            }
//            return false;
//        }
//        private void ReorderQuestionNos(Guid quizId, int deletedQuestionNo)
//        {
//            var subsequentQuestions = _LXPDbContext.QuizQuestions
//                .Where(q => q.QuizId == quizId && q.QuestionNo > deletedQuestionNo)
//                .ToList();
//            foreach (var question in subsequentQuestions)
//            {
//                question.QuestionNo--;
//            }
//            _LXPDbContext.SaveChanges();
//        }

//        public List<QuizQuestionDto> GetAllQuestions()
//        {
//            return _LXPDbContext.QuizQuestions
//                .Select(q => new QuizQuestionDto
//                {
//                    QuizId = q.QuizId,
//                    Question = q.Question,
//                    QuestionType = q.QuestionType,
//                    Options = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == q.QuizQuestionId)
//                        .Select(o => new QuestionOptionDto
//                        {
//                            Option = o.Option,
//                            IsCorrect = o.IsCorrect
//                        })
//                        .ToList()
//                })
//                .ToList();
//        }

//        public int GetNextQuestionNo(Guid quizId)
//        {
//            return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
//        }

//        public void DecrementQuestionNos(Guid deletedQuestionId)
//        {
//            var deletedQuestion = _LXPDbContext.QuizQuestions.Find(deletedQuestionId);
//            if (deletedQuestion != null)
//            {
//                var subsequentQuestions = _LXPDbContext.QuizQuestions
//                    .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
//                    .ToList();
//                foreach (var question in subsequentQuestions)
//                {
//                    question.QuestionNo--;
//                }
//                _LXPDbContext.SaveChanges();
//            }
//        }

//        public Guid AddQuestionOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
//{
//            var questionOptionEntity = new QuestionOption
//            {
//                QuizQuestionId = quizQuestionId,
//                Option = questionOptionDto.Option,
//                IsCorrect = questionOptionDto.IsCorrect,
//                CreatedBy = "SystemUser",
//                CreatedAt = DateTime.UtcNow
//            };

//            _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//            _LXPDbContext.SaveChanges();

//            return questionOptionEntity.QuestionOptionId;
//        }
//        public List<QuestionOptionDto> GetQuestionOptionsById(Guid quizQuestionId)
//        {
//            return _LXPDbContext.QuestionOptions
//                .Where(o => o.QuizQuestionId == quizQuestionId)

//                .Select(o => new QuestionOptionDto
//                {
//                    Option = o.Option,
//                    IsCorrect = o.IsCorrect
//                })
//                .ToList();
//        }


//        public bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionDto> options)
//        {
//            switch (questionType)
//            {
//                case QuestionTypes.MultiSelectQuestion:
//                    return options.Count > 4 && options.Count <= 8 && options.Count(o => o.IsCorrect) >= 2 && options.Count(o => o.IsCorrect) <= 3;
//                case QuestionTypes.MultiChoiceQuestion:
//                    return options.Count == 4 && options.Count(o => o.IsCorrect) == 1;
//                case QuestionTypes.TrueFalseQuestion:
//                    return options.Count == 2 && options.Count(o => o.IsCorrect) == 1;
//                default:
//                    return false;
//            }
//        }

//    }
//}



//////////using System;
//////////using System.Collections.Generic;
//////////using System.Linq;
//////////using System.Text;
//////////using System.Threading.Tasks;

//////////namespace LXP.Data.Repository
//////////{
//////////    internal class Class1
//////////    {
//////////    }
//////////}
////////using System;
////////using System.Collections.Generic;
////////using System.Linq;
////////using System.Threading.Tasks;
////////using LXP.Data.IRepository;
//////////using LXP.Core.DTOs;
////////using LXP.Common.DTO;

////////namespace LXP.Data.Repository
////////{
////////    public class QuizQuestionRepository : IQuizQuestionRepository
////////    {
////////        // Replace with actual data access implementation (e.g., Entity Framework)
////////        private readonly List<QuizQuestionDto> _questions = new List<QuizQuestionDto>();

////////        public async Task AddQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options)
////////        {
////////            _questions.Add(question);
////////            if (options != null)
////////            {
////////                question.Options = options.ToList(); // Assign options to the question object
////////            }
////////        }

////////        public async Task DeleteQuestionAsync(Guid id)
////////        {
////////            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == id);
////////            if (question != null)
////////            {
////////                _questions.Remove(question);
////////            }
////////        }

////////        public async Task UpdateQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options)
////////        {
////////            var existingQuestion = _questions.FirstOrDefault(q => q.QuizQuestionId == question.QuizQuestionId);
////////            if (existingQuestion == null)
////////            {
////////                throw new Exception("Question with ID not found."); // Handle missing question appropriately
////////            }

////////            existingQuestion.Question = question.Question;
////////            existingQuestion.QuestionType = question.QuestionType;

////////            // Update or add options based on logic (consider using a separate method for clarity)
////////            if (options != null)
////////            {
////////                // Clear existing options
////////                existingQuestion.Options?.Clear();

////////                foreach (var option in options)
////////                {
////////                    existingQuestion.Options?.Add(option); // Add new options
////////                }
////////            }
////////        }

////////        public async Task<IEnumerable<QuizQuestionDto>> GetQuestionsAsync()
////////        {
////////            return _questions.AsEnumerable(); // Return a copy of the internal list to avoid modification
////////        }
////////    }


////////}
//////using LXP.Common.DTO;

//////public class QuizQuestionRepository : IQuizQuestionRepository
//////{

//////    // Replace with actual data access implementation (e.g., Entity Framework)
//////    private readonly List<QuizQuestionDto> _questions = new List<QuizQuestionDto>();

//////    public async Task AddQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options)
//////    {
//////        _questions.Add(question);
//////        if (options != null)
//////        {
//////            // Iterate through options and add them to the question
//////            foreach (var option in options)
//////            {
//////                question.Options?.Add(option); // Add each option to the question's Options list
//////            }
//////        }
//////    }

//////    public async Task DeleteQuestionAsync(Guid id)
//////    {
//////        var question = _questions.FirstOrDefault(q => q.QuizQuestionId == id);
//////        if (question != null)
//////        {
//////            _questions.Remove(question);
//////        }
//////    }

//////    public async Task UpdateQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options)
//////    {
//////        var existingQuestion = _questions.FirstOrDefault(q => q.QuizQuestionId == question.QuizQuestionId);
//////        if (existingQuestion == null)
//////        {
//////            throw new Exception("Question with ID not found."); // Handle missing question appropriately
//////        }

//////        existingQuestion.Question = question.Question;
//////        existingQuestion.QuestionType = question.QuestionType;

//////        // Update or add options based on logic
//////        if (options != null)
//////        {
//////            existingQuestion.Options?.Clear(); // Clear existing options from the question object
//////            foreach (var option in options)
//////            {
//////                existingQuestion.Options?.Add(option); // Add new options
//////            }
//////        }
//////    }

//////    public async Task<IEnumerable<QuizQuestionDto>> GetQuestionsAsync()
//////    {
//////        return _questions.AsEnumerable(); // Return a copy of the internal list to avoid modification
//////    }

//////    // Implement missing methods from the interface
//////    public async Task<QuizQuestionDto> GetById(Guid questionId)
//////    {
//////        return _questions.FirstOrDefault(q => q.QuizQuestionId == questionId);
//////    }

//////    public async Task<IEnumerable<QuizQuestionDto>> GetByQuizId(Guid quizId)
//////    {
//////        return _questions.Where(q => q.QuizId == quizId).AsEnumerable(); // Filter questions by QuizId
//////    }
//////}
//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using LXP.Data.IRepository;
//////using System.Threading.Tasks;
//////using LXP.Common.DTO; // Assuming QuizQuestionDto and QuestionOptionDto are defined here

//////namespace LXP.Data.Repository
//////{
//////    public class QuizQuestionRepository : IQuizQuestionRepository
//////    {
//////        // Replace with actual data access implementation (e.g., Entity Framework)
//////        private readonly List<QuizQuestionDto> _questions = new List<QuizQuestionDto>();

//////        public async Task AddQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options)
//////        {
//////            _questions.Add(question);
//////            if (options != null)
//////            {
//////                // Iterate through options and add them to the question
//////                foreach (var option in options)
//////                {
//////                    question.Options?.Add(option); // Add each option to the question's Options list
//////                }
//////            }
//////        }

//////        public async Task DeleteQuestionAsync(Guid id)
//////        {
//////            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == id);
//////            if (question != null)
//////            {
//////                _questions.Remove(question);
//////            }
//////        }

//////        public async Task UpdateQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options)
//////        {
//////            var existingQuestion = _questions.FirstOrDefault(q => q.QuizQuestionId == question.QuizQuestionId);
//////            if (existingQuestion == null)
//////            {
//////                throw new Exception("Question with ID not found."); // Handle missing question appropriately
//////            }

//////            existingQuestion.Question = question.Question;
//////            existingQuestion.QuestionType = question.QuestionType;

//////            // Update or add options based on logic
//////            if (options != null)
//////            {
//////                existingQuestion.Options?.Clear(); // Clear existing options from the question object
//////                foreach (var option in options)
//////                {
//////                    existingQuestion.Options?.Add(option); // Add new options
//////                }
//////            }
//////        }

//////        public async Task<IEnumerable<QuizQuestionDto>> GetQuestionsAsync()
//////        {
//////            return _questions.AsEnumerable(); // Return a copy of the internal list to avoid modification
//////        }

//////        public async Task<QuizQuestionDto> GetById(Guid questionId)
//////        {
//////            return _questions.FirstOrDefault(q => q.QuizQuestionId == questionId);
//////        }

//////        public async Task<IEnumerable<QuizQuestionDto>> GetByQuizId(Guid quizId)
//////        {
//////            return _questions.Where(q => q.QuizId == quizId).AsEnumerable(); // Filter questions by QuizId
//////        }
//////    }
//////}
//////using LXP.Common.DTO;
//////using System;
//////using System.Collections.Generic;
//////using LXP.Data.IRepository;

//////namespace LXP.Data.Repository
//////{
//////    public class QuizQuestionRepository : IQuizQuestionRepository
//////    {
//////        // Add implementation for database operations

//////        public Guid AddQuestion(QuizQuestionDto quizQuestionDto)
//////        {
//////            // Implementation to add a new question
//////            // Generate a new Guid for QuizQuestionId
//////            quizQuestionDto.QuizQuestionId = Guid.NewGuid();

//////            // Set random values for QuizId, CreatedBy, and CreatedAt
//////            quizQuestionDto.QuizId = Guid.NewGuid();
//////            quizQuestionDto.CreatedBy = "SystemUser";
//////            quizQuestionDto.CreatedAt = DateTime.Now;

//////            // Add logic to save the question to the database

//////            return quizQuestionDto.QuizQuestionId;
//////        }

//////        public bool UpdateQuestion(QuizQuestionDto quizQuestionDto)
//////        {
//////            // Implementation to update an existing question
//////            // Add logic to update the question in the database
//////            return true;
//////        }

//////        public bool DeleteQuestion(Guid quizQuestionId)
//////        {
//////            // Implementation to delete a question
//////            // Add logic to delete the question from the database
//////            return true;
//////        }

//////        public List<QuizQuestionDto> GetAllQuestions()
//////        {
//////            // Implementation to retrieve all questions
//////            // Add logic to fetch questions from the database
//////            return new List<QuizQuestionDto>();
//////        }

//////        public Guid AddOption(QuestionOptionDto questionOptionDto)
//////        {
//////            // Implementation to add a new option
//////            // Generate a new Guid for QuestionOptionId
//////            questionOptionDto.QuestionOptionId = Guid.NewGuid();

//////            // Set random values for CreatedBy and CreatedAt
//////            questionOptionDto.CreatedBy = "SystemUser";
//////            questionOptionDto.CreatedAt = DateTime.Now;

//////            // Add logic to save the option to the database

//////            return questionOptionDto.QuestionOptionId;
//////        }
//////    }
//////}
////using LXP.Common.DTO;
////using LXP.Data.IRepository;
////using System;
////using System.Collections.Generic;
////using System.Linq;

////namespace LXP.Data.Repository
////{
////    public class QuizQuestionRepository : IQuizQuestionRepository
////    {
////        private List<QuizQuestionDto> _questions = new List<QuizQuestionDto>();
////        private List<QuestionOptionDto> _options = new List<QuestionOptionDto>();

////        public Guid AddQuestion(QuizQuestionDto quizQuestionDto)
////        {
////            // Set random values for QuizId, CreatedBy, and CreatedAt
////            quizQuestionDto.QuizId = Guid.NewGuid();
////            quizQuestionDto.CreatedBy = "SystemUser";
////            quizQuestionDto.CreatedAt = DateTime.Now;

////            _questions.Add(quizQuestionDto);
////            return quizQuestionDto.QuizQuestionId;
////        }

////        public bool UpdateQuestion(QuizQuestionDto quizQuestionDto)
////        {
////            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionDto.QuizQuestionId);
////            if (question != null)
////            {
////                question.Question = quizQuestionDto.Question;
////                question.QuestionType = quizQuestionDto.QuestionType;
////                question.ModifiedBy = "SystemUser";
////                question.ModifiedAt = DateTime.Now;
////                return true;
////            }
////            return false;
////        }

////        public bool DeleteQuestion(Guid quizQuestionId)
////        {
////            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionId);
////            if (question != null)
////            {
////                _questions.Remove(question);
////                return true;
////            }
////            return false;
////        }

////        public List<QuizQuestionDto> GetAllQuestions()
////        {
////            return _questions;
////        }

////        public Guid AddOption(QuestionOptionDto questionOptionDto)
////        {
////            // Set random values for CreatedBy and CreatedAt
////            questionOptionDto.CreatedBy = "SystemUser";
////            questionOptionDto.CreatedAt = DateTime.Now;

////            _options.Add(questionOptionDto);
////            return questionOptionDto.QuestionOptionId;
////        }

////        public int GetNextQuestionNo()
////        {
////            return _questions.Count + 1;
////        }

////        public void DecrementQuestionNos(Guid deletedQuestionId)
////        {
////            var deletedQuestionNo = _questions.FirstOrDefault(q => q.QuizQuestionId == deletedQuestionId)?.QuestionNo;
////            if (deletedQuestionNo.HasValue)
////            {
////                var subsequentQuestions = _questions.Where(q => q.QuestionNo > deletedQuestionNo.Value).ToList();
////                foreach (var question in subsequentQuestions)
////                {
////                    question.QuestionNo--;
////                }
////            }
////        }

////    }
////}
//using LXP.Common.DTO;
//using LXP.Data.IRepository;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LXP.Data.Repository
//{
//    public class QuizQuestionRepository : IQuizQuestionRepository
//    {
//        private List<QuizQuestionDto> _questions = new List<QuizQuestionDto>();
//        private List<QuestionOptionDto> _options = new List<QuestionOptionDto>();

//        public Guid AddQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            // Set random values for QuizId, CreatedBy, and CreatedAt
//            quizQuestionDto.QuizId = Guid.NewGuid();
//            quizQuestionDto.CreatedBy = "SystemUser";
//            quizQuestionDto.CreatedAt = DateTime.Now;

//            _questions.Add(quizQuestionDto);
//            return quizQuestionDto.QuizQuestionId;
//        }

//        public bool UpdateQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionDto.QuizQuestionId);
//            if (question != null)
//            {
//                question.Question = quizQuestionDto.Question;
//                question.QuestionType = quizQuestionDto.QuestionType;
//                question.ModifiedBy = "SystemUser";
//                question.ModifiedAt = DateTime.Now;
//                return true;
//            }
//            return false;
//        }

//        public bool DeleteQuestion(Guid quizQuestionId)
//        {
//            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionId);
//            if (question != null)
//            {
//                _questions.Remove(question);
//                return true;
//            }
//            return false;
//        }

//        public List<QuizQuestionDto> GetAllQuestions()
//        {
//            return _questions;
//        }

//        public Guid AddOption(QuestionOptionDto questionOptionDto)
//        {
//            // Set random values for CreatedBy and CreatedAt
//            questionOptionDto.CreatedBy = "SystemUser";
//            questionOptionDto.CreatedAt = DateTime.Now;

//            _options.Add(questionOptionDto);
//            return questionOptionDto.QuestionOptionId;
//        }

//        public int GetNextQuestionNo()
//        {
//            return _questions.Count + 1;
//        }

//        public void DecrementQuestionNos(Guid deletedQuestionId)
//        {
//            var deletedQuestionNo = _questions.FirstOrDefault(q => q.QuizQuestionId == deletedQuestionId)?.QuestionNo;
//            if (deletedQuestionNo.HasValue)
//            {
//                var subsequentQuestions = _questions.Where(q => q.QuestionNo > deletedQuestionNo.Value).ToList();
//                foreach (var question in subsequentQuestions)
//                {
//                    question.QuestionNo--;
//                }
//            }
//        }

//        public List<QuestionOptionDto> GetOptionsByQuestionId(Guid quizQuestionId)
//        {
//            return _options.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//        }

//        public bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionDto> options)
//        {
//            switch (questionType)
//            {
//                case "MSQ":
//                    return options.Count > 4 && options.Count <= 8 && options.Count(o => o.IsCorrect) >= 2 && options.Count(o => o.IsCorrect) <= 3;
//                case "MCQ":
//                    return options.Count == 4 && options.Count(o => o.IsCorrect) == 1;
//                case "T/F":
//                    return options.Count == 2 && options.Count(o => o.IsCorrect) == 1;
//                default:
//                    return false;
//            }
//        }
//    }
//}
//using LXP.Common.DTO;
//using LXP.Data.IRepository;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LXP.Data.Repository
//{
//    public class QuizQuestionRepository : IQuizQuestionRepository
//    {
//        private List<QuizQuestionDto> _questions = new List<QuizQuestionDto>();
//        private List<QuestionOptionDto> _options = new List<QuestionOptionDto>();

//        public Guid AddQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            // Set random values for CreatedBy and CreatedAt
//            quizQuestionDto.CreatedBy = "SystemUser";
//            quizQuestionDto.CreatedAt = DateTime.Now;

//            _questions.Add(quizQuestionDto);
//            return quizQuestionDto.QuizQuestionId;
//        }

//        public bool UpdateQuestion(QuizQuestionDto quizQuestionDto)
//        {
//            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionDto.QuizQuestionId);
//            if (question != null)
//            {
//                question.Question = quizQuestionDto.Question;
//                question.QuestionType = quizQuestionDto.QuestionType;
//                question.ModifiedBy = "SystemUser";
//                question.ModifiedAt = DateTime.Now;
//                return true;
//            }
//            return false;
//        }

//        public bool DeleteQuestion(Guid quizQuestionId)
//        {
//            var question = _questions.FirstOrDefault(q => q.QuizQuestionId == quizQuestionId);
//            if (question != null)
//            {
//                _questions.Remove(question);
//                return true;
//            }
//            return false;
//        }

//        public List<QuizQuestionDto> GetAllQuestions()
//        {
//            return _questions;
//        }

//        public Guid AddOption(QuestionOptionDto questionOptionDto)
//        {
//            // Set random values for CreatedBy and CreatedAt
//            questionOptionDto.CreatedBy = "SystemUser";
//            questionOptionDto.CreatedAt = DateTime.Now;

//            _options.Add(questionOptionDto);
//            return questionOptionDto.QuestionOptionId;
//        }

//        public int GetNextQuestionNo(Guid quizId)
//        {
//            return _questions.Where(q => q.QuizId == quizId).Count() + 1;
//        }

//        public void DecrementQuestionNos(Guid deletedQuestionId)
//        {
//            var deletedQuestion = _questions.FirstOrDefault(q => q.QuizQuestionId == deletedQuestionId);
//            if (deletedQuestion != null)
//            {
//                var subsequentQuestions = _questions
//                    .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
//                    .ToList();
//                foreach (var question in subsequentQuestions)
//                {
//                    question.QuestionNo--;
//                }
//            }
//        }

//        public List<QuestionOptionDto> GetOptionsByQuestionId(Guid quizQuestionId)
//        {
//            return _options.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//        }

//        public bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionDto> options)
//        {
//            switch (questionType)
//            {
//                case "MSQ":
//                    return options.Count > 4 && options.Count <= 8 && options.Count(o => o.IsCorrect) >= 2 && options.Count(o => o.IsCorrect) <= 3;
//                case "MCQ":
//                    return options.Count == 4 && options.Count(o => o.IsCorrect) == 1;
//                case "T/F":
//                    return options.Count == 2 && options.Count(o => o.IsCorrect) == 1;
//                default:
//                    return false;
//            }
//        }
//    }
//}
// Validate the question type
// if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//     throw new Exception("Invalid question type.");

// // Validate the options based on the question type
// if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//     throw new Exception("Invalid options for the given question type.");

// Validate the required fields
// public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
// {
//     try
//     {

//         if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
//             throw new ArgumentException("Question cannot be null or empty.");

//         if (string.IsNullOrWhiteSpace(quizQuestionDto.CreatedBy))
//             throw new ArgumentException("CreatedBy cannot be null or empty.");

//         if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
//             throw new ArgumentException("QuestionType cannot be null or empty.");

//         // Validate the question type
//         if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//             throw new Exception("Invalid question type.");

//         // Validate the options based on the question type
//         if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//             throw new Exception("Invalid options for the given question type.");

//         // Get the next question number for the quiz
//         int nextQuestionNo = GetNextQuestionNo(quizQuestionDto.QuizId);

//         var quizQuestionEntity = new QuizQuestion
//         {
//             QuizId = quizQuestionDto.QuizId,
//             QuestionNo = nextQuestionNo,
//             Question = quizQuestionDto.Question,
//             QuestionType = quizQuestionDto.QuestionType,
//             CreatedBy = quizQuestionDto.CreatedBy,
//             CreatedAt = quizQuestionDto.CreatedAt
//         };

//         _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
//         _LXPDbContext.SaveChanges();

//         // Add options for the question
//         foreach (var option in options)
//         {
//             var questionOptionEntity = new QuestionOption
//             {
//                 QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                 Option = option.Option,
//                 IsCorrect = option.IsCorrect,
//                 CreatedBy = option.CreatedBy,
//                 CreatedAt = option.CreatedAt
//             };

//             _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//         }

//         _LXPDbContext.SaveChanges();

//         return quizQuestionEntity.QuizQuestionId;
//     }
//     catch (Exception ex)
//     {
//         throw ex;
//     }
// }
// public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
// {
//     try
//     {
//         if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
//             throw new ArgumentException("Question cannot be null or empty.");

//         if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
//             throw new ArgumentException("QuestionType cannot be null or empty.");

//         // Validate the question type
//         if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//             throw new Exception("Invalid question type.");

//         // Validate the options based on the question type
//         if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//             throw new Exception("Invalid options for the given question type.");

//         var quizQuestionEntity = new QuizQuestion
//         {
//             QuizId = quizQuestionDto.QuizId,
//             Question = quizQuestionDto.Question,
//             QuestionType = quizQuestionDto.QuestionType,
//             CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//             CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//         };

//         _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
//         _LXPDbContext.SaveChanges();

//         // Add options for the question
//         foreach (var option in options)
//         {
//             var questionOptionEntity = new QuestionOption
//             {
//                 QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                 Option = option.Option,
//                 IsCorrect = option.IsCorrect,
//                 CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                 CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//             };

//             _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//         }

//         _LXPDbContext.SaveChanges();

//         return quizQuestionEntity.QuizQuestionId;
//     }
//     catch (Exception ex)
//     {
//         throw ex;
//     }
// }
// public Guid AddOption(QuestionOptionDto questionOptionDto)
// {
//     var questionOptionEntity = new QuestionOption
//     {
//         QuestionOptionId = questionOptionDto.QuestionOptionId,
//         QuizQuestionId = questionOptionDto.QuizQuestionId,
//         Option = questionOptionDto.Option,
//         IsCorrect = questionOptionDto.IsCorrect,
//         CreatedBy = questionOptionDto.CreatedBy,
//         CreatedAt = questionOptionDto.CreatedAt
//     };

//     _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//     _LXPDbContext.SaveChanges();

//     return questionOptionEntity.QuestionOptionId;
// }
// public class QuizQuestionRepository : IQuizQuestionRepository
// {
//     private readonly LXPDbContext _LXPDbContext;



//     public QuizQuestionRepository(LXPDbContext dbContext)
//     {
//         _LXPDbContext = dbContext;
//     }



//     public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
// {
//     try
//     {
//         if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
//             throw new ArgumentException("Question cannot be null or empty.");

//         if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
//             throw new ArgumentException("QuestionType cannot be null or empty.");

//         // Validate the question type
//         if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//             throw new Exception("Invalid question type.");

//         // Validate the options based on the question type
//         if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//             throw new Exception("Invalid options for the given question type.");

//         var quizQuestionEntity = new QuizQuestion
//         {
//             QuizId = quizQuestionDto.QuizId,
//             Question = quizQuestionDto.Question,
//             QuestionType = quizQuestionDto.QuestionType,
//             CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//             CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//         };

//         _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
//         _LXPDbContext.SaveChanges();

//         // Add options for the question
//         foreach (var option in options)
//         {
//             var questionOptionEntity = new QuestionOption
//             {
//                 QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                 Option = option.Option,
//                 IsCorrect = option.IsCorrect,
//                 CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                 CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//             };

//             _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//         }

//         _LXPDbContext.SaveChanges();

//         return quizQuestionEntity.QuizQuestionId;
//     }
//     catch (Exception ex)
//     {
//         throw ex;
//     }
// }





//     private bool IsValidQuestionType(string questionType)
//     {
//         return questionType == "MSQ" || questionType == "MCQ" || questionType == "T/F";
//     }
//     public bool UpdateQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//     {
//         var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionDto.QuizQuestionId);
//         if (quizQuestionEntity != null)
//         {
//             quizQuestionEntity.Question = quizQuestionDto.Question;
//             quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;
//             quizQuestionEntity.ModifiedBy = quizQuestionDto.ModifiedBy;
//             quizQuestionEntity.ModifiedAt = quizQuestionDto.ModifiedAt;

//             _LXPDbContext.SaveChanges();

//             // Update options for the question
//             var existingOptions = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionDto.QuizQuestionId).ToList();
//             _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//             // Add options for the question
//             foreach (var option in options)
//             {
//                 var questionOptionEntity = new QuestionOption
//                 {
//                     QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                     Option = option.Option,
//                     IsCorrect = option.IsCorrect,
//                     CreatedBy = option.CreatedBy,
//                     CreatedAt = option.CreatedAt,
//                     ModifiedBy = quizQuestionDto.ModifiedBy,
//                     ModifiedAt = quizQuestionDto.ModifiedAt
//                 };

//                 _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//             }

//             _LXPDbContext.SaveChanges();

//             return true;
//         }
//         return false;
//     }

//     public bool DeleteQuestion(Guid quizQuestionId)
//     {
//         var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//         if (quizQuestionEntity != null)
//         {
//             _LXPDbContext.QuestionOptions.RemoveRange(_LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId));
//             _LXPDbContext.QuizQuestions.Remove(quizQuestionEntity);
//             _LXPDbContext.SaveChanges();

//             DecrementQuestionNos(quizQuestionId);
//             return true;
//         }
//         return false;
//     }

//     public List<QuizQuestionDto> GetAllQuestions()
//     {
//         return _LXPDbContext.QuizQuestions
//             .Select(q => new QuizQuestionDto
//             {
//                 QuizQuestionId = q.QuizQuestionId,
//                 QuizId = q.QuizId,
//                 QuestionNo = q.QuestionNo,
//                 Question = q.Question,
//                 QuestionType = q.QuestionType,
//                 CreatedBy = q.CreatedBy,
//                 CreatedAt = q.CreatedAt,
//                 ModifiedBy = q.ModifiedBy,
//                 ModifiedAt = q.ModifiedAt,
//                 Options = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == q.QuizQuestionId)
//                     .Select(o => new QuestionOptionDto
//                     {
//                         QuestionOptionId = o.QuestionOptionId,
//                         QuizQuestionId = o.QuizQuestionId,
//                         Option = o.Option,
//                         IsCorrect = o.IsCorrect,
//                         CreatedBy = o.CreatedBy,
//                         CreatedAt = o.CreatedAt,
//                         ModifiedBy = o.ModifiedBy,
//                         ModifiedAt = o.ModifiedAt
//                     })
//                     .ToList()
//             })
//             .ToList();
//     }

//     public int GetNextQuestionNo(Guid quizId)
//     {
//         return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
//     }

//     public void DecrementQuestionNos(Guid deletedQuestionId)
//     {
//         var deletedQuestion = _LXPDbContext.QuizQuestions.Find(deletedQuestionId);
//         if (deletedQuestion != null)
//         {
//             var subsequentQuestions = _LXPDbContext.QuizQuestions
//                 .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
//                 .ToList();
//             foreach (var question in subsequentQuestions)
//             {
//                 question.QuestionNo--;
//             }
//             _LXPDbContext.SaveChanges();
//         }
//     }

//     public Guid AddOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
// {
//     var questionOptionEntity = new QuestionOption
//     {
//         QuizQuestionId = quizQuestionId,
//         Option = questionOptionDto.Option,
//         IsCorrect = questionOptionDto.IsCorrect,
//         CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//         CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//     };

//     _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//     _LXPDbContext.SaveChanges();

//     return questionOptionEntity.QuestionOptionId;
// }
//     public List<QuestionOptionDto> GetOptionsByQuestionId(Guid quizQuestionId)
//     {
//         return _LXPDbContext.QuestionOptions
//             .Where(o => o.QuizQuestionId == quizQuestionId)
//             .Select(o => new QuestionOptionDto
//             {
//                 QuestionOptionId = o.QuestionOptionId,
//                 QuizQuestionId = o.QuizQuestionId,
//                 Option = o.Option,
//                 IsCorrect = o.IsCorrect,
//                 CreatedBy = o.CreatedBy,
//                 CreatedAt = o.CreatedAt,
//                 ModifiedBy = o.ModifiedBy,
//                 ModifiedAt = o.ModifiedAt
//             })
//             .ToList();
//     }





// using LXP.Common.DTO;
// using LXP.Data.IRepository;
// using LXP.Data;
// using LXP.Common.Entities;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using LXP.Common;
// using LXP.Data.DBContexts;


// namespace LXP.Data.Repository
// {

//     public class QuizQuestionRepository : IQuizQuestionRepository
//     {
//         private readonly LXPDbContext _LXPDbContext;

//         public QuizQuestionRepository(LXPDbContext dbContext)
//         {
//             _LXPDbContext = dbContext;
//         }

//         public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//         {
//             try
//             {
//                 if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
//                     throw new ArgumentException("Question cannot be null or empty.");

//                 if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
//                     throw new ArgumentException("QuestionType cannot be null or empty.");

//                 // Validate the question type
//                 if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//                     throw new Exception("Invalid question type.");

//                 // Validate the options based on the question type
//                 if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//                     throw new Exception("Invalid options for the given question type.");

//                 var quizQuestionEntity = new QuizQuestion
//                 {
//                     QuizId = quizQuestionDto.QuizId,
//                     Question = quizQuestionDto.Question,
//                     QuestionType = quizQuestionDto.QuestionType,
//                     CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                     CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                 };

//                 _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
//                 _LXPDbContext.SaveChanges();

//                 // Add options for the question
//                 foreach (var option in options)
//                 {
//                     var questionOptionEntity = new QuestionOption
//                     {
//                         QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                         Option = option.Option,
//                         IsCorrect = option.IsCorrect,
//                         CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                         CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                     };

//                     _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//                 }

//                 _LXPDbContext.SaveChanges();

//                 return quizQuestionEntity.QuizQuestionId;
//             }
//             catch (Exception ex)
//             {
//                 throw ex;
//             }
//         }
//         private bool IsValidQuestionType(string questionType)
//         {
//             return questionType == "MSQ" || questionType == "MCQ" || questionType == "T/F";
//         }

//         public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//         {
//             var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//             if (quizQuestionEntity != null)
//             {
//                 quizQuestionEntity.Question = quizQuestionDto.Question;
//                 quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;

//                 _LXPDbContext.SaveChanges();

//                 // Update options for the question
//                 var existingOptions = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//                 _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//                 // Add options for the question
//                 foreach (var option in options)
//                 {
//                     var questionOptionEntity = new QuestionOption
//                     {
//                         QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                         Option = option.Option,
//                         IsCorrect = option.IsCorrect,
//                         CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                         CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                     };

//                     _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//                 }

//                 _LXPDbContext.SaveChanges();

//                 return true;
//             }
//             return false;
//         }
//         public bool DeleteQuestion(Guid quizQuestionId)
//         {
//             var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//             if (quizQuestionEntity != null)
//             {
//                 _LXPDbContext.QuestionOptions.RemoveRange(_LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId));
//                 _LXPDbContext.QuizQuestions.Remove(quizQuestionEntity);
//                 _LXPDbContext.SaveChanges();

//                 DecrementQuestionNos(quizQuestionId);
//                 return true;
//             }
//             return false;
//         }

//         public List<QuizQuestionDto> GetAllQuestions()
//         {
//             return _LXPDbContext.QuizQuestions
//                 .Select(q => new QuizQuestionDto
//                 {
//                     QuizId = q.QuizId,
//                     Question = q.Question,
//                     QuestionType = q.QuestionType,
//                     Options = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == q.QuizQuestionId)
//                         .Select(o => new QuestionOptionDto
//                         {
//                             Option = o.Option,
//                             IsCorrect = o.IsCorrect
//                         })
//                         .ToList()
//                 })
//                 .ToList();
//         }

//         public int GetNextQuestionNo(Guid quizId)
//         {
//             return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
//         }

//         public void DecrementQuestionNos(Guid deletedQuestionId)
//         {
//             var deletedQuestion = _LXPDbContext.QuizQuestions.Find(deletedQuestionId);
//             if (deletedQuestion != null)
//             {
//                 var subsequentQuestions = _LXPDbContext.QuizQuestions
//                     .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
//                     .ToList();
//                 foreach (var question in subsequentQuestions)
//                 {
//                     question.QuestionNo--;
//                 }
//                 _LXPDbContext.SaveChanges();
//             }
//         }

//         public Guid AddOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
//         {
//             var questionOptionEntity = new QuestionOption
//             {
//                 QuizQuestionId = quizQuestionId,
//                 Option = questionOptionDto.Option,
//                 IsCorrect = questionOptionDto.IsCorrect,
//                 CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                 CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//             };

//             _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//             _LXPDbContext.SaveChanges();

//             return questionOptionEntity.QuestionOptionId;
//         }

//         public List<QuestionOptionDto> GetOptionsByQuestionId(Guid quizQuestionId)
//         {
//             return _LXPDbContext.QuestionOptions
//                 .Where(o => o.QuizQuestionId == quizQuestionId)
//                 .Select(o => new QuestionOptionDto
//                 {
//                     Option = o.Option,
//                     IsCorrect = o.IsCorrect
//                 })
//                 .ToList();
//         }
//using LXP.Common.DTO;
//using LXP.Data.IRepository;
//using LXP.Data;
//using LXP.Common.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LXP.Common;
//using LXP.Data.DBContexts;

//namespace LXP.Data.Repository
//{
//    public class QuizQuestionRepository : IQuizQuestionRepository
//    {
//        private readonly LXPDbContext _LXPDbContext;

//        public QuizQuestionRepository(LXPDbContext dbContext)
//        {
//            _LXPDbContext = dbContext;
//        }

//        public Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(quizQuestionDto.Question))
//                    throw new ArgumentException("Question cannot be null or empty.");

//                if (string.IsNullOrWhiteSpace(quizQuestionDto.QuestionType))
//                    throw new ArgumentException("QuestionType cannot be null or empty.");

//                // Validate the question type
//                if (!IsValidQuestionType(quizQuestionDto.QuestionType))
//                    throw new Exception("Invalid question type.");

//                // Validate the options based on the question type
//                if (!ValidateOptionsByQuestionType(quizQuestionDto.QuestionType, options))
//                    throw new Exception("Invalid options for the given question type.");

//                var quizQuestionEntity = new QuizQuestion
//                {
//                     QuizId = quizQuestionDto.QuizId,

//                    Question = quizQuestionDto.Question,
//                    QuestionType = quizQuestionDto.QuestionType,
//                    CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                    CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                };

//                _LXPDbContext.QuizQuestions.Add(quizQuestionEntity);
//                _LXPDbContext.SaveChanges();

//                // Add options for the question
//                foreach (var option in options)
//                {
//                    var questionOptionEntity = new QuestionOption
//                    {
//                        QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                        Option = option.Option,
//                        IsCorrect = option.IsCorrect,
//                        CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                        CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                    };

//                    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//                }

//                _LXPDbContext.SaveChanges();

//                return quizQuestionEntity.QuizQuestionId;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        private bool IsValidQuestionType(string questionType)
//        {
//            return questionType == "MSQ" || questionType == "MCQ" || questionType == "T/F";
//        }

//        public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//        {
//            var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//            if (quizQuestionEntity != null)
//            {
//                quizQuestionEntity.Question = quizQuestionDto.Question;
//                quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;

//                _LXPDbContext.SaveChanges();

//                // Update options for the question
//                var existingOptions = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//                _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//                // Add options for the question
//                foreach (var option in options)
//                {
//                    var questionOptionEntity = new QuestionOption
//                    {
//                        QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                        Option = option.Option,
//                        IsCorrect = option.IsCorrect,
//                        CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                        CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//                    };

//                    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//                }

//                _LXPDbContext.SaveChanges();

//                return true;
//            }
//            return false;
//        }

//        public bool DeleteQuestion(Guid quizQuestionId)
//        {
//            var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//            if (quizQuestionEntity != null)
//            {
//                _LXPDbContext.QuestionOptions.RemoveRange(_LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId));
//                _LXPDbContext.QuizQuestions.Remove(quizQuestionEntity);
//                _LXPDbContext.SaveChanges();

//                DecrementQuestionNos(quizQuestionId);
//                return true;
//            }
//            return false;
//        }

//        public List<QuizQuestionDto> GetAllQuestions()
//        {
//            return _LXPDbContext.QuizQuestions
//                .Select(q => new QuizQuestionDto
//                {
//                    QuizId = q.QuizId,
//                    Question = q.Question,
//                    QuestionType = q.QuestionType,
//                    Options = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == q.QuizQuestionId)
//                        .Select(o => new QuestionOptionDto
//                        {
//                            Option = o.Option,
//                            IsCorrect = o.IsCorrect
//                        })
//                        .ToList()
//                })
//                .ToList();
//        }

//        public int GetNextQuestionNo(Guid quizId)
//        {
//            return _LXPDbContext.QuizQuestions.Where(q => q.QuizId == quizId).Count() + 1;
//        }

//        public void DecrementQuestionNos(Guid deletedQuestionId)
//        {
//            var deletedQuestion = _LXPDbContext.QuizQuestions.Find(deletedQuestionId);
//            if (deletedQuestion != null)
//            {
//                var subsequentQuestions = _LXPDbContext.QuizQuestions
//                    .Where(q => q.QuizId == deletedQuestion.QuizId && q.QuestionNo > deletedQuestion.QuestionNo)
//                    .ToList();
//                foreach (var question in subsequentQuestions)
//                {
//                    question.QuestionNo--;
//                }
//                _LXPDbContext.SaveChanges();
//            }
//        }

//        public Guid AddOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
//        {
//            var questionOptionEntity = new QuestionOption
//            {
//                QuizQuestionId = quizQuestionId,
//                Option = questionOptionDto.Option,
//                IsCorrect = questionOptionDto.IsCorrect,
//                CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//            };

//            _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//            _LXPDbContext.SaveChanges();

//            return questionOptionEntity.QuestionOptionId;
//        }

//        public List<QuestionOptionDto> GetOptionsByQuestionId(Guid quizQuestionId)
//        {
//            return _LXPDbContext.QuestionOptions
//                .Where(o => o.QuizQuestionId == quizQuestionId)
//                .Select(o => new QuestionOptionDto
//                {
//                    Option = o.Option,
//                    IsCorrect = o.IsCorrect
//                })
//                .ToList();
//        }


//        public bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionDto> options)
//        {
//            try
//            {

//                switch (questionType)
//                {
//                    case "MSQ":
//                        return options.Count > 4 && options.Count <= 8 && options.Count(o => o.IsCorrect) >= 2 && options.Count(o => o.IsCorrect) <= 3;
//                    case "MCQ":
//                        return options.Count == 4 && options.Count(o => o.IsCorrect) == 1;
//                    case "T/F":
//                        return options.Count == 2 && options.Count(o => o.IsCorrect) == 1;
//                    default:
//                        return false;
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//    }
//}

//public Guid AddOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId)
//{
//    var questionOptionEntity = new QuestionOption
//    {
//       // QuestionOptionId = Guid.NewGuid(),
//        QuizQuestionId = quizQuestionId,
//        Option = questionOptionDto.Option,
//        IsCorrect = questionOptionDto.IsCorrect,
//        CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//        CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//    };

//    _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//    _LXPDbContext.SaveChanges();

//    return questionOptionEntity.QuestionOptionId;
//}
//public bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options)
//{
//    var quizQuestionEntity = _LXPDbContext.QuizQuestions.Find(quizQuestionId);
//    if (quizQuestionEntity != null)
//    {
//        quizQuestionEntity.Question = quizQuestionDto.Question;
//        quizQuestionEntity.QuestionType = quizQuestionDto.QuestionType;

//        _LXPDbContext.SaveChanges();

//        // Update options for the question
//        var existingOptions = _LXPDbContext.QuestionOptions.Where(o => o.QuizQuestionId == quizQuestionId).ToList();
//        _LXPDbContext.QuestionOptions.RemoveRange(existingOptions);

//        // Add options for the question
//        foreach (var option in options)
//        {
//            var questionOptionEntity = new QuestionOption
//            {
//               // QuestionOptionId = Guid.NewGuid(),
//                QuizQuestionId = quizQuestionEntity.QuizQuestionId,
//                Option = option.Option,
//                IsCorrect = option.IsCorrect,
//                CreatedBy = "SystemUser", // Hardcoded value for CreatedBy
//                CreatedAt = DateTime.UtcNow // Using system time for CreatedAt
//            };

//            _LXPDbContext.QuestionOptions.Add(questionOptionEntity);
//        }

//        _LXPDbContext.SaveChanges();

//        return true;
//    }
//    return false;
//}
