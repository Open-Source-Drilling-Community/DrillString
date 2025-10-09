using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace NORCE.Drilling.DrillString.Service.Managers
{
    /// <summary>
    /// A manager for DrillString. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class DrillStringManager
    {
        private static DrillStringManager? _instance = null;
        private readonly ILogger<DrillStringManager> _logger;
        private readonly object _lock = new();
        private readonly SqlConnectionManager _connectionManager;

        private DrillStringManager(ILogger<DrillStringManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _connectionManager = connectionManager;
        }

        public static DrillStringManager GetInstance(ILogger<DrillStringManager> logger, SqlConnectionManager connectionManager)
        {
            _instance ??= new DrillStringManager(logger, connectionManager);
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
                    command.CommandText = "SELECT COUNT(*) FROM DrillStringTable";
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
                        _logger.LogError(ex, "Impossible to count records in the DrillStringTable");
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
                        //empty DrillStringTable
                        var command = connection.CreateCommand();
                        command.CommandText = "DELETE FROM DrillStringTable";
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        success = true;
                    }
                    catch (SqliteException ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Impossible to clear the DrillStringTable");
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
                command.CommandText = $"SELECT COUNT(*) FROM DrillStringTable WHERE ID = '{guid}'";
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
                    _logger.LogError(ex, "Impossible to count rows from DrillStringTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return count >= 1;
        }

        /// <summary>
        /// Returns the list of Guid of all DrillString present in the microservice database 
        /// </summary>
        /// <returns>the list of Guid of all DrillString present in the microservice database</returns>
        public List<Guid>? GetAllDrillStringId()
        {
            List<Guid> ids = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM DrillStringTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        Guid id = reader.GetGuid(0);
                        ids.Add(id);
                    }
                    _logger.LogInformation("Returning the list of ID of existing records from DrillStringTable");
                    return ids;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from DrillStringTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of MetaInfo of all DrillString present in the microservice database 
        /// </summary>
        /// <returns>the list of MetaInfo of all DrillString present in the microservice database</returns>
        public List<MetaInfo?>? GetAllDrillStringMetaInfo()
        {
            List<MetaInfo?> metaInfos = new();
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MetaInfo FROM DrillStringTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string mInfo = reader.GetString(0);
                        MetaInfo? metaInfo = JsonSerializer.Deserialize<MetaInfo>(mInfo, JsonSettings.Options);
                        metaInfos.Add(metaInfo);
                    }
                    _logger.LogInformation("Returning the list of MetaInfo of existing records from DrillStringTable");
                    return metaInfos;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get IDs from DrillStringTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the DrillString identified by its Guid from the microservice database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the DrillString identified by its Guid from the microservice database</returns>
        public Model.DrillString? GetDrillStringById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    Model.DrillString? drillString;
                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT DrillString FROM DrillStringTable WHERE ID = '{guid}'";
                    try
                    {
                        using var reader = command.ExecuteReader();
                        if (reader.Read() && !reader.IsDBNull(0))
                        {
                            string data = reader.GetString(0);
                            drillString = JsonSerializer.Deserialize<Model.DrillString>(data, JsonSettings.Options);
                            if (drillString != null && drillString.MetaInfo != null && !drillString.MetaInfo.ID.Equals(guid))
                                throw new SqliteException("SQLite database corrupted: returned DrillString is null or has been jsonified with the wrong ID.", 1);
                        }
                        else
                        {
                            _logger.LogInformation("No DrillString of given ID in the database");
                            return null;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to get the DrillString with the given ID from DrillStringTable");
                        return null;
                    }
                    _logger.LogInformation("Returning the DrillString of given ID from DrillStringTable");
                    return drillString;
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The given DrillString ID is null or empty");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of all DrillString present in the microservice database 
        /// </summary>
        /// <returns>the list of all DrillString present in the microservice database</returns>
        public List<Model.DrillString?>? GetAllDrillString()
        {
            List<Model.DrillString?> vals = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT DrillString FROM DrillStringTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string data = reader.GetString(0);
                        Model.DrillString? drillString = JsonSerializer.Deserialize<Model.DrillString>(data, JsonSettings.Options);
                        vals.Add(drillString);
                    }
                    _logger.LogInformation("Returning the list of existing DrillString from DrillStringTable");
                    return vals;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get DrillString from DrillStringTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Returns the list of all DrillStringLight present in the microservice database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the list of DrillStringLight present in the microservice database</returns>
        public List<Model.DrillStringLight>? GetAllDrillStringLight()
        {
            List<Model.DrillStringLight>? drillStringLightList = [];
            var connection = _connectionManager.GetConnection();
            if (connection != null)
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MetaInfo, Name, Description, CreationDate, LastModificationDate, WellBoreID FROM DrillStringTable";
                try
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read() && !reader.IsDBNull(0))
                    {
                        string metaInfoStr = reader.GetString(0);
                        MetaInfo? metaInfo = JsonSerializer.Deserialize<MetaInfo>(metaInfoStr, JsonSettings.Options);
                        string name = reader.GetString(1);
                        string descr = reader.GetString(2);
                        // make sure DateTimeOffset are properly instantiated when stored values are null (and parsed as empty string)
                        DateTimeOffset? creationDate = null;
                        if (DateTimeOffset.TryParse(reader.GetString(3), out DateTimeOffset cDate))
                            creationDate = cDate;
                        DateTimeOffset? lastModificationDate = null;
                        if (DateTimeOffset.TryParse(reader.GetString(4), out DateTimeOffset lDate))
                            lastModificationDate = lDate;
                        Guid? wellBoreID = null;
                        if (Guid.TryParse(reader.GetString(5), out Guid wID))
                            wellBoreID = wID;
                        drillStringLightList.Add(new Model.DrillStringLight(
                                metaInfo,
                                string.IsNullOrEmpty(name) ? null : name,
                                string.IsNullOrEmpty(descr) ? null : descr,
                                creationDate,
                                lastModificationDate,
                                wellBoreID));
                    }
                    _logger.LogInformation("Returning the list of existing DrillStringLight from DrillStringTable");
                    return drillStringLightList;
                }
                catch (SqliteException ex)
                {
                    _logger.LogError(ex, "Impossible to get light datas from DrillStringTable");
                }
            }
            else
            {
                _logger.LogWarning("Impossible to access the SQLite database");
            }
            return null;
        }

        /// <summary>
        /// Performs calculation on the given DrillString and adds it to the microservice database
        /// </summary>
        /// <param name="drillString"></param>
        /// <returns>true if the given DrillString has been added successfully to the microservice database</returns>
        public bool AddDrillString(Model.DrillString? drillString)
        {
            if (drillString != null && drillString.MetaInfo != null && drillString.MetaInfo.ID != Guid.Empty)
            {
                //if successful, check if another parent data with the same ID was calculated/added during the calculation time
                Model.DrillString? newDrillString = GetDrillStringById(drillString.MetaInfo.ID);
                if (newDrillString == null)
                {
                    //update DrillStringTable
                    var connection = _connectionManager.GetConnection();
                    if (connection != null)
                    {
                        using SqliteTransaction transaction = connection.BeginTransaction();
                        bool success = true;
                        try
                        {
                            //add the DrillString to the DrillStringTable
                            string metaInfo = JsonSerializer.Serialize(drillString.MetaInfo, JsonSettings.Options);
                            string? cDate = null;
                            if (drillString.CreationDate != null)
                                cDate = ((DateTimeOffset)drillString.CreationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            string? lDate = null;
                            if (drillString.LastModificationDate != null)
                                lDate = ((DateTimeOffset)drillString.LastModificationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                            string data = JsonSerializer.Serialize(drillString, JsonSettings.Options);
                            var command = connection.CreateCommand();
                            command.CommandText = "INSERT INTO DrillStringTable (" +
                                "ID, " +
                                "MetaInfo, " +
                                "Name, " +
                                "Description, " +
                                "CreationDate, " +
                                "LastModificationDate, " +
                                "WellBoreID, " +
                                "DrillString" +
                                ") VALUES (" +
                                $"'{drillString.MetaInfo.ID}', " +
                                $"'{metaInfo}', " +
                                $"'{drillString.Name}', " +
                                $"'{drillString.Description}', " +
                                $"'{cDate}', " +
                                $"'{lDate}', " +
                                $"'{drillString.WellBoreID}', " +
                                $"'{data}'" +
                                ")";
                            int count = command.ExecuteNonQuery();
                            if (count != 1)
                            {
                                _logger.LogWarning("Impossible to insert the given DrillString into the DrillStringTable");
                                success = false;
                            }
                        }
                        catch (SqliteException ex)
                        {
                            _logger.LogError(ex, "Impossible to add the given DrillString into DrillStringTable");
                            success = false;
                        }
                        //finalizing SQL transaction
                        if (success)
                        {
                            transaction.Commit();
                            _logger.LogInformation("Added the given DrillString of given ID into the DrillStringTable successfully");
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                        return success;
                    }
                    else
                    {
                        _logger.LogWarning("Impossible to access the SQLite database");
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to post DrillString. ID already found in database.");
                    return false;
                }

            }
            else
            {
                _logger.LogWarning("The DrillString ID or the ID of its input are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Performs calculation on the given DrillString and updates it in the microservice database
        /// </summary>
        /// <param name="drillString"></param>
        /// <returns>true if the given DrillString has been updated successfully</returns>
        public bool UpdateDrillStringById(Guid guid, Model.DrillString? drillString)
        {
            bool success = true;
            if (guid != Guid.Empty && drillString != null && drillString.MetaInfo != null && drillString.MetaInfo.ID == guid)
            {
                //update DrillStringTable
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using SqliteTransaction transaction = connection.BeginTransaction();
                    //update fields in DrillStringTable
                    try
                    {
                        string metaInfo = JsonSerializer.Serialize(drillString.MetaInfo, JsonSettings.Options);
                        string? cDate = null;
                        if (drillString.CreationDate != null)
                            cDate = ((DateTimeOffset)drillString.CreationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                        string? lDate = null;
                        if (drillString.LastModificationDate != null)
                            lDate = ((DateTimeOffset)drillString.LastModificationDate).ToString(SqlConnectionManager.DATE_TIME_FORMAT);
                        string data = JsonSerializer.Serialize(drillString, JsonSettings.Options);
                        var command = connection.CreateCommand();
                        command.CommandText = $"UPDATE DrillStringTable SET " +
                            $"MetaInfo = '{metaInfo}', " +
                            $"Name = '{drillString.Name}', " +
                            $"Description = '{drillString.Description}', " +
                            $"CreationDate = '{cDate}', " +
                            $"LastModificationDate = '{lDate}', " +
                            $"WellBoreID = '{drillString.WellBoreID}', " +
                            $"DrillString = '{data}' " +
                            $"WHERE ID = '{guid}'";
                        int count = command.ExecuteNonQuery();
                        if (count != 1)
                        {
                            _logger.LogWarning("Impossible to update the DrillString");
                            success = false;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to update the DrillString");
                        success = false;
                    }

                    // Finalizing
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Updated the given DrillString successfully");
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The DrillString ID or the ID of some of its attributes are null or empty");
            }
            return false;
        }

        /// <summary>
        /// Deletes the DrillString of given ID from the microservice database
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the DrillString was deleted from the microservice database</returns>
        public bool DeleteDrillStringById(Guid guid)
        {
            if (!guid.Equals(Guid.Empty))
            {
                var connection = _connectionManager.GetConnection();
                if (connection != null)
                {
                    using var transaction = connection.BeginTransaction();
                    bool success = true;
                    //delete DrillString from DrillStringTable
                    try
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = $"DELETE FROM DrillStringTable WHERE ID = '{guid}'";
                        int count = command.ExecuteNonQuery();
                        if (count < 0)
                        {
                            _logger.LogWarning("Impossible to delete the DrillString of given ID from the DrillStringTable");
                            success = false;
                        }
                    }
                    catch (SqliteException ex)
                    {
                        _logger.LogError(ex, "Impossible to delete the DrillString of given ID from DrillStringTable");
                        success = false;
                    }
                    if (success)
                    {
                        transaction.Commit();
                        _logger.LogInformation("Removed the DrillString of given ID from the DrillStringTable successfully");
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                    return success;
                }
                else
                {
                    _logger.LogWarning("Impossible to access the SQLite database");
                }
            }
            else
            {
                _logger.LogWarning("The DrillString ID is null or empty");
            }
            return false;
        }
    }
}