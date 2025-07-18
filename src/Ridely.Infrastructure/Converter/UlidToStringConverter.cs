﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ridely.Infrastructure.Converter;
public class UlidToStringConverter : ValueConverter<Ulid, string>
{
    private static readonly ConverterMappingHints _defaultHints = new ConverterMappingHints(size: 26);

    public UlidToStringConverter() : this(null)
    {
        
    }

    public UlidToStringConverter(ConverterMappingHints mappingHints = null)
        : base(
            convertToProviderExpression: x => x.ToString(),
            convertFromProviderExpression: x => Ulid.Parse(x),
            mappingHints: _defaultHints.With(mappingHints)
            )
    {
        
    }
}
