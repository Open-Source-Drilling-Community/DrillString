using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using NORCE.Drilling.DrillString.Model;
using NORCE.Drilling.DrillString.Service.Managers;

namespace NORCE.Drilling.DrillString.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class DrillStringComponentController : ControllerBase
    {
        private readonly ILogger<DrillStringComponentManager> _logger;
        private readonly DrillStringComponentManager _drillStringComponentManager;

        public DrillStringComponentController(ILogger<DrillStringComponentManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _drillStringComponentManager = DrillStringComponentManager.GetInstance(_logger, connectionManager);
        }

        /// <summary>
        /// Returns the list of Guid of all DrillStringComponent present in the microservice database at endpoint DrillString/api/DrillStringComponent
        /// </summary>
        /// <returns>the list of Guid of all DrillStringComponent present in the microservice database at endpoint DrillString/api/DrillStringComponent</returns>
        [HttpGet(Name = "GetAllDrillStringComponentId")]
        public ActionResult<IEnumerable<Guid?>> GetAllDrillStringComponentId()
        {
            var ids = _drillStringComponentManager.GetAllDrillStringComponentId();
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
        /// Returns the list of MetaInfo of all DrillStringComponent present in the microservice database, at endpoint DrillString/api/DrillStringComponent/MetaInfo
        /// </summary>
        /// <returns>the list of MetaInfo of all DrillStringComponent present in the microservice database, at endpoint DrillString/api/DrillStringComponent/MetaInfo</returns>
        [HttpGet("MetaInfo", Name = "GetAllDrillStringComponentMetaInfo")]
        public ActionResult<IEnumerable<MetaInfo?>> GetAllDrillStringComponentMetaInfo()
        {
            var vals = _drillStringComponentManager.GetAllDrillStringComponentMetaInfo();
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
        /// Returns the DrillStringComponent identified by its Guid from the microservice database, at endpoint DrillString/api/DrillStringComponent/MetaInfo/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the DrillStringComponent identified by its Guid from the microservice database, at endpoint DrillString/api/DrillStringComponent/MetaInfo/id</returns>
        [HttpGet("{id}", Name = "GetDrillStringComponentById")]
        public ActionResult<DrillStringComponent?> GetDrillStringComponentById(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                var val = _drillStringComponentManager.GetDrillStringComponentById(id);
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
        /// Returns the list of all DrillStringComponent present in the microservice database, at endpoint DrillString/api/DrillStringComponent/HeavyData
        /// </summary>
        /// <returns>the list of all DrillStringComponent present in the microservice database, at endpoint DrillString/api/DrillStringComponent/HeavyData</returns>
        [HttpGet("HeavyData", Name = "GetAllDrillStringComponent")]
        public ActionResult<IEnumerable<DrillStringComponent?>> GetAllDrillStringComponent()
        {
            var vals = _drillStringComponentManager.GetAllDrillStringComponent();
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
        /// Performs calculation on the given DrillStringComponent and adds it to the microservice database, at the endpoint DrillString/api/DrillStringComponent
        /// </summary>
        /// <param name="drillStringComponent"></param>
        /// <returns>true if the given DrillStringComponent has been added successfully to the microservice database, at the endpoint DrillString/api/DrillStringComponent</returns>
        [HttpPost(Name = "PostDrillStringComponent")]
        public ActionResult PostDrillStringComponent([FromBody] Model.DrillStringComponent? data)
        {
            if (data != null &&  data.MetaInfo.ID != Guid.Empty)
            {
                var existingData = _drillStringComponentManager.GetDrillStringComponentById(data.MetaInfo.ID);
                if (existingData == null)
                {
                    if (_drillStringComponentManager.AddDrillStringComponent(data))
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
                    _logger.LogWarning("The given DrillStringComponent already exists and will not be added");
                    return StatusCode(StatusCodes.Status409Conflict);
                }
            }
            else
            {
                _logger.LogWarning("The given DrillStringComponent is null or its ID is empty");
                return BadRequest();
            }
        }

        /// <summary>
        /// Performs calculation on the given DrillStringComponent and updates it in the microservice database, at the endpoint DrillString/api/DrillStringComponent/id
        /// </summary>
        /// <param name="drillStringComponent"></param>
        /// <returns>true if the given DrillStringComponent has been updated successfully to the microservice database, at the endpoint DrillString/api/DrillStringComponent/id</returns>
        [HttpPut("{id}", Name = "PutDrillStringComponentById")]
        public ActionResult PutDrillStringComponentById(Guid id, [FromBody] Model.DrillStringComponent data)
        {
            if (data != null &&  data.MetaInfo.ID.Equals(id))
            {
                var existingData = _drillStringComponentManager.GetDrillStringComponentById(id);
                if (existingData != null)
                {
                    if (_drillStringComponentManager.UpdateDrillStringComponentById(id, data))
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
                    _logger.LogWarning("The given DrillStringComponent has not been found in the database");
                    return NotFound();
                }
            }
            else
            {
                _logger.LogWarning("The given DrillStringComponent is null or its does not match the ID to update");
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes the DrillStringComponent of given ID from the microservice database, at the endpoint DrillString/api/DrillStringComponent/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the DrillStringComponent was deleted from the microservice database, at the endpoint DrillString/api/DrillStringComponent/id</returns>
        [HttpDelete("{id}", Name = "DeleteDrillStringComponentById")]
        public ActionResult DeleteDrillStringComponentById(Guid id)
        {
            if (_drillStringComponentManager.GetDrillStringComponentById(id) != null)
            {
                if (_drillStringComponentManager.DeleteDrillStringComponentById(id))
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
                _logger.LogWarning("The DrillStringComponent of given ID does not exist");
                return NotFound();
            }
        }
    }
}
