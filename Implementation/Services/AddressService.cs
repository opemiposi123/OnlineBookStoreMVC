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
            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                AddittionalPhoneNumber = model.AddittionalPhoneNumber,
                City = model.City,
                Region = model.Region,
                DeliveryAddress = model.DeliveryAddress,
                IsDefault = model.IsDefault
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                AddittionalPhoneNumber = address.AddittionalPhoneNumber,
                City = address.City,
                Region = address.Region,
                DeliveryAddress = address.DeliveryAddress,
                IsDefault = address.IsDefault
            };
        }
        public async Task<AddressDto> UpdateAddressAsync(Guid addressId, AddressRequestModel model)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId);

            if (address == null)
            {
                return null; 
            }

            address.FullName = model.FullName;
            address.PhoneNumber = model.PhoneNumber;
            address.AddittionalPhoneNumber = model.AddittionalPhoneNumber;
            address.City = model.City;
            address.Region = model.Region;
            address.DeliveryAddress = model.DeliveryAddress;
            address.IsDefault = model.IsDefault;


            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return new AddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                AddittionalPhoneNumber = address.AddittionalPhoneNumber,
                City = address.City,
                Region = address.Region,
                DeliveryAddress = address.DeliveryAddress
            };
        }

        public async Task SetDefaultAddressAsync(string userId, Guid selectedAddressId)
        {
            var userAddresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();

            foreach (var address in userAddresses)
            {
                address.IsDefault = false;
            }

            var defaultAddress = userAddresses.FirstOrDefault(a => a.Id == selectedAddressId);
            if (defaultAddress != null)
            {
                defaultAddress.IsDefault = true;
            }
            await _context.SaveChangesAsync();
        }

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
                PhoneNumber = address.PhoneNumber,
                AddittionalPhoneNumber = address.AddittionalPhoneNumber,
                City = address.City,
                Region = address.Region,
                DeliveryAddress = address.DeliveryAddress
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
                PhoneNumber = a.PhoneNumber,
                AddittionalPhoneNumber = a.AddittionalPhoneNumber,
                City = a.City,
                Region = a.Region,
                DeliveryAddress = a.DeliveryAddress,
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
                PhoneNumber = address.PhoneNumber,
                AddittionalPhoneNumber = address.AddittionalPhoneNumber,
                City = address.City,
                Region = address.Region,
                DeliveryAddress = address.DeliveryAddress,
            };
        }

        //public async Task<List<AddressDto>> GetUniqueAddressesByUserIdAsync(string userId)
        //{
        //    var addresses = await _context.Addresses
        //        .Where(a => a.UserId == userId)
        //        .ToListAsync();
        //    var uniqueAddresses = addresses
        //        .GroupBy(a => new { a.FullName, a.Street, a.City, a.State, a.PostalCode, a.Country, a.PhoneNumber, a.Email })
        //        .Select(g => g.First())
        //        .ToList();

        //    return uniqueAddresses.Select(a => new AddressDto
        //    {
        //        Id = a.Id,
        //        UserId = a.UserId,
        //        FullName = a.FullName,
        //        Email = a.Email,
        //        PhoneNumber = a.PhoneNumber,
        //        Street = a.Street,
        //        City = a.City,
        //        State = a.State,
        //        PostalCode = a.PostalCode,
        //        Country = a.Country
        //    }).ToList();
        //}
    }
}
