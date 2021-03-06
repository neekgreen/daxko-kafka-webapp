﻿using System;
using System.Linq;
using System.Net.Http;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Http;
using WebApp.Models;

namespace WebApp.Api.LocationCheckins
{
    public class CheckinCommandHandler : IRequestHandler<CheckinCommand, CheckinResponse>
    {
        private readonly AppDbContext context;
        private readonly IMediator mediator;

        public CheckinCommandHandler(AppDbContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public CheckinResponse Handle(CheckinCommand message)
        {
            var member = context.Members.FirstOrDefault(x => x.MemberId == message.memberId);
            var location = context.Locations.FirstOrDefault(x => x.LocationId == message.locationId);
            
            if (member == null)
                throw new ArgumentException("Unable to find member");

            if (location == null)
                throw new ArgumentException("Unable to find location");
            
            var checkin = new LocationCheckin(new LocationCheckinUpdate
            {
                Member = member,
                Location = location,
                CheckinCompleted = DateTime.UtcNow
            });
            
            context.LocationCheckin.AddRange(checkin);

            foreach (var notification in checkin.Events.OfType<INotification>())
                mediator.Publish(notification);
                
            context.SaveChanges();

            return new CheckinResponse
            {
                firstName = member.FirstName,
                lastName = member.LastName,
                locationName = location.LocationName
            };
        }
    }
}