using OrderApi.Service.v1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderApi.Service.v1.Services
{
    public interface ICustomerNameUpdateService
    {
        void UpdateCustomerNameInOrders(UpdateCustomerFullNameModel updateCustomerFullNameModel);
    }
}
