using Microsoft.EntityFrameworkCore;
using Ridely.Application.Helper;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Rides;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Repositories;
internal sealed class RideRepository(ApplicationDbContext context) :
    GenericRepository<Ride>(context), IRideRepository
{
    private readonly ApplicationDbContext _context = context;

    public override async Task<Ride?> GetAsync(long id)
    {
        return await _context.Set<Ride>()
            .Where(ride => ride.Id == id)
            .Include(ride => ride.Payment)
            .FirstOrDefaultAsync();
    }

    public async Task<Ride?> GetLatestDriverRideDetails(long driverId) =>
        await _context.Set<Ride>()
        .Where(x => x.Status == RideStatus.Matched || x.Status == RideStatus.Started)
        .Include(x => x.Payment)
        .Include(x => x.Rider)
        .Include(x => x.Driver)
        .ThenInclude(y => y.Cab)
        .OrderByDescending(x => x.CreatedAtUtc)
        .FirstOrDefaultAsync(x => x.DriverId == driverId);

    public async Task<Ride?> GetRideDetails(long rideId) =>
        await _context.Set<Ride>()
        .Include(x => x.Payment)
        .Include(x => x.Rider)
        .Include(x => x.Driver)
            .ThenInclude(y => y.Cab)
        .FirstOrDefaultAsync(x => x.Id == rideId);

    public async Task<int> GetTotalCountAsync(RideStatus status) =>
        await _context.Set<Ride>()
            .Where(ride => ride.Status == status)
            .CountAsync();

    public async Task<PaginatedList<RideModel>> Search(RideSearchParams searchParams)
    {
        var query = _context.Set<Ride>()
            .Include(x => x.Payment)
            .Include(ride => ride.Rider)
            .Include(ride => ride.Driver)
            .AsQueryable();

        if (searchParams.DriverId.HasValue)
            query = query.Where(x => x.DriverId == searchParams.DriverId.Value);

        if (searchParams.RiderId.HasValue)
            query = query.Where(x => x.RiderId == searchParams.RiderId.Value);

        if (!string.IsNullOrWhiteSpace(searchParams.DriverPhoneNo))
            query = query.Where(ride => ride.Driver != null && 
            ride.Driver.PhoneNo == DataFormatter.FormatPhoneNo(searchParams.DriverPhoneNo));

        if (!string.IsNullOrWhiteSpace(searchParams.RiderPhoneNo))
            query = query.Where(ride => ride.Rider.PhoneNo == DataFormatter.FormatPhoneNo(searchParams.RiderPhoneNo));

        if (searchParams.From.HasValue)
            query = query.Where(x => x.CreatedAtUtc.Date >= searchParams.From.Value);

        if (searchParams.To.HasValue)
            query = query.Where(x => x.CreatedAtUtc.Date <= searchParams.To.Value);

        if (searchParams.RideStatus.Count > 0)
            query = query.Where(x => searchParams.RideStatus.Any(s => s == x.Status));

        int totalItems = await query.CountAsync();

        var queryProjection = query.Select(x => new RideModel
        {
            Id = x.Id,
            Amount = x.EstimatedFare,
            PaymentMethod = x.Payment.Method,
            PaymentStatus = x.Payment.Status,
            CreatedAt = x.CreatedAtUtc,
            Destination = x.DestinationAddress,
            Source = x.SourceAddress,
            Rider = x.Rider.FirstName + " " + x.Rider.LastName,
            Driver = x.Driver.FirstName  + " " + x.Driver.LastName ?? string.Empty,
        }).OrderByDescending(x => x.CreatedAt);

        int take = searchParams.PageSize;

        int skip = (searchParams.PageNumber - 1) * take;

        var records = await queryProjection
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PaginatedList<RideModel>
        {
            Items = records,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalItems = totalItems
        };
    }
}
