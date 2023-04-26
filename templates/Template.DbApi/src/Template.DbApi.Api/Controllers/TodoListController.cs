using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using Dapper;
using System.Threading.Tasks;
using Template.DbApi.Model;
using MediatR;
using System.Collections.Generic;
using Template.DbApi.Handlers;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.IdentityModel.Logging;

namespace Template.DbApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoListController : ControllerBase
{
    private readonly IMediator _mediator;

    public TodoListController(IMediator store)
    {
        _mediator = store;
    }

    [HttpGet("GetItems")]
    [Authorize(Roles = Roles.AppUser)]
    public async Task<IActionResult> GetItems()
    {
        var result = await _mediator.Send(new SelectTodo());
        return Ok(result);
    }

    [HttpPost("InsertItem")]
    [Authorize(Roles = Roles.AppUser)]
    public async Task<IActionResult> InsertItem(InsertTodo insert)
    {
        var result = await _mediator.Send(insert);
        return Ok(result);
    }
}