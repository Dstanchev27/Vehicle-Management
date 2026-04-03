using Microsoft.EntityFrameworkCore;
using VMAPP.Data;
using VMAPP.Data.Models;
using VMAPP.Services.DTOs;
using VMAPP.Services.Interfaces;

namespace VMAPP.Services
{
    public class VSInsurance : IVSInsuranceService
    {
        private readonly ApplicationDbContext dbContext;

        public VSInsurance(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IReadOnlyList<InsuranceCompanyDto>> GetAllAsync()
        {
            return await this.dbContext.InsuranceCompanies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new InsuranceCompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedOn = c.CreatedOn,
                    City = c.City,
                    Address = c.Address,
                    Email = c.Email,
                    Phone = c.Phone
                })
                .ToListAsync();
        }

        public async Task<InsuranceCompanyDto?> GetByIdAsync(int id)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return null;
            }

            return new InsuranceCompanyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                City = entity.City,
                Address = entity.Address,
                Email = entity.Email,
                Phone = entity.Phone
            };
        }

        public async Task<int> CreateAsync(InsuranceCompanyDto dto)
        {
            var createdById = dto.CreatedById;

            if (string.IsNullOrEmpty(createdById))
            {
                createdById = await this.dbContext.Users
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
            }

            if (createdById == null)
            {
                throw new InvalidOperationException("No user found to assign as creator.");
            }

            var entity = new InsuranceCompany
            {
                Name = dto.Name,
                Description = dto.Description,
                City = dto.City,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                CreatedOn = dto.CreatedOn == default ? DateTime.UtcNow : dto.CreatedOn,
                CreatedById = createdById
            };

            await this.dbContext.InsuranceCompanies.AddAsync(entity);
            await this.dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<bool> UpdateAsync(InsuranceCompanyDto dto)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .FirstOrDefaultAsync(c => c.Id == dto.Id);

            if (entity == null)
            {
                return false;
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.City = dto.City;
            entity.Address = dto.Address;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.ModifiedOn = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(dto.ModifiedById))
            {
                entity.ModifiedById = dto.ModifiedById;
            }

            this.dbContext.InsuranceCompanies.Update(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await this.dbContext.InsuranceCompanies
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
            {
                return false;
            }

            this.dbContext.InsuranceCompanies.Remove(entity);
            await this.dbContext.SaveChangesAsync();

            return true;
        }
    }
}

