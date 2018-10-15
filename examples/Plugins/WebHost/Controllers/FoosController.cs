namespace Plugins.WebHost.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Shared;

    [Route("[controller]")]
    public class FoosController : Controller
    {
        private readonly IFooRepository _fooRepository; 

        public FoosController(IFooRepository fooRepository)
        {
            this._fooRepository = fooRepository; 
        }

        [HttpGet]
        public IEnumerable<Foo> Get()
        {
            return this._fooRepository.Get(); 
        }
    }
}
