// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly CatalogoMateriaBdCoreContext _catalogoContext;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            CatalogoMateriaBdCoreContext catalogoContext)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _catalogoContext = catalogoContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "La cédula es obligatoria")]
            [MaxLength(10, ErrorMessage = "Máximo 10 caracteres")]
            [Display(Name = "Cédula")]
            public string Cedula { get; set; }

            [Required]
            [Display(Name = "Usuario")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Seleccione un rol")]
            [Display(Name = "Rol")]
            public string RolSeleccionado { get; set; }

            public int? DepartamentoId { get; set; }
            public int? CarreraId { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["Departamentos"] = new SelectList(_catalogoContext.Departamentos, "DepartamentoId", "NombreDepartamento");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            try { ViewData["Departamentos"] = new SelectList(_catalogoContext.Departamentos, "DepartamentoId", "NombreDepartamento"); }
            catch { ViewData["Departamentos"] = new SelectList(Enumerable.Empty<object>()); }

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Guardar cédula en PhoneNumber (columna existente en AspNetUsers)
                    await _userManager.SetPhoneNumberAsync(user, Input.Cedula);

                    // Asignar rol seleccionado
                    if (!string.IsNullOrEmpty(Input.RolSeleccionado))
                        await _userManager.AddToRoleAsync(user, Input.RolSeleccionado);

                    // Crear Persona en el catálogo para todos los roles
                    try
                    {
                        // Buscar el RolPersonaId en la tabla rolpersona según el rol seleccionado
                        var rolPersona = _catalogoContext.Rolpersonas
                            .FirstOrDefault(r => r.RolNombre == Input.RolSeleccionado);

                        var persona = new Persona
                        {
                            CedulaPersona = Input.Cedula,
                            NombrePersona = Input.UserName,
                            ApellidoPersona = "",
                            Foto = null,
                            RolPersonaId = rolPersona?.RolPersonaId
                        };
                        _catalogoContext.Personas.Add(persona);
                        await _catalogoContext.SaveChangesAsync();

                        // Además crear Profesor o Estudiante según el rol
                        if (Input.RolSeleccionado == "PROFESOR" && Input.DepartamentoId.HasValue)
                        {
                            _catalogoContext.Profesors.Add(new Profesor
                            {
                                PersonalId = persona.PersonalId,
                                DepartamentoId = Input.DepartamentoId.Value,
                                SueldoProfesor = 0
                            });
                            await _catalogoContext.SaveChangesAsync();
                        }
                        else if (Input.RolSeleccionado == "ESTUDIANTE")
                        {
                            _catalogoContext.Estudiantes.Add(new Estudiante
                            {
                                PersonalId = persona.PersonalId,
                                CarreraId = Input.CarreraId
                            });
                            await _catalogoContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "No se pudo crear el registro en el catálogo para {User}", Input.UserName);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
