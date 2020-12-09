using CustomerApi.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerApi.Service.v1.Query
{
    public class GetAllCustomerQuery : IRequest<List<Customer>>
    {
    }
}
