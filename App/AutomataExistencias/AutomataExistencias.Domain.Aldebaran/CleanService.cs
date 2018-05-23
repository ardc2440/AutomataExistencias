using System;
using System.Data;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Core.Contract;
using FirebirdSql.Data.FirebirdClient;
using NLog;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class CleanService : ICleanService
    {
        public CleanService(IUnitOfWorkAldebaran unitOfWork, IAldebaranApplicationEnvironment applicationEnvironment)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
            _connectionString = applicationEnvironment.GetConnectionString();
        }

        public void Clean(int daysToKeep)
        {
            using (var connection = new FbConnection(_connectionString))
            {
                using (var command = new FbCommand("BORRARINTEGRACION", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NUMERODIAS", daysToKeep);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error on clean SyncTables | Exception: {ex.ToJson()}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;
        private readonly Logger _logger;
        private readonly string _connectionString;

        #endregion
    }
}