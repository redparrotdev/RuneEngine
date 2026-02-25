using RuneEngine.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuneEngine.Converters;

public sealed class InputBindingJsonConverter : JsonConverter<InputBinding>
{
    public override InputBinding? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tokenType = reader.TokenType;
        if (tokenType is JsonTokenType.StartArray)
        {
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.String)
                    list.Add(reader.GetString()!);
            }
            
            if (list.Count == 2)
                return new ConnectionBinding(list[0], list[1]);
            
            throw new JsonException("Array must contain exactly 2 strings for ConnectionBinding");
        }
        else
        {
            object? value = tokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.TryGetInt32(out var intValue) ? intValue : reader.GetDecimal(),
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Null => null,
                _ => JsonSerializer.Deserialize<object>(ref reader, options)
            };
            return new ConstantBinding(value);
        }
    }

    public override void Write(Utf8JsonWriter writer, InputBinding value, JsonSerializerOptions options)
    {
        if (value is ConnectionBinding connection)
        {
            writer.WriteStartArray();
            writer.WriteStringValue(connection.RuneId);
            writer.WriteStringValue(connection.OutputName);
            writer.WriteEndArray();
        }
        else if (value is ConstantBinding constant)
        {
            var constantValue = constant.Value;
            if (constantValue is null)
                writer.WriteNullValue();
            else if (constantValue is string str)
                writer.WriteStringValue(str);
            else if (constantValue is bool b)
                writer.WriteBooleanValue(b);
            else if (constantValue is long l)
                writer.WriteNumberValue(l);
            else if (constantValue is int i)
                writer.WriteNumberValue(i);
            else if (constantValue is double d)
                writer.WriteNumberValue(d);
            else if (constantValue is float f)
                writer.WriteNumberValue(f);
            else
                JsonSerializer.Serialize(writer, constantValue, options);
        }
        else
        {
            throw new NotSupportedException($"Unknown InputBinding type: {value.GetType()}");
        }
    }
}
