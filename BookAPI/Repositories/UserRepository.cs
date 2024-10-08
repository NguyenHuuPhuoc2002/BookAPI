﻿using BookAPI.Data;
using BookAPI.Models;
using BookAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(int page, int pageSize)
        {
            await Task.CompletedTask;
            try
            {
                var data = _userManager.Users.AsQueryable();
                var result = PaginatedList<ApplicationUser>.Create(data, page, pageSize).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new KeyNotFoundException("User không tồn tại");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationUser>> Search(string key, int page, int pageSize)
        {
            await Task.CompletedTask;
            try
            {
                var data = _userManager.Users.Where(p => p.FirstName.ToLower().Contains(key.ToLower().Trim())).AsQueryable();
                var result = PaginatedList<ApplicationUser>.Create(data, page, pageSize);
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> AddAsync(UserModel model)
        {
            try
            {
                var findUser = await _userManager.FindByEmailAsync(model.Email);
                if(findUser != null)
                {
                    throw new MissingFieldException("Email đã tồn tại");
                }
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = model.Gender,
                    DateOfBirth = model.BirthDate,
                    PhoneNumber = model.PhoneNumber,
                    Image = model.Image,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> DeleteAsync(string email)
        {
            try
            {
                var findEmail = await _userManager.FindByEmailAsync(email);
                if (findEmail == null)
                {
                    throw new KeyNotFoundException("User không tồn tại");
                }
                var result = await _userManager.DeleteAsync(findEmail);
                if (result.Succeeded) return true;
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ApplicationUser model)
        {
            try
            {
                var findEmail = await _userManager.FindByEmailAsync(model.Email);
                if (findEmail == null)
                {
                    throw new KeyNotFoundException("User không tồn tại");
                }
                findEmail.Email = model.Email;
                findEmail.FirstName = model.FirstName;
                findEmail.LastName = model.LastName;
                findEmail.PhoneNumber = model.PhoneNumber;
                findEmail.UserName = model.Email;
                findEmail.Image = model.Image;
                findEmail.Gender = model.Gender;
                findEmail.DateOfBirth = model.DateOfBirth;

                var result = await _userManager.UpdateAsync(findEmail);
                if (result.Succeeded)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
    }
}
