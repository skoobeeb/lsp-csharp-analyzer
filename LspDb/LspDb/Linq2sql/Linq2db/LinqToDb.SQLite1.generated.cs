//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModels.Symbols
{
	/// <summary>
	/// Database       : Symbol
	/// Data Source    : Symbol
	/// Server Version : 3.14.2
	/// </summary>
	public partial class SYMBOLDB : LinqToDB.Data.DataConnection
	{
		public ITable<CodeItemKinds> CodeItemKinds { get { return this.GetTable<CodeItemKinds>(); } }
		public ITable<CodeItems>     CodeItems     { get { return this.GetTable<CodeItems>(); } }
		public ITable<File>          Files         { get { return this.GetTable<File>(); } }

		public SYMBOLDB()
		{
			InitDataContext();
		}

		public SYMBOLDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table("code_item_kinds")]
	public partial class CodeItemKinds
	{
		[Column("id"),   PrimaryKey,  NotNull] public int   Id   { get; set; } // integer, fixed values due to LSP protocol
		[Column("name"),             NotNull] public string Name { get; set; } // text(max)
	}

	[Table("code_items")]
	public partial class CodeItems
	{
		[Column("id"),                PrimaryKey, Identity] public int   Id              { get; set; } // int
		[Column("file_id"),                        NotNull] public int   FileId          { get; set; } // int
		[Column("parent_id"),                      NotNull] public int   ParentId        { get; set; } // int
		[Column("kind"),                           NotNull] public int   Kind            { get; set; } // integer
        [Column("signature"),                      NotNull] public string Signature      { get; set; } // The signature defined by LSP
		[Column("name"),                           NotNull] public string Name            { get; set; } // text(max)
		[Column("type"),                           Nullable] public string Type            { get; set; } // text(max)
		[Column("start_column"),                   NotNull] public int   StartColumn     { get; set; } // integer
		[Column("start_line"),                     NotNull] public int   StartLine       { get; set; } // integer
		[Column("end_column"),                     NotNull] public int   EndColumn       { get; set; } // integer
		[Column("end_line"),                       NotNull] public int   EndLine         { get; set; } // integer
		[Column("name_start_column"),              NotNull] public int   NameStartColumn { get; set; } // integer
		[Column("name_start_line"),                NotNull] public int   NameStartLine   { get; set; } // integer
		[Column("name_end_column"),                NotNull] public int   NameEndColumn   { get; set; } // integer
		[Column("name_end_line"),                  NotNull] public int   NameEndLine     { get; set; } // integer
	}

	[Table("files")]
	public partial class File
	{
		[Column("id"),        PrimaryKey, Identity] public int   Id        { get; set; } // int
		[Column("timestamp"),                     ] public int   Timestamp { get; set; } // int
		[Column("name"),                  NotNull] public string Name      { get; set; } // text(max)
		[Column("leaf_name"),             NotNull] public string LeafName  { get; set; } // text(max)
	}

	public static partial class TableExtensions
	{
		public static CodeItemKinds Find(this ITable<CodeItemKinds> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static CodeItems Find(this ITable<CodeItems> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static File Find(this ITable<File> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}
