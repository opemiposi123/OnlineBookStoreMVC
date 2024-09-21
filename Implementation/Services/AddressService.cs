using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AddressDto> AddAddressAsync(AddressRequestModel model, string userId)
        {
            var existingAddress = await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingAddress != null)
            {
                existingAddress.FullName = model.FullName;
                existingAddress.Email = model.Email;
                existingAddress.PhoneNumber = model.PhoneNumber;
                existingAddress.Street = model.Street;
                existingAddress.City = model.City;
                existingAddress.State = model.State;
                existingAddress.PostalCode = model.PostalCode;
                existingAddress.Country = model.Country;
                existingAddress.UpdatedAt = DateTime.UtcNow;

                _context.Update(existingAddress);
            }
            else
            {
                var address = new Address
                {
                    UserId = userId,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    PostalCode = model.PostalCode,
                    Country = model.Country
                };

                _context.Addresses.Add(address);
            }

            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = existingAddress?.Id ?? address.Id,
                UserId = userId,
                FullName = existingAddress?.FullName ?? address.FullName,
                Email = existingAddress?.Email ?? address.Email,
                PhoneNumber = existingAddress?.PhoneNumber ?? address.PhoneNumber,
                Street = existingAddress?.Street ?? address.Street,
                City = existingAddress?.City ?? address.City,
                State = existingAddress?.State ?? address.State,
                PostalCode = existingAddress?.PostalCode ?? address.PostalCode,
                Country = existingAddress?.Country ?? address.Country
            };
        }

        //public async Task<AddressDto> AddAddressAsync(AddressRequestModel model, string userId)
        //{
        //    var address = new Address
        //    {
        //        Id = Guid.NewGuid(),
        //        UserId = userId,
        //        FullName = model.FullName,
        //        Email = model.Email,
        //        PhoneNumber = model.PhoneNumber,
        //        Street = model.Street,
        //        City = model.City,
        //        State = model.State,
        //        PostalCode = model.PostalCode,
        //        Country = model.Country
        //    };

        //    _context.Addresses.Add(address);
        //    await _context.SaveChangesAsync();

        //    return new AddressDto
        //    {
        //        Id = address.Id,
        //        UserId = address.UserId,
        //        FullName = address.FullName,
        //        Email = address.Email,
        //        PhoneNumber = address.PhoneNumber,
        //        Street = address.Street,
        //        City = address.City,
        //        State = address.State,
        //        PostalCode = address.PostalCode,
        //        Country = address.Country
        //    };
        //}


        public async Task<AddressDto> GetAddressByUserIdAsync(string userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (address == null) return null;

            return new AddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                FullName = address.FullName,
                Email = address.Email,
                PhoneNumber = address.PhoneNumber,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
        }

        public async Task<IEnumerable<AddressDto>> GetAllAddressesByUserIdAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            }).ToList();
        }

        public async Task<AddressDto> GetAddressByIdAsync(Guid addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId);

            if (address == null)
            {
                return null;
            }

            return new AddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                FullName = address.FullName,
                Email = address.Email,
                PhoneNumber = address.PhoneNumber,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
        }

        public async Task<List<AddressDto>> GetUniqueAddressesByUserIdAsync(string userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
            var uniqueAddresses = addresses
                .GroupBy(a => new { a.FullName, a.Street, a.City, a.State, a.PostalCode, a.Country, a.PhoneNumber, a.Email })
                .Select(g => g.First())
                .ToList();

            return uniqueAddresses.Select(a => new AddressDto
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            }).ToList();
        }


    }
}
