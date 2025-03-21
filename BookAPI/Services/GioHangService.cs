﻿using BookAPI.Data;
using BookAPI.Repositories.Interfaces;
using BookAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookAPI.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly IGioHangRepository _cart;

        public GioHangService(IGioHangRepository cart) 
        {
            _cart = cart;
        }
        public async Task AddAsync(GioHang cart)
        {
            await _cart.AddAsync(cart);
        }

        public async Task<IEnumerable<GioHang>> GetAllCartsAsync()
        {
            return await _cart.GetAllCartsAsync();
        }

        public async Task<GioHang> GetCartByIdAsync(int id)
        {
            return await _cart.GetCartByIdAsync(id);
        }

        public async Task<GioHang> GetCartByMaKhAsync(string maKh)
        {
            return await _cart.GetCartByMaKhAsync(maKh);
        }
        public async Task AddRangeChiTietHdAsync(IEnumerable<ChiTietHoaDon> cthds)
        {
            await _cart.AddRangeChiTietHdAsync(cthds);
        }
        public async Task AddHoaDonAsync(HoaDon model)
        {
            await _cart.AddHoaDonAsync(model);
        }
        public async Task BeginTransactionAsync()
        {
            await _cart.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _cart.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _cart.RollbackTransactionAsync();
        }
    }
}
