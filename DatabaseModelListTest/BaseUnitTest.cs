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

		protected DbType CreateDatabase()
		{
			DbType database;

			database = new DbType();
			return database;
		}
		protected async Task<DataType> AssertSelectAsync<DataType, PrimaryKeyType>(bool SuccessExpected,PrimaryKeyType PrimaryKey)
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
		protected async Task AssertInsertAsync<DataType, PrimaryKeyType>(bool SuccessExpected, DataType Item)
			where DataType : new()
		{
			PrimaryKeyType key;
			DataType other;

			try
			{
				await db.InsertAsync<DataType>(Item);
			}
			catch (Exception ex)
			{
				if (SuccessExpected) Assert.Fail($"Insert command of item type {typeof(DataType).Name} failed with exception {ex.Message}");
				return;
			}
			key = (PrimaryKeyType)Schema<DataType>.PrimaryKey.GetValue(Item);

			Assert.AreNotEqual(default(PrimaryKeyType), key, $"Insert command of item type {typeof(DataType).Name} failed, primary key not updated");

			other = await AssertSelectAsync<DataType, PrimaryKeyType>(true,key);
			if (other == null) Assert.Fail($"Insert command of item type {typeof(DataType).Name} failed, cannot find item in database");
			Assert.IsTrue(Schema<DataType>.AreEquals(Item, other), $"Insert command of item type {typeof(DataType).Name} failed, returned result is not identical");
		}
		protected async Task AssertUpdateAsync<DataType, PrimaryKeyType>(bool SuccessExpected, DataType Item)
			where DataType : new()
		{
			PrimaryKeyType key;
			DataType other;

			try
			{
				await db.UpdateAsync<DataType>(Item);
			}
			catch (Exception ex)
			{
				if (SuccessExpected) Assert.Fail($"Update command of item type {typeof(DataType).Name} failed with exception {ex.Message}");
				return;
			}

			key = (PrimaryKeyType)Schema<DataType>.PrimaryKey.GetValue(Item);

			other = await AssertSelectAsync<DataType, PrimaryKeyType>(true, key);
			if (other == null) Assert.Fail($"Update command of item type {typeof(DataType).Name} failed, cannot find item in database");
			Assert.IsTrue(Schema<DataType>.AreEquals(Item, other), $"Update command of item type {typeof(DataType).Name} failed, returned result is not identical");

		}
		protected async Task AssertDeleteAsync<DataType, PrimaryKeyType>(bool SuccessExpected, DataType Item)
			where DataType : new()
		{
			PrimaryKeyType key;
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

			key = (PrimaryKeyType)Schema<DataType>.PrimaryKey.GetValue(Item);

			other = await AssertSelectAsync<DataType, PrimaryKeyType>(false, key);
			if (other != null) Assert.Fail($"Delete command of item type {typeof(DataType).Name} failed, item found in database");
		}

		protected async Task AssertCRUDAsync<DataType,PrimaryKeyType>(DataType Item,params Action<DataType>[] UpdateActions)
			where DataType:new()
		{
			await AssertInsertAsync<DataType, PrimaryKeyType>(true, Item);
			foreach (Action<DataType> action in UpdateActions)
			{
				action(Item);
				await AssertUpdateAsync<DataType, PrimaryKeyType>(true, Item);
			}
			await AssertDeleteAsync<DataType, PrimaryKeyType>(true, Item);
		}


	}
}
