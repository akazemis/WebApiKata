using AutoMapper;
using DataAccess.Models;
using WebApiKata.ResourceModels;

namespace WebApiKata.ModelMapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            AllowNullCollections = true;
            CreateMap<User, UserModel>();
            CreateMap<Product, ProductModel>();

            CreateMap<TrolleyInfoModel, TrolleyInfo>();
            CreateMap<TrolleyProductModel, TrolleyProduct>();
            CreateMap<TrolleyQuantityModel, TrolleyQuantity>();
            CreateMap<TrolleySpecialModel, TrolleySpecial>();
        }
    }
}
