using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Controllers
{
    /// <summary>
    /// Version 1
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/BerrasGeoApp")]
    [ApiController]
    public class GeMessageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GeMessageController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<GeoMessageDto>>> Get()
        {
            var entities = await _context.GeoMessages.ToListAsync();
            var entitiesV2 = await _context.GeoMessagesV2.ToListAsync();
            if (entities.Count < 1 && entitiesV2.Count < 1) return NoContent();
            var result = Enumerable.Empty<GeoMessageDto>()
                .Concat(Format(entities))
                .Concat(FormatV2(entitiesV2));
            return Ok(result);
        }

        /// <summary>
        /// Gets a specific geo comment based on id
        /// </summary>
        /// <param name="id">
        /// The id of a specific comment
        /// </param>
        /// <returns>If found returns one geo comment <see cref="GeoMessageDto"/></returns>
        [HttpGet("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GeoMessageDto>> Get(int id)
        {
            var entity = await _context.GeoMessages.FindAsync(id);
            if (entity == null) return NoContent();
            var entityDto = new GeoMessageDto()
            {
                Message = entity.Message,
                Longitude = entity.Longitude,
                Latitude = entity.Latitude
            };
            return Ok(entityDto);
        }

        /// <summary>
        /// Post a comment from a specified location defined by longitute and latitude
        /// </summary>
        /// <param name="msg">
        /// <para>Supply message as string with a maximum of 200 characters.</para>
        /// <para>Define longitude and latitude with a maximum of 5 decimals.</para>
        /// </param>
        /// <returns>201 Created response and the created geo message <see cref="GeoMessageDto"/></returns>
        [Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<GeoMessageDto>> Post([FromBody] GeoMessageDto msg)
        {


            var entity = await _context.AddAsync(new GeoMessage()
            {
                Message = msg.Message,
                Longitude = msg.Longitude,
                Latitude = msg.Latitude
            });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Entity.Id }, msg);
        }

        private IEnumerable<GeoMessageDto> Format(List<GeoMessage> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                yield return new GeoMessageDto()
                {
                    Message = entities[i].Message,
                    Longitude = entities[i].Longitude,
                    Latitude = entities[i].Latitude
                };
            }
        }

        private IEnumerable<GeoMessageDto> FormatV2(List<GeoMessageV2> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                yield return new GeoMessageDto()
                {
                    Message = entities[i].Body,
                    Longitude = entities[i].Longitude,
                    Latitude = entities[i].Latitude
                };
            }
        }
    }

    /// <summary>
    /// Version 2
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/BerrasGeoApp")]
    [ApiController]
    public class GeMessageV2Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GeMessageV2Controller(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<GeoMessageV2Dto>>> Get()
        {
            var entitiesV1 = await _context.GeoMessages.ToListAsync();
            var entitiesV2 = await _context.GeoMessagesV2.ToListAsync();

            if (entitiesV1.Count < 1 && entitiesV2.Count < 1) return NoContent();

            var result = Enumerable.Empty<GeoMessageV2Dto>()
                .Concat(FormatV1(entitiesV1))
                .Concat(FormatV2(entitiesV2));

            return Ok(result);
        }

        /// <summary>
        /// Gets a specific geo comment based on id
        /// </summary>
        /// <param name="id">
        /// The id of a specific comment
        /// </param>
        /// <returns>If found returns one geo comment <see cref="GeoMessageV2Dto"/></returns>
        [HttpGet("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GeoMessageV2Dto>> Get(int id)
        {
            var entity = await _context.GeoMessagesV2.FindAsync(id);
            if (entity == null) return NoContent();
            var entityDto = new GeoMessageV2Dto()
            {
                Title = entity.Title,
                Body = entity.Body,
                Author = entity.Author,
                Longitude = entity.Longitude,
                Latitude = entity.Latitude
            };
            return Ok(entityDto);
        }

        /// <summary>
        /// Gets a specific geo comment based on id
        /// </summary>
        /// <param name="minLon">
        /// Minimun lon range
        /// </param>
        /// /// <param name="minLat">
        /// Minimun lat range
        /// </param>
        /// <param name="maxLon">
        /// Maximum lon range
        /// </param>
        /// /// <param name="maxLat">
        /// Maximum lat range
        /// </param>
        /// <returns>If found returns all geo comments matching the lon/lat requirements <see cref="GeoMessageV2Dto"/></returns>
        [HttpGet("[action]/Range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GeoMessageV2Dto>> Get(int minLon, int maxLon, int minLat, int maxLat)
        {
            var entitiesV1 = await _context.GeoMessages
                .Where(gm => IsInRange(gm, minLon, maxLon, minLat, maxLat))
                .ToListAsync();
            var entitiesV2 = await _context.GeoMessagesV2
                .Where(gm => IsInRange(gm, minLon, maxLon, minLat, maxLat))
                .ToListAsync();
            var result = Enumerable.Empty<GeoMessageV2Dto>()
                .Concat(FormatV1(entitiesV1))
                .Concat(FormatV2(entitiesV2));

            if (result.Count() < 1) return NoContent();

            return Ok(result);
        }

        /// <summary>
        /// Post a comment from a specified location defined by longitute and latitude
        /// </summary>
        /// <param name="msg">
        /// <para>Supply message as string with a maximum of 200 characters.</para>
        /// <para>Define longitude and latitude with a maximum of 5 decimals.</para>
        /// </param>
        /// <returns>201 Created response and the created geo message <see cref="GeoMessageV2Dto"/></returns>
        [Authorize]
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<GeoMessageV2Dto>> Post([FromBody] GeoMessageV2Dto msg)
        {
            var user = await _context.AppUsers.FindAsync(_userManager.GetUserId(User));

            var entity = await _context.AddAsync(new GeoMessageV2()
            {
                Title = msg.Title,
                Body = msg.Body,
                Author = $"{user.FirstName} {user.LastName}",
                Longitude = msg.Longitude,
                Latitude = msg.Latitude
            });
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get),
                new { id = entity.Entity.Id },
                FormatV2(new List<GeoMessageV2> { entity.Entity }).First());
        }

        private IEnumerable<GeoMessageV2Dto> FormatV1(List<GeoMessage> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                yield return new GeoMessageV2Dto()
                {
                    Title = String.Join(" ", entities[i].Message.Split(" ").Take(3)),
                    Body = entities[i].Message,
                    Author = "Rando",
                    Longitude = entities[i].Longitude,
                    Latitude = entities[i].Latitude
                };
            }
        }

        private IEnumerable<GeoMessageV2Dto> FormatV2(List<GeoMessageV2> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                yield return new GeoMessageV2Dto()
                {
                    Title = entities[i].Title,
                    Body = entities[i].Body,
                    Author = entities[i].Author,
                    Longitude = entities[i].Longitude,
                    Latitude = entities[i].Latitude
                };
            }
        }

        private bool IsInRange(GeoMessageV2 gm, int minLon, int maxLon, int minLat, int maxLat)
        {
            return gm.Longitude > minLon && gm.Longitude < maxLon
                && gm.Latitude > minLat && gm.Latitude < maxLat;
        }

        private bool IsInRange(GeoMessage gm, int minLon, int maxLon, int minLat, int maxLat)
        {
            return gm.Longitude > minLon && gm.Longitude < maxLon
                && gm.Latitude > minLat && gm.Latitude < maxLat;
        }
    }
}
