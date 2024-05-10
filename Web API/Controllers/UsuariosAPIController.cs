using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_API.Data;
using Web_API.Models;
using Web_API.Repositorio.DAO;

namespace Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosAPIController : ControllerBase
    {
        private readonly UsuarioDAO _usuarioDAO;

        public UsuariosAPIController(UsuarioDAO usuarioDAO)
        {
            _usuarioDAO = usuarioDAO;
        }

        [HttpGet("ObtenerUsuarios")]
        public async Task<ActionResult<IEnumerable<Usuarios>>> ObtenerUsuarios()
        {
            var usuarios = await Task.Run(() => _usuarioDAO.ObtenerUsuarios());
            return Ok(usuarios);
        }

        [HttpGet("ObtenerUsuario/{id}")]
        public async Task<ActionResult<Usuarios>> ObtenerUsuario(int id)
        {
            var usuario = await Task.Run(() => _usuarioDAO.ObtenerUsuarioPorId(id));
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPost("InsertarUsuario")]
        public async Task<ActionResult<string>> InsertarUsuario(Usuarios usuario)
        {
            await Task.Run(() => _usuarioDAO.InsertarUsuario(usuario));
            return Ok("Usuario insertado correctamente");
        }

        [HttpPut("EditarUsuario/{id}")]
        public async Task<ActionResult<string>> EditarUsuario(int id, Usuarios usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }
            await Task.Run(() => _usuarioDAO.EditarUsuario(usuario));
            return Ok("Usuario actualizado correctamente");
        }

        [HttpDelete("EliminarUsuario/{id}")]
        public async Task<ActionResult<string>> EliminarUsuario(int id)
        {
            var usuario = await Task.Run(() => _usuarioDAO.ObtenerUsuarioPorId(id));
            if (usuario == null)
            {
                return NotFound();
            }
            await Task.Run(() => _usuarioDAO.EliminarUsuario(id));
            return Ok("Usuario eliminado correctamente");
        }
    }
}
