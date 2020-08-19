namespace WebApi.Controllers.v1
{

    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using Application.Dtos.Sale;
    using Application.Enums;
    using Application.Interfaces.Sale;
    using Application.Validation.Sale;
    using Domain.Entities;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/Sales")]
    [ApiVersion("1.0")]
    public class SalesController: Controller
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public SalesController(ISaleRepository saleRepository
            , IMapper mapper)
        {
            _saleRepository = saleRepository ??
                throw new ArgumentNullException(nameof(saleRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetSales")]
        public ActionResult<IEnumerable<SaleDto>> GetSales([FromQuery] SaleParametersDto saleParametersDto)
        {
            var salesFromRepo = _saleRepository.GetSales(saleParametersDto);
            
            var previousPageLink = salesFromRepo.HasPrevious
                    ? CreateSalesResourceUri(saleParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = salesFromRepo.HasNext
                ? CreateSalesResourceUri(saleParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = salesFromRepo.TotalCount,
                pageSize = salesFromRepo.PageSize,
                pageNumber = salesFromRepo.PageNumber,
                totalPages = salesFromRepo.TotalPages,
                hasPrevious = salesFromRepo.HasPrevious,
                hasNext = salesFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var salesDto = _mapper.Map<IEnumerable<SaleDto>>(salesFromRepo);
            return Ok(salesDto);
        }


        [HttpGet("{saleId}", Name = "GetSale")]
        public ActionResult<SaleDto> GetSale(int saleId)
        {
            var saleFromRepo = _saleRepository.GetSale(saleId);

            if (saleFromRepo == null)
            {
                return NotFound();
            }

            var saleDto = _mapper.Map<SaleDto>(saleFromRepo);

            return Ok(saleDto);
        }

        [HttpPost]
        public ActionResult<SaleDto> AddSale(SaleForCreationDto saleForCreation)
        {
            var validationResults = new SaleForCreationDtoValidator().Validate(saleForCreation);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            var sale = _mapper.Map<Sale>(saleForCreation);
            _saleRepository.AddSale(sale);
            var saveSuccessful = _saleRepository.Save();

            if(saveSuccessful)
            {
                var saleDto = _mapper.Map<SaleDto>(sale);
                return CreatedAtRoute("GetSale",
                    new { saleDto.SaleId },
                    saleDto);
            }

            return StatusCode(500);
        }

        [HttpDelete("{saleId}")]
        public ActionResult DeleteSale(int saleId)
        {
            var saleFromRepo = _saleRepository.GetSale(saleId);

            if (saleFromRepo == null)
            {
                return NotFound();
            }

            _saleRepository.DeleteSale(saleFromRepo);
            _saleRepository.Save();

            return NoContent();
        }

        [HttpPut("{saleId}")]
        public IActionResult UpdateSale(int saleId, SaleForUpdateDto sale)
        {
            var saleFromRepo = _saleRepository.GetSale(saleId);

            if (saleFromRepo == null)
            {
                return NotFound();
            }

            var validationResults = new SaleForUpdateDtoValidator().Validate(sale);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            _mapper.Map(sale, saleFromRepo);
            _saleRepository.UpdateSale(saleFromRepo);

            _saleRepository.Save();

            return NoContent();
        }

        [HttpPatch("{saleId}")]
        public IActionResult PartiallyUpdateSale(int saleId, JsonPatchDocument<SaleForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingSale = _saleRepository.GetSale(saleId);

            if (existingSale == null)
            {
                return NotFound();
            }

            var saleToPatch = _mapper.Map<SaleForUpdateDto>(existingSale); // map the sale we got from the database to an updatable sale model
            patchDoc.ApplyTo(saleToPatch, ModelState); // apply patchdoc updates to the updatable sale

            if (!TryValidateModel(saleToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(saleToPatch, existingSale); // apply updates from the updatable sale to the db entity so we can apply the updates to the database
            _saleRepository.UpdateSale(existingSale); // apply business updates to data if needed

            _saleRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateSalesResourceUri(
            SaleParametersDto saleParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetSales",
                        new
                        {
                            filters = saleParametersDto.Filters,
                            orderBy = saleParametersDto.SortOrder,
                            pageNumber = saleParametersDto.PageNumber - 1,
                            pageSize = saleParametersDto.PageSize
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetSales",
                        new
                        {
                            filters = saleParametersDto.Filters,
                            orderBy = saleParametersDto.SortOrder,
                            pageNumber = saleParametersDto.PageNumber + 1,
                            pageSize = saleParametersDto.PageSize
                        });

                default:
                    return Url.Link("GetSales",
                        new
                        {
                            filters = saleParametersDto.Filters,
                            orderBy = saleParametersDto.SortOrder,
                            pageNumber = saleParametersDto.PageNumber,
                            pageSize = saleParametersDto.PageSize
                        });
            }
        }
    }
}