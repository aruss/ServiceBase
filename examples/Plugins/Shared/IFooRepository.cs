namespace Shared
{
    using System.Collections.Generic;

    public interface IFooRepository
    {
        IEnumerable<Foo> Get();       
    }
}
