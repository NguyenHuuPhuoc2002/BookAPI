﻿using BookAPI.Data;
using BookAPI.Models;

namespace BookAPI.Repositories.Interfaces
{
    public interface IKhachHangRepository
    {
        Task<KhachHang> CheckLogIn(LogInModel model);
        Task<KhachHang> GetUserById(string maKH);
        Task Register(KhachHang user);
    }
}
