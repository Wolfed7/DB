using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly DataBaseHandler _dataBaseHandler;

	public HomeController(ILogger<HomeController> logger)
	{
		_logger = logger;
		_dataBaseHandler = new DataBaseHandler();
	}

	public IActionResult Index()
	{
		List<SelectListItem> items_n_izd = _dataBaseHandler.GetIzds();
		if (_dataBaseHandler.ErrorMessage is not null)
			TempData["alertIzd"] = _dataBaseHandler.ErrorMessage;
		List<SelectListItem> items_n_cl = _dataBaseHandler.GetClients();
		if (_dataBaseHandler.ErrorMessage is not null)
			TempData["alertCl"] = _dataBaseHandler.ErrorMessage;
		var ret = (items_n_izd, items_n_cl);
		return View(ret);
	}

	public IActionResult ResetTables()
	{
		_dataBaseHandler.Reset();
		if (_dataBaseHandler.ErrorMessage is not null)
			TempData["alertMessage"] = _dataBaseHandler.ErrorMessage;
		return View();
	}

	public IActionResult Query(string n_izd)
	{
		var result = _dataBaseHandler.Query1(n_izd);

		if (_dataBaseHandler.ErrorMessage is not null)
			TempData["alertMessage"] = _dataBaseHandler.ErrorMessage;
		return View(result);
	}

	public IActionResult QueryUpdate(string n_izd, string n_cl, string date_order, string date_pay, string date_ship, string kol, string cost)
	{
		var result = _dataBaseHandler.QueryUpdate(n_izd, n_cl, date_order, date_pay, date_ship, kol, cost);
		if (_dataBaseHandler.ErrorMessage is not null)
			TempData["alertMessage"] = _dataBaseHandler.ErrorMessage;
		return View(result);
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}