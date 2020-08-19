namespace WebApi.Controllers.v1
{

    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using Application.Dtos.Salesperson;
    using Application.Enums;
    using Application.Interfaces.Salesperson;
    using Application.Validation.Salesperson;
    using Domain.Entities;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/Salespersons")]
    [ApiVersion("1.0")]
    public class SalespersonsController: Controller
    {
        private readonly ISalespersonRepository _salespersonRepository;
        private readonly IMapper _mapper;

        public SalespersonsController(ISalespersonRepository salespersonRepository
            , IMapper mapper)
        {
            _salespersonRepository = salespersonRepository ??
                throw new ArgumentNullException(nameof(salespersonRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetSalespersons")]
        public ActionResult<IEnumerable<SalespersonDto>> GetSalespersons([FromQuery] SalespersonParametersDto salespersonParametersDto)
        {
            var salespersonsFromRepo = _salespersonRepository.GetSalespersons(salespersonParametersDto);
            
            var previousPageLink = salespersonsFromRepo.HasPrevious
                    ? CreateSalespersonsResourceUri(salespersonParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = salespersonsFromRepo.HasNext
                ? CreateSalespersonsResourceUri(salespersonParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = salespersonsFromRepo.TotalCount,
                pageSize = salespersonsFromRepo.PageSize,
                pageNumber = salespersonsFromRepo.PageNumber,
                totalPages = salespersonsFromRepo.TotalPages,
                hasPrevious = salespersonsFromRepo.HasPrevious,
                hasNext = salespersonsFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var salespersonsDto = _mapper.Map<IEnumerable<SalespersonDto>>(salespersonsFromRepo);
            return Ok(salespersonsDto);
        }


        [HttpGet("{salespersonId}", Name = "GetSalesperson")]
        public ActionResult<SalespersonDto> GetSalesperson(int salespersonId)
        {
            var salespersonFromRepo = _salespersonRepository.GetSalesperson(salespersonId);

            if (salespersonFromRepo == null)
            {
                return NotFound();
            }

            var salespersonDto = _mapper.Map<SalespersonDto>(salespersonFromRepo);

            return Ok(salespersonDto);
        }

        [HttpPost]
        public ActionResult<SalespersonDto> AddSalesperson(SalespersonForCreationDto salespersonForCreation)
        {
            var validationResults = new SalespersonForCreationDtoValidator().Validate(salespersonForCreation);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            var salesperson = _mapper.Map<Salesperson>(salespersonForCreation);
            _salespersonRepository.AddSalesperson(salesperson);
            var saveSuccessful = _salespersonRepository.Save();

            if(saveSuccessful)
            {
                var salespersonDto = _mapper.Map<SalespersonDto>(salesperson);
                return CreatedAtRoute("GetSalesperson",
                    new { salespersonDto.SalespersonId },
                    salespersonDto);
            }

            return StatusCode(500);
        }

        [HttpDelete("{salespersonId}")]
        public ActionResult DeleteSalesperson(int salespersonId)
        {
            var salespersonFromRepo = _salespersonRepository.GetSalesperson(salespersonId);

            if (salespersonFromRepo == null)
            {
                return NotFound();
            }

            _salespersonRepository.DeleteSalesperson(salespersonFromRepo);
            _salespersonRepository.Save();

            return NoContent();
        }

        [HttpPut("{salespersonId}")]
        public IActionResult UpdateSalesperson(int salespersonId, SalespersonForUpdateDto salesperson)
        {
            var salespersonFromRepo = _salespersonRepository.GetSalesperson(salespersonId);

            if (salespersonFromRepo == null)
            {
                return NotFound();
            }

            var validationResults = new SalespersonForUpdateDtoValidator().Validate(salesperson);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            _mapper.Map(salesperson, salespersonFromRepo);
            _salespersonRepository.UpdateSalesperson(salespersonFromRepo);

            _salespersonRepository.Save();

            return NoContent();
        }

        [HttpPatch("{salespersonId}")]
        public IActionResult PartiallyUpdateSalesperson(int salespersonId, JsonPatchDocument<SalespersonForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingSalesperson = _salespersonRepository.GetSalesperson(salespersonId);

            if (existingSalesperson == null)
            {
                return NotFound();
            }

            var salespersonToPatch = _mapper.Map<SalespersonForUpdateDto>(existingSalesperson); // map the salesperson we got from the database to an updatable salesperson model
            patchDoc.ApplyTo(salespersonToPatch, ModelState); // apply patchdoc updates to the updatable salesperson

            if (!TryValidateModel(salespersonToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(salespersonToPatch, existingSalesperson); // apply updates from the updatable salesperson to the db entity so we can apply the updates to the database
            _salespersonRepository.UpdateSalesperson(existingSalesperson); // apply business updates to data if needed

            _salespersonRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateSalespersonsResourceUri(
            SalespersonParametersDto salespersonParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetSalespersons",
                        new
                        {
                            filters = salespersonParametersDto.Filters,
                            orderBy = salespersonParametersDto.SortOrder,
                            pageNumber = salespersonParametersDto.PageNumber - 1,
                            pageSize = salespersonParametersDto.PageSize
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetSalespersons",
                        new
                        {
                            filters = salespersonParametersDto.Filters,
                            orderBy = salespersonParametersDto.SortOrder,
                            pageNumber = salespersonParametersDto.PageNumber + 1,
                            pageSize = salespersonParametersDto.PageSize
                        });

                default:
                    return Url.Link("GetSalespersons",
                        new
                        {
                            filters = salespersonParametersDto.Filters,
                            orderBy = salespersonParametersDto.SortOrder,
                            pageNumber = salespersonParametersDto.PageNumber,
                            pageSize = salespersonParametersDto.PageSize
                        });
            }
        }
    }
}