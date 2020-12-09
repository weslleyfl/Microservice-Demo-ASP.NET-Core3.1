using AutoMapper;
using OrderApi.Domain;
using OrderApi.Models.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Infrastructure.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderModel, Order>()
                .ForMember(x => x.OrderState, opt => opt.MapFrom(src => 1));
        }
    }
}
