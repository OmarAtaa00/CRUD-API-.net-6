using System;
using Microsoft.AspNetCore.Mvc;

namespace BreakfastMaker.Controllers;

public class ErrorController : ControllerBase
{

    [Route("/error")]
    public IActionResult Error()
    {
        return Problem();

    }

}

