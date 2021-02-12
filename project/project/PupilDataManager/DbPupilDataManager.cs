﻿using ADOX;
using project.Interfaces;
using project.PupilDataManager.SharedResources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static project.PupilDataManager.SharedResources.Types;

namespace project.PupilDataManager {
    class DbPupilDataManager : BasePupilDataManager {
        public static readonly string VERSION = "0.1.3.6";
        public static readonly int BUILD = 6;
        private static readonly string DEFAULT_DATABASE_LOCATION = Environment.GetEnvironmentVariable("LocalAppData") + "\\PupilRecordsProgram\\Databases";
        private static readonly string RELATIVE_PUPIL_PICTURES_LOCATION = "\\Pictures";
        private static readonly string DATABASE_NAME = "PleaseDontDeleteThis";
        private static readonly string CONNECTION_STRING_TEMPLATE = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = ";
        private static readonly string[] PUPIL_TABLE_COLUMN_NAMES = new string[]{"PupilUUID", "PupilID", "Name", "Company", "A2E", "ImgRef"};
        public string DatabasePath;
        public DbPupilDataManager(string p_DatabasePath) : base(p_DatabasePath) {
            DatabasePath = p_DatabasePath;
        }

        public DbPupilDataManager() : base(DEFAULT_DATABASE_LOCATION) {
            DatabasePath = DEFAULT_DATABASE_LOCATION;
        }
        public override bool CheckIfInstalled(string Location) {
            this.DatabasePath = Location;
            if (Directory.Exists(Location)) {
                try {
                    Catalog v_Catalog = new Catalog();
                    v_Catalog.let_ActiveConnection(CONNECTION_STRING_TEMPLATE + Location + "\\" + DATABASE_NAME);
                    //TODO: Check database integrity.
                    v_Catalog = null;
                } catch (Exception) {
                    return false;
                }
                return true;
            }
            return false;
        }
        public override void Install(string Location) {
            if (!Directory.Exists(Location)) Directory.CreateDirectory(Location);
            if (!Directory.Exists(Location + RELATIVE_PUPIL_PICTURES_LOCATION)) Directory.CreateDirectory(Location + RELATIVE_PUPIL_PICTURES_LOCATION);

            Action<Table, ValueTuple<string, ADOX.DataTypeEnum, int>[]> AddColumns = delegate(Table CurrentTable, ValueTuple<string, ADOX.DataTypeEnum, int>[] ColumnProperties){
                foreach (var Column in ColumnProperties){
                    if(Column.Item3 == -1) CurrentTable.Columns.Append(Column.Item1, Column.Item2);
                    else CurrentTable.Columns.Append(Column.Item1, Column.Item2, Column.Item3);
                }
            };

            Catalog v_Catalog = new Catalog();
            v_Catalog.Create(CONNECTION_STRING_TEMPLATE + Location + "\\" + DATABASE_NAME + "; Jet OLEDB:Engine Type=5");
            Table PupilsTable = new Table();
            PupilsTable.Name = "Pupil";
            
            AddColumns(PupilsTable, new ValueTuple<string, ADOX.DataTypeEnum, int>[]{
                ("PupilUUID", ADOX.DataTypeEnum.adVarWChar, 36),
                ("PupilID", ADOX.DataTypeEnum.adVarWChar, 16),
                ("Name", ADOX.DataTypeEnum.adVarWChar, 127),
                ("Company", ADOX.DataTypeEnum.adVarWChar, 160),
                ("A2E", ADOX.DataTypeEnum.adBoolean, -1),
                ("ImgRef", ADOX.DataTypeEnum.adVarWChar, 255)
            });

            PupilsTable.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, "PupilUUID", null, null);
            v_Catalog.Tables.Append(PupilsTable);

            Table NotesTable = new Table();
            NotesTable.Name = "Note";
            
            AddColumns(NotesTable, new ValueTuple<string, ADOX.DataTypeEnum, int>[]{
                ("PupilUUID", ADOX.DataTypeEnum.adVarWChar, 36),
                ("Text", ADOX.DataTypeEnum.adLongVarWChar, 511),
                ("Date", ADOX.DataTypeEnum.adVarWChar, 22),
                ("UUID", ADOX.DataTypeEnum.adVarWChar, 36)
            });

            NotesTable.Keys.Append("ForeignKey", KeyTypeEnum.adKeyForeign, "PupilUUID", "Pupil", "PupilUUID");
            NotesTable.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, "UUID", null, null);
            v_Catalog.Tables.Append(NotesTable);

            Table MetadataTable = new Table();
            MetadataTable.Name = "Metadata";

            AddColumns(MetadataTable, new ValueTuple<string, ADOX.DataTypeEnum, int>[]{
                ("Version", ADOX.DataTypeEnum.adVarWChar, 16),
                ("Build", ADOX.DataTypeEnum.adVarWChar, -1),
                ("DateCreated", ADOX.DataTypeEnum.adVarWChar, 22)
            });
            v_Catalog.Tables.Append(MetadataTable);

            v_Catalog = null;

            OleDbConnection Connection = new OleDbConnection(CONNECTION_STRING_TEMPLATE + Location + "\\" + DATABASE_NAME);

            OleDbCommand InsertCommand = new OleDbCommand();
            InsertCommand.CommandType = CommandType.Text;
            InsertCommand.CommandText = "INSERT INTO [Metadata] ([Version], [Build], [DateCreated]) VALUES (@Version, @Build, @DateCreated)";
            InsertCommand.Parameters.AddWithValue("@Version", VERSION);
            InsertCommand.Parameters.AddWithValue("@Build", BUILD);
            InsertCommand.Parameters.AddWithValue("@DateCreated", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.s"));
            InsertCommand.Connection = Connection;

            Connection.Open();
            InsertCommand.ExecuteNonQuery();
            Connection.Close();
        }

        public override List<T> CollatePropertyValuesFromPupils<T>(string Property) {
            OleDbConnection Connection = new OleDbConnection(CONNECTION_STRING_TEMPLATE + this.DatabasePath + "\\" + DATABASE_NAME);

            OleDbCommand SelectCommand = new OleDbCommand();
            SelectCommand.CommandType = CommandType.Text;
            SelectCommand.Connection = Connection;
            //SelectCommand.CommandText = "SELECT @Property FROM [Pupil];";
            //SelectCommand.Parameters.AddWithValue("@Property", Property);
            SelectCommand.CommandText = "SELECT * FROM [Pupil];";

            List<T> PropertyValues = new List<T>();

            int Index = Array.IndexOf(PUPIL_TABLE_COLUMN_NAMES, Property);
            if(Index == -1) return PropertyValues;

            Connection.Open();
            OleDbDataReader DataReader = SelectCommand.ExecuteReader();

            while(DataReader.Read()) if(!PropertyValues.Contains((T)DataReader[Index])) PropertyValues.Add((T)DataReader[Index]);

            Connection.Close();
            return PropertyValues;
        }

        public override bool ConfirmPupilIDIntegrity() {
            List<Pupil> Pupils = this.GetPupilsByProperties(new {});
            List<string> PupilIDs = new List<string>();
            foreach(Pupil i_Pupil in Pupils){
                if(PupilIDs.Contains(i_Pupil.PupilID)) return false;
                PupilIDs.Add(i_Pupil.PupilID);
            }
            return true;
        }
        private static readonly string[] CHECK_NESTED_MATCH_PROPERTY_BLACKLIST = { "Length", "Chars" };
        public override List<Pupil> GetPupilsByProperties(object Properties) {
            List<(string, object)> StringProperties = new List<(string, object)>();
            foreach (PropertyInfo Property in Properties.GetType().GetProperties()) {
                if (CHECK_NESTED_MATCH_PROPERTY_BLACKLIST.Contains(Property.Name)) continue;
                dynamic PatternValue = Properties.GetType().GetProperty(Property.Name).GetValue(Properties, null);
                if (PatternValue == null) continue;
                StringProperties.Add((Property.Name, PatternValue));
            }

            OleDbConnection Connection = new OleDbConnection(CONNECTION_STRING_TEMPLATE + this.DatabasePath + "\\" + DATABASE_NAME);

            OleDbCommand SelectCommand = new OleDbCommand();
            SelectCommand.CommandType = CommandType.Text;
            SelectCommand.Connection = Connection;
            string CommandText = "SELECT * FROM [Pupil]";

            for (int i = 0; i < StringProperties.Count; i++){
                (string, object) StringProperty = StringProperties[i];

                if(i == 0) CommandText += " WHERE ";
                else CommandText += " AND ";
                CommandText += StringProperty.Item1 + " = @Value" + i;
            }
            CommandText += ";";
            SelectCommand.CommandText = CommandText;

            for(int i = 0; i < StringProperties.Count; i++){
                (string, object) StringProperty = StringProperties[i];
                SelectCommand.Parameters.AddWithValue("@Value" + i, StringProperty.Item2);
            }

            Connection.Open();
            OleDbDataReader DataReader = SelectCommand.ExecuteReader();
            List<Pupil> Pupils = new List<Pupil>();
            while(DataReader.Read()){
                //(string, object)[] PropertyAndValue = new (string, object)[DataReader.FieldCount];
                Pupil CurrentPupil = new Pupil();
                for(int i = 0; i < DataReader.FieldCount; i++){
                    CurrentPupil.GetType().GetProperty(PUPIL_TABLE_COLUMN_NAMES[i]).SetValue(CurrentPupil, DataReader[i], null);
                }
                Pupils.Add(CurrentPupil);
            }
            Connection.Close();
            return Pupils;
        }
        
        private static readonly string[] UPDATE_PUPIL_DATA_PROPERTY_BLACKLIST = new string[]{"PupilUUID", "Notes"};
        public override void WritePupilData(Pupil p_Pupil) {
            OleDbConnection Connection = new OleDbConnection(CONNECTION_STRING_TEMPLATE + this.DatabasePath + "\\" + DATABASE_NAME);

            {

                OleDbCommand Command = new OleDbCommand();
                Command.CommandType = CommandType.Text;
                Command.Connection = Connection;

                //DbCommandBuilder CommandBuilder = DbProviderFactories.GetFactory("System.Data.OleDb").CreateCommandBuilder();

                if(this.GetPupilsByProperties(new {PupilUUID = p_Pupil.PupilUUID}).Count == 0){ //If the pupil doesn't already exist
                    Command = BuildInsertCommand<Pupil>(Connection, p_Pupil, "Pupil", new string[]{"Notes"});
                }else{
                    Command = BuildUpdateCommand<Pupil>(Connection, p_Pupil, "Pupil", UPDATE_PUPIL_DATA_PROPERTY_BLACKLIST, " WHERE [PupilUUID] = @PupilUUID");
                    Command.Parameters.AddWithValue("@PupilUUID", p_Pupil.PupilUUID);
                }
            
                Connection.Open();
                Command.ExecuteNonQuery();
                Connection.Close();

            }

            {
                OleDbCommand SelectCommand = new OleDbCommand();
                SelectCommand.CommandType = CommandType.Text;
                SelectCommand.Connection = Connection;
                SelectCommand.CommandText = "SELECT [UUID] FROM [Note] WHERE [PupilUUID] = @PupilUUID;";
                SelectCommand.Parameters.AddWithValue("@PupilUUID", p_Pupil.PupilUUID);

                List<string> StoredNoteUUIDs = new List<string>();

                Connection.Open();

                OleDbDataReader DataReader = SelectCommand.ExecuteReader();

                while(DataReader.Read()) StoredNoteUUIDs.Add((string)DataReader[0]);

                Connection.Close();

                List<string> CurrentNoteUUIDs = new List<string>();
                Dictionary<string, Note> NoteUUIDLookup = new Dictionary<string,Note>();
                foreach(Note i_Note in p_Pupil.Notes){
                    CurrentNoteUUIDs.Add(i_Note.UUID);
                    NoteUUIDLookup.Add(i_Note.UUID, i_Note);
                }

                foreach(string NoteUUID in CurrentNoteUUIDs.Union(StoredNoteUUIDs).ToList()){
                    bool Current = CurrentNoteUUIDs.Contains(NoteUUID);
                    bool Stored = StoredNoteUUIDs.Contains(NoteUUID);
                    bool Both = Current && Stored;

                    OleDbCommand Command = new OleDbCommand();
                    Command.CommandType = CommandType.Text;
                    Command.Connection = Connection;

                    if(Both){ //Update
                        Command = BuildUpdateCommand<Note>(Connection, NoteUUIDLookup[NoteUUID], "Note", new string[]{}, " WHERE [PupilUUID] = @PupilUUID");
                        Command.Parameters.AddWithValue("@PupilUUID", p_Pupil.PupilUUID);
                    }else if(Current){ //Insert
                        Command = BuildInsertCommand<Note>(Connection, NoteUUIDLookup[NoteUUID], "Note", new string[]{}, new string[]{"PupilUUID"}, new dynamic[]{p_Pupil.PupilUUID});
                    }else if(Stored){ //Delete
                        Command.CommandText = "DELETE FROM [Note] WHERE [PupilUUID] = @PupilUUID;";
                        Command.Parameters.AddWithValue("@PupilUUID", p_Pupil.PupilUUID);
                    }
                    Connection.Open();
                    Command.ExecuteNonQuery();
                    Connection.Close();
                }
            }
        }

        private static OleDbCommand BuildUpdateCommand<T>(OleDbConnection Connection, T Instance, string Table, string[] OmittedProperties, string Condition = ""){
            OleDbCommand Command = new OleDbCommand();
            Command.CommandType = CommandType.Text;
            Command.Connection = Connection;

            Type InstanceType = typeof(T);

            List<string> TypePropertyNames = new List<string>();
            int Length = InstanceType.GetProperties().Length;
            for(int i = 0; i < Length; i++) TypePropertyNames.Add(InstanceType.GetProperties()[i].Name);
            
            string CommandText = "UPDATE [" + Table + "]";
            for (dynamic i = 0, FirstTrueIteration = true; i < Length; i++) {
                string PropertyName = TypePropertyNames[i];
                if(OmittedProperties.Contains(PropertyName)) continue;
                if(FirstTrueIteration){
                    FirstTrueIteration = false;
                    CommandText += " SET ";
                }
                else CommandText += ", ";
                CommandText += "[" + PropertyName + "] = @Value" + i;
            }

            CommandText += Condition + ";";
            Command.CommandText = CommandText;

            for (int i = 0; i < Length; i++) {
                if(OmittedProperties.Contains(TypePropertyNames[i])) continue;
                var Value = InstanceType.GetProperty(TypePropertyNames[i]).GetValue(Instance, null);
                Command.Parameters.AddWithValue("@Value" + i, Value);
            }

            return Command;
        }

        private static OleDbCommand BuildInsertCommand<T>(OleDbConnection Connection, T Instance, string Table, string[] OmittedProperties, string[] AddedProperties = null, dynamic[] AddedPropertyValues = null){
            AddedProperties = AddedProperties ?? new string[]{};
            AddedPropertyValues = AddedPropertyValues ?? new dynamic[]{};
            OleDbCommand Command = new OleDbCommand();
            Command.CommandType = CommandType.Text;
            Command.Connection = Connection;

            Type InstanceType = typeof(T);

            List<(string Name, dynamic Value)> PropertyInfo = new List<(string, dynamic)>();
            List<string> ListedOmittedProperties = OmittedProperties.ToList();
            for(int i = 0, ExistingPropertiesCount = InstanceType.GetProperties().Length; i < ExistingPropertiesCount; i++) if(!ListedOmittedProperties.Contains(InstanceType.GetProperties()[i].Name)) PropertyInfo.Add((InstanceType.GetProperties()[i].Name, InstanceType.GetProperties()[i].GetValue(Instance, null)));
            for(int i = 0, AddedPropertiesCount = AddedProperties.Length; i < AddedPropertiesCount; i++) PropertyInfo.Add((AddedProperties[i], AddedPropertyValues[i]));
            
            int Length = PropertyInfo.Count;

            string CommandText = "INSERT INTO [" + Table + "]";

            for (int i = 0; i < Length; i++) {
                string PropertyName = PropertyInfo[i].Name;
                if(i == 0) CommandText += " (";
                else CommandText += ", ";
                CommandText += "[" + PropertyName + "]";
            }

            CommandText += ") VALUES";

            for (int i = 0; i < Length; i++) {
                if(i == 0) CommandText += " (";
                else CommandText += ", ";
                CommandText += "@Value" + i;
            }

            CommandText += ");";

            Command.CommandText = CommandText;
            for (int i = 0; i < Length; i++) {
                Command.Parameters.AddWithValue("@Value" + i, PropertyInfo[i].Value);
            }

            return Command;
        }
    }
}
