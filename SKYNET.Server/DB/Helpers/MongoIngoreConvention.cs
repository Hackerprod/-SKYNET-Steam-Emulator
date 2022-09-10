using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SKYNET.DB
{
	public class MongoIngoreConvention : IClassMapConvention, IConvention
	{
		public string Name
		{
			get;
		}

		public void Apply(BsonClassMap classMap)
		{
			classMap.SetIgnoreExtraElements(ignoreExtraElements: true);
			foreach (BsonMemberMap item in classMap.DeclaredMemberMaps.ToList())
			{
				if (item.MemberInfo.GetCustomAttributes(inherit: false).OfType<BsonIgnoreAttribute>().FirstOrDefault() != null)
				{
					classMap.UnmapMember(item.MemberInfo);
				}
			}
		}

		public MongoIngoreConvention()
		{
						
		}
	}
}
