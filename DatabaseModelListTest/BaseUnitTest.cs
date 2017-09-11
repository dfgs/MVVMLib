using DatabaseModelLib;
using DatabaseModelLib.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelLibTest
{
	public abstract class BaseUnitTest<DbType>
		where DbType:IDatabase, new()
	{
		private static DbType db = new DbType();
		public static DbType Database
		{
			get { return db; }
		}

		protected DbType CreateDatabase()
		{
			DbType database;

			database = new DbType();
			return database;
		}
		protected async Task<DataType> AssertSelectAsync<DataType>(bool SuccessExpected,object PrimaryKey)
			where DataType : new()
		{
			IEnumerable<DataType> items;
			DataType result;
			
			try
			{
				items = await db.SelectAsync<DataType>(new EqualFilter<DataType>(Schema<DataType>.PrimaryKey, PrimaryKey));
			}
			catch (Exception ex)
			{
				if (SuccessExpected) Assert.Fail($"Select command of item type {typeof(DataType).Name} failed with exception {ex.Message}");
				return default(DataType);
			}
			result=items.FirstOrDefault();
			if (SuccessExpected)
			{
				if (result == null) Assert.Fail($"Select command of item type {typeof(DataType).Name} failed, no result returned");
			}
			else
			{
				if (result != null) Assert.Fail($"Select command of item type {typeof(DataType).Name} failed, a result was returned");
			}
			return result;
		}
		protected async Task AssertInsertAsync<DataType>(bool SuccessExpected, DataType Item)
			where DataType : new()
		{
			object originalKey,key;
			DataType other;

			originalKey = Schema<DataType>.PrimaryKey.GetValue(Item);
			try
			{
				await db.InsertAsync<DataType>(Item);
			}
			catch (Exception ex)
			{
				if (SuccessExpected) Assert.Fail($"Insert command of item type {typeof(DataType).Name} failed with exception {ex.Message}");
				return;
			}
			key = Schema<DataType>.PrimaryKey.GetValue(Item);

			Assert.AreNotEqual(originalKey, key, $"Insert command of item type {typeof(DataType).Name} failed, primary key not updated");

			other = await AssertSelectAsync<DataType>(true,key);
			if (other == null) Assert.Fail($"Insert command of item type {typeof(DataType).Name} failed, cannot find item in database");
			Assert.IsTrue(Schema<DataType>.AreEquals(Item, other), $"Insert command of item type {typeof(DataType).Name} failed, returned result is not identical");
		}
		protected async Task AssertUpdateAsync<DataType>(bool SuccessExpected, DataType Item,Action<DataType> UpdateAction)
			where DataType : new()
		{
			object key;
			DataType other;
			DataType clone;

			key = Schema<DataType>.PrimaryKey.GetValue(Item);

			clone = new DataType();
			Schema<DataType>.Clone(Item, clone);
			Schema<DataType>.PrimaryKey.SetValue(clone, key);
			UpdateAction(clone);

			try
			{
				await db.UpdateAsync<DataType>(clone);
			}
			catch (Exception ex)
			{
				if (SuccessExpected) Assert.Fail($"Update command of item type {typeof(DataType).Name} failed with exception {ex.Message}");
				return;
			}


			other = await AssertSelectAsync<DataType>(true, key);
			if (other == null) Assert.Fail($"Update command of item type {typeof(DataType).Name} failed, cannot find item in database");
			Assert.IsTrue(Schema<DataType>.AreEquals(clone, other), $"Update command of item type {typeof(DataType).Name} failed, returned result is not identical");

		}
		protected async Task AssertDeleteAsync<DataType>(bool SuccessExpected, DataType Item)
			where DataType : new()
		{
			object key;
			DataType other;

			try
			{
				await db.DeleteAsync<DataType>(Item);
			}
			catch (Exception ex)
			{
				if (SuccessExpected) Assert.Fail($"Delete command of item type {typeof(DataType).Name} failed with exception {ex.Message}");
				return;
			}

			key = Schema<DataType>.PrimaryKey.GetValue(Item);

			other = await AssertSelectAsync<DataType>(false, key);
			if (other != null) Assert.Fail($"Delete command of item type {typeof(DataType).Name} failed, item found in database");
		}

		protected async Task AssertCRUDAsync<DataType>(DataType Item,params Action<DataType>[] UpdateActions)
			where DataType:new()
		{
			await AssertInsertAsync<DataType>(true, Item);
			foreach (Action<DataType> action in UpdateActions)
			{
				await AssertUpdateAsync<DataType>(true, Item,action);
			}
			await AssertDeleteAsync<DataType>(true, Item);
		}


	}
}
