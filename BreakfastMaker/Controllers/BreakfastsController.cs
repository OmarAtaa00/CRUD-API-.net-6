using System;
using BreakfastMaker.Contracts.BreakfastMaker;

using BreakfastMaker.Models;
using BreakfastMaker.ServiceError;
using BreakfastMaker.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BreakfastMaker.Controllers
{

    public class BreakfastsController : ApiController
    {
        private readonly IBreakfastService _breakfastService;
        public BreakfastsController(IBreakfastService breakfastService)
        {
            _breakfastService = breakfastService;
        }

        [HttpPost()]
        public IActionResult CreateBreakfast(CreateBreakfastRequest request)
        {
            var breakfast = new Breakfast(
                Guid.NewGuid(),
                request.Name,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                DateTime.UtcNow,
                request.Savory,
                request.Sweet
            );

            //save to database 
            ErrorOr<Created> createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

            return createBreakfastResult.Match(
            created => CreatedAtGetBreakfast(breakfast),
            errors => Problem(errors)
        );

        }



        [HttpGet("{id:guid}")]
        public IActionResult GetBreakfast(Guid id)
        {
            ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

            return getBreakfastResult.Match(
                breakfast => Ok(MapBreakfastResponse(breakfast)),
                errors => Problem()
            );
            //NotFound
            // if (getBreakfastResult.IsError &&
            //  getBreakfastResult.FirstError == Errors.Breakfast.NotFound)
            // {
            //     return NotFound();
            // }
            // var breakfast = getBreakfastResult.Value;
            // BreakfastResponse response = MapBreakfastResponse(breakfast);
            // return Ok(response);
        }



        [HttpPut("{id:guid}")]
        public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
        {
            var breakfast = new Breakfast(
                id,
                request.Name,
                request.Description,
                request.StartDateTime,
                request.EndDateTime,
                DateTime.UtcNow,
                request.Savory,
                request.Sweet
            );

            ErrorOr<Updated> updatedResult = _breakfastService.UpsertBreakfast(breakfast);

            //return 201 created if breakfast was created //if not make new dinner 
            return updatedResult.Match(
            upserted => CreatedAtGetBreakfast(breakfast),
            errors => Problem(errors));


        }
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteBreakfast(Guid id)
        {

            ErrorOr<Deleted> deleteResult = _breakfastService.DeleteBreakfast(id);

            return deleteResult.Match(
                deleted => NoContent(),
                errors => Problem(errors));


        }

        private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
        {
            return new BreakfastResponse(
               breakfast.Id,
               breakfast.Name,
               breakfast.Description,
               breakfast.StartDateTime,
               breakfast.EndDateTime,
               breakfast.LastModifiedDateTime,
               breakfast.Savory,
               breakfast.Sweet

           );
        }

        private IActionResult CreatedAtGetBreakfast(Breakfast breakfast)
        {
            return CreatedAtAction(
                actionName: nameof(GetBreakfast),
                routeValues: new { id = breakfast.Id },
                value: MapBreakfastResponse(breakfast));
        }

    }
}
