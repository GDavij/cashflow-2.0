﻿using Cashflow.Core;

namespace Cashflow.Domain.Entities;

public class Role : ValueObject<short>
{
    public string Name { get; init; }
}