using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Rides.AcceptRejectRide;
using Ridely.Application.Features.Rides.CancelRideRequest;
using Ridely.Application.Features.Rides.DriverSourceArrival;
using Ridely.Application.Features.Rides.EndRide;
using Ridely.Application.Features.Rides.GetChatMessages;
using Ridely.Application.Features.Rides.GetFareEstimate;
using Ridely.Application.Features.Rides.GetRide;
using Ridely.Application.Features.Rides.Reassign;
using Ridely.Application.Features.Rides.Reroute;
using Ridely.Application.Features.Rides.RideRating;
using Ridely.Application.Features.Rides.RideRequest;
using Ridely.Application.Features.Rides.Search;
using Ridely.Application.Features.Rides.StartRide;
using Ridely.Application.Features.Rides.UpdatePaymentMethod;
using Ridely.Application.Models.Shared;
using Ridely.Domain.Models;
using Ridely.Domain.Rides;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Rides;

[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Authorize]
[ResourceAuthorizationFilter]
[Route("api/v{v:apiVersion}/ride")]
public class RidesController : BaseController<RidesController>
{
    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("fare-estimate")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetFareEstimateResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetFareEstimate(RideFareEstimateRequest request)
    {
        var response = await Sender.Send(new GetFareEstimateCommand(
            new LocationRequest(request.Source.Lat, request.Source.Long),
            new LocationRequest(request.Destination.Lat, request.Destination.Long),
            request.Source.Address, request.Destination.Address,
            RiderId));

        return response.Match(value => Ok(new ApiResponse<GetFareEstimateResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("request")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<RideRequestResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> RequestRide(RideRequest rideRequest)
    {
        var response = await Sender.Send(new RideRequestCommand(rideRequest.RideId,
            rideRequest.PaymentMethod, rideRequest.MusicGenre,
            rideRequest.RideConversation, rideRequest.PaymentCardId,
            rideRequest.RideCategory, RiderId));

        return response.Match(value => Ok(new ApiResponse<RideRequestResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("reassign")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<ReassignRideResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> ReassignRide(ReassignRideRequest rideRequest)
    {
        long riderId = RiderId;

        var response = await Sender.Send(new ReassignRideCommand(rideRequest.RideId,
            new Location { Latitude = rideRequest.Source.Lat, Longitude = rideRequest.Source.Long },
            rideRequest.Source.Address, rideRequest.RerouteReason));

        return response.Match(value => Ok(new ApiResponse<ReassignRideResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("reroute")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<RerouteRideResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> ExtendRideDestination(RerouteRideRequest rideUpdate)
    {
        long riderId = RiderId;

        var response = await Sender.Send(new RerouteRideCommand(rideUpdate.RideId,
            rideUpdate.Source.Address,
            new Location { Latitude = rideUpdate.Source.Lat, Longitude = rideUpdate.Source.Long },
            rideUpdate.Destination.Address, new Location
            {
                Latitude = rideUpdate.Destination.Lat,
                Longitude = rideUpdate.Destination.Long
            }));

        return response.Match(value => Ok(new ApiResponse<RerouteRideResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("cancel")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> CancelRide(CancelRideRequest rideDto)
    {
        var response = await Sender.Send(new CancelRideRequestCommand(rideDto.RideId, rideDto.CancellationReason));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("status")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<AcceptRejectResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> AcceptRejectRide(AcceptRejectRideRequest rideDto)
    {
        var response = await Sender.Send(new AcceptRejectRideCommand(rideDto.RideId, DriverId, rideDto.AcceptRide));

        return response.Match(value => Ok(new ApiResponse<AcceptRejectResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("driver-arrival")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> DriverArrivalToSource(DriverArrivalRequest arrivalDto)
    {
        var response = await Sender.Send(new DriverSourceArrivalCommand(arrivalDto.RideId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("start")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<StartRideResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> StartRide(StartRideRequest request)
    {
        var response = await Sender.Send(new StartRideCommand(request.RideId));

        return response.Match(value => Ok(new ApiResponse<StartRideResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("end")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<EndRideResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> EndRide(EndRideRequest request)
    {
        var response = await Sender.Send(new EndRideCommand(request.RideId));

        return response.Match(value => Ok(new ApiResponse<EndRideResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("rating")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> RideRating(RideRatingRequest request)
    {
        var response = await Sender.Send(new RideRatingCommand(request.RideId, request.Ratings,
            request.Feedback));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("search")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchRideResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetRides([FromQuery] SearchRidesRequest request)
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new SearchRideQuery
        {
            DriverId = driverId,
            RiderId = riderId,
            From = request.From,
            To = request.To,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            RideStatus = [RideStatus.Completed, RideStatus.Reassigned]
        });

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchRideResponse>>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("chats/search")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<GetChatMessagesResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetRideChats([FromQuery] SearchChatRequest request)
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new GetChatMessagesQuery
        {
            RideId = request.RideId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            RiderId = riderId,
            DriverId = driverId
        });

        return response.Match(value => Ok(new ApiResponse<PaginatedList<GetChatMessagesResponse>>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<GetRideResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetRideChats(int id)
    {
        var response = await Sender.Send(new GetRideQuery(id));

        return response.Match(value => Ok(new ApiResponse<GetRideResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("payment-method")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> UpdateRidePaymentToCash(UpdatePaymentToCashRequest request)
    {
        var response = await Sender.Send(new UpdatePaymentMethodCommand(
            request.RideId, request.Method, request.PaymentCardId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }
}
