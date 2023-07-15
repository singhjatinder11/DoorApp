using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace MordenDoors.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly MordenDoorsEntities mordenDoorsEntities = new MordenDoorsEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult IsEmployeeExist(string EmoployeeNumber) { return Json(IsEmpAvailable(EmoployeeNumber)); }

        public bool IsEmpAvailable(string EmoployeeNumber)
        {
            var RegEmailId = (from u in mordenDoorsEntities.AspNetUsers where u.EmoployeeNumber.ToUpper() == EmoployeeNumber.ToUpper() select new { EmoployeeNumber }).FirstOrDefault();
            bool status;
            if (RegEmailId != null) { status = false; }
            else { status = true; }
            return status;
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult IsMailExist(string Email)
        {
            bool res = IsUserAvailable(Email);
            TempData["UserExist"] = res;
            return Json(res);

        }
        public bool IsUserAvailable(string Email)
        {
            var RegEmailId = (from u in mordenDoorsEntities.AspNetUsers where u.Email.ToUpper() == Email.ToUpper() && u.status== true select new { Email }).FirstOrDefault();
            bool status;
            if (RegEmailId != null)
            {
                status = false;
            }
            else
            {
                status = true;
            }
            TempData["UserExist"] = status;
            return status;
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (User.IsInRole("Employee")) {
                return RedirectToAction("Home", "User");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    ModelState.AddModelError("", "You must have a confirmed email to log on.");
                    return View();
                }
                if (user.status == false)
                {
                    ModelState.AddModelError("", "Inactive mail address ask admin for activation");
                    return View();
                }
            }
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    using (MordenDoorsEntities db = new MordenDoorsEntities())
                    {
                        Session["UserId"] = user.Id;
                        var roleIds = user.Roles.Select(s => s.RoleId).ToList();
                        var roles = db.AspNetRoles.Where(s => roleIds.Contains(s.Id)).Select(s => s.Name).ToList();
                        if (roles.Contains("Admin"))
                            return RedirectToLocal(returnUrl);
                        else if (roles.Contains("Employee"))
                            return RedirectToAction("Home", "User");
                        else
                            return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Incorrect Email/Password.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            var selectListItems = new List<SelectListItem>();
            var stateListItems = new List<SelectListItem>();
            var userRoles = new List<SelectListItem>();
            selectListItems = mordenDoorsEntities.WorkStages.Select(x => new SelectListItem { Text = x.StageName, Value = x.Id.ToString() }).ToList();
            stateListItems = mordenDoorsEntities.State.Select(x => new SelectListItem { Text = x.stateName, Value = x.Id.ToString() }).ToList();
            var userRole = string.Empty;
            if (User.IsInRole("SuperAdmin"))
                userRoles = mordenDoorsEntities.AspNetRoles.Select(x => new SelectListItem { Text = x.Name, Value = x.Name }).ToList();
            else
                userRole = "Employee";

            RegisterViewModel model = new RegisterViewModel()
            {
                UserSkills = selectListItems,
                UserRoles = userRoles,
                UserRole = userRole,
                StateList= stateListItems
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmoployeeNumber = model.EmoployeeNumber,
                    HiredDate = System.DateTime.Now,
                    CreateTime = System.DateTime.Now,
                    LastUpdateTime = System.DateTime.Now,
                    status = true,
                    AddrLine1 = model.AddrLine1,
                    AddrLine2 = model.AddrLine2,
                    City = model.City,
                    Country = model.Country,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    PrintOnCheckName = model.PrintOnCheckName,
                    Postalcode =model.Postalcode,
                    SSN = model.SSN,
                    StateId=model.State
                };
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    if (!roleManager.RoleExists("Admin"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                        role.Name = "Admin";
                        roleManager.Create(role);
                    }
                }
                var result = await UserManager.CreateAsync(user, model.Password);
                var error = result.Errors.ToList();
                if (result.Succeeded)
                {
                    IdentityResult identityResult = UserManager.AddToRole(user.Id, model.UserRole);
                    using (MordenDoorsEntities db = new MordenDoorsEntities())
                    {
                        var unselectedSkills = db.EmployeeSkills.Where(s => !model.UserSkill.Contains(s.SkillId) && s.UserId == user.Id);
                        db.EmployeeSkills.RemoveRange(unselectedSkills);
                        foreach (int skillId in model.UserSkill)
                        {
                            var empSkills = db.EmployeeSkills.Where(s => s.SkillId == skillId && s.UserId == user.Id).FirstOrDefault();
                            if (empSkills == null)
                                db.EmployeeSkills.Add(new EmployeeSkills() { SkillId = skillId, UserId = user.Id });
                        }
                        db.SaveChanges();
                    }
                    //var result1 = UserManager.AddToRole(user.Id, model.UserRole.ToString());
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/MailTemplate/AccountConfirmation.cshtml")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{ConfirmationLink}", callbackUrl);
                    body = body.Replace("{UserName}", model.FirstName);
                    body = body.Replace("{TempPassword}", model.Password);
                    bool IsSendEmail = SendEmail.EmailSend(model.Email, "Confirm your account", body, true);
                    if (IsSendEmail)
                        TempData["AddEmployee"] = "<script>alert('Employee Added Successfully !!!')</script>";
                    return RedirectToAction("UsersWithRoles", "Manage");
                }
                else
                {
                    if (error!=null)
                    {
                        model.Password = UserManager.PasswordHasher.HashPassword(model.Password);
                        AdminController admin = new AdminController();
                        admin.EditEmployee(model);
                        return RedirectToAction("UsersWithRoles", "Manage");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                
            }
            return View(model);
        }


        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }


        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPasswordConfirmation");
                }
                TempData["Email"] = model.Email;
                string To = model.Email, UserID, Password, SMTPPort, Host;
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                var lnkHref = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                string subject = "A link to change your password";
                string body = "\bHello " + user.FirstName + "\n" + "To change your Morder Doors Account password, click the link below:\n \n" + lnkHref +
                    "\n\n Once you change your password, remember to log in again with your new password to continue using your account.\n\n" +
                   "If you did not make this request, please ignore this email.\n \n" + "Thanks \n" + "Morden Doors Team ";
                EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);

                SendEmail.EmailSend(model.Email, subject, body, true);
                //EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                TempData["ForgotPassword"] = "Succuss";
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            System.Web.HttpCookie cookie1 = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            System.Web.HttpCookie cookie2 = new System.Web.HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
        #region Helpers
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        public List<string> GetCountryList()
        {
            List<string> CountryList = new List<string>();
            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                if (!(CountryList.Contains(R.EnglishName)))
                {
                    CountryList.Add(R.EnglishName);
                }
            }

            CountryList.Sort();
            return CountryList;
        }
        #endregion
    }
}