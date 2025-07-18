﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Repositories;
internal sealed class DriverWalletRepository(ApplicationDbContext context) :
    GenericRepository<DriverWallet>(context), IDriverWalletRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<DriverWallet?> GetByDriverAsync(long driverId) =>
        await _context.Set<DriverWallet>()
        .FirstOrDefaultAsync(wallet => wallet.DriverId == driverId);
}
