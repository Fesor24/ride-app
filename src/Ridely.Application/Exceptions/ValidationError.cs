﻿namespace Ridely.Application.Exceptions;
public sealed record ValidationError(string PropertyName, string Error);
