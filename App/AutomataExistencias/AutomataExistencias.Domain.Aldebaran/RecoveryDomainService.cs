using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class RecoveryDomainService : IRecoveryDomainService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;

        public RecoveryDomainService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<int> GetCandidateItemIds(int syncAttempts)
        {
            var set = new System.Collections.Generic.HashSet<int>();

            var items = _unitOfWork.Repository<Item>().Get(w => w.Attempts >= syncAttempts && w.Exception != null).Select(s => s.ItemId);
            foreach (var id in items) if (id > 0) set.Add(id);

            var bycolors = _unitOfWork.Repository<ItemByColor>().Get(w => w.Attempts >= syncAttempts && w.Exception != null).Select(s => s.ItemId);
            foreach (var id in bycolors)
            {
                if (id.HasValue && id.Value > 0) set.Add(id.Value);
            }

            var stocks = _unitOfWork.Repository<Stock>().Get(w => w.Attempts >= syncAttempts && w.Exception != null).Select(s => s.ItemId);
            foreach (var id in stocks) if (id > 0) set.Add(id);

            var packs = _unitOfWork.Repository<Packaging>().Get(w => w.Attempts >= syncAttempts && w.Exception != null).Select(s => s.ItemId);
            foreach (var id in packs) if (id.HasValue && id.Value > 0) set.Add(id.Value);

            var trans = _unitOfWork.Repository<TransitOrder>().Get(w => w.Attempts >= syncAttempts && w.Exception != null).Select(s => s.ColorItemId);
            foreach (var id in trans) if (id.HasValue && id.Value > 0) set.Add(id.Value);

            // Bulk homologation mapping: if any candidate is actually a homologated id (ItemIdHomologado),
            // map it to the Aldebaran ItemId (ItemHomologado.ItemId). This returns source ids ready for UpdateVisibility.
            var candidates = set.ToList();
            if (!candidates.Any()) return candidates;

            try
            {
                var homologs = _unitOfWork.Repository<ItemHomologado>().Get(h => candidates.Contains(h.ItemIdHomologado)).ToList();
                if (homologs != null && homologs.Any())
                {
                    var map = homologs.ToDictionary(h => h.ItemIdHomologado, h => h.ItemId);
                    var resultSet = new System.Collections.Generic.HashSet<int>();
                    foreach (var id in candidates)
                    {
                        if (map.TryGetValue(id, out var src)) resultSet.Add(src);
                        else resultSet.Add(id);
                    }
                    return resultSet.ToList();
                }
            }
            catch { }

            return candidates;
        }

        public void MarkEventsAsFlagged(int itemId, int flagAttempts)
        {
            var items = _unitOfWork.Repository<Item>().Get(w => w.ItemId == itemId).ToList();
            foreach (var it in items)
            {
                it.Attempts = flagAttempts;
                _unitOfWork.Repository<Item>().Update(it);
            }

            var bycolors = _unitOfWork.Repository<ItemByColor>().Get(w => w.ItemId == itemId).ToList();
            foreach (var b in bycolors)
            {
                b.Attempts = flagAttempts;
                _unitOfWork.Repository<ItemByColor>().Update(b);
            }

            var stocks = _unitOfWork.Repository<Stock>().Get(w => w.ItemId == itemId).ToList();
            foreach (var s in stocks)
            {
                s.Attempts = flagAttempts;
                _unitOfWork.Repository<Stock>().Update(s);
            }

            var packs = _unitOfWork.Repository<Packaging>().Get(w => w.ItemId == itemId).ToList();
            foreach (var p in packs)
            {
                p.Attempts = flagAttempts;
                _unitOfWork.Repository<Packaging>().Update(p);
            }

            var trans = _unitOfWork.Repository<TransitOrder>().Get(w => (w.ColorItemId.HasValue && w.ColorItemId.Value == itemId) || w.TransitOrderItemId == itemId).ToList();
            foreach (var t in trans)
            {
                t.Attempts = flagAttempts;
                _unitOfWork.Repository<TransitOrder>().Update(t);
            }

            _unitOfWork.Repository<Item>().SaveChanges();
            _unitOfWork.Repository<ItemByColor>().SaveChanges();
            _unitOfWork.Repository<Stock>().SaveChanges();
            _unitOfWork.Repository<Packaging>().SaveChanges();
            _unitOfWork.Repository<TransitOrder>().SaveChanges();
        }

        public int CountPendingEventsForItems(IEnumerable<int> itemIds, int syncAttempts)
        {
            var count = 0;
            foreach (var id in itemIds)
            {
                count += CountPendingEvents(id, syncAttempts);
            }
            return count;
        }

        public int CountPendingEvents(int itemId, int syncAttempts)
        {
            var count = 0;
            count += _unitOfWork.Repository<Item>().Get(w => w.ItemId == itemId && w.Attempts < syncAttempts).Count();
            count += _unitOfWork.Repository<ItemByColor>().Get(w => w.ItemId == itemId && w.Attempts < syncAttempts).Count();
            count += _unitOfWork.Repository<Stock>().Get(w => w.ItemId == itemId && w.Attempts < syncAttempts).Count();
            count += _unitOfWork.Repository<Packaging>().Get(w => w.ItemId == itemId && w.Attempts < syncAttempts).Count();
            count += _unitOfWork.Repository<TransitOrder>().Get(w => ((w.ColorItemId.HasValue && w.ColorItemId.Value == itemId) || w.TransitOrderItemId == itemId) && w.Attempts < syncAttempts).Count();
            return count;
        }

        public void ClearEventsForItem(int itemId, int flagAttempts)
        {
            var items = _unitOfWork.Repository<Item>().Get(w => w.ItemId == itemId && w.Attempts >= flagAttempts).ToList();
            _unitOfWork.Repository<Item>().Remove(items);

            var bycolors = _unitOfWork.Repository<ItemByColor>().Get(w => w.ItemId == itemId && w.Attempts >= flagAttempts).ToList();
            _unitOfWork.Repository<ItemByColor>().Remove(bycolors);

            var stocks = _unitOfWork.Repository<Stock>().Get(w => w.ItemId == itemId && w.Attempts >= flagAttempts).ToList();
            _unitOfWork.Repository<Stock>().Remove(stocks);

            var packs = _unitOfWork.Repository<Packaging>().Get(w => w.ItemId == itemId && w.Attempts >= flagAttempts).ToList();
            _unitOfWork.Repository<Packaging>().Remove(packs);

            var trans = _unitOfWork.Repository<TransitOrder>().Get(w => ((w.ColorItemId.HasValue && w.ColorItemId.Value == itemId) || w.TransitOrderItemId == itemId) && w.Attempts >= flagAttempts).ToList();
            _unitOfWork.Repository<TransitOrder>().Remove(trans);

            _unitOfWork.Repository<Item>().SaveChanges();
            _unitOfWork.Repository<ItemByColor>().SaveChanges();
            _unitOfWork.Repository<Stock>().SaveChanges();
            _unitOfWork.Repository<Packaging>().SaveChanges();
            _unitOfWork.Repository<TransitOrder>().SaveChanges();
        }
    }
}
