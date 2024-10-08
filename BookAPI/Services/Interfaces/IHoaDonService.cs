﻿using BookAPI.Models;

namespace BookAPI.Services.Interfaces
{
    public interface IHoaDonService
    {
        Task<IEnumerable<HoaDonModel>> GetOrdersByMaKhAsync(string maKh, int page, int pageSize);
        Task<HoaDonModel> GetOrderByIdAsync(Guid id, string maKh);
        Task UpdateOrderStateAsync(Guid id, int state);
        Task<IEnumerable<HoaDonModel>> GetOderConfirm(string email, int page, int pageSize);
    }
}
