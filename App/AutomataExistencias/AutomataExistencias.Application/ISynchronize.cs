namespace AutomataExistencias.Application
{
    public interface ISynchronize
    {
        void StockSync();
        void ItemsSync();
        void ItemsByColorSync();
        void LinesSync();
        void MoneySync();
        void TransitOrderSync();
        void UnitMeasuredSync();
        void UpdateProcess();
    }
}
