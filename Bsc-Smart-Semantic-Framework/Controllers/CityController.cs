using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bsc_Smart_Semantic_Framework.MasterModels;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bsc_Smart_Semantic_Framework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        private readonly DbContextGremlin _dbg;

        public CityController(ILogger<CityController> logger, DbContextGremlin dbg)
        {
            _logger = logger;
            _dbg = dbg;
        }

        //[HttpGet]
        //[EnableQuery()]
        ////GetInstance()
        //public IEnumerable<Object> GetAllEntries()
        //{
        //    IEnumerable<Object> l = _dbg.DefaultModel.GetVertex<Object>();
        //    return l;
        //}

        [HttpPost("add")]
        // PostNewInstance()
        public void Post([FromBody]City city)
        {
            _dbg.DefaultModel.AddVertex(city);
        }

        [HttpPost("update/{vtx_id}")]
        // PostChangeResource()
        public void Put([FromBody]City city, string vtx_id)
        {
            _dbg.DefaultModel.UpdateVertex(city, vtx_id);
        }

        [HttpDelete("{vtx_id}")]
        // PostDeleteInstance()
        public void Delete(string vtx_id)
        {
            _dbg.DefaultModel.DeleteVertex(vtx_id);
        }
    }
}