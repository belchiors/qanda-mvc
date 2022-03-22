using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Qanda.Data;
using Qanda.Models;
using Qanda.ViewModels;
using Qanda.Services;

namespace Qanda.Controllers;
public class QuestionController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;
    
    public QuestionController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public IActionResult AskQuestion() => View();

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AskQuestion(AskQuestionViewModel model)
    {
        if (!ModelState.IsValid)
            return View();

        var user = _dbContext.Users?.SingleOrDefault(u => u.Username!.Equals(User.Identity!.Name));

        var newQuestion = new Question
        {
            UserId = user!.UserId,
            Author = user!.Username,
            Title = model.Title,
            Body = model.Body,
            Url = QuestionServices.GenerateUrl(model.Title!),
            CreationDate = DateTime.UtcNow
        };

        await _dbContext.AddAsync<Question>(newQuestion);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> ViewQuestion(int questionId, string questionUrl)
        => View(await _dbContext.Questions!.FindAsync(questionId));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Reply(int questionId, string content)
    {
        if (!ModelState.IsValid)
            return Redirect(Request.Headers["Referer"].ToString());

        var user = _dbContext.Users?.SingleOrDefault(u => u.Username!.Equals(User.Identity!.Name));

        var newAnswer = new Answer
        {
            UserId = user!.UserId,
            Author = user.Username,
            Content = content,
            CreationDate = DateTime.UtcNow
        };

        var question = await _dbContext.FindAsync<Question>(questionId);
        question!.Answers!.Add(newAnswer);
        await _dbContext.SaveChangesAsync();

        return Redirect(Request.Headers["Referer"].ToString());
    }
}