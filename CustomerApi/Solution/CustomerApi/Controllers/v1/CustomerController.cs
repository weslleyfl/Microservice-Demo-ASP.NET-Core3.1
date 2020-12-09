using AutoMapper;
using CustomerApi.Domain.Entities;
using CustomerApi.Models.v1;
using CustomerApi.Service.v1.Command;
using CustomerApi.Service.v1.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerApi.Controllers.v1
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CustomerController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<List<GetAllCustomerModel>>> Customer()
        {
            try
            {
                List<Customer> customers = await _mediator.Send(new GetAllCustomerQuery());

                if (customers?.Count <= 0)
                {
                    return BadRequest("No customer found ");
                }

                return _mapper.Map<List<GetAllCustomerModel>>(customers);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Action to create a new customer in the database.
        /// </summary>
        /// <param name="createCustomerModel">Model to create a new customer</param>
        /// <returns>Returns the created customer</returns>
        /// /// <response code="200">Returned if the customer was created</response>
        /// /// <response code="400">Returned if the model couldn't be parsed or the customer couldn't be saved</response>
        /// /// <response code="422">Returned when the validation failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost("Customter")]
        public async Task<ActionResult<Customer>> Customer([FromBody] CreateCustomerModel createCustomerModel)
        {
            try
            {
                return await _mediator.Send(new CreateCustomerCommand()
                {
                    Customer = _mapper.Map<Customer>(createCustomerModel)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Action to update an existing customer
        /// </summary>
        /// <param name="id">Id to update an existing customer</param>
        /// <param name="updateCustomerModel">Model to update an existing customer</param>
        /// <returns>Returns the updated customer</returns>
        /// /// <response code="200">Returned if the customer was updated</response>
        /// /// <response code="400">Returned if the model couldn't be parsed or the customer couldn't be found</response>
        /// /// <response code="422">Returned when the validation failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPut("Customter")]
        public async Task<ActionResult<Customer>> Customer(Guid id, [FromBody] UpdateCustomerModel updateCustomerModel)
        {
            try
            {

                var customer = await _mediator.Send(new GetCustomerByIdQuery()
                {
                    Id = id
                });

                if (customer == null)
                {
                    return BadRequest($"No customer found with the id {updateCustomerModel.Id}");
                }

                return await _mediator.Send(new UpdateCustomerCommand()
                {
                    Customer = _mapper.Map(updateCustomerModel, customer)

                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
       

    }
}