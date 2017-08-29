
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseModelLib
{
	public class Relation<PrimaryDataType,ForeignDataType,ValueType>:IRelation
		where ValueType:struct
	{
		private static Regex nameRegex = new Regex(@"^(.*)Table$");

		private string name;
		public string Name
		{
			get { return name; }
		}

		private DeleteReferentialAction deleteReferentialAction;
		public DeleteReferentialAction DeleteReferentialAction
		{
			get { return deleteReferentialAction; }
		}

		private Column<PrimaryDataType, ValueType> primaryColumn;
		public Column<PrimaryDataType, ValueType> PrimaryColumn
		{
			get { return primaryColumn; }
		}

		IColumn IRelation.PrimaryColumn
		{
			get { return primaryColumn; }
		}

		private Column<ForeignDataType, ValueType> foreignColumn;
		public Column<ForeignDataType, ValueType> ForeignColumn
		{
			get { return foreignColumn; }
		}

		IColumn IRelation.ForeignColumn
		{
			get { return foreignColumn; }
		}
		int IRevision.Revision
		{
			get;
			set;
		}


		public Relation(Column<PrimaryDataType,ValueType> PrimaryColumn,Column<ForeignDataType, ValueType> ForeignColumn, DeleteReferentialAction DeleteReferentialAction=DeleteReferentialAction.Delete,  [CallerMemberName]string Name = null)
		{
			Match match;

			match = nameRegex.Match(Name);
			if (match.Success) name = match.Groups[1].Value;
			else name = Name;
			this.deleteReferentialAction = DeleteReferentialAction;
			this.primaryColumn = PrimaryColumn;this.foreignColumn = ForeignColumn;
		}

	}
}
