using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CustomerApi.Data.Repository.v1;
using CustomerApi.Domain.Entities;
using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Service.v1.Query
{
    public class GetAllCustomerQueryHandler : IRequestHandler<GetAllCustomerQuery, List<Customer>>
    {
        private readonly IRepository<Customer> _repository;

        public GetAllCustomerQueryHandler(IRepository<Customer> repository)
        {
            _repository = repository;
        }

        public async Task<List<Customer>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAll().ToListAsync(cancellationToken);
        }
    }
}
