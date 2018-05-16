using System;
using System.Data.SqlClient;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class UpdateProcessService : IUpdateProcessService
    {
        public UpdateProcessService(IUnitOfWorkCataprom unitOfWork,
            ICatapromApplicationEnvironment applicationEnvironment)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
            _connectionString = applicationEnvironment.GetConnectionString();
        }

        public void Update()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    var query = $"UPDATE tbl_rActualizacion SET fechaActualizacion=GETDATE()";
                    command.CommandText = query;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error on update UpdateDate in tbl_rActualizacion | Exception: {ex.ToJson()}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        #region Properties

        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;
        private readonly string _connectionString;

        #endregion
    }
}