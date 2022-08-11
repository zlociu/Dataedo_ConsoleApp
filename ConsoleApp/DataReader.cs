namespace ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DataReader
    {
        private readonly IList<DatabaseObject> _databaseObjectList;
        private readonly string _fileName;

        public DataReader(string fileName)
        {
            _fileName = fileName;
            _databaseObjectList = new List<DatabaseObject>();
        }

        private void ImportData()
        {   
            // skip first row because it contains column names
            var lines = File.ReadAllLines(_fileName).Skip(1);

            foreach(var line in lines)
            {
                // if line is empty, just skip it
                if (line != string.Empty) 
                {
                    try
                    {
                        var obj = DatabaseObject.FromString(line);
                        _databaseObjectList.Add(obj);
                    }
                    catch(Exception ex)
                    {
                        Console.Error.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void AssingChildren()
        {
            for(int i = 0; i < _databaseObjectList.Count; i++) 
            {
                var parent = _databaseObjectList[i];
                parent.NumberOfChildren = _databaseObjectList
                    .Where(child => child.ParentType == parent.Type && child.ParentName == parent.Name)
                    .Count();
            }
        }

        
        private void PrintData()
        {
            var databases = _databaseObjectList.Where(x => x.Type == DbObjectType.DATABASE);
            foreach (var database in databases)
            {
                Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");
                var tables = _databaseObjectList
                    .Where(x => x.Type == DbObjectType.TABLE && 
                        x.ParentType == DbObjectType.DATABASE && 
                        x.ParentName == database.Name);

                // print all database's tables
                foreach (var table in tables)
                {
                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");
                    var columns = _databaseObjectList
                        .Where(x => x.Type == DbObjectType.COLUMN && 
                            x.ParentType == DbObjectType.TABLE && 
                            x.ParentName == table.Name);

                    // print all table's columns
                    foreach (var column in columns)
                    {
                        Console.WriteLine(
                            $"\t\tColumn '{column.Name}' with {column.DataType} data type " +
                            $"{(column.IsNullable == true ? "accepts nulls" : "with no nulls")}");
                    }
                }
            }
        }
        
        private void PrintDataLINQ()
        {
            _databaseObjectList
                .Where(x => x.Type == DbObjectType.DATABASE)
                .ToList().ForEach(database =>
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");
                    _databaseObjectList
                      .Where(x => x.Type == DbObjectType.TABLE &&
                          x.ParentType == DbObjectType.DATABASE &&
                          x.ParentName == database.Name)
                      .ToList().ForEach(table => 
                      {
                            Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");
                            _databaseObjectList
                                .Where(x => x.Type == DbObjectType.COLUMN &&
                                    x.ParentType == DbObjectType.TABLE &&
                                    x.ParentName == table.Name)
                                .ToList().ForEach( column => 
                                {
                                    Console.WriteLine(
                                       $"\t\tColumn '{column.Name}' with {column.DataType} data type " +
                                       $"{(column.IsNullable == true ? "accepts nulls" : "with no nulls")}");
                                } );
                      } ); 
                } );
        }

        public void PrepareAndPrintData()
        {
            ImportData();
            AssingChildren();
            PrintData();
            //PrintDataLINQ(); // alternative version of printData
            Console.ReadLine();
        }

        /*
        public void ImportAndPrintData(string fileToImport, bool printData = true)
        {
            ImportedObjects = new List<ImportedObject>() { new ImportedObject() };

            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            for (int i = 0; i <= importedLines.Count; i++)
            {
                var importedLine = importedLines[i];
                var values = importedLine.Split(';');
                var importedObject = new ImportedObject();
                importedObject.Type = values[0];
                importedObject.Name = values[1];
                importedObject.Schema = values[2];
                importedObject.ParentName = values[3];
                importedObject.ParentType = values[4];
                importedObject.DataType = values[5];
                importedObject.IsNullable = values[6];
                ((List<ImportedObject>)ImportedObjects).Add(importedObject);
            }

            // clear and correct imported data
            foreach (var importedObject in ImportedObjects)
            {
                importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            }

            // assign number of children
            for (int i = 0; i < ImportedObjects.Count(); i++)
            {
                var importedObject = ImportedObjects.ToArray()[i];
                foreach (var impObj in ImportedObjects)
                {
                    if (impObj.ParentType == importedObject.Type)
                    {
                        if (impObj.ParentName == importedObject.Name)
                        {
                            importedObject.NumberOfChildren = 1 + importedObject.NumberOfChildren;
                        }
                    }
                }
            }

            foreach (var database in ImportedObjects)
            {
                if (database.Type == "DATABASE")
                {
                    Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");

                    // print all database's tables
                    foreach (var table in ImportedObjects)
                    {
                        if (table.ParentType.ToUpper() == database.Type)
                        {
                            if (table.ParentName == database.Name)
                            {
                                Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");

                                // print all table's columns
                                foreach (var column in ImportedObjects)
                                {
                                    if (column.ParentType.ToUpper() == table.Type)
                                    {
                                        if (column.ParentName == table.Name)
                                        {
                                            Console.WriteLine($"\t\tColumn '{column.Name}' with {column.DataType} data type {(column.IsNullable == "1" ? "accepts nulls" : "with no nulls")}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Console.ReadLine();
        }
        */
    }
}
