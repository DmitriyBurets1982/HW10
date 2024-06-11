using AutoMapper;
using Contracts.AccountService;
using Contracts.OrderService;

namespace OrderService.Profiles
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile()
        {
            // Source -> Target
            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order, AccountOperationDto>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Price));
        }
    }
}
