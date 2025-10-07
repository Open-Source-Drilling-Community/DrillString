using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using NORCE.Drilling.DrillString.Service.Managers;

namespace NORCE.Drilling.DrillString.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class DrillStringController : ControllerBase
    {
        private readonly ILogger<DrillStringManager> _logger;
        private readonly DrillStringManager _drillStringManager;

        public DrillStringController(ILogger<DrillStringManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _drillStringManager = DrillStringManager.GetInstance(_logger, connectionManager);
        }

        /// <summary>
        /// Returns the list of Guid of all DrillString present in the microservice database at endpoint DrillString/api/DrillString
        /// </summary>
        /// <returns>the list of Guid of all DrillString present in the microservice database at endpoint DrillString/api/DrillString</returns>
        [HttpGet(Name = "GetAllDrillStringId")]
        public ActionResult<IEnumerable<Guid>> GetAllDrillStringId()
        {
            var ids = _drillStringManager.GetAllDrillStringId();
            if (ids != null)
            {
                return Ok(ids);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns the list of MetaInfo of all DrillString present in the microservice database, at endpoint DrillString/api/DrillString/MetaInfo
        /// </summary>
        /// <returns>the list of MetaInfo of all DrillString present in the microservice database, at endpoint DrillString/api/DrillString/MetaInfo</returns>
        [HttpGet("MetaInfo", Name = "GetAllDrillStringMetaInfo")]
        public ActionResult<IEnumerable<MetaInfo>> GetAllDrillStringMetaInfo()
        {
            var vals = _drillStringManager.GetAllDrillStringMetaInfo();
            if (vals != null)
            {
                return Ok(vals);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns the DrillString identified by its Guid from the microservice database, at endpoint DrillString/api/DrillString/MetaInfo/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the DrillString identified by its Guid from the microservice database, at endpoint DrillString/api/DrillString/MetaInfo/id</returns>
        [HttpGet("{id}", Name = "GetDrillStringById")]
        public ActionResult<Model.DrillString?> GetDrillStringById(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                var val = _drillStringManager.GetDrillStringById(id);
                if (val != null)
                {
                    return Ok(val);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Returns the list of all DrillStringLight present in the microservice database, at endpoint DrillString/api/DrillString/LightData
        /// </summary>
        /// <returns>the list of all DrillStringLight present in the microservice database, at endpoint DrillString/api/DrillString/LightData</returns>
        [HttpGet("LightData", Name = "GetAllDrillStringLight")]
        public ActionResult<IEnumerable<Model.DrillStringLight>> GetAllDrillStringLight()
        {
            var vals = _drillStringManager.GetAllDrillStringLight();
            if (vals != null)
            {
                return Ok(vals);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns the list of all DrillString present in the microservice database, at endpoint DrillString/api/DrillString/HeavyData
        /// </summary>
        /// <returns>the list of all DrillString present in the microservice database, at endpoint DrillString/api/DrillString/HeavyData</returns>
        [HttpGet("HeavyData", Name = "GetAllDrillString")]
        public ActionResult<IEnumerable<Model.DrillString?>> GetAllDrillString()
        {
            var vals = _drillStringManager.GetAllDrillString();
            if (vals != null)
            {
                return Ok(vals);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Performs calculation on the given DrillString and adds it to the microservice database, at the endpoint DrillString/api/DrillString
        /// </summary>
        /// <param name="drillString"></param>
        /// <returns>true if the given DrillString has been added successfully to the microservice database, at the endpoint DrillString/api/DrillString</returns>
        [HttpPost(Name = "PostDrillString")]
        public ActionResult PostDrillString([FromBody] Model.DrillString? data)
        {
            // Check if drillString exists in the database through ID
            if (data != null && data.MetaInfo != null && data.MetaInfo.ID != Guid.Empty)
            {
                var existingData = _drillStringManager.GetDrillStringById(data.MetaInfo.ID);
                if (existingData == null)
                {   
                    //  If drillString was not found, call AddDrillString, where the drillString.Calculate()
                    // method is called. 
                    if (_drillStringManager.AddDrillString(data))
                    {
                        return Ok(); // status=OK is used rather than status=Created because NSwag auto-generated controllers use 200 (OK) rather than 201 (Created) as return codes
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                else
                {
                    _logger.LogWarning("The given DrillString already exists and will not be added");
                    return StatusCode(StatusCodes.Status409Conflict);
                }
            }
            else
            {
                _logger.LogWarning("The given DrillString is null, badly formed, or its ID is empty");
                return BadRequest();
            }
        }

        /// <summary>
        /// Performs calculation on the given DrillString and updates it in the microservice database, at the endpoint DrillString/api/DrillString/id
        /// </summary>
        /// <param name="drillString"></param>
        /// <returns>true if the given DrillString has been updated successfully to the microservice database, at the endpoint DrillString/api/DrillString/id</returns>
        [HttpPut("{id}", Name = "PutDrillStringById")]
        public ActionResult PutDrillStringById(Guid id, [FromBody] Model.DrillString? data)
        {
            // Check if DrillString is in the data base
            if (data != null && data.MetaInfo != null && data.MetaInfo.ID.Equals(id))
            {
                var existingData = _drillStringManager.GetDrillStringById(id);
                if (existingData != null)
                {
                    if (_drillStringManager.UpdateDrillStringById(id, data))
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                else
                {
                    _logger.LogWarning("The given DrillString has not been found in the database");
                    return NotFound();
                }
            }
            else
            {
                _logger.LogWarning("The given DrillString is null, badly formed, or its does not match the ID to update");
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes the DrillString of given ID from the microservice database, at the endpoint DrillString/api/DrillString/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the DrillString was deleted from the microservice database, at the endpoint DrillString/api/DrillString/id</returns>
        [HttpDelete("{id}", Name = "DeleteDrillStringById")]
        public ActionResult DeleteDrillStringById(Guid id)
        {
            if (_drillStringManager.GetDrillStringById(id) != null)
            {
                if (_drillStringManager.DeleteDrillStringById(id))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
            {
                _logger.LogWarning("The DrillString of given ID does not exist");
                return NotFound();
            }
        }
    }
}
