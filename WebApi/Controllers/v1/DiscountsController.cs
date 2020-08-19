namespace WebApi.Controllers.v1
{

    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using Application.Dtos.Discount;
    using Application.Enums;
    using Application.Interfaces.Discount;
    using Application.Validation.Discount;
    using Domain.Entities;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/Discounts")]
    [ApiVersion("1.0")]
    public class DiscountsController: Controller
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;

        public DiscountsController(IDiscountRepository discountRepository
            , IMapper mapper)
        {
            _discountRepository = discountRepository ??
                throw new ArgumentNullException(nameof(discountRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetDiscounts")]
        public ActionResult<IEnumerable<DiscountDto>> GetDiscounts([FromQuery] DiscountParametersDto discountParametersDto)
        {
            var discountsFromRepo = _discountRepository.GetDiscounts(discountParametersDto);
            
            var previousPageLink = discountsFromRepo.HasPrevious
                    ? CreateDiscountsResourceUri(discountParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = discountsFromRepo.HasNext
                ? CreateDiscountsResourceUri(discountParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = discountsFromRepo.TotalCount,
                pageSize = discountsFromRepo.PageSize,
                pageNumber = discountsFromRepo.PageNumber,
                totalPages = discountsFromRepo.TotalPages,
                hasPrevious = discountsFromRepo.HasPrevious,
                hasNext = discountsFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var discountsDto = _mapper.Map<IEnumerable<DiscountDto>>(discountsFromRepo);
            return Ok(discountsDto);
        }


        [HttpGet("{discountId}", Name = "GetDiscount")]
        public ActionResult<DiscountDto> GetDiscount(int discountId)
        {
            var discountFromRepo = _discountRepository.GetDiscount(discountId);

            if (discountFromRepo == null)
            {
                return NotFound();
            }

            var discountDto = _mapper.Map<DiscountDto>(discountFromRepo);

            return Ok(discountDto);
        }

        [HttpPost]
        public ActionResult<DiscountDto> AddDiscount(DiscountForCreationDto discountForCreation)
        {
            var validationResults = new DiscountForCreationDtoValidator().Validate(discountForCreation);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            var discount = _mapper.Map<Discount>(discountForCreation);
            _discountRepository.AddDiscount(discount);
            var saveSuccessful = _discountRepository.Save();

            if(saveSuccessful)
            {
                var discountDto = _mapper.Map<DiscountDto>(discount);
                return CreatedAtRoute("GetDiscount",
                    new { discountDto.DiscountId },
                    discountDto);
            }

            return StatusCode(500);
        }

        [HttpDelete("{discountId}")]
        public ActionResult DeleteDiscount(int discountId)
        {
            var discountFromRepo = _discountRepository.GetDiscount(discountId);

            if (discountFromRepo == null)
            {
                return NotFound();
            }

            _discountRepository.DeleteDiscount(discountFromRepo);
            _discountRepository.Save();

            return NoContent();
        }

        [HttpPut("{discountId}")]
        public IActionResult UpdateDiscount(int discountId, DiscountForUpdateDto discount)
        {
            var discountFromRepo = _discountRepository.GetDiscount(discountId);

            if (discountFromRepo == null)
            {
                return NotFound();
            }

            var validationResults = new DiscountForUpdateDtoValidator().Validate(discount);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            _mapper.Map(discount, discountFromRepo);
            _discountRepository.UpdateDiscount(discountFromRepo);

            _discountRepository.Save();

            return NoContent();
        }

        [HttpPatch("{discountId}")]
        public IActionResult PartiallyUpdateDiscount(int discountId, JsonPatchDocument<DiscountForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingDiscount = _discountRepository.GetDiscount(discountId);

            if (existingDiscount == null)
            {
                return NotFound();
            }

            var discountToPatch = _mapper.Map<DiscountForUpdateDto>(existingDiscount); // map the discount we got from the database to an updatable discount model
            patchDoc.ApplyTo(discountToPatch, ModelState); // apply patchdoc updates to the updatable discount

            if (!TryValidateModel(discountToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(discountToPatch, existingDiscount); // apply updates from the updatable discount to the db entity so we can apply the updates to the database
            _discountRepository.UpdateDiscount(existingDiscount); // apply business updates to data if needed

            _discountRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateDiscountsResourceUri(
            DiscountParametersDto discountParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetDiscounts",
                        new
                        {
                            filters = discountParametersDto.Filters,
                            orderBy = discountParametersDto.SortOrder,
                            pageNumber = discountParametersDto.PageNumber - 1,
                            pageSize = discountParametersDto.PageSize
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetDiscounts",
                        new
                        {
                            filters = discountParametersDto.Filters,
                            orderBy = discountParametersDto.SortOrder,
                            pageNumber = discountParametersDto.PageNumber + 1,
                            pageSize = discountParametersDto.PageSize
                        });

                default:
                    return Url.Link("GetDiscounts",
                        new
                        {
                            filters = discountParametersDto.Filters,
                            orderBy = discountParametersDto.SortOrder,
                            pageNumber = discountParametersDto.PageNumber,
                            pageSize = discountParametersDto.PageSize
                        });
            }
        }
    }
}