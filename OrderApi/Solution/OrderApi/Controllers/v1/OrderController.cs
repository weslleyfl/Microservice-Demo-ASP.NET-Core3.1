using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderApi.Domain;
using OrderApi.Models.v1;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Query;

namespace OrderApi.Controllers.v1
{
    [Produces(contentType: "application/json")]
    [Route("v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, IMapper mapper, ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        ///     Action to create a new order in the database.
        /// </summary>
        /// <param name="orderModel">Model to create a new order</param>
        /// <returns>Returns the created order</returns>
        /// <response code="200">Returned if the order was created</response>
        /// <response code="400">Returned if the model couldn't be parsed or saved</response>
        /// <response code="422">Returned when the validation failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]
        public async Task<ActionResult<Order>> Order([FromBody] OrderModel orderModel)
        {
            try
            {
                _logger.LogInformation("Criando um novo pedido (Order) no banco de dados.");

                return await _mediator.Send(new CreateOrderCommand()
                {
                    Order = _mapper.Map<Order>(orderModel)
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Action to retrieve all pay orders.
        /// </summary>
        /// <returns>Returns a list of all paid orders or an empty list, if no order is paid yet</returns>
        /// <response code="200">Returned if the list of orders was retrieved</response>
        /// <response code="400">Returned if the orders could not be retrieved</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> Order()
        {
            try
            {
                _logger.LogInformation("Obtendo todos os pedidos que já foram pagos {pay orders}");

                return await _mediator.Send(new GetPaidOrderQuery());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///     Action to pay an order.
        /// </summary>
        /// <param name="id">O Id do pedido que foi pago</param>
        /// <returns>Returns the paid order</returns>
        /// <response code="200">Returned if the order was updated (paid)</response>
        /// <response code="400">Returned if the order could not be found with the provided id</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("pay/{id}")]
        public async Task<ActionResult<Order>> Pay(Guid id)
        {
            try
            {
                var order = await _mediator.Send(new GetOrderByIdQuery()
                {
                    Id = id
                });

                if (order == null)
                {
                    return BadRequest($"No order found with the id {id}");
                }

                order.OrderState = OrderState.NotPaidOrder;

                // Update the state
                return await _mediator.Send(new PayOrderCommand()
                {
                    Order = order
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}