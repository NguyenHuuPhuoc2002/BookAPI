﻿using AutoMapper;
using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Sach, SachModel>().ReverseMap();
        }

    }
}
