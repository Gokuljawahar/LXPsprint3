﻿
using LXP.Common.DTO;
using System;
using System.Collections.Generic;

namespace LXP.Data.IRepository
{
    public interface IQuizQuestionRepository
    {
        Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options);
        bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options);
        bool DeleteQuestion(Guid quizQuestionId);
        List<QuizQuestionNoDto> GetAllQuestions();
        void DecrementQuestionNos(Guid deletedQuestionId);
        int GetNextQuestionNo(Guid quizId);
        Guid AddQuestionOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId);
        List<QuestionOptionDto> GetQuestionOptionsById(Guid quizQuestionId);
        QuizQuestionNoDto GetQuestionById(Guid quizQuestionId);
        bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionDto> options);
        List<QuizQuestionNoDto> GetAllQuestionsByQuizId(Guid quizId);
    }
}
////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace LXP.Data.IRepository
////{
////    internal interface Interface1
////    {
////    }
////}
////public interface IQuizQuestionRepository
////{
////    IEnumerable<QuizQuestionDto> GetByQuizId(Guid quizId);
////    QuizQuestionDto GetById(Guid questionId);
////    void Create(QuizQuestionDto question);
////    void Update(QuizQuestionDto question);
////    void Delete(Guid questionId);
////}
////using LXP.Common.DTO;

////public interface IQuizQuestionRepository
////{
////    Task<IEnumerable<QuizQuestionDto>> GetByQuizId(Guid quizId);
////    Task<QuizQuestionDto> GetById(Guid questionId);
////    Task AddQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options);
////    Task UpdateQuestionAsync(QuizQuestionDto question, IEnumerable<QuestionOptionDto> options);
////    Task DeleteQuestionAsync(Guid questionId);
////}
//using LXP.Common.DTO;
//using System;
//using System.Collections.Generic;

//namespace LXP.Data.IRepository
//{
//    public interface IQuizQuestionRepository
//    {
//        Guid AddQuestion(QuizQuestionDto quizQuestionDto);
//        bool UpdateQuestion(QuizQuestionDto quizQuestionDto);
//        bool DeleteQuestion(Guid quizQuestionId);
//        List<QuizQuestionDto> GetAllQuestions();
//        Guid AddOption(QuestionOptionDto questionOptionDto);
//    }
//}
// using LXP.Common.DTO;
// using System;
// using System.Collections.Generic;

// namespace LXP.Data.IRepository
// {
//     public interface IQuizQuestionRepository
//     {
//         Guid AddQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options);
//         // bool UpdateQuestion(QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options);
//         bool UpdateQuestion(Guid quizQuestionId, QuizQuestionDto quizQuestionDto, List<QuestionOptionDto> options);
//         bool DeleteQuestion(Guid quizQuestionId);
//         List<QuizQuestionDto> GetAllQuestions();
//         void DecrementQuestionNos(Guid deletedQuestionId);
//         int GetNextQuestionNo(Guid quizId);
//         Guid AddOption(QuestionOptionDto questionOptionDto, Guid quizQuestionId);
//         List<QuestionOptionDto> GetOptionsByQuestionId(Guid quizQuestionId);
//         bool ValidateOptionsByQuestionType(string questionType, List<QuestionOptionDto> options);
//     }
// }