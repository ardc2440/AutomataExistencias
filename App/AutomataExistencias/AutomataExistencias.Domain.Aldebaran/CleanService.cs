using System;
using System.Data;
using System.Data.SqlClient;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Core.Contract;
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
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SP_BORRAR_INTEGRACION", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DAYS_NUMBER", daysToKeep);
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