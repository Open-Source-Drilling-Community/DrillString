using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using NORCE.Drilling.DrillString.Model;

namespace NORCE.Drilling.DrillString.Service.Managers
{
    /// <summary>
    /// A manager for DrillStringComponent. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class DrillStringComponentManager
    {
        private static DrillStringComponentManager? _instance = null;
        private readonly ILogger<DrillStringComponentManager> _logger;
        private readonly object _lock = new();
        private readonly SqlConnectionManager _connectionManager;

        private DrillStringComponentManager(ILogger<DrillStringComponentManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        public static DrillStringComponentManager GetInstance(ILogger<DrillStringComponentManager> logger, SqlConnectionManager connectionManager)
        {
            _instance ??= new DrillStringComponentManager(logger, connectionManager);
            return _instance;
        }

        public int Count
        {
            get
            {
                int count = 0;
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(*) FROM DrillStringComponentTable";
                    try
                    {
                        using SqliteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            count = (int)reader.GetInt64(0);
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to count records in the DrillStringComponentTable");
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
                return count;
            }
        }

        public bool Clear()
        {
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                bool success = false;
                lock (_lock)
                {
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        //empty DrillStringComponentTable
                        var command = connection.CreateCommand();
                        command.CommandText = "DELETE FROM DrillStringComponentTable";
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        success = true;
                    }
                    catch (SqliteException ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Impossible to clear the DrillStringComponentTable");
                    }
                }
                return success;
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
                return false;
            }
        }

        public bool Contains(Guid guid)
        {
            int count = 0;
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT COUNT(*) FROM DrillStringComponentTable WHERE ID = ' {guid}'";
                try
                {
                    using SqliteDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        count = (int)reader.GetInt64(0);
                    }
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to count rows from DrillStringComponentTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return count >= 1;
        }

        /// <summary>
        /// Returns the list of Guid of all DrillStringComponent present in the microservice database 
        /// </summary>
        /// <returns>the list of Guid of all DrillStringComponent present in the microservice database</returns>
        public List<Guid>? GetAllDrillStringComponentId()
        {
            List<Guid> ids = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM DrillStringComponentTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        Guid id = reader.GetGuid(0);
                        ids.Add(id);
                    }
                    _logger.LogInformation("Returning the list of ID of existing records from DrillStringComponentTable");
                    return ids;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from DrillStringComponentTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of MetaInfo of all DrillStringComponent present in the microservice database 
        /// </summary>
        /// <returns>the list of MetaInfo of all DrillStringComponent present in the microservice database</returns>
        public List<MetaInfo?>? GetAllDrillStringComponentMetaInfo()
        {
            List<MetaInfo?> metaInfos = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MetaInfo FROM DrillStringComponentTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string mInfo = reader.GetString(0);
                        MetaInfo? metaInfo = JsonSerializer.Deserialize<MetaInfo>(mInfo, JsonSettings.Options);
                        metaInfos.Add(metaInfo);
                    }
                    _logger.LogInformation("Returning the list of MetaInfo of existing records from DrillStringComponentTable");
                    return metaInfos;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from DrillStringComponentTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns a DrillStringComponent identified by its Guid from the microservice database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the DrillStringComponent retrieved from the database</returns>
        public DrillStringComponent? GetDrillStringComponentById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    DrillStringComponent? drillStringComponent = null;
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT DrillStringComponent FROM DrillStringComponentTable WHERE ID = '{guid}'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string data = reader.GetString(0);
                            drillStringComponent = JsonSerializer.Deserialize<DrillStringComponent>(data, JsonSettings.Options);
                            if (drillStringComponent != null  && !drillStringComponent.MetaInfo.ID.Equals(guid))
                                throw new SqliteException("SQLite database corrupted: retrieved DrillStringComponent is null or has been jsonified with the wrong ID.", 1);
                        }
                        else
                        {
                            _logger.LogInformation("No DrillStringComponent of given ID in the database");
                            return null;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to get the DrillStringComponent with the given ID from DrillStringComponentTable");
                        return null;
                    }

                    // Finalizing
                    _logger.LogInformation("Returning the DrillStringComponent of given ID from DrillStringComponentTable");
                    return drillStringComponent;
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The given DrillStringComponent ID is null or empty");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of all DrillStringComponent present in the microservice database 
        /// </summary>
        /// <returns>the list of all DrillStringComponent present in the microservice database</returns>
        public List<DrillStringComponent?>? GetAllDrillStringComponent()
        {
            List<DrillStringComponent?> vals = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT DrillStringComponent FROM DrillStringComponentTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string data = reader.GetString(0);
                        DrillStringComponent? drillStringComponent = JsonSerializer.Deserialize<DrillStringComponent>(data, JsonSettings.Options);
                        vals.Add(drillStringComponent);
                    }
                    _logger.LogInformation("Returning the list of existing DrillStringComponent from DrillStringComponentTable");
                    return vals;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get DrillStringComponent from DrillStringComponentTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Adds the given DrillStringComponent to the microservice database
        /// </summary>
        /// <param name="drillStringComponent"></param>
        /// <returns>true if the given DrillStringComponent has been added successfully</returns>
        public bool AddDrillStringComponent(DrillStringComponent? drillStringComponent)
        {
            if (drillStringComponent != null && drillStringComponent.MetaInfo.ID != Guid.Empty)
            {
                //update DrillStringComponentTable
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    lock (_lock)
                    {
                        using SqliteTransaction transaction = connection.BeginTransaction();
                        bool success = true;
                        try
                        {
                            string metaInfo = JsonSerializer.Serialize(drillStringComponent.MetaInfo, JsonSettings.Options);
                            //add the DrillStringComponent to the DrillStringComponentTable
                            string data = JsonSerializer.Serialize(drillStringComponent, JsonSettings.Options);
                            string? cDate = null;
                            if (drillStringComponent.CreationDate != null)
                                cDate = ((DateTimeOffset)drillStringComponent.CreationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            string? lDate = null;
                            if (drillStringComponent.LastModificationDate != null)
                                lDate = ((DateTimeOffset)drillStringComponent.LastModificationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);

                            var command = connection.CreateCommand();
                            command.CommandText = "INSERT INTO DrillStringComponentTable (" +
                                "ID, " +
                                "MetaInfo, " +
                                "Name, " +
                                "Description, " +
                                "CreationDate, " +
                                "LastModificationDate, " +
                                "DrillStringComponent" +
                                ") VALUES (" +
                                $"'{drillStringComponent.MetaInfo.ID}', " +
                                $"'{metaInfo}'," +
                                $"'{drillStringComponent.Name}'," +
                                $"'{drillStringComponent.Description}'," +
                                $"'{cDate}', " +
                                $"'{lDate}', " +
                                $"'{data}'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                _logger.LogWarning("Impossible to insert the given DrillStringComponent into the DrillStringComponentTable");
                                success = false;
                            }
                        }
                        catch (SqliteException ex)
                        {
                            _logger.LogError(ex, "Impossible to add the given DrillStringComponent into DrillStringComponentTable");
                            success = false;
                        }
                        //finalizing
                        if (success)
                        {
                            transaction.Commit();
                            _logger.LogInformation("Added the given DrillStringComponent of given ID into the DrillStringComponentTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        return success;
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The DrillStringComponent ID or the ID of its input are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Performs calculation on the given DrillStringComponent and updates it in the microservice database
        /// </summary>
        /// <param name="drillStringComponent"></param>
        /// <returns>true if the given DrillStringComponent has been updated successfully</returns>
        public bool UpdateDrillStringComponentById(Guid guid, DrillStringComponent? drillStringComponent)
        {
            bool success = true;
            if (guid != Guid.Empty && drillStringComponent != null && drillStringComponent.MetaInfo.ID == guid)
            {
                //update DrillStringComponentTable
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    lock (_lock)
                    {
                        using SqliteTransaction transaction = connection.BeginTransaction();
                        //update fields in DrillStringComponentTable
                        try
                        {
                            string metaInfo = JsonSerializer.Serialize(drillStringComponent.MetaInfo, JsonSettings.Options);
                            string data = JsonSerializer.Serialize(drillStringComponent, JsonSettings.Options);
                            string? cDate = null;
                            if (drillStringComponent.CreationDate != null)
                                cDate = ((DateTimeOffset)drillStringComponent.CreationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            string? lDate = null;
                            if (drillStringComponent.LastModificationDate != null)
                                lDate = ((DateTimeOffset)drillStringComponent.LastModificationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            var command = connection.CreateCommand();
                        
                            command.CommandText = $"UPDATE DrillStringComponentTable SET " +                     
                                $"MetaInfo = '{metaInfo}', " +
                                $"Name = '{drillStringComponent.Name}', " +
                                $"Description = '{drillStringComponent.Description}', " +
                                $"CreationDate = '{cDate}', " +
                                $"LastModificationDate = '{lDate}', " +
                                $"DrillStringComponent = '{data}' " +
                                $"WHERE ID = '{guid}'";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                _logger.LogWarning("Impossible to update the DrillStringComponent");
                                success = false;
                            }
                        }
                        catch (SqliteException ex)
                        {
                            _logger.LogError(ex, "Impossible to update the DrillStringComponent");
                            success = false;
                        }

                        // Finalizing
                        if (success)
                        {
                            transaction.Commit();
                            _logger.LogInformation("Updated the given DrillStringComponent successfully");
                            return true;
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The DrillStringComponent ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Deletes the DrillStringComponent of given ID from the microservice database
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the DrillStringComponent was deleted from the microservice database</returns>
        public bool DeleteDrillStringComponentById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    lock (_lock)
                    {
                        using var transaction = connection.BeginTransaction();
                        bool success = true;
                        //delete DrillStringComponent from DrillStringComponentTable
                        try
                        {
                            var command = connection.CreateCommand();
                            command.CommandText = $"DELETE FROM DrillStringComponentTable WHERE ID = '{guid}'";
                            int count = command.ExecuteNonQuery();
                            if (count < 0)
                            {
                                _logger.LogWarning("Impossible to delete the DrillStringComponent of given ID from the DrillStringComponentTable");
                                success = false;
                            }
                        }
                        catch (SqliteException ex)
                        {
                            _logger.LogError(ex, "Impossible to delete the DrillStringComponent of given ID from DrillStringComponentTable");
                            success = false;
                        }
                        if (success)
                        {
                            transaction.Commit();
                            _logger.LogInformation("Removed the DrillStringComponent of given ID from the DrillStringComponentTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        return success;
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The DrillStringComponent ID is null or empty");
            }
            return false;
        }
    }
}