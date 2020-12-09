using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderApi.Controllers.v1;
using OrderApi.Domain;
using OrderApi.Models.v1;
using OrderApi.Service.v1;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Query;
using Xunit;

namespace OrderApi.Test.Controllers.v1
{
    public class OrderControllerTests
    {
        private readonly OrderController _testee;
        private readonly OrderModel _orderModel;
        private readonly Guid _id = Guid.Parse("35296ce1-e20f-4dc6-83c8-25b9152995e0");
        private readonly IMediator _mediator;

        public OrderControllerTests()
        {
            _mediator = A.Fake<IMediator>();
            _testee = new OrderController(_mediator, A.Fake<IMapper>(), A.Fake<ILogger<OrderController>>());

            _orderModel = new OrderModel
            {
                CustomerFullName = "Darth Vader",
                CustomerGuid = Guid.Parse("f1eef36b-d525-416b-acc5-effa50389db1")
            };

            var orders = new List<Order>
            {
                new Order
                {
                    CustomerFullName = "Darth Vader",
                    CustomerGuid = Guid.Parse("f1eef36b-d525-416b-acc5-effa50389db1"),
                    Id = _id,
                    OrderState = OrderState.PaidOrder
                },
                new Order
                {
                    CustomerFullName = "Son Goku",
                    CustomerGuid = Guid.Parse("29c4c5f6-e907-4956-9491-659cb838d41e"),
                    Id = Guid.Parse("270b0d0f-cfde-4846-a67d-098166c333a1"),
                    OrderState = OrderState.NotPaidOrder
                }
            };


            A.CallTo(() => _mediator.Send(A<CreateOrderCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(orders.First());
            A.CallTo(() => _mediator.Send(A<PayOrderCommand>.Ignored, A<CancellationToken>.Ignored))
                .Returns(orders.Last());
            A.CallTo(() => _mediator.Send(A<GetOrderByIdQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(orders.Last());
            A.CallTo(() => _mediator.Send(A<GetPaidOrderQuery>.Ignored, A<CancellationToken>.Ignored))
                .Returns(orders.FindAll(x => x.OrderState == OrderState.NotPaidOrder));

        }

        [Theory]
        [InlineData("CreateOrderAsync: order is null")]
        public async void Order_WhenAnExceptionOccurs_ShouldReturnBadRequest(string exceptionMessage)
        {
            A.CallTo(() => _mediator.Send(A<CreateOrderCommand>.Ignored, default))
                .Throws(new ArgumentException(exceptionMessage));

            var result = await _testee.Order(_orderModel);

            (result.Result as StatusCodeResult)?.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            (result.Result as BadRequestObjectResult)?.Value.Should().Be(exceptionMessage);
        }

    }
}
