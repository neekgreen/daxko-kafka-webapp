﻿namespace WebApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IEntity
    {
        ICollection<IDomainEvent> Events { get; }
    }
}