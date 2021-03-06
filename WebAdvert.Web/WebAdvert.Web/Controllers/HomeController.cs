using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAdvert.Web.Clients.Interfaces;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Home;

namespace WebAdvert.Web.Controllers
{
  public class HomeController : Controller
  {
    private readonly ISearchApiClient _searchApiClient;
    private readonly IMapper _mapper;
    private readonly IAdvertApiClient _apiClient;

    public HomeController(ISearchApiClient searchApiClient, IMapper mapper, IAdvertApiClient apiClient)
    {
      _searchApiClient = searchApiClient;
      _mapper = mapper;
      _apiClient = apiClient;
    }

    [Authorize]
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Search(string keyword)
    {
      var viewModel = new List<SearchViewModel>();

      var searchResult = await _searchApiClient.Search(keyword);
      
      searchResult.ForEach(advertDoc =>
      {
        var viewModelItem = _mapper.Map<SearchViewModel>(advertDoc);
        viewModel.Add(viewModelItem);
      });

      return View("Search", viewModel);
    }
  }
}
