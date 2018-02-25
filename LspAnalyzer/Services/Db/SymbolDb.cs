﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DataModels.Symbols;
using LinqToDB;
using LinqToDB.DataProvider;
using LspDb.Linq2sql.LinqUtils;
using File = DataModels.Symbols.File;

namespace LspAnalyzer.Services.Db
{
    public class SymbolDb
    {
        readonly IDataProvider _dbProvider;
        private readonly string _dbPath;
        private readonly string _connectionString;
        private Dictionary<int,string> _dicKind = new Dictionary<int,string>();
        public SymbolDb(string dbPath)
        {
            _dbPath = dbPath;
            _connectionString = LinqUtil.GetConnectionString(dbPath, out _dbProvider);
           

        }

        public bool Create()
        {
            // Delete Symbol database
            DeleteOldDatabase();
 

            //// accessing with LINQ to SQL
            using (var db = new DataModels.Symbols.SYMBOLDB(_dbProvider, _connectionString))

            { 
                var sp = db.DataProvider.GetSchemaProvider();
                var dbSchema = sp.GetSchema(db);

                db.BeginTransaction();
                if (dbSchema.Tables.All(t => t.TableName != "code_item_kinds"))
                {
                    db.CreateTable<CodeItemKinds>();
                }

                if (dbSchema.Tables.All(t => t.TableName != "files"))
                {
                    db.CreateTable<File>();
                }

                if (dbSchema.Tables.All(t => t.TableName != "code_items"))
                {
                    db.CreateTable<CodeItems>();
                }

                db.GetTable<CodeItemKinds>()
                    .Delete();
                db.Insert(new CodeItemKinds { Id = 1, Name = "File" });
                db.Insert(new CodeItemKinds { Id = 2, Name = "Module" });
                db.Insert(new CodeItemKinds { Id = 3, Name = "Namespace" });
                db.Insert(new CodeItemKinds { Id = 4, Name = "Package" });
                db.Insert(new CodeItemKinds { Id = 5, Name = "Class" });
                db.Insert(new CodeItemKinds { Id = 6, Name = "Method" });
                db.Insert(new CodeItemKinds { Id = 7, Name = "Property" });
                db.Insert(new CodeItemKinds { Id = 8, Name = "Field" });
                db.Insert(new CodeItemKinds { Id = 9, Name = "Constructor" });
                db.Insert(new CodeItemKinds { Id = 10, Name = "Enum" });
                db.Insert(new CodeItemKinds { Id = 11, Name = "Method" });
                db.Insert(new CodeItemKinds { Id = 12, Name = "Function" });
                db.Insert(new CodeItemKinds { Id = 13, Name = "Variable" });
                db.Insert(new CodeItemKinds { Id = 14, Name = "Constant" });
                db.Insert(new CodeItemKinds { Id = 15, Name = "String" });
                db.Insert(new CodeItemKinds { Id = 16, Name = "Number" });
                db.Insert(new CodeItemKinds { Id = 17, Name = "Boolean" });
                db.Insert(new CodeItemKinds { Id = 18, Name = "Array" });
                db.Insert(new CodeItemKinds { Id = 19, Name = "Object" });
                db.Insert(new CodeItemKinds { Id = 20, Name = "Key" });
                db.Insert(new CodeItemKinds { Id = 21, Name = "Null" });
                db.Insert(new CodeItemKinds { Id = 22, Name = "EnumMember" });
                db.Insert(new CodeItemKinds { Id = 23, Name = @"Struct" });
                db.Insert(new CodeItemKinds { Id = 24, Name = "Event" });
                db.Insert(new CodeItemKinds { Id = 25, Name = "Operator" });
                db.Insert(new CodeItemKinds { Id = 26, Name = "TypeParameter" });
                db.Insert(new CodeItemKinds { Id = 255, Name = "Macro" });

                MakeKindDictionary(db);

                db.GetTable<CodeItems>()
                    .Delete();
                db.CommitTransaction();

            }

           return true;
        }

        /// <summary>
        /// Make kind dictionary
        /// </summary>
        /// <param name="db"></param>
        private void MakeKindDictionary(SYMBOLDB db)
        {
            var lKind = from f in db.CodeItemKinds
                select new {
                    Id=  f.Id,
                    Kind = f.Name
                };
            var kind = new Dictionary<int,string>();
            foreach (var k in lKind)
            {
                _dicKind.Add((int)k.Id,
                    k.Kind);
            }
        }
        /// <summary>
        /// Load files of workspace
        /// </summary>
        /// <param name="workspace"></param>
        public void LoadFiles(string workspace)
        {
            using (SYMBOLDB db = new DataModels.Symbols.SYMBOLDB(_dbProvider, _connectionString))
            {
                // delete all
                db
                    .GetTable<File>()
                    .Delete();

                var files = from f in Directory.GetFiles(workspace, "*.*", SearchOption.AllDirectories)
                    where Path.GetExtension(f).ToLower().StartsWith(".c") ||
                          Path.GetExtension(f).ToLower().StartsWith(".h")
                    select f.ToLower().Replace(@"\" , "/");

                db.BeginTransaction();
                foreach (var file in files)
                {
                    db
                        .Insert(
                            new File
                            {
                                Name = file,
                                LeafName = Path.GetFileName(file)
                            });
                        
                }
                db.CommitTransaction();
            }
        }

        /// <summary>
        /// Load items from data table
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="dt"></param>
        public int LoadItems(string workspace, DataTable dt)
        {

            int count = 0;
            // create all files
            using (var db = new DataModels.Symbols.SYMBOLDB(_dbProvider, _connectionString))

            {

                // delete all items that are part of the input symbol table
                var itemsToDelete = (from DataRow g in dt.Rows
                    //join k in db.CodeItemKinds on g.Field<string>("Kind") equals k.Name
                    join i in db.CodeItems on g.Field<string>("Name").Trim() equals i.Name.Trim()
                    where g.Field<string>("Kind") != "File"
                    select new
                    {
                        Name = i.Name,
                        Id = i.Id
                    }).ToDataTable();

                int itemsToDeleteCount = itemsToDelete.Rows.Count; 
               // all symbols
               var items = from DataRow i in dt.Rows
                    join f in db.Files on Path.Combine(workspace.ToLower().Replace(@"\","/"), i.Field<string>("File")).ToLower().Replace(@"\","/") equals f.Name
                    join k in db.CodeItemKinds on i.Field<string>("Kind") equals k.Name 
                    where i.Field<string>("Kind") != "File"
                    select new
                    {
                        Signature = i.Field<string>("Intern"),
                        Name = i.Field<string>("Name"),
                        Kind = k.Id,
                        StartLine = i.Field<long>("StartLine"),
                        EndLine = i.Field<long>("EndLine"),
                        StartChar = i.Field<long>("StartChar"),
                        EndChar = i.Field<long>("EndChar"),
                        FileId = f.Id


                        


                    };
                db.BeginTransaction();
                foreach (var i in items)
                {
                    db.Insert<CodeItems>(new CodeItems
                    {
                        Name = i.Name,
                        Signature = i.Signature,
                        Kind = i.Kind,
                        StartLine = (int)i.StartLine,
                        StartColumn = (int)i.StartChar,
                        EndLine = (int)i.StartLine,
                        EndColumn = (int)i.StartChar,
                        FileId = i.FileId

                    });
                }
                db.CommitTransaction();
                count = items.Count();
            }

            return count;

        }

        /// <summary>
        /// Load function usage
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="dt"></param>
        public int LoadFunctionUsage(string workspace, DataTable dt)
        {

            int count = 0;
            // create all files
            using (var db = new DataModels.Symbols.SYMBOLDB(_dbProvider, _connectionString))

            {
                var functions = from f in db.CodeItems
                    select new
                    {
                        FName = f.Name
                    };



            }

            return count;

        }

        private bool DeleteOldDatabase()
        {
            if (System.IO.File.Exists(_dbPath))
            {
                try
                {
                    System.IO.File.Delete(_dbPath);
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
