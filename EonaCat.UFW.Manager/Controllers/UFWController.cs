using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EonaCat.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EonaCat.UFW.Manager.Helpers;
using EonaCat.UFW.Manager.Models;

namespace EonaCat.UFW.Manager.Controllers
{
    public class UFWController : Controller
    {
        private readonly UserHelper _userHelper = new UserHelper();
        private RuleViewModel _rulesModel = null;

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrWhiteSpace(_userHelper.UFWUser.Username))
            {
                _userHelper.UFWUser.Username = await _userHelper.GetUsernameAsync();
            }

            if (_userHelper.UFWUser.HasPassword)
            {
                _rulesModel = new RuleViewModel
                {
                    IsActive = await Linux.UFW.Manager.IsEnabledAsync()
                };

                if (_rulesModel.IsActive)
                {
                    _rulesModel.Rules = await Linux.UFW.Manager.GetRulesAsync();
                }
            }
            ViewData["UFWUser"] = _userHelper.UFWUser;
            HttpContext.Set("UserHelper", _userHelper);
            HttpContext.Set("Rules", _rulesModel);
            return View(_rulesModel);
        }

        [HttpPost]
        public async Task<IActionResult> SwitchUFW(string status)
        {
            if (status.ToLower() == "active")
            {
                await Linux.UFW.Manager.DisableAsync();
            }
            else
            {
                await Linux.UFW.Manager.EnableAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Index(string password)
        {
            var userHelper = HttpContext.Get<UserHelper>("UserHelper");
            await userHelper.SetPasswordAsync(password);
            return await Index();
        }

        // GET: UFW/Details/1
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rulesModel = HttpContext.Get<RuleViewModel>("Rules");
            var currentRule = rulesModel.Rules.FirstOrDefault(x => x.RuleIndex == id);
            return View(currentRule.Adapt<Rule>());
        }

        // GET: UFW/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UFW/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Rule rule)
        {
            if (ModelState.IsValid)
            {
                await Linux.UFW.Manager.ExecuteRule(rule.Adapt<Linux.UFW.Models.Rule>());
                return RedirectToAction(nameof(Index));
            }
            return View(rule);
        }

        // GET: UFW/Edit/1
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var rulesModel = HttpContext.Get<RuleViewModel>("Rules");
            var currentRule = rulesModel.Rules.FirstOrDefault(x => x.RuleIndex == id);
            return View(currentRule.Adapt<Rule>());
        }

        // POST: Rules/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(int id, Rule rule)
        {
            if (id != rule.RuleIndex)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var rulesModel = HttpContext.Get<RuleViewModel>("Rules");
                var currentRule = rulesModel.Rules.FirstOrDefault(x => x.RuleIndex == rule.RuleIndex);
                await Linux.UFW.Manager.ExecuteRule(rule.Adapt<Linux.UFW.Models.Rule>());
                return RedirectToAction(nameof(Index));
            }
            return View(rule);
        }

        // GET: UFW/Delete/1
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rulesModel = HttpContext.Get<RuleViewModel>("Rules");
            var currentRule = rulesModel.Rules.FirstOrDefault(x => x.RuleIndex == id);
            return View(currentRule.Adapt<Rule>());
        }

        // POST: UFW/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAsync(int id)
        {
            var rulesModel = HttpContext.Get<RuleViewModel>("Rules");
            var currentRule = rulesModel.Rules.FirstOrDefault(x => x.RuleIndex == id);
            currentRule.Operation = Linux.UFW.Models.RuleOperation.Delete;
            await Linux.UFW.Manager.ExecuteRule(currentRule);
            return RedirectToAction(nameof(Index));
        }
    }

    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
    }
}
