using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;

public class ADBAccessor
{
	IMongoDatabase db;
	public ADBAccessor(string connect, string dbName)
	{
		db = ADBManager.Instance.GetDB(connect, dbName);
	}
	public static FilterDefinition<BsonDocument> filter_empty()
	{
		return Builders<BsonDocument>.Filter.Empty;
	}
	public static FilterDefinition<BsonDocument> filter_contains(string param)
	{
		return Builders<BsonDocument>.Filter.Exists(param);
	}
	public static FilterDefinition<BsonDocument> filter_eq<T>(string param, T value)
	{
		return Builders<BsonDocument>.Filter.Eq(param, value);
	}
	public static FilterDefinition<BsonDocument> filter_Gt<T>(string param, T value)
	{
		return Builders<BsonDocument>.Filter.Gt(param, value);
	}
	public static FilterDefinition<BsonDocument> filter_Gte<T>(string param, T value)
	{
		return Builders<BsonDocument>.Filter.Gte(param, value);
	}
	public static FilterDefinition<BsonDocument> filter_Lt<T>(string param, T value)
	{
		return Builders<BsonDocument>.Filter.Lt(param, value);
	}
	public static FilterDefinition<BsonDocument> filter_Lte<T>(string param, T value)
	{
		return Builders<BsonDocument>.Filter.Lte(param, value);
	}
	/// <summary>
	/// 降序
	/// </summary>
	/// <param name="param">降序属性</param>
	/// <returns></returns>
	public static SortDefinition<BsonDocument> sort_Descending(params string[] param)
	{
		SortDefinition<BsonDocument>[] sorts = new SortDefinition<BsonDocument>[param.Length];
		for (var i = 0; i < param.Length; i++)
			sorts[i] = Builders<BsonDocument>.Sort.Descending(param[i]);
		return Builders<BsonDocument>.Sort.Combine(sorts);
	}
	/// <summary>
	/// 升序
	/// </summary>
	/// <param name="param">升序属性</param>
	/// <returns></returns>
	public static SortDefinition<BsonDocument> sort_Ascending(params string[] param)
	{
		SortDefinition<BsonDocument>[] sorts = new SortDefinition<BsonDocument>[param.Length];
		for (var i = 0; i < param.Length; i++)
			sorts[i] = Builders<BsonDocument>.Sort.Ascending(param[i]);
		return Builders<BsonDocument>.Sort.Combine(sorts);
	}

	public static UpdateDefinition<BsonDocument> updates_build(params UpdateDefinition<BsonDocument>[] updates)
	{
		var updateBuilder = Builders<BsonDocument>.Update;
		var u = new List<UpdateDefinition<BsonDocument>>();
		foreach (var up in updates)
		{
			u.Add(up);
		}
		return updateBuilder.Combine(u);
	}
	public static UpdateDefinition<BsonDocument> update<T>(string param, T value)
	{
		return Builders<BsonDocument>.Update.Set(param, value);
	}
	public static UpdateDefinition<BsonDocument> update_unset(string param)
	{
		return Builders<BsonDocument>.Update.Unset(param);
	}
	public static ProjectionDefinition<BsonDocument> projections(params string[] sparams)
	{
		ProjectionDefinition<BsonDocument> p = null;
		foreach (var s in sparams)
		{
			if (p == null)
			{
				p = Builders<BsonDocument>.Projection.Include(s);
			}
			else
			{
				p = Builders<BsonDocument>.Projection.Combine(Builders<BsonDocument>.Projection.Include(s), p);
			}
		}
		return p;
	}
	public bool InsertOneData(string collectionName
		, BsonDocument document)
	{
		bool bSuccess = true;
		string serror = "";
		try
		{
			var col = db.GetCollection<BsonDocument>(collectionName);
			col.InsertOne(document);
		}
		catch (Exception ex)
		{
			bSuccess = false;
			serror = ex.Message;
		}
		return bSuccess;
	}
	public bool DeleteOneData(string collectionName
		, FilterDefinition<BsonDocument> filter
		)
	{
		bool bSuccess = true;
		string serror = "";
		try
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			var result = collection.DeleteOne(filter);
		}
		catch (Exception ex)
		{
			bSuccess = false;
			serror = ex.Message;
		}
		return bSuccess;
	}
	public bool UpdateOneData(string collectionName
		, FilterDefinition<BsonDocument> filter
		, UpdateDefinition<BsonDocument> update
		, bool upsert = false)
	{
		bool bSuccess = true;
		string serror = "";
		try
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			UpdateOptions uo = new UpdateOptions();
			uo.IsUpsert = upsert;
			if (filter == null)
			{
				filter = FilterDefinition<BsonDocument>.Empty;
			}
			var result = collection.UpdateOne(filter, update, uo);
		}
		catch (Exception ex)
		{
			bSuccess = false;
			serror = ex.Message;
		}
		return bSuccess;
	}
	public long UpdateManyData(string collectionName
		, FilterDefinition<BsonDocument> filter
		, UpdateDefinition<BsonDocument> update
		, bool upsert)
	{
		bool bSuccess = true;
		string serror = "";
		long modifiedNum = 0;
		try
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			UpdateOptions uo = new UpdateOptions();
			uo.IsUpsert = upsert;
			var result = collection.UpdateMany(filter, update, uo);
			modifiedNum = result.ModifiedCount;
		}
		catch (Exception ex)
		{
			bSuccess = false;
			serror = ex.Message;
		}
		return modifiedNum;
	}
	public BsonDocument FindOneData(string collectionName
		, FilterDefinition<BsonDocument> filter = null
		, ProjectionDefinition<BsonDocument> projection = null)
	{
		bool bSuccess = true;
		string serror = "";
		BsonDocument bsonElement = null;
		try
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			if (filter == null)
			{
				filter = FilterDefinition<BsonDocument>.Empty;
			}
			var doc = collection.Find(filter);
			if (projection != null)
			{
				doc = doc.Project(projection);
			}
			bsonElement = doc.FirstOrDefault();
		}
		catch (Exception ex)
		{
			bSuccess = false;
			serror = ex.Message;
		}
		return bsonElement;
	}
	public long Count(string collectionName
		, FilterDefinition<BsonDocument> filter = null
		)
	{
		var collection = db.GetCollection<BsonDocument>(collectionName);
		if (filter == null)
		{
			filter = FilterDefinition<BsonDocument>.Empty;
		}
		return collection.Count(filter);
	}
	public List<BsonDocument> FindManyData(string collectionName
			, FilterDefinition<BsonDocument> filter = null
			, ProjectionDefinition<BsonDocument> projection = null
			, int limit = 0, int skip = 0
			, SortDefinition<BsonDocument> sort = null)
	{
		string serror = "";
		List<BsonDocument> lBsonElement = new List<BsonDocument>();
		try
		{
			var collection = db.GetCollection<BsonDocument>(collectionName);
			if (filter == null)
			{
				filter = FilterDefinition<BsonDocument>.Empty;
			}
			var doc = collection.Find(filter);
			if (sort != null)
			{
				doc = doc.Sort(sort);
			}
			if (projection != null)
			{
				doc = doc.Project(projection);
			}
			if (limit > 0)
			{
				doc = doc.Limit(limit);
			}
			if (skip > 0)
			{
				doc = doc.Skip(skip);
			}
			var cursor = doc.ToCursor();
			foreach (var d in cursor.ToEnumerable())
			{
				lBsonElement.Add(d);
			}
		}
		catch (Exception ex)
		{
			serror = ex.Message;
		}
		return lBsonElement;
	}
}
