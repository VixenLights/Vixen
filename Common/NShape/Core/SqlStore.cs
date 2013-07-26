/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Dataweb.NShape.Advanced;
using System.Data.Common;


namespace Dataweb.NShape
{
	/// <summary>
	/// Cache implementation for MS SQL Server
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof (SqlStore), "SqlStore.bmp")]
	public class SqlStore : AdoNetStore
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.SqlStore" />.
		/// </summary>
		public SqlStore()
			: base()
		{
			ProviderName = "System.Data.SqlClient";
			serverName = "localhost";
			databaseName = "NShapeDB";
			ConnectionString = CalcConnectionString(serverName, databaseName);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.SqlStore" />.
		/// </summary>
		public SqlStore(string serverName, string databaseName)
			: base()
		{
			if (serverName == null) throw new ArgumentNullException("serverName");
			if (databaseName == null) throw new ArgumentNullException("databaseName");
			this.ProviderName = "System.Data.SqlClient";
			this.serverName = serverName;
			this.databaseName = databaseName;
			ConnectionString = CalcConnectionString(serverName, databaseName);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string DatabaseName
		{
			get { return this.databaseName; }
			set
			{
				ConnectionString = CalcConnectionString(this.serverName, value);
				this.databaseName = value;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string ServerName
		{
			get { return this.serverName; }
			set
			{
				ConnectionString = CalcConnectionString(value, databaseName);
				this.serverName = value;
			}
		}


		/// <override></override>
		public override void CreateDbCommands(IStoreCache storeCache)
		{
			base.CreateDbCommands(storeCache);
			//
			// === Commands to create and drop the schema ===
			SetCreateTablesCommand(CreateCreateTablesCommand(storeCache));
			SetCommand("All", RepositoryCommandType.Delete, CreateDropTablesCommand(storeCache));
			//
			CreateProjectInfoCommands();
			//
			CreateProjectCommands();
			//
			CreateDesignCommands();
			//
			CreateStyleCommands(storeCache);
			//
			CreateDiagramCommands();
			//
			CreateLayerCommands();
			//
			CreateShapeCommands(storeCache);
			//
			CreateModelCommands();
			//
			CreateModelObjectCommands(storeCache);
			//
			CreateTemplateCommands();
			//
			CreateModelMappingCommands();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected IDbCommand CreateCreateTablesCommand(IStoreCache storeCache)
		{
			StringBuilder cmdText = new StringBuilder();
			//
			cmdText.Append("CREATE TABLE ProjectInfo (Id INT IDENTITY PRIMARY KEY, Name VARCHAR(40), Version INT);" +
			               Environment.NewLine);
			cmdText.Append("CREATE TABLE Project ("
			               + "ProjectInfo INT REFERENCES ProjectInfo (Id) ON DELETE CASCADE ON UPDATE CASCADE,"
			               + "Id INT IDENTITY PRIMARY KEY, LastSavedUtc DATETIME);" + Environment.NewLine);
			cmdText.Append("CREATE TABLE Library ("
			               + "Project INT REFERENCES Project (Id) ON DELETE CASCADE ON UPDATE CASCADE,"
			               + "Name VARCHAR(40), AssemblyName VARCHAR(512), Version INT);" + Environment.NewLine);
			cmdText.Append(
				CreateCreateTableCommand(storeCache.FindEntityTypeByName(Design.EntityTypeName), "Project", "Project") + ";" +
				Environment.NewLine);
			cmdText.Append(
				"CREATE TABLE Style (Design INT REFERENCES Design (Id) ON DELETE CASCADE ON UPDATE CASCADE, Id INT IDENTITY PRIMARY KEY);" +
				Environment.NewLine);
			foreach (IEntityType et in storeCache.EntityTypes)
				if (et.Category == EntityCategory.Style)
					cmdText.Append(CreateCreateTableCommand(storeCache.FindEntityTypeByName(et.FullName), "Style", null) + ";" +
					               Environment.NewLine);
			//
			cmdText.Append(
				CreateCreateTableCommand(storeCache.FindEntityTypeByName(Diagram.EntityTypeName), "Project", "Project") + ";" +
				Environment.NewLine);
			cmdText.Append("CREATE TABLE Layer ("
			               + "Diagram INT REFERENCES Diagram (Id) ON DELETE CASCADE ON UPDATE CASCADE,"
			               + "Id INT, Name VARCHAR(40), Title VARCHAR(40), UpperZoomBound INT, LowerZoomBound INT);" +
			               Environment.NewLine);
			cmdText.Append("CREATE TABLE Shape (Id INT IDENTITY PRIMARY KEY);" + Environment.NewLine);
			foreach (IEntityType et in storeCache.EntityTypes)
				if (et.Category == EntityCategory.Shape)
					cmdText.Append(CreateCreateTableCommand(storeCache.FindEntityTypeByName(et.FullName), "Shape", null) + ";" +
					               Environment.NewLine);
			cmdText.Append("CREATE TABLE DiagramShape(Diagram INT, Shape INT PRIMARY KEY);" + Environment.NewLine);
			cmdText.Append("CREATE TABLE TemplateShape(Template INT, Shape INT PRIMARY KEY);" + Environment.NewLine);
			cmdText.Append("CREATE TABLE ChildShape(Parent INT, Shape INT PRIMARY KEY);" + Environment.NewLine);
			cmdText.Append(
				"CREATE TABLE ShapeConnection (ActiveShape INT, ActivePoint INT, PassiveShape INT, PassivePoint INT);" +
				Environment.NewLine);
			//
			cmdText.Append("CREATE TABLE Vertices (Owner INT, SeqNo INT, Id INT, X INT, Y INT);" + Environment.NewLine);
			cmdText.Append("CREATE TABLE ConnectionPoints (Owner INT, SeqNo INT, Id INT, A INT, B INT, C INT);" +
			               Environment.NewLine);
			//
			cmdText.Append(
				"CREATE TABLE Model (Project INT REFERENCES Project (Id) ON DELETE CASCADE ON UPDATE CASCADE, Id INT IDENTITY PRIMARY KEY);" +
				Environment.NewLine);
			//
			cmdText.Append("CREATE TABLE ModelObject (Id INT IDENTITY PRIMARY KEY);" + Environment.NewLine);
			foreach (IEntityType et in storeCache.EntityTypes)
				if (et.Category == EntityCategory.ModelObject)
					cmdText.Append(CreateCreateTableCommand(storeCache.FindEntityTypeByName(et.FullName), "ModelObject", null) + ";" +
					               Environment.NewLine);
			cmdText.Append("CREATE TABLE TemplateModelObject (Template INT, ModelObject INT PRIMARY KEY);" + Environment.NewLine);
			cmdText.Append("CREATE TABLE ModelModelObject (Model INT, ModelObject INT PRIMARY KEY);" + Environment.NewLine);
			cmdText.Append("CREATE TABLE ChildModelObject (PARENT INT, ModelObject INT PRIMARY KEY);" + Environment.NewLine);
			//
			cmdText.Append("CREATE TABLE Template ("
			               + "Project INT REFERENCES Project (Id) ON DELETE CASCADE ON UPDATE CASCADE, "
			               + "Id INT IDENTITY PRIMARY KEY, [Name] VARCHAR(40), Title VARCHAR(40), Description VARCHAR(40), "
			               + "ConnectionPointMappings VARCHAR(1000));" + Environment.NewLine);
			//
			cmdText.Append("CREATE TABLE ModelMapping ("
			               + "Template INT REFERENCES TEMPLATE (id) ON DELETE CASCADE ON UPDATE CASCADE, "
			               + "Id INT IDENTITY PRIMARY KEY);" + Environment.NewLine);
			cmdText.Append(string.Format("CREATE TABLE [{0}] ("
			                             + "ModelMapping INT REFERENCES ModelMapping (Id) ON DELETE CASCADE ON UPDATE CASCADE, "
			                             + "ShapePropertyId INT, ModelPropertyId INT, "
			                             + "MappingType INT, Intercept REAL, Slope REAL);" + Environment.NewLine,
			                             SqlTableNameForEntityName(NumericModelMapping.EntityTypeName)));
			cmdText.Append(string.Format("CREATE TABLE [{0}] ("
			                             + "ModelMapping INT REFERENCES ModelMapping (Id) ON DELETE CASCADE ON UPDATE CASCADE, "
			                             + "ShapePropertyId INT, ModelPropertyId INT, MappingType INT, Format NVARCHAR(120));" +
			                             Environment.NewLine,
			                             SqlTableNameForEntityName(FormatModelMapping.EntityTypeName)));
			cmdText.Append(string.Format("CREATE TABLE [{0}] ("
			                             + "ModelMapping INT REFERENCES ModelMapping (Id) ON DELETE CASCADE ON UPDATE CASCADE, "
			                             + "ShapePropertyId INT, ModelPropertyId INT, MappingType INT, DefaultStyleType INT, "
			                             + "DefaultStyle INT REFERENCES Style (Id) ON DELETE NO ACTION ON UPDATE NO ACTION, "
			                             + "ValueRanges NVARCHAR(4000));" + Environment.NewLine,
			                             SqlTableNameForEntityName(StyleModelMapping.EntityTypeName)));
			//
			cmdText.Append(
				string.Format(
					"CREATE TABLE SysCommand (Id INT IDENTITY PRIMARY KEY, Kind VARCHAR({0}), EntityType VARCHAR({1}), Text NVARCHAR(MAX))",
					ColumnSizeSysCmdKind, ColumnSizeSysCmdEntityType, ColumnSizeSysCmdCommandText));
			cmdText.Append(
				"CREATE TABLE SysParameter (Command INT REFERENCES SysCommand (Id) ON DELETE CASCADE ON UPDATE CASCADE, No INT, Name VARCHAR(40), Type VARCHAR(10))");
			//
			return CreateCommand(cmdText.ToString());
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected string CreateCreateTableCommand(IEntityType entityType, string parentTableName, string parentColumnName)
		{
			if (parentTableName == null) throw new ArgumentNullException("parentTableName");
			StringBuilder sb = new StringBuilder();
			if (parentColumnName == null)
				// Table has a supertype table
				sb.AppendFormat("CREATE TABLE [{0}] (Id INT PRIMARY KEY REFERENCES {1} (Id) ON DELETE CASCADE ON UPDATE CASCADE, ",
				                SqlTableNameForEntityName(entityType.FullName), parentTableName);
			else
				sb.AppendFormat(
					"CREATE TABLE [{0}] ({1} INT REFERENCES {2} (Id) ON DELETE CASCADE ON UPDATE CASCADE, Id INT IDENTITY PRIMARY KEY, ",
					SqlTableNameForEntityName(entityType.FullName), parentColumnName, parentTableName);
			foreach (EntityPropertyDefinition pi in entityType.PropertyDefinitions) {
				if (pi is EntityFieldDefinition) {
					sb.Append(pi.Name);
					sb.Append(" ");
					sb.Append(SqlTypeForDotNetType(((EntityFieldDefinition) pi).Type));
					/* Templates can also be null and other links depending on the meaning.
					 * Must be managed by the application not by the database.
					if (DbTypeForDotNetType(((EntityFieldDefinition)pi).Type) != DbType.String && DbTypeForDotNetType(((EntityFieldDefinition)pi).Type) != DbType.Binary)
						sb.Append(" NOT NULL");*/
				}
				else if (IsComposition(pi)) {
					sb.Append(pi.Name);
					sb.Append(" ");
					sb.Append("NVARCHAR(1000)");
				}
				sb.Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);
			sb.Append(")");
			return sb.ToString();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected IDbCommand CreateDropTablesCommand(IStoreCache storeCache)
		{
			string dropCommand = "IF OBJECT_ID('[{0}]') IS NOT NULL DROP TABLE [{0}];" + Environment.NewLine;
			StringBuilder cmdText = new StringBuilder();
			// Drop inner objects
			cmdText.AppendFormat(dropCommand, "Vertices");
			cmdText.AppendFormat(dropCommand, "ConnectionPoints");
			// Drop property mappings
			cmdText.AppendFormat(dropCommand, "NumericModelMapping");
			cmdText.AppendFormat(dropCommand, "FormatModelMapping");
			cmdText.AppendFormat(dropCommand, "StyleModelMapping");
			cmdText.AppendFormat(dropCommand, "ModelMapping");
			// Drop shape connections
			cmdText.AppendFormat(dropCommand, "ShapeConnection");
			cmdText.AppendFormat(dropCommand, "ChildShape");
			cmdText.AppendFormat(dropCommand, "TemplateShape");
			cmdText.AppendFormat(dropCommand, "DiagramShape");
			// Drop modelObject tables
			foreach (IEntityType et in storeCache.EntityTypes)
				if (et.Category == EntityCategory.ModelObject)
					cmdText.AppendFormat(dropCommand, SqlTableNameForEntityName(et.FullName));
			cmdText.AppendFormat(dropCommand, "ChildModelObject");
			cmdText.AppendFormat(dropCommand, "ModelModelObject");
			cmdText.AppendFormat(dropCommand, "TemplateModelObject");
			cmdText.AppendFormat(dropCommand, "ModelObject");
			cmdText.AppendFormat(dropCommand, "Model");
			// Drop shape tables
			foreach (IEntityType et in storeCache.EntityTypes)
				if (et.Category == EntityCategory.Shape)
					cmdText.AppendFormat(dropCommand, SqlTableNameForEntityName(et.FullName));
			cmdText.AppendFormat(dropCommand, "Shape");
			cmdText.AppendFormat(dropCommand, "Layer");
			cmdText.AppendFormat(dropCommand, "Diagram");
			// Drop style tables
			foreach (IEntityType et in storeCache.EntityTypes)
				if (et.Category == EntityCategory.Style)
					cmdText.AppendFormat(dropCommand, SqlTableNameForEntityName(et.FullName));
			cmdText.AppendFormat(dropCommand, "Style");
			cmdText.AppendFormat(dropCommand, "Design");
			cmdText.AppendFormat(dropCommand, "Template");
			cmdText.AppendFormat(dropCommand, "Library");
			cmdText.AppendFormat(dropCommand, "Project");
			cmdText.AppendFormat(dropCommand, "ProjectInfo");
			// Drop system tables
			cmdText.AppendFormat(dropCommand, "SysParameter");
			cmdText.AppendFormat(dropCommand, "SysCommand");
			return CreateCommand(cmdText.ToString());
		}


		/// <override></override>
		public override IDbCommand GetInsertSysCommandCommand()
		{
			IDbCommand result = base.GetInsertSysCommandCommand();
			((SqlParameter) result.Parameters[0]).Size = ColumnSizeSysCmdKind;
			((SqlParameter) result.Parameters[1]).Size = ColumnSizeSysCmdEntityType;
			((SqlParameter) result.Parameters[2]).Size = ColumnSizeSysCmdCommandText;
			return result;
		}


		/// <override></override>
		public override IDbCommand GetInsertSysParameterCommand()
		{
			IDbCommand result = base.GetInsertSysParameterCommand();
			((SqlParameter) result.Parameters[2]).Size = 40;
			((SqlParameter) result.Parameters[3]).Size = 10;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected string SqlTableNameForEntityName(string entityName)
		{
			string result;
			int idx = entityName.IndexOf('.');
			if (idx < 0)
				result = entityName;
			else if (entityName.Substring(0, idx).Equals("Core", StringComparison.InvariantCultureIgnoreCase)
			         || entityName.Substring(0, idx).Equals("GeneralShapes", StringComparison.InvariantCultureIgnoreCase))
				result = entityName.Substring(idx + 1, entityName.Length - idx - 1);
			else result = entityName.Replace('.', '_');
			return result;
			//return string.Format("\"{0}\"", result);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected DbType DbTypeForDotNetType(Type type)
		{
			if (type == typeof (Object))
				return DbType.Int32;
			else if (type == typeof (Int32))
				return DbType.Int32;
			else if (type == typeof (char))
				return DbType.String;
			else if (type == typeof (String))
				return DbType.String;
			else if (type == typeof (DateTime))
				return DbType.DateTime;
			else if (type == typeof (Color))
				return DbType.Int32;
			else if (type == typeof (Byte))
				return DbType.Byte;
			else if (type == typeof (Single))
				return DbType.Single;
			else if (type == typeof (Image))
				return DbType.Binary;
			else if (type == typeof (Int16))
				return DbType.Int16;
			else if (type == typeof (bool))
				return DbType.Boolean;
			else {
				Debug.Fail("Unexpected property type");
				return DbType.Xml;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected string SqlTypeForDotNetType(Type type)
		{
			if (type == typeof (Object))
				return "INT";
			else if (type == typeof (Int32))
				return "INT";
			else if (type == typeof (char))
				return "NCHAR";
			else if (type == typeof (String))
				return "VARCHAR(40)";
			else if (type == typeof (DateTime))
				return "DATETIME";
			else if (type == typeof (Color))
				return "INT";
			else if (type == typeof (Byte))
				return "TINYINT";
			else if (type == typeof (Single))
				return "REAL";
			else if (type == typeof (Image))
				return "IMAGE";
			else if (type == typeof (Int16))
				return "SMALLINT";
			else if (type == typeof (bool))
				return "BIT";
			else {
				Debug.Fail("Unexpected property type");
				return "XXX";
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected string CalcConnectionString(string serverName, string databaseName)
		{
			return
				string.Format(
					"Data Source={0};Initial Catalog={1};Integrated Security=True;MultipleActiveResultSets=True;Pooling=True",
					serverName, databaseName);
		}


		private void CreateProjectInfoCommands()
		{
			SetCommand(projectInfoEntityTypeName, RepositoryCommandType.SelectByName,
			           CreateCommand("SELECT Name, Version FROM ProjectInfo WHERE Name = @Name",
			                         CreateParameter("Name", DbType.String)));
			SetCommand(projectInfoEntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(
			           	"INSERT INTO ProjectInfo (Name, Version) VALUES (@Name, @Version); SELECT CAST(IDENT_CURRENT('ProjectInfo') AS INT)",
			           	CreateParameter("Name", DbType.String),
			           	CreateParameter("Version", DbType.Int32)));
			SetCommand(projectInfoEntityTypeName, RepositoryCommandType.Update,
			           CreateCommand("UPDATE ProjectInfo SET Name = @Name, Version = @Version WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Version", DbType.Int32)));
			SetCommand(projectInfoEntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM ProjectInfo WHERE Name = @Name",
			                         CreateParameter("Name", DbType.String)));
		}


		private void CreateProjectCommands()
		{
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.SelectByName,
			           // DesignId must be the first column because the value is read by the cache
			           CreateCommand(
			           	"SELECT P.Id, ProjectInfo, LastSavedUTC FROM Project P JOIN ProjectInfo PI ON P.ProjectInfo = PI.Id WHERE PI.Name = @Name",
			           	CreateParameter("Name", DbType.String)));
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand("INSERT INTO Project (ProjectInfo, LastSavedUTC) "
			                         + "VALUES (@ProjectInfo, @LastSavedUTC); "
			                         + "SELECT CAST(IDENT_CURRENT('Project') AS INT)",
			                         CreateParameter("ProjectInfo", DbType.Int32),
			                         CreateParameter("LastSavedUTC", DbType.DateTime)));
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand("UPDATE Project SET LastSavedUTC = @LastSavedUTC WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("ProjectInfo", DbType.Int32),
			                         CreateParameter("LastSavedUTC", DbType.DateTime)));
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Project WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32)));

			// Create commands for inner objects (Libraries)
			CreateLibraryCommands();
		}


		private void CreateLibraryCommands()
		{
			SetCommand("Core.Library", RepositoryCommandType.SelectById,
			           CreateCommand("SELECT Name, AssemblyName, Version FROM Library WHERE Project = @Project",
			                         CreateParameter("Project", DbType.Int32)));
			SetCommand("Core.Library", RepositoryCommandType.Insert,
			           CreateCommand(
			           	"INSERT INTO Library (Project, Name, AssemblyName, Version) VALUES (@Project, @Name, @AssemblyName, @Version)",
			           	CreateParameter("Project", DbType.Int32),
			           	CreateParameter("Name", DbType.String),
			           	CreateParameter("AssemblyName", DbType.String),
			           	CreateParameter("Version", DbType.Int32)));
			SetCommand("Core.Library", RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Library WHERE Project = @Project",
			                         CreateParameter("Project", DbType.Int32)));
		}


		private void CreateDesignCommands()
		{
			string cmdTxt = "SELECT Id, Project, Name, ";
			if (Version >= 4) cmdTxt += "Title, ";
			cmdTxt += "Description FROM Design";
			SetCommand(Design.EntityTypeName, RepositoryCommandType.SelectAll, CreateCommand(cmdTxt));

			cmdTxt = "SELECT Id, Project, Name, ";
			if (Version >= 4) cmdTxt += "Title, ";
			cmdTxt += "Description FROM Design WHERE Id = @Id";
			SetCommand(Design.EntityTypeName, RepositoryCommandType.SelectById,
			           CreateCommand(cmdTxt, CreateParameter("Id", DbType.Int32)));

			cmdTxt = "SELECT Id, Project, Name, ";
			if (Version >= 4) cmdTxt += "Title, ";
			cmdTxt += "Description FROM Design WHERE Project = @Project";
			SetCommand(Design.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand(cmdTxt, CreateParameter("Project", DbType.Int32)));

			cmdTxt = "INSERT INTO Design (Project, Name, ";
			if (Version >= 4) cmdTxt += "Title, ";
			cmdTxt += "Description) VALUES (@Project, @Name, ";
			if (Version > 3) cmdTxt += "@Title, ";
			cmdTxt += "@Description); SELECT CAST(IDENT_CURRENT('Design') AS INT)";
			SetCommand(Design.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(cmdTxt,
			                         CreateParameter("Project", DbType.Int32),
			                         // Not used, always null, needed for generic statement handling
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Title", DbType.String),
			                         CreateParameter("Description", DbType.String)));

			cmdTxt = "UPDATE Design SET Name = @Name, ";
			if (Version >= 4) cmdTxt += "Title = @Title, ";
			cmdTxt += "@Description = Description WHERE Id = @Id";
			SetCommand(Design.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand(cmdTxt,
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Title", DbType.String),
			                         CreateParameter("Description", DbType.String)));

			SetCommand(Design.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Design WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32)));
		}


		private void CreateStyleCommands(IStoreCache storeCache)
		{
			// === Generic Select Style Commands ===
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Style) continue;
				IDbCommand selectStyleCmd = CreateCommand();
				StringBuilder selectStyleList = new StringBuilder();
				selectStyleCmd.Parameters.Add(CreateParameter("Design", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityInnerObjectsDefinition) continue;
					selectStyleList.Append(", ");
					selectStyleList.Append(pi.Name);
				}
				selectStyleCmd.CommandText = string.Format(
					"SELECT Style.Id, Design{0} FROM [{1}] JOIN Style ON [{1}].Id = Style.Id WHERE Design = @Design",
					selectStyleList.ToString(), SqlTableNameForEntityName(et.FullName));
				SetCommand(et.FullName, RepositoryCommandType.SelectByOwnerId, selectStyleCmd);
			}
			// === Specific Style Commands ===
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Style) continue;
				string tableName = SqlTableNameForEntityName(et.FullName);

				// === INSERT commands ===
				IDbCommand insertStyleCmd = CreateCommand();
				StringBuilder insertStyleCmdText1 = new StringBuilder();
				StringBuilder insertStyleCmdText2 = new StringBuilder();
				insertStyleCmdText1.Append("Id");
				insertStyleCmdText2.Append("@@IDENTITY");
				insertStyleCmd.Parameters.Add(CreateParameter("Design", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityInnerObjectsDefinition) continue;
					insertStyleCmdText1.Append(", ");
					insertStyleCmdText1.Append(pi.Name);
					insertStyleCmdText2.Append(", ");
					insertStyleCmdText2.Append("@" + pi.Name);
					insertStyleCmd.Parameters.Add(
						CreateParameter(pi.Name, DbTypeForDotNetType(((EntityFieldDefinition) pi).Type)));
				}
				insertStyleCmd.CommandText = string.Format("INSERT INTO Style (Design) VALUES (@Design); "
				                                           +
				                                           "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT CAST(IDENT_CURRENT('Style') AS INT)",
				                                           tableName, insertStyleCmdText1.ToString(), insertStyleCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.Insert, insertStyleCmd);

				// === UPDATE commands ===
				IDbCommand updateStyleCmd = CreateCommand();
				StringBuilder updateStyleCmdText = new StringBuilder();
				updateStyleCmd.Parameters.Add(CreateParameter("Id", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityInnerObjectsDefinition) continue;
					updateStyleCmdText.Append(pi.Name);
					updateStyleCmdText.Append(" = @");
					updateStyleCmdText.Append(pi.Name);
					updateStyleCmdText.Append(", ");
					updateStyleCmd.Parameters.Add(CreateParameter(pi.Name, DbTypeForDotNetType(((EntityFieldDefinition) pi).Type)));
				}
				updateStyleCmdText.Remove(updateStyleCmdText.Length - 2, 2);
				updateStyleCmd.CommandText = string.Format("UPDATE [{0}] SET {1} Where Id = @Id",
				                                           tableName, updateStyleCmdText.ToString());
				SetCommand(et.FullName, RepositoryCommandType.Update, updateStyleCmd);

				// === DELETE commands ===
				SetCommand(et.FullName, RepositoryCommandType.Delete,
				           CreateCommand(string.Format("DELETE FROM [{0}] WHERE Id = @Id;"
				                                       + "DELETE FROM [Style] WHERE Id = @Id;", tableName),
				                         CreateParameter("@Id", DbType.Int32)
				           	)
					);
			}
		}


		private void CreateDiagramCommands()
		{
			string cmdTxt;

			// Create "SelectByOwnerId" command
			cmdTxt = string.Empty;
			cmdTxt += "SELECT Id, Project, Name, ";
			if (Version >= 3) cmdTxt += "Title, ";
			if (Version >= 4) cmdTxt += "SecurityDomain, ";
			cmdTxt += "Width, Height, ";
			cmdTxt += "BackgroundColor, BackgroundGradientEndColor, BackgroundImageFileName, BackgroundImage, ";
			cmdTxt += "ImageLayout, ImageGamma, ImageTransparency, ImageGrayScale, ImageTransparentColor ";
			cmdTxt += "FROM Diagram WHERE Project = @Project";
			SetCommand(Diagram.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand(cmdTxt, CreateParameter("Project", DbType.Int32)));

			// Create "SelectById" command
			cmdTxt = string.Empty;
			cmdTxt += "SELECT Id, Project, Name, ";
			if (Version >= 3) cmdTxt += "Title, ";
			if (Version >= 4) cmdTxt += "SecurityDomain, ";
			cmdTxt += "Width, Height, BackgroundColor, BackgroundGradientEndColor, ";
			cmdTxt += "BackgroundImageLayout, BackgroundImageFileName, BackgroundImage FROM Diagram ";
			cmdTxt += "WHERE Id = @Id";
			SetCommand(Diagram.EntityTypeName, RepositoryCommandType.SelectById,
			           CreateCommand(cmdTxt, CreateParameter("Id", DbType.Int32)));

			// Create "Insert" command
			cmdTxt = string.Empty;
			cmdTxt += "INSERT INTO Diagram (Project, Name, ";
			if (Version >= 3) cmdTxt += "Title, ";
			if (Version >= 4) cmdTxt += "SecurityDomain, ";
			cmdTxt += "Width, Height, BackgroundColor, BackgroundGradientEndColor, BackgroundImageFileName, ";
			cmdTxt += "BackgroundImage, ImageLayout, ImageGamma, ImageTransparency, ImageGrayScale, ";
			cmdTxt += "ImageTransparentColor) VALUES (@Project, @Name, ";
			if (Version >= 3) cmdTxt += "@Title, ";
			if (Version >= 4) cmdTxt += "@SecurityDomain, ";
			cmdTxt += "@Width, @Height, @BackgroundColor, @BackgroundGradientEndColor, ";
			cmdTxt += "@BackgroundImageFileName, @BackgroundImage, @ImageLayout, @ImageGamma, @ImageTransparency, ";
			cmdTxt += "@ImageGrayScale, @ImageTransparentColor); ";
			cmdTxt += "SELECT CAST(IDENT_CURRENT('Diagram') AS INT)";
			SetCommand(Diagram.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(cmdTxt,
			                         CreateParameter("Project", DbType.Int32),
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Title", DbType.String),
			                         CreateParameter("SecurityDomain", DbType.String),
			                         CreateParameter("Width", DbType.Int32),
			                         CreateParameter("Height", DbType.Int32),
			                         CreateParameter("BackgroundColor", DbType.Int32),
			                         CreateParameter("BackgroundGradientEndColor", DbType.Int32),
			                         CreateParameter("BackgroundImageFileName", DbType.String),
			                         CreateParameter("BackgroundImage", DbType.Binary),
			                         CreateParameter("ImageLayout", DbType.Byte),
			                         CreateParameter("ImageGamma", DbType.Double),
			                         CreateParameter("ImageTransparency", DbType.Byte),
			                         CreateParameter("ImageGrayScale", DbType.Boolean),
			                         CreateParameter("ImageTransparentColor", DbType.Int32)
			           	));

			// Create "Update" command
			cmdTxt = string.Empty;
			cmdTxt += "UPDATE Diagram SET Name = @Name, ";
			if (Version >= 3) cmdTxt += "Title = @Title, ";
			if (Version >= 4) cmdTxt += "SecurityDomain = @SecurityDomain, ";
			cmdTxt += "Width = @Width, Height = @Height, BackgroundColor = @BackgroundColor, ";
			cmdTxt += "BackgroundGradientEndColor = @BackgroundGradientEndColor, ";
			cmdTxt += "BackgroundImageFileName = @BackgroundImageFileName, ";
			cmdTxt += "BackgroundImage = @BackgroundImage, ImageLayout = @ImageLayout, ";
			cmdTxt += "ImageGamma = @ImageGamma, ImageTransparency = @ImageTransparency, ";
			cmdTxt += "ImageGrayScale = @ImageGrayScale, ImageTransparentColor = @ImageTransparentColor ";
			cmdTxt += "WHERE Id = @Id";
			SetCommand(Diagram.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand(cmdTxt,
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Title", DbType.String),
			                         CreateParameter("SecurityDomain", DbType.String),
			                         CreateParameter("Width", DbType.Int32),
			                         CreateParameter("Height", DbType.Int32),
			                         CreateParameter("BackgroundColor", DbType.Int32),
			                         CreateParameter("BackgroundGradientEndColor", DbType.Int32),
			                         CreateParameter("BackgroundImageFileName", DbType.String),
			                         CreateParameter("BackgroundImage", DbType.Binary),
			                         CreateParameter("ImageLayout", DbType.Byte),
			                         CreateParameter("ImageGamma", DbType.Double),
			                         CreateParameter("ImageTransparency", DbType.Byte),
			                         CreateParameter("ImageGrayScale", DbType.Boolean),
			                         CreateParameter("ImageTransparentColor", DbType.Int32)
			           	));

			// Create "Delete" command
			SetCommand(Diagram.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Diagram WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32)));
		}


		private void CreateLayerCommands()
		{
			SetCommand("Core.Layer", RepositoryCommandType.SelectById,
			           CreateCommand(
			           	"SELECT Id, Name, Title, UpperZoomBound, LowerZoomBound FROM Layer WHERE Diagram = @Diagram",
			           	CreateParameter("Diagram", DbType.Int32)));
			SetCommand("Core.Layer", RepositoryCommandType.Insert,
			           CreateCommand(
			           	"INSERT INTO Layer (Diagram, Id, Name, Title, UpperZoomBound, LowerZoomBound) VALUES (@Diagram, @Id, @Name, @Title, @UpperZoomBound, @LowerZoomBound)",
			           	CreateParameter("Diagram", DbType.Int32),
			           	CreateParameter("Id", DbType.Int32),
			           	CreateParameter("Name", DbType.String),
			           	CreateParameter("Title", DbType.String),
			           	CreateParameter("UpperZoomBound", DbType.Int32),
			           	CreateParameter("LowerZoomBound", DbType.Int32)));
			SetCommand("Core.Layer", RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Layer WHERE Diagram = @Diagram",
			                         CreateParameter("Diagram", DbType.Int32)));
		}


		private void CreateShapeCommands(IStoreCache storeCache)
		{
			// === Generic Shape Commands ===
			// SELECT by parent id command
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Shape) continue;
				StringBuilder selectCmdText = new StringBuilder();
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						selectCmdText.Append(", S.");
						selectCmdText.Append(pi.Name);
					}
					else if (IsComposition(pi)) {
						selectCmdText.Append(", S.");
						selectCmdText.Append(pi.Name);
					}
				}
				IDbCommand selectDiagramShapeCmd = CreateCommand(
					string.Format(
						"SELECT DiagramShape.Shape, Diagram{0} FROM [{1}] S JOIN DiagramShape ON S.Id = DiagramShape.Shape WHERE DiagramShape.Diagram = @Diagram",
						selectCmdText.ToString(), SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Diagram", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.SelectDiagramShapes, selectDiagramShapeCmd);
				IDbCommand selectTemplateShapeCmd = CreateCommand(
					string.Format(
						"SELECT TS.Shape, TS.Template{0} FROM [{1}] S JOIN TemplateShape TS ON S.Id = TS.Shape JOIN Template T ON TS.Template = T.Id WHERE T.Project = @Project",
						selectCmdText.ToString(), SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Project", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.SelectTemplateShapes, selectTemplateShapeCmd);
				IDbCommand selectChildShapeCmd = CreateCommand(
					string.Format(
						"SELECT ChildShape.Shape, Parent{0} FROM [{1}] S JOIN ChildShape ON S.Id = ChildShape.Shape WHERE ChildShape.Parent = @Parent",
						selectCmdText.ToString(), SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Parent", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.SelectChildShapes, selectChildShapeCmd);
			}
			// INSERT commands
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Shape) continue;
				IDbCommand insertShapeCmd = CreateCommand();
				StringBuilder insertShapeCmdText1 = new StringBuilder();
				StringBuilder insertShapeCmdText2 = new StringBuilder();
				insertShapeCmdText1.Append("Id");
				insertShapeCmdText2.Append("@Ident");
				insertShapeCmd.Parameters.Add(CreateParameter("Parent", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						insertShapeCmdText1.Append(", ");
						insertShapeCmdText1.Append(pi.Name);
						insertShapeCmdText2.Append(", ");
						insertShapeCmdText2.Append("@" + pi.Name);
						insertShapeCmd.Parameters.Add(CreateParameter(pi.Name, DbTypeForDotNetType(((EntityFieldDefinition) pi).Type)));
					}
					else if (IsComposition(pi)) {
						insertShapeCmdText1.Append(", ");
						insertShapeCmdText1.Append(pi.Name);
						insertShapeCmdText2.Append(", ");
						insertShapeCmdText2.Append("@" + pi.Name);
						insertShapeCmd.Parameters.Add(CreateParameter(pi.Name, DbType.String));
					}
					else Debug.Fail("Unexpected inner objects type in CreateDbCommands.");
				}
				IDbCommand insertDiagramShapeCmd = (IDbCommand) ((ICloneable) insertShapeCmd).Clone();
				insertDiagramShapeCmd.CommandText = string.Format("DECLARE @Ident INT; "
				                                                  + "INSERT INTO Shape DEFAULT VALUES; SET @Ident = @@IDENTITY; "
				                                                  +
				                                                  "INSERT INTO DiagramShape (Diagram, Shape) VALUES (@Parent, @Ident); "
				                                                  + "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @Ident",
				                                                  SqlTableNameForEntityName(et.FullName),
				                                                  insertShapeCmdText1.ToString(), insertShapeCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.InsertDiagramShape, insertDiagramShapeCmd);
				IDbCommand insertTemplateShapeCmd = (IDbCommand) ((ICloneable) insertShapeCmd).Clone();
				insertTemplateShapeCmd.CommandText = string.Format("DECLARE @Ident INT; "
				                                                   + "INSERT INTO Shape DEFAULT VALUES; SET @Ident = @@IDENTITY; "
				                                                   +
				                                                   "INSERT INTO TemplateShape (Template, Shape) VALUES (@Parent, @Ident); "
				                                                   + "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @Ident",
				                                                   SqlTableNameForEntityName(et.FullName),
				                                                   insertShapeCmdText1.ToString(), insertShapeCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.InsertTemplateShape, insertTemplateShapeCmd);
				IDbCommand insertChildShapeCmd = (IDbCommand) ((ICloneable) insertShapeCmd).Clone();
				insertChildShapeCmd.CommandText = string.Format("DECLARE @Ident INT; "
				                                                + "INSERT INTO Shape DEFAULT VALUES; SET @Ident = @@IDENTITY; "
				                                                +
				                                                "INSERT INTO ChildShape (Parent, Shape) VALUES (@Parent, @Ident); "
				                                                + "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @Ident",
				                                                SqlTableNameForEntityName(et.FullName),
				                                                insertShapeCmdText1.ToString(), insertShapeCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.InsertChildShape, insertChildShapeCmd);
			}
			// UPDATE commands
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Shape) continue;
				IDbCommand updateShapeCmd = CreateCommand();
				StringBuilder cmdText = new StringBuilder();
				cmdText.AppendFormat("UPDATE [{0}] SET ", SqlTableNameForEntityName(et.FullName));
				// Id must be first parameter because it is written first by the writer client.
				updateShapeCmd.Parameters.Add(CreateParameter("Id", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						cmdText.AppendFormat("[{0}] = @{0}, ", pi.Name);
						updateShapeCmd.Parameters.Add(CreateParameter(pi.Name, DbTypeForDotNetType(((EntityFieldDefinition) pi).Type)));
					}
					else if (IsComposition(pi)) {
						cmdText.AppendFormat("[{0}] = @{0}, ", pi.Name);
						updateShapeCmd.Parameters.Add(CreateParameter(pi.Name, DbType.String));
					}
					else Debug.Fail("Unexpected inner objects type in CreateDbCommands.");
				}
				cmdText.Length -= 2; // RemoveRange last comma + space
				cmdText.Append(" WHERE Id = @Id");
				updateShapeCmd.CommandText = cmdText.ToString();
				SetCommand(et.FullName, RepositoryCommandType.Update, updateShapeCmd);
			}
			SetCommand(shapeEntityTypeName, RepositoryCommandType.UpdateOwnerDiagram,
			           CreateCommand("DELETE FROM DiagramShape WHERE Shape = @Id; DELETE FROM ChildShape WHERE Shape = @Id; "
			                         + "INSERT INTO DiagramShape (Diagram, Shape) VALUES (@Diagram, @Id)",
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("Diagram", DbType.Int32)));
			SetCommand(shapeEntityTypeName, RepositoryCommandType.UpdateOwnerShape,
			           CreateCommand("DELETE FROM DiagramShape WHERE Shape = @Id; DELETE FROM ChildShape WHERE Shape = @Id; "
			                         + "INSERT INTO ChildShape (Parent, Shape) VALUES (@Parent, @Id)",
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("Parent", DbType.Int32)));
			//
			// DELETE command
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Shape) continue;
				IDbCommand deleteShapeCommand = CreateCommand(
					string.Format("DELETE FROM DiagramShape WHERE Shape = @Id; "
					              + "DELETE FROM TemplateShape WHERE Shape = @Id; "
					              + "DELETE FROM ChildShape WHERE Shape = @Id; "
					              + "DELETE FROM Shape WHERE Id = @Id; "
					              + "DELETE FROM [{0}] WHERE Id = @Id",
					              SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Id", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.Delete, deleteShapeCommand);
			}
			//
			// Build CheckStyleInUse and CheckTemplateInUse command
			const string checkShapeCmdFmt =
				"EXISTS(SELECT {0}.Id FROM {0} WHERE {0}.Id IN (SELECT Shape FROM DiagramShape JOIN Diagram ON DiagramShape.Diagram = Diagram.Id WHERE Diagram.Project = @Project AND Diagram.Id = @Diagram) {1})";
			string styleExistsStatement = string.Empty;
			string templateExistsStatement = string.Empty;
			string modelObjExistsStatement = string.Empty;
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Shape) continue;
				if (et.FullName == "Core.ShapeGroup") continue;
				string whereStatement = string.Empty;
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						EntityFieldDefinition fieldDef = (EntityFieldDefinition) pi;
						if (fieldDef.Type == typeof (object)) {
							if (fieldDef.ElementName == "model_object" || fieldDef.ElementName == "template") continue;
							if (whereStatement.Length > 0) whereStatement += " OR ";
							whereStatement += fieldDef.Name + " = @Style";
						}
					}
				}
				// Build statement for checking templates, model objects and styles
				if (templateExistsStatement.Length > 0) templateExistsStatement += " OR ";
				templateExistsStatement += string.Format(checkShapeCmdFmt, SqlTableNameForEntityName(et.FullName),
				                                         " AND (Template = @Template)");
				if (modelObjExistsStatement.Length > 0) modelObjExistsStatement += " OR ";
				modelObjExistsStatement += string.Format(checkShapeCmdFmt, SqlTableNameForEntityName(et.FullName),
				                                         " AND (ModelObject = @ModelObject)");
				// Build statement for checking styles
				if (string.IsNullOrEmpty(whereStatement)) continue;
				if (styleExistsStatement.Length > 0) styleExistsStatement += " OR ";
				styleExistsStatement += string.Format(checkShapeCmdFmt, SqlTableNameForEntityName(et.FullName),
				                                      " AND (" + whereStatement + ")");
			}
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.CheckTemplateInUse,
			           CreateCommand("SELECT CASE WHEN (" + templateExistsStatement + ") THEN 1 ELSE 0 END",
			                         CreateParameter("Project", DbType.Int32),
			                         CreateParameter("Diagram", DbType.Int32),
			                         CreateParameter("Template", DbType.Int32)
			           	)
				);
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.CheckModelObjectInUse,
			           CreateCommand("SELECT CASE WHEN (" + modelObjExistsStatement + ") THEN 1 ELSE 0 END",
			                         CreateParameter("Project", DbType.Int32),
			                         CreateParameter("Diagram", DbType.Int32),
			                         CreateParameter("ModelObject", DbType.Int32)
			           	)
				);
			SetCommand(ProjectSettings.EntityTypeName, RepositoryCommandType.CheckStyleInUse,
			           CreateCommand("SELECT CASE WHEN (" + styleExistsStatement + ") THEN 1 ELSE 0 END",
			                         CreateParameter("Project", DbType.Int32),
			                         CreateParameter("Diagram", DbType.Int32),
			                         CreateParameter("Style", DbType.Int32)
			           	)
				);

			// Build CheckShapeTypeInUse and CheckModelObjectTypeInUse command 
			const string checkModelObjCmdFmt =
				"EXISTS(SELECT {0}.Id FROM {0} WHERE {0}.Id IN (SELECT ModelObject FROM ModelModelObject JOIN Model ON ModelModelObject.Model = Model.Id WHERE Model.Project = @Project AND Model.Id = @Model) {1})";
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.Shape && et.Category != EntityCategory.ModelObject)
					continue;
				SetCommand(et.FullName, RepositoryCommandType.CheckShapeTypeInUse,
				           CreateCommand("SELECT CASE WHEN (" +
				                         string.Format(
				                         	(et.Category == EntityCategory.Shape) ? checkShapeCmdFmt : checkModelObjCmdFmt,
				                         	SqlTableNameForEntityName(et.FullName), string.Empty)
				                         + ") THEN 1 ELSE 0 END",
				                         CreateParameter("Project", DbType.Int32),
				                         CreateParameter("Diagram", DbType.Int32)
				           	)
					);
			}

			//
			// === Shape Connection Commands ===
			SetCommand(connectionEntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand("SELECT ActiveShape, ActivePoint, PassiveShape, PassivePoint FROM ShapeConnection "
			                         +
			                         "JOIN DiagramShape ON ActiveShape = DiagramShape.Shape WHERE DiagramShape.Diagram = @Diagram",
			                         CreateParameter("Diagram", DbType.Int32)));
			SetCommand(connectionEntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand("INSERT INTO ShapeConnection (ActiveShape, ActivePoint, PassiveShape, PassivePoint) "
			                         + "VALUES (@ActiveShape, @ActivePoint, @PassiveShape, @PassivePoint)",
			                         CreateParameter("ActiveShape", DbType.Int32),
			                         CreateParameter("ActivePoint", DbType.Int32),
			                         CreateParameter("PassiveShape", DbType.Int32),
			                         CreateParameter("PassivePoint", DbType.Int32)));
			SetCommand(connectionEntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand(
			           	"DELETE FROM ShapeConnection WHERE ActiveShape = @ActiveShape AND ActivePoint = @ActivePoint",
			           	CreateParameter("ActiveShape", DbType.Int32),
			           	CreateParameter("ActivePoint", DbType.Int32)));
			//
			// === Inner Objects Commands ===
			SetCommand(vertexEntityTypeName, RepositoryCommandType.SelectById,
			           CreateCommand("SELECT SeqNo, Id, X, Y FROM Vertices WHERE Owner = @Owner",
			                         CreateParameter("Owner", DbType.Int32)));
			SetCommand(vertexEntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Vertices WHERE Owner = @Owner",
			                         CreateParameter("Owner", DbType.Int32)));
			SetCommand(vertexEntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand("INSERT INTO Vertices (Owner, SeqNo, Id, X, Y) VALUES (@Owner, @SeqNo, @Id, @X, @Y)",
			                         CreateParameter("Owner", DbType.Int32),
			                         CreateParameter("SeqNo", DbType.Int32),
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("X", DbType.Int32),
			                         CreateParameter("Y", DbType.Int32)));
			SetCommand(connectionPointEntityTypeName, RepositoryCommandType.SelectById,
			           CreateCommand("SELECT SeqNo, Id, A, B, C FROM ConnectionPoints WHERE Owner = @Owner",
			                         CreateParameter("Owner", DbType.Int32)));
			SetCommand(connectionPointEntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM ConnectionPoints WHERE Owner = @Owner",
			                         CreateParameter("Owner", DbType.Int32)));
			SetCommand(connectionPointEntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(
			           	"INSERT INTO ConnectionPoints (Owner, SeqNo, Id, A, B, C) VALUES (@Owner, @SeqNo, @Id, @A, @B, @C)",
			           	CreateParameter("Owner", DbType.Int32),
			           	CreateParameter("SeqNo", DbType.Int32),
			           	CreateParameter("Id", DbType.Int32),
			           	CreateParameter("A", DbType.Int32),
			           	CreateParameter("B", DbType.Int32),
			           	CreateParameter("C", DbType.Int32)));
		}


		private void CreateModelCommands()
		{
			SetCommand(Model.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand("SELECT Id, Project FROM Model WHERE Project = @Project",
			                         CreateParameter("Project", DbType.Int32)));
			SetCommand(Model.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand("INSERT INTO Model (Project) VALUES (@Project); SELECT CAST(IDENT_CURRENT('Model') AS INT)",
			                         CreateParameter("Project", DbType.Int32)));
			SetCommand(Model.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand("UPDATE Model SET Project = @Project WHERE Id = @Id",
			                         CreateParameter("Project", DbType.Int32)));
			SetCommand(Model.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Model WHERE Project = @Project",
			                         CreateParameter("Project", DbType.Int32)));
		}


		private void CreateModelObjectCommands(IStoreCache storeCache)
		{
			// === Generic Shape Commands ===
			// SELECT by parent id command
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.ModelObject) continue;
				StringBuilder selectCmdText = new StringBuilder();
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						selectCmdText.Append(", M.");
						selectCmdText.Append(pi.Name);
					}
				}
				IDbCommand selectModelModelObjCmd = CreateCommand(
					string.Format(
						"SELECT ModelModelObject.ModelObject, Model{0} FROM [{1}] M JOIN ModelModelObject ON M.Id = ModelModelObject.ModelObject WHERE ModelModelObject.Model = @Model",
						selectCmdText.ToString(), SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Model", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.SelectModelModelObjects, selectModelModelObjCmd);
				IDbCommand selectTemplateModelObjCmd = CreateCommand(
					string.Format(
						"SELECT TM.ModelObject, TM.Template{0} FROM [{1}] M JOIN TemplateModelObject TM ON M.Id = TM.ModelObject JOIN Template T ON TM.Template = T.Id WHERE T.Project = @Project",
						selectCmdText.ToString(), SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Project", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.SelectTemplateModelObjects, selectTemplateModelObjCmd);
				IDbCommand selectChildModelObjCmd = CreateCommand(
					string.Format(
						"SELECT ChildModelObject.ModelObject, Parent{0} FROM [{1}] M JOIN ChildModelObject ON M.Id = ChildModelObject.ModelObject WHERE ChildModelObject.Parent = @Parent",
						selectCmdText.ToString(), SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Parent", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.SelectChildModelObjects, selectChildModelObjCmd);
			}
			// INSERT commands
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.ModelObject) continue;
				IDbCommand insertModelObjectCmd = CreateCommand();
				StringBuilder insertModelObjectCmdText1 = new StringBuilder();
				StringBuilder insertModelObjectCmdText2 = new StringBuilder();
				insertModelObjectCmdText1.Append("Id");
				insertModelObjectCmdText2.Append("@Ident");
				insertModelObjectCmd.Parameters.Add(CreateParameter("Parent", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						insertModelObjectCmdText1.Append(", ");
						insertModelObjectCmdText1.Append(pi.Name);
						insertModelObjectCmdText2.Append(", ");
						insertModelObjectCmdText2.Append("@" + pi.Name);
						insertModelObjectCmd.Parameters.Add(CreateParameter(pi.Name,
						                                                    DbTypeForDotNetType(((EntityFieldDefinition) pi).Type)));
					}
					else if (IsComposition(pi)) {
						insertModelObjectCmdText1.Append(", ");
						insertModelObjectCmdText1.Append(pi.Name);
						insertModelObjectCmdText2.Append(", ");
						insertModelObjectCmdText2.Append("@" + pi.Name);
						insertModelObjectCmd.Parameters.Add(CreateParameter(pi.Name, DbType.String));
					}
					else Debug.Fail("Unexpected inner objects type in CreateDbCommands.");
				}
				IDbCommand insertModelModelObjectCmd = (IDbCommand) ((ICloneable) insertModelObjectCmd).Clone();
				insertModelModelObjectCmd.CommandText = string.Format("DECLARE @Ident INT; "
				                                                      +
				                                                      "INSERT INTO ModelObject DEFAULT VALUES; SET @Ident = @@IDENTITY; "
				                                                      +
				                                                      "INSERT INTO ModelModelObject (Model, ModelObject) VALUES (@Parent, @Ident); "
				                                                      + "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @Ident",
				                                                      SqlTableNameForEntityName(et.FullName),
				                                                      insertModelObjectCmdText1.ToString(),
				                                                      insertModelObjectCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.InsertModelModelObject, insertModelModelObjectCmd);
				IDbCommand insertTemplateModelObjectCmd = (IDbCommand) ((ICloneable) insertModelObjectCmd).Clone();
				insertTemplateModelObjectCmd.CommandText = string.Format("DECLARE @Ident INT; "
				                                                         +
				                                                         "INSERT INTO ModelObject DEFAULT VALUES; SET @Ident = @@IDENTITY; "
				                                                         +
				                                                         "INSERT INTO TemplateModelObject (Template, ModelObject) VALUES (@Parent, @Ident); "
				                                                         + "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @Ident",
				                                                         SqlTableNameForEntityName(et.FullName),
				                                                         insertModelObjectCmdText1.ToString(),
				                                                         insertModelObjectCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.InsertTemplateModelObject, insertTemplateModelObjectCmd);
				IDbCommand insertChildModelObjectCmd = (IDbCommand) ((ICloneable) insertModelObjectCmd).Clone();
				insertChildModelObjectCmd.CommandText = string.Format("DECLARE @Ident INT; "
				                                                      +
				                                                      "INSERT INTO ModelObject DEFAULT VALUES; SET @Ident = @@IDENTITY; "
				                                                      +
				                                                      "INSERT INTO ChildModelObject (Parent, ModelObject) VALUES (@Parent, @Ident); "
				                                                      + "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @Ident",
				                                                      SqlTableNameForEntityName(et.FullName),
				                                                      insertModelObjectCmdText1.ToString(),
				                                                      insertModelObjectCmdText2.ToString());
				SetCommand(et.FullName, RepositoryCommandType.InsertChildModelObject, insertChildModelObjectCmd);
			}
			// UPDATE commands
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.ModelObject) continue;
				IDbCommand updateModelObjectCmd = CreateCommand();
				StringBuilder cmdText = new StringBuilder();
				cmdText.AppendFormat("UPDATE [{0}] SET ", SqlTableNameForEntityName(et.FullName));
				// Id must be first parameter because it is written first by the writer client.
				updateModelObjectCmd.Parameters.Add(CreateParameter("Id", DbType.Int32));
				foreach (EntityPropertyDefinition pi in et.PropertyDefinitions) {
					if (pi is EntityFieldDefinition) {
						cmdText.AppendFormat("[{0}] = @{0}, ", pi.Name);
						updateModelObjectCmd.Parameters.Add(CreateParameter(pi.Name,
						                                                    DbTypeForDotNetType(((EntityFieldDefinition) pi).Type)));
					}
					else if (IsComposition(pi)) {
						cmdText.AppendFormat("[{0}] = @{0}, ", pi.Name);
						updateModelObjectCmd.Parameters.Add(CreateParameter(pi.Name, DbType.String));
					}
					else Debug.Fail("Unexpected inner objects type in CreateDbCommands.");
				}
				cmdText.Length -= 2; // RemoveRange last comma + space
				cmdText.Append(" WHERE Id = @Id");
				updateModelObjectCmd.CommandText = cmdText.ToString();
				SetCommand(et.FullName, RepositoryCommandType.Update, updateModelObjectCmd);
			}
			SetCommand("Core.ModelObject", RepositoryCommandType.UpdateOwnerModel,
			           CreateCommand(
			           	"DELETE FROM ModelModelObject WHERE ModelObject = @Id; DELETE FROM ChildModelObject WHERE ModelObject = @Id; "
			           	+ "INSERT INTO ModelModelObject (Model, ModelObject) VALUES (@Model, @Id)",
			           	CreateParameter("Id", DbType.Int32),
			           	CreateParameter("Model", DbType.Int32)));
			SetCommand("Core.ModelObject", RepositoryCommandType.UpdateOwnerModelObject,
			           CreateCommand(
			           	"DELETE FROM ModelModelObject WHERE ModelObject = @Id; DELETE FROM ChildModelObject WHERE ModelObject = @Id; "
			           	+ "INSERT INTO ChildModelObject (Parent, ModelObject) VALUES (@Parent, @Id)",
			           	CreateParameter("Id", DbType.Int32),
			           	CreateParameter("Parent", DbType.Int32)));
			//
			// DELETE command
			foreach (IEntityType et in storeCache.EntityTypes) {
				if (et.Category != EntityCategory.ModelObject) continue;
				IDbCommand deleteModelObjectCommand = CreateCommand(
					string.Format("DELETE FROM ModelModelObject WHERE ModelObject = @Id; "
					              + "DELETE FROM TemplateModelObject WHERE ModelObject = @Id; "
					              + "DELETE FROM ChildModelObject WHERE ModelObject = @Id; "
					              + "DELETE FROM ModelObject WHERE Id = @Id; "
					              + "DELETE FROM [{0}] WHERE Id = @Id",
					              SqlTableNameForEntityName(et.FullName)),
					CreateParameter("Id", DbType.Int32));
				SetCommand(et.FullName, RepositoryCommandType.Delete, deleteModelObjectCommand);
			}
		}


		private void CreateTemplateCommands()
		{
			SetCommand(Template.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand("SELECT Id, Project, Name, Title, Description, ConnectionPointMappings"
			                         + " FROM Template "
			                         + "WHERE Project = @Project",
			                         CreateParameter("Project", DbType.Int32)));
			SetCommand(Template.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand("INSERT INTO Template (Project, Name, Title, Description, ConnectionPointMappings) "
			                         + "VALUES (@Project, @Name, @Title, @Description, @ConnectionPointMappings); "
			                         + "SELECT CAST(IDENT_CURRENT('Template') AS INT)",
			                         CreateParameter("Project", DbType.Int32),
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Title", DbType.String),
			                         CreateParameter("Description", DbType.String),
			                         CreateParameter("ConnectionPointMappings", DbType.String)));
			SetCommand(Template.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand("UPDATE Template SET Name = @Name, Title = @Title, "
			                         + "Description = @Description, ConnectionPointMappings = @ConnectionPointMappings "
			                         + "WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32),
			                         CreateParameter("Name", DbType.String),
			                         CreateParameter("Title", DbType.String),
			                         CreateParameter("Description", DbType.String),
			                         CreateParameter("ConnectionPointMappings", DbType.String)
			           	)
				);
			SetCommand(Template.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand("DELETE FROM Template WHERE Id = @Id",
			                         CreateParameter("Id", DbType.Int32)
			           	)
				);
		}


		private void CreateModelMappingCommands()
		{
			string tableName = string.Empty;
			string InsertModelMappingId = "DECLARE @Ident INT; INSERT INTO ModelMapping "
			                              + "(Template) VALUES (@TemplateId); SET @Ident = @@Identity; ";
			string ReturnModelMappingId = "SELECT @Ident";

			// === NumericModelMapping Commands ===
			tableName = SqlTableNameForEntityName(NumericModelMapping.EntityTypeName);
			SetCommand(NumericModelMapping.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand(string.Format("SELECT Id, Template, ShapePropertyId, ModelPropertyId, MappingType, "
			                                       +
			                                       "Intercept, Slope FROM [{0}] JOIN ModelMapping ON [{0}].ModelMapping = ModelMapping.Id "
			                                       + "WHERE Template = @Template", tableName),
			                         CreateParameter("@Template", DbType.Int32)));
			SetCommand(NumericModelMapping.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(string.Format(InsertModelMappingId + "INSERT INTO [{0}] (ModelMapping, ShapePropertyId, "
			                                       +
			                                       "ModelPropertyId, MappingType, Intercept, Slope) VALUES (@Ident, @ShapePropertyId, "
			                                       + "@ModelPropertyId, @MappingType, @Intercept, @Slope);" +
			                                       ReturnModelMappingId, tableName),
			                         CreateParameter("@TemplateId", DbType.Int32),
			                         CreateParameter("@ShapePropertyId", DbType.Int32),
			                         CreateParameter("@ModelPropertyId", DbType.Int32),
			                         CreateParameter("@MappingType", DbType.Int32),
			                         CreateParameter("@Intercept", DbType.Single),
			                         CreateParameter("@Slope", DbType.Single)));
			SetCommand(NumericModelMapping.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand(string.Format("UPDATE [{0}] SET "
			                                       + "ShapePropertyId = @ShapePropertyId, ModelPropertyId = @ModelPropertyId, "
			                                       + "MappingType = @MappingType, Intercept = @Intercept, Slope = @Slope "
			                                       + "WHERE ModelMapping = @Id", tableName),
			                         CreateParameter("@Id", DbType.Int32),
			                         CreateParameter("@ShapePropertyId", DbType.Int32),
			                         CreateParameter("@ModelPropertyId", DbType.Int32),
			                         CreateParameter("@MappingType", DbType.Int32),
			                         CreateParameter("@Intercept", DbTypeForDotNetType(typeof (float))),
			                         CreateParameter("@Slope", DbType.Single)
			           	)
				);
			SetCommand(NumericModelMapping.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand(string.Format("DELETE FROM [{0}] WHERE ModelMapping = @Id;"
			                                       + "DELETE FROM [{1}] WHERE Id = @Id;", tableName, "ModelMapping"),
			                         CreateParameter("@Id", DbType.Int32)
			           	)
				);
			//
			// === FormatModelMapping Commands ===
			tableName = SqlTableNameForEntityName(FormatModelMapping.EntityTypeName);
			SetCommand(FormatModelMapping.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand(string.Format("SELECT Id, Template, ShapePropertyId, ModelPropertyId, MappingType, "
			                                       +
			                                       "Format FROM [{0}] JOIN ModelMapping ON [{0}].ModelMapping = ModelMapping.Id "
			                                       + "WHERE Template = @Template", tableName),
			                         CreateParameter("@Template", DbType.Int32)));
			SetCommand(FormatModelMapping.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(string.Format(InsertModelMappingId +
			                                       "INSERT INTO [{0}] (ModelMapping, ShapePropertyId, ModelPropertyId, MappingType, Format) "
			                                       +
			                                       "VALUES (@Ident, @ShapePropertyId, @ModelPropertyId, @MappingType, @Format); "
			                                       + ReturnModelMappingId, tableName),
			                         CreateParameter("@TemplateId", DbType.Int32),
			                         CreateParameter("@ShapePropertyId", DbType.Int32),
			                         CreateParameter("@ModelPropertyId", DbType.Int32),
			                         CreateParameter("@MappingType", DbType.Int32),
			                         CreateParameter("@Format", DbType.String)));
			SetCommand(FormatModelMapping.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand(string.Format("UPDATE [{0}] SET "
			                                       + "ShapePropertyId = @ShapePropertyId, ModelPropertyId = @ModelPropertyId, "
			                                       + "MappingType = @MappingType, Format = @Format "
			                                       + "WHERE ModelMapping = @Id", tableName),
			                         CreateParameter("@Id", DbType.Int32),
			                         CreateParameter("@ShapePropertyId", DbType.Int32),
			                         CreateParameter("@ModelPropertyId", DbType.Int32),
			                         CreateParameter("@MappingType", DbType.Int32),
			                         CreateParameter("@Format", DbType.String)
			           	)
				);
			SetCommand(FormatModelMapping.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand(string.Format("DELETE FROM [{0}] WHERE ModelMapping = @Id;"
			                                       + "DELETE FROM [{1}] WHERE Id = @Id;", tableName, "ModelMapping"),
			                         CreateParameter("@Id", DbType.Int32)
			           	)
				);
			//
			// StyleModelMapping Commands
			tableName = SqlTableNameForEntityName(StyleModelMapping.EntityTypeName);
			SetCommand(StyleModelMapping.EntityTypeName, RepositoryCommandType.SelectByOwnerId,
			           CreateCommand(string.Format("SELECT Id, Template, ShapePropertyId, ModelPropertyId, MappingType, "
			                                       +
			                                       "DefaultStyleType, DefaultStyle, ValueRanges FROM [{0}] JOIN ModelMapping ON [{0}].ModelMapping = ModelMapping.Id "
			                                       + "WHERE Template = @Template", tableName),
			                         CreateParameter("@Template", DbType.Int32)));
			SetCommand(StyleModelMapping.EntityTypeName, RepositoryCommandType.Insert,
			           CreateCommand(string.Format(InsertModelMappingId
			                                       + "INSERT INTO [{0}] (ModelMapping, ShapePropertyId, ModelPropertyId, "
			                                       + "MappingType, DefaultStyleType, DefaultStyle, ValueRanges) VALUES ("
			                                       +
			                                       "@Ident, @ShapePropertyId, @ModelPropertyId, @MappingType, @DefaultStyleType, "
			                                       + "@DefaultStyle, @ValueRanges);" + ReturnModelMappingId, tableName),
			                         CreateParameter("@TemplateId", DbType.Int32),
			                         CreateParameter("@ShapePropertyId", DbType.Int32),
			                         CreateParameter("@ModelPropertyId", DbType.Int32),
			                         CreateParameter("@MappingType", DbType.Int32),
			                         CreateParameter("@DefaultStyleType", DbType.Int32),
			                         CreateParameter("@DefaultStyle", DbType.Int32),
			                         CreateParameter("@ValueRanges", DbType.String)
			           	)
				);
			SetCommand(StyleModelMapping.EntityTypeName, RepositoryCommandType.Update,
			           CreateCommand(string.Format("UPDATE [{0}] SET "
			                                       + "ShapePropertyId = @ShapePropertyId, ModelPropertyId = @ModelPropertyId, "
			                                       +
			                                       "MappingType = @MappingType, DefaultStyleType = @DefaultStyleType, DefaultStyle = @DefaultStyle, ValueRanges = @ValueRanges "
			                                       + "WHERE ModelMapping = @Id", tableName),
			                         CreateParameter("@Id", DbType.Int32),
			                         CreateParameter("@ShapePropertyId", DbType.Int32),
			                         CreateParameter("@ModelPropertyId", DbType.Int32),
			                         CreateParameter("@MappingType", DbType.Int32),
			                         CreateParameter("@DefaultStyleType", DbType.Int32),
			                         CreateParameter("@DefaultStyle", DbType.Int32),
			                         CreateParameter("@ValueRanges", DbType.String)
			           	)
				);
			SetCommand(StyleModelMapping.EntityTypeName, RepositoryCommandType.Delete,
			           CreateCommand(string.Format("DELETE FROM [{0}] WHERE ModelMapping = @Id;"
			                                       + "DELETE FROM [{1}] WHERE Id = @Id;", tableName, "ModelMapping"),
			                         CreateParameter("@Id", DbType.Int32)
			           	)
				);
		}


		private string serverName;
		private string databaseName;

		// Sizing parameters for command storage tables
		private const int ColumnSizeSysCmdKind = 40;
		private const int ColumnSizeSysCmdEntityType = 120;
		private const int ColumnSizeSysCmdCommandText = int.MaxValue;
	}
}