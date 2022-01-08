using System;
namespace FlapKapVendingMachine.Helpers.Interfaces
{
    public interface IMapper
    {
        public void Map<TSource, TTarget>(TSource source, ref TTarget target);
    }
}
