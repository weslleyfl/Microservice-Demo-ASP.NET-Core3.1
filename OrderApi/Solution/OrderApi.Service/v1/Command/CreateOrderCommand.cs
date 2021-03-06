﻿using OrderApi.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace OrderApi.Service.v1.Command
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public Order Order { get; set; }
    }
}
