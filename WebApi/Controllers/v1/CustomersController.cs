namespace WebApi.Controllers.v1
{

    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using FluentValidation.AspNetCore;
    using Application.Dtos.Customer;
    using Application.Enums;
    using Application.Interfaces.Customer;
    using Application.Validation.Customer;
    using Domain.Entities;
    using Microsoft.AspNetCore.JsonPatch;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/Customers")]
    [ApiVersion("1.0")]
    public class CustomersController: Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerRepository customerRepository
            , IMapper mapper)
        {
            _customerRepository = customerRepository ??
                throw new ArgumentNullException(nameof(customerRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetCustomers")]
        public ActionResult<IEnumerable<CustomerDto>> GetCustomers([FromQuery] CustomerParametersDto customerParametersDto)
        {
            var customersFromRepo = _customerRepository.GetCustomers(customerParametersDto);
            
            var previousPageLink = customersFromRepo.HasPrevious
                    ? CreateCustomersResourceUri(customerParametersDto,
                        ResourceUriType.PreviousPage)
                    : null;

            var nextPageLink = customersFromRepo.HasNext
                ? CreateCustomersResourceUri(customerParametersDto,
                    ResourceUriType.NextPage)
                : null;

            var paginationMetadata = new
            {
                totalCount = customersFromRepo.TotalCount,
                pageSize = customersFromRepo.PageSize,
                pageNumber = customersFromRepo.PageNumber,
                totalPages = customersFromRepo.TotalPages,
                hasPrevious = customersFromRepo.HasPrevious,
                hasNext = customersFromRepo.HasNext,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customersFromRepo);
            return Ok(customersDto);
        }


        [HttpGet("{customerId}", Name = "GetCustomer")]
        public ActionResult<CustomerDto> GetCustomer(int customerId)
        {
            var customerFromRepo = _customerRepository.GetCustomer(customerId);

            if (customerFromRepo == null)
            {
                return NotFound();
            }

            var customerDto = _mapper.Map<CustomerDto>(customerFromRepo);

            return Ok(customerDto);
        }

        [HttpPost]
        public ActionResult<CustomerDto> AddCustomer(CustomerForCreationDto customerForCreation)
        {
            var validationResults = new CustomerForCreationDtoValidator().Validate(customerForCreation);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            var customer = _mapper.Map<Customer>(customerForCreation);
            _customerRepository.AddCustomer(customer);
            var saveSuccessful = _customerRepository.Save();

            if(saveSuccessful)
            {
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return CreatedAtRoute("GetCustomer",
                    new { customerDto.CustomerId },
                    customerDto);
            }

            return StatusCode(500);
        }

        [HttpDelete("{customerId}")]
        public ActionResult DeleteCustomer(int customerId)
        {
            var customerFromRepo = _customerRepository.GetCustomer(customerId);

            if (customerFromRepo == null)
            {
                return NotFound();
            }

            _customerRepository.DeleteCustomer(customerFromRepo);
            _customerRepository.Save();

            return NoContent();
        }

        [HttpPut("{customerId}")]
        public IActionResult UpdateCustomer(int customerId, CustomerForUpdateDto customer)
        {
            var customerFromRepo = _customerRepository.GetCustomer(customerId);

            if (customerFromRepo == null)
            {
                return NotFound();
            }

            var validationResults = new CustomerForUpdateDtoValidator().Validate(customer);
            validationResults.AddToModelState(ModelState, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return ValidationProblem();
            }

            _mapper.Map(customer, customerFromRepo);
            _customerRepository.UpdateCustomer(customerFromRepo);

            _customerRepository.Save();

            return NoContent();
        }

        [HttpPatch("{customerId}")]
        public IActionResult PartiallyUpdateCustomer(int customerId, JsonPatchDocument<CustomerForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var existingCustomer = _customerRepository.GetCustomer(customerId);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            var customerToPatch = _mapper.Map<CustomerForUpdateDto>(existingCustomer); // map the customer we got from the database to an updatable customer model
            patchDoc.ApplyTo(customerToPatch, ModelState); // apply patchdoc updates to the updatable customer

            if (!TryValidateModel(customerToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(customerToPatch, existingCustomer); // apply updates from the updatable customer to the db entity so we can apply the updates to the database
            _customerRepository.UpdateCustomer(existingCustomer); // apply business updates to data if needed

            _customerRepository.Save(); // save changes in the database

            return NoContent();
        }

        private string CreateCustomersResourceUri(
            CustomerParametersDto customerParametersDto,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCustomers",
                        new
                        {
                            filters = customerParametersDto.Filters,
                            orderBy = customerParametersDto.SortOrder,
                            pageNumber = customerParametersDto.PageNumber - 1,
                            pageSize = customerParametersDto.PageSize
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCustomers",
                        new
                        {
                            filters = customerParametersDto.Filters,
                            orderBy = customerParametersDto.SortOrder,
                            pageNumber = customerParametersDto.PageNumber + 1,
                            pageSize = customerParametersDto.PageSize
                        });

                default:
                    return Url.Link("GetCustomers",
                        new
                        {
                            filters = customerParametersDto.Filters,
                            orderBy = customerParametersDto.SortOrder,
                            pageNumber = customerParametersDto.PageNumber,
                            pageSize = customerParametersDto.PageSize
                        });
            }
        }
    }
}