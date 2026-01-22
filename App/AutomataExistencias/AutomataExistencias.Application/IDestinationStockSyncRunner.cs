namespace AutomataExistencias.Application
{
    public interface ICatapromDestinationRunner
    {
        /// <summary>
        /// Ejecuta una acción para cada destino Cataprom activo, proveyendo el IUnitOfWorkCataprom correspondiente.
        /// </summary>
        /// <param name="action">Acción a ejecutar por destino.</param>
        void RunForAllDestinations(System.Action<AutomataExistencias.DataAccess.Core.Contract.IUnitOfWorkCataprom> action);
    }
}
