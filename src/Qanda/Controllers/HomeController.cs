using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qanda.Data;
using Qanda.ViewModels;

namespace Qanda.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index(string filters, int page, string q)
    {
        var currentFilter = String.IsNullOrEmpty(filters) ? "All Questions" 
            : char.ToUpper(filters[0]) + filters.Substring(1);
        ViewData["CurrentFilter"] = currentFilter;

        var questions = from question in _dbContext.Questions select question;
        switch (filters)
        {
            case "questions":
                questions = questions.OrderByDescending(q => q.CreationDate);
                break;
            case "answered":
                questions = questions.Where(q => q.Answers != null && q.Answers!.Count > 0);
                break;
            case "unanswered":
                questions = questions.Where(q => q.Answers == null || q.Answers.Count.Equals(0));
                break;
            case "leaderboard":
                questions = questions.OrderByDescending(q => q.Answers.Count);
                break;
        }

        if (!String.IsNullOrEmpty(q))
            questions = questions.Where(question 
            => question.Title.Contains(q) || question.Body.Contains(q));
        return View(await questions.ToListAsync());
    }

    public IActionResult Search(string q)
    {
        var suggestions = _dbContext.Questions?.Where(
            item => item.Title!.Contains(q) || item.Body!.Contains(q));
        return View(suggestions.ToList());
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
