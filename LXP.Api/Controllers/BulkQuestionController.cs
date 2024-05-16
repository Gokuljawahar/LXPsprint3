using LXP.Common.ViewModels;
using LXP.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using OfficeOpenXml;

namespace LXP.Api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkQuestionController : ControllerBase
    {
        private readonly IBulkQuestionService _excelService;

        public BulkQuestionController(IBulkQuestionService excelService)
        {
            _excelService = excelService;
        }

        [HttpPost("ImportQuizData")]
        public IActionResult ImportQuizData(IFormFile file)
        {
            try
            {
                var result = _excelService.ImportQuizData(file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while importing quiz data: {ex.Message}");
            }
        }
    }
}