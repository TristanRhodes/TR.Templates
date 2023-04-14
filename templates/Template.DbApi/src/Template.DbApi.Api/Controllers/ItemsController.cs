using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using Dapper;
using System.Threading.Tasks;
using Template.DbApi.Model;

namespace Template.DbApi.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemStore _store;

        public ItemsController(IItemStore store)
        {
            _store = store;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var results = await _store.GetItems();

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(Item item)
        {
            var result = await _store.AddItem(item);

            return Ok(result);
        }
    }
}