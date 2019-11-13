using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bsc_Smart_Semantic_Framework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly ILogger<UniversityController> _logger;

        public UniversityController(ILogger<UniversityController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //[EnableQuery()]
        //// GetInstance()
        //public IEnumerable<LpgUniversity> GetAllEntries()
        //{
        //    IEnumerable<LpgUniversity> UniversityList = _trinity.DefaultModel.GetResources<LpgUniversity>(true);
        //    return UniversityList.ToList();
        //}

        //[HttpPost("add")]
        //// PostNewInstance()
        //public void Post([FromBody]Object university)
        //{
        //    dynamic data = university;
        //    data.uri = _trinity.DefaultModel.Uri + "#University_" + DateTime.Now.ToString("HHmmss_ddMMyyyy");
        //    Add(data.ToObject<LpgUniversity>());
        //}

        //[HttpPost("update/{uri_id}")]
        //// PostChangeResource()
        //public void Put([FromBody]Object university, string uri_id)
        //{
        //    string nspaced_uri = $"{_trinity.DefaultModel.Uri}#{uri_id}";
        //    dynamic data = university;
        //    data.uri = nspaced_uri;
        //    _trinity.DefaultModel.UpdateResource(data.ToObject<LpgUniversity>());
        //}

        //[HttpDelete("{uri_id}")]
        //// PostDeleteInstance()
        //public void Delete(string uri_id)
        //{
        //    string nspaced_uri = $"{_trinity.DefaultModel.Uri}#{uri_id}";
        //    _trinity.DefaultModel.DeleteResource(new Uri(nspaced_uri));
        //}

        //private void Add(LpgUniversity university)
        //{
        //    _trinity.DefaultModel.AddResource(university);
        //    university.Commit();
        //}
    }
}