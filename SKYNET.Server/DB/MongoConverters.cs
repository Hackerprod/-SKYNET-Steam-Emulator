using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace SKYNET.DB
{
	public class MongoConverters : StructSerializerBase<uint>, IRepresentationConfigurable<MongoConverters>, IRepresentationConverterConfigurable<MongoConverters>, IRepresentationConfigurable, IRepresentationConverterConfigurable
	{
		private readonly BsonType bsonType;

		private readonly RepresentationConverter representationConverter;

		public RepresentationConverter Converter => representationConverter;

		public BsonType Representation => bsonType;

		public MongoConverters()
		{
		}

		public MongoConverters(BsonType representation)
		{
		}

		public MongoConverters(BsonType representation, RepresentationConverter converter)
		{
			if ((uint)(representation - 1) > 1u && representation != BsonType.Int32 && (uint)(representation - 18) > 1u)
			{
				throw new ArgumentException($"{representation} is not a valid representation for a MyUInt32Serializer.");
			}
			bsonType = representation;
			representationConverter = converter;
		}

		public override uint Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			if (context.Reader.CurrentBsonType != BsonType.Null)
			{
				IBsonReader reader = context.Reader;
				BsonType currentBsonType = reader.GetCurrentBsonType();
				switch (currentBsonType)
				{
				case BsonType.String:
					return JsonConvert.ToUInt32(reader.ReadString() ?? "0");
				case BsonType.Double:
					return representationConverter.ToUInt32(reader.ReadDouble());
				case BsonType.Int32:
					return representationConverter.ToUInt32(reader.ReadInt32());
				case BsonType.Int64:
					return representationConverter.ToUInt32(reader.ReadInt64());
				case BsonType.Decimal128:
					return representationConverter.ToUInt32(reader.ReadDecimal128());
				case BsonType.Undefined:
				case BsonType.Null:
					return 0u;
				}
			}
			context.Reader.ReadNull();
			return 0u;
		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, uint value)
		{
			IBsonWriter writer = context.Writer;
			switch (bsonType)
			{
			case BsonType.Int32:
				writer.WriteInt32(representationConverter.ToInt32(value));
				break;
			default:
				throw new BsonSerializationException($"'{bsonType}' is not a valid UInt32 representation.");
			case BsonType.Int64:
				writer.WriteInt64(representationConverter.ToInt64(value));
				break;
			case BsonType.Decimal128:
				writer.WriteDecimal128(representationConverter.ToDecimal128(value));
				break;
			case BsonType.String:
				writer.WriteString(JsonConvert.ToString(value));
				break;
			case BsonType.Double:
				writer.WriteDouble(representationConverter.ToDouble(value));
				break;
			}
		}

		public MongoConverters WithConverter(RepresentationConverter converter)
		{
			if (converter == representationConverter)
			{
				return this;
			}
			return new MongoConverters(bsonType, converter);
		}

		public MongoConverters WithRepresentation(BsonType representation)
		{
			if (representation == bsonType)
			{
				return this;
			}
			return new MongoConverters(representation, representationConverter);
		}

		IBsonSerializer IRepresentationConverterConfigurable.WithConverter(RepresentationConverter converter)
		{
			return WithConverter(converter);
		}

		IBsonSerializer IRepresentationConfigurable.WithRepresentation(BsonType representation)
		{
			return WithRepresentation(representation);
		}

	}
}
