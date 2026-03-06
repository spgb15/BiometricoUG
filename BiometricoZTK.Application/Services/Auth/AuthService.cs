using BiometricoZTK.Application.DTO;
using BiometricoZTK.Application.Interfaces.Auth;
using BiometricoZTK.Domain.Entities.Auth;
using BiometricoZTK.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiometricoZTK.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ILoginRepository _repository;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IJwtSerivce _jwtSerivce;

        public AuthService(
            ILoginRepository repository,
            IPasswordHasher<Usuario> passwordHasher,
            IJwtSerivce jwtService)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _jwtSerivce = jwtService;
        }

        public async Task<AuthResponse> LoginAsync(AuthRequest login)
        {
            if(login == null) { throw new ArgumentNullException(nameof(login)); }

            var usuario = await _repository.GetUserByEmail(login.Email);
            
            if(usuario == null)
            {
                return new AuthResponse { Success = false, Message = "Usuario no encontrado" };
            }

            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, login.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                return new AuthResponse { Success = false, Message = "Contraseña Incorrecta" };
            }

            var token = _jwtSerivce.GenerarToken(usuario.Correo, usuario.RolId, usuario.Nombre, usuario.Id);

            return new AuthResponse
            {
                Success = true,
                Message = "Login Exitoso",
                Token = token
            };
        }

        public async Task<AuthResponse> RegistrarUsuarioAsync(RegisterRequest register)
        {
            if(register == null) { throw new ArgumentNullException(nameof(register)); }

            if (await _repository.ExisteCorreoAsync(register.Email))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "El correo ya esta registrado"
                };
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = register.Nombre,
                Correo = register.Email,
                RolId = register.RolId,
                Activo = true,
                Created = DateTime.Now,
                CreatedBy = ""
            };

            nuevoUsuario.PasswordHash = _passwordHasher.HashPassword(nuevoUsuario, register.Password);

            var resultado = await _repository.CrearUsuario(nuevoUsuario);

            if (resultado)
            {
                return new AuthResponse
                {
                    Success = true,
                    Message = "Usuario Creado Exitosamente"
                };
            }

            return new AuthResponse
            {
                Success = false,
                Message = "Error al guardar usuario"
            };
        }

        public async Task<AuthResponse> CambiarPasswordAsync(long usuarioId, string passwordActual, string nuevaPassword)
        {
            var usuario = await _repository.GetByIdAsync(usuarioId);

            var verificacion = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordActual);

            if(verificacion == PasswordVerificationResult.Failed)
            {
                return new AuthResponse { Success = false, Message = "La contraseña actual es incorrecta" }; 
            }

            var nuevoHash = _passwordHasher.HashPassword(usuario, nuevaPassword);
            return new AuthResponse
            {
                Success = await _repository.ActualizarPassword(usuarioId, nuevoHash),
                Message = "Contraseña actualizada"
            };
        }

    }
}
